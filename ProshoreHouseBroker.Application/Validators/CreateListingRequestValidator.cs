using FluentValidation;
using ProshoreHouseBroker.Application.DTOs;

namespace ProshoreHouseBroker.Application.Validators;

public class CreateListingRequestValidator : AbstractValidator<CreateListingRequestDto>
{
    public CreateListingRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(250);
        RuleFor(x => x.PropertyType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Features).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.ImageUrls)
            .NotNull()
            .Must(x => x.Count <= 10)
            .WithMessage("A maximum of 10 images is allowed.");
        RuleForEach(x => x.ImageUrls)
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("Each image URL must be a valid absolute URL.");
    }
}
