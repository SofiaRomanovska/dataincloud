using CatalogCloud.Application.DTOs;
using FluentValidation;

namespace CatalogCloud.Application.Validators;

public class CreateAuthorDtoValidator : AbstractValidator<CreateAuthorDto>
{
    public CreateAuthorDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Biography).NotEmpty().MinimumLength(20).MaximumLength(2_000);
        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .LessThan(DateTime.UtcNow.Date)
            .GreaterThan(new DateTime(1800, 1, 1));
        RuleFor(x => x.PublishedBooksCount).GreaterThanOrEqualTo(0).LessThanOrEqualTo(10_000);
        RuleFor(x => x.IsActive).NotNull();
    }
}

public class UpdateAuthorDtoValidator : AbstractValidator<UpdateAuthorDto>
{
    public UpdateAuthorDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Biography).NotEmpty().MinimumLength(20).MaximumLength(2_000);
        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .LessThan(DateTime.UtcNow.Date)
            .GreaterThan(new DateTime(1800, 1, 1));
        RuleFor(x => x.PublishedBooksCount).GreaterThanOrEqualTo(0).LessThanOrEqualTo(10_000);
        RuleFor(x => x.IsActive).NotNull();
    }
}
