using Bookstore.Application.DTOs;
using FluentValidation;

namespace Bookstore.Application.Validators;

public class CreateProductAuthorLinkDtoValidator : AbstractValidator<CreateProductAuthorLinkDto>
{
    public CreateProductAuthorLinkDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.AuthorId).NotEmpty().MaximumLength(128);
    }
}

public class UpdateProductAuthorLinkDtoValidator : AbstractValidator<UpdateProductAuthorLinkDto>
{
    public UpdateProductAuthorLinkDtoValidator()
    {
        RuleFor(x => x.AuthorId).NotEmpty().MaximumLength(128);
    }
}
