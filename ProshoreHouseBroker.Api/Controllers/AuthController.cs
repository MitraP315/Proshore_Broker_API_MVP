using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProshoreHouseBroker.Api.Extensions;
using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Domain.Entities;
using ProshoreHouseBroker.Domain.Enums;
using ProshoreHouseBroker.Infrastructure.Authentication;

namespace ProshoreHouseBroker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthController(
        UserManager<AppUser> userManager,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto request)
    {
        var allowedRoles = new[] { UserRoles.Admin, UserRoles.Broker, UserRoles.HouseSeeker };

        if (!allowedRoles.Contains(request.Role, StringComparer.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(request.Role), "Role must be Admin, Broker, or HouseSeeker.");
            return ValidationProblem(ModelState);
        }

        var role = allowedRoles.First(x => x.Equals(request.Role, StringComparison.OrdinalIgnoreCase));
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            UserName = request.Email.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim()
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        await _userManager.AddToRoleAsync(user, role);

        var requiresEmailConfirmation = role == UserRoles.Broker;
        var emailToken = requiresEmailConfirmation
            ? await _userManager.GenerateEmailConfirmationTokenAsync(user)
            : null;

        return Ok(new RegisterResponseDto
        {
            UserId = user.Id.ToString(),
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Role = role,
            RequiresEmailConfirmation = requiresEmailConfirmation,
            EmailConfirmationToken = emailToken,
            Message = requiresEmailConfirmation
                ? "Broker registered. Confirm email before login."
                : "User registered successfully."
        });
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());

        if (user is null)
        {
            return NotFound(new { message = "User not found." });
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        return Ok(new { message = "Email confirmed successfully." });
    }

    [HttpPost("request-email-confirmation")]
    public async Task<IActionResult> RequestEmailConfirmation(RequestEmailConfirmationDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());

        if (user is null)
        {
            return NotFound(new { message = "User not found." });
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        return Ok(new { message = "Email confirmation token generated.", token });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var primaryRole = roles.FirstOrDefault() ?? string.Empty;

        if (roles.Contains(UserRoles.Broker))
        {
            if (!user.EmailConfirmed)
            {
                return BadRequest(new { message = "Broker email must be verified before login." });
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber) || !string.Equals(user.PhoneNumber, request.PhoneNumber.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(new { message = "Broker mobile validation failed." });
            }

            if (user.TwoFactorEnabled)
            {
                var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

                return Ok(new LoginResponseDto
                {
                    UserId = user.Id.ToString(),
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    Role = primaryRole,
                    RequiresMfa = true,
                    Message = "MFA code generated. Complete verification to finish login.",
                    DebugMfaCode = code
                });
            }
        }

        if (roles.Contains(UserRoles.Admin) && !user.EmailConfirmed)
        {
            return BadRequest(new { message = "Admin email must be verified before login." });
        }

        return Ok(await CreateSuccessfulLoginResponseAsync(user, primaryRole));
    }

    [HttpPost("verify-mfa")]
    public async Task<ActionResult<LoginResponseDto>> VerifyMfa(VerifyMfaRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());

        if (user is null)
        {
            return Unauthorized(new { message = "Invalid MFA request." });
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (!roles.Contains(UserRoles.Broker))
        {
            return BadRequest(new { message = "MFA verification is only configured for brokers." });
        }

        if (!string.Equals(user.PhoneNumber, request.PhoneNumber.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized(new { message = "Broker mobile validation failed." });
        }

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, request.Code.Trim());

        if (!isValid)
        {
            return Unauthorized(new { message = "Invalid MFA code." });
        }

        return Ok(await CreateSuccessfulLoginResponseAsync(user, UserRoles.Broker));
    }

    [HttpPost("broker/mfa")]
    [Authorize(Roles = UserRoles.Broker)]
    public async Task<IActionResult> SetBrokerMfa(EnableMfaRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(User.GetRequiredUserId().ToString());

        if (user is null)
        {
            return NotFound(new { message = "Broker not found." });
        }

        user.TwoFactorEnabled = request.Enable;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        return Ok(new { message = request.Enable ? "Broker MFA enabled." : "Broker MFA disabled." });
    }

    private async Task<LoginResponseDto> CreateSuccessfulLoginResponseAsync(AppUser user, string role)
    {
        var token = await _tokenService.GenerateTokenAsync(user);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            UserId = user.Id.ToString(),
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Role = role,
            RequiresMfa = false,
            Message = "Login successful."
        };
    }
    [HttpPost("admin/change-password")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> AdminChangePassword(
    AdminChangePasswordDto request)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            ModelState.AddModelError(
                nameof(request.ConfirmPassword),
                "Password confirmation does not match."
            );

            return ValidationProblem(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(
            request.UsernameOrEmail.Trim()
        );

        user ??= await _userManager.FindByNameAsync(
            request.UsernameOrEmail.Trim()
        );

        if (user is null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(
            user,
            token,
            request.NewPassword
        );

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    error.Code,
                    error.Description
                );
            }

            return ValidationProblem(ModelState);
        }

        return Ok(new
        {
            message = "Password changed successfully."
        });
    }
}
