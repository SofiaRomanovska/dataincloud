using Bookstore.Application.DTOs;
using Bookstore.Application.Validators;
using FluentValidation.TestHelper;

namespace Bookstore.Tests.Unit;

public class ProductAuthorLinkValidatorsTests
{
    private readonly CreateProductAuthorLinkDtoValidator _createValidator;
    private readonly UpdateProductAuthorLinkDtoValidator _updateValidator;

    public ProductAuthorLinkValidatorsTests()
    {
        _createValidator = new CreateProductAuthorLinkDtoValidator();
        _updateValidator = new UpdateProductAuthorLinkDtoValidator();
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenProductIdIsEmpty()
    {
        var dto = new CreateProductAuthorLinkDto { ProductId = Guid.Empty, AuthorId = "author-id" };

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenAuthorIdIsEmpty()
    {
        var dto = new CreateProductAuthorLinkDto { ProductId = Guid.NewGuid(), AuthorId = string.Empty };

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.AuthorId);
    }

    [Fact]
    public void CreateValidator_ShouldNotHaveErrors_WhenPayloadIsValid()
    {
        var dto = new CreateProductAuthorLinkDto { ProductId = Guid.NewGuid(), AuthorId = "author-id" };

        var result = _createValidator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateValidator_ShouldHaveError_WhenAuthorIdIsEmpty()
    {
        var dto = new UpdateProductAuthorLinkDto { AuthorId = string.Empty };

        var result = _updateValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.AuthorId);
    }
}
