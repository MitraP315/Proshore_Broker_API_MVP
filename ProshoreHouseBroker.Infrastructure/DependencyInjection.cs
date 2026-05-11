using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Application.Services;
using ProshoreHouseBroker.Application.Validators;
using ProshoreHouseBroker.Domain.Entities;
using ProshoreHouseBroker.Infrastructure.Authentication;
using ProshoreHouseBroker.Infrastructure.Caching;
using ProshoreHouseBroker.Infrastructure.Persistence;
using ProshoreHouseBroker.Infrastructure.Repositories;
using System.Text;

namespace ProshoreHouseBroker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        services.AddMemoryCache();
        services.AddScoped<IValidator<CreateListingRequestDto>, CreateListingRequestValidator>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<ICommissionRuleRepository, CommissionRuleRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<ICommissionService, CommissionService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<PropertyService>();
        services.AddScoped<IPropertyService, CachedPropertyService>();
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }
}
