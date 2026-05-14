using CatalogCloud.Application.DTOs;
using CatalogCloud.Domain.Entities;
using FluentValidation;

namespace CatalogCloud.Application.Validators;

public class CreateAuthorDtoValidator : AbstractValidator<CreateAuthorDto>
{
    public CreateAuthorDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(Author.MaxFullNameLength);
        RuleFor(x => x.Biography).NotEmpty().MinimumLength(Author.MinBiographyLength).MaximumLength(Author.MaxBiographyLength);
        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .LessThan(DateTime.UtcNow.Date)
            .GreaterThan(Author.MinimumBirthDate);
        RuleFor(x => x.PublishedBooksCount).GreaterThanOrEqualTo(0).LessThanOrEqualTo(Author.MaxPublishedBooksCount);
        RuleFor(x => x.IsActive).NotNull();
    }
}

public class UpdateAuthorDtoValidator : AbstractValidator<UpdateAuthorDto>
{
    public UpdateAuthorDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(Author.MaxFullNameLength);
        RuleFor(x => x.Biography).NotEmpty().MinimumLength(Author.MinBiographyLength).MaximumLength(Author.MaxBiographyLength);
        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .LessThan(DateTime.UtcNow.Date)
            .GreaterThan(Author.MinimumBirthDate);
        RuleFor(x => x.PublishedBooksCount).GreaterThanOrEqualTo(0).LessThanOrEqualTo(Author.MaxPublishedBooksCount);
        RuleFor(x => x.IsActive).NotNull();
    }
}
