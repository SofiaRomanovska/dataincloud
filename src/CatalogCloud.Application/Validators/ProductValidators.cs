using CatalogCloud.Application.DTOs;
using CatalogCloud.Domain.Entities;
using FluentValidation;

namespace CatalogCloud.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Product.MaxNameLength);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(Product.MaxDescriptionLength);
        RuleFor(x => x.Price).GreaterThan(0).LessThanOrEqualTo(Product.MaxPrice);
        RuleFor(x => x.QuantityInStock).GreaterThanOrEqualTo(0).LessThanOrEqualTo(Product.MaxQuantityInStock);
    }
}

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Product.MaxNameLength);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(Product.MaxDescriptionLength);
        RuleFor(x => x.Price).GreaterThan(0).LessThanOrEqualTo(Product.MaxPrice);
        RuleFor(x => x.QuantityInStock).GreaterThanOrEqualTo(0).LessThanOrEqualTo(Product.MaxQuantityInStock);
    }
}
