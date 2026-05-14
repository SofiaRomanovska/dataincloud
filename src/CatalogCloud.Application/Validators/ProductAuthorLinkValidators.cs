using CatalogCloud.Application.DTOs;
using CatalogCloud.Domain.Entities;
using FluentValidation;

namespace CatalogCloud.Application.Validators;

public class CreateProductAuthorLinkDtoValidator : AbstractValidator<CreateProductAuthorLinkDto>
{
    public CreateProductAuthorLinkDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.AuthorId).NotEmpty().MaximumLength(ProductAuthorLink.MaxAuthorIdLength);
    }
}

public class UpdateProductAuthorLinkDtoValidator : AbstractValidator<UpdateProductAuthorLinkDto>
{
    public UpdateProductAuthorLinkDtoValidator()
    {
        RuleFor(x => x.AuthorId).NotEmpty().MaximumLength(ProductAuthorLink.MaxAuthorIdLength);
    }
}
