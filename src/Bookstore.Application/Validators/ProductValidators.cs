using Bookstore.Application.DTOs;
using FluentValidation;

namespace Bookstore.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Price).GreaterThan(0).LessThanOrEqualTo(1_000_000);
        RuleFor(x => x.QuantityInStock).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100_000);
    }
}

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Price).GreaterThan(0).LessThanOrEqualTo(1_000_000);
        RuleFor(x => x.QuantityInStock).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100_000);
    }
}
