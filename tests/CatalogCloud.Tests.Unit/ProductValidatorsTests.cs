using CatalogCloud.Application.DTOs;
using CatalogCloud.Application.Validators;
using FluentValidation.TestHelper;

namespace CatalogCloud.Tests.Unit;

public class ProductValidatorsTests
{
    private readonly CreateProductDtoValidator _createValidator;
    private readonly UpdateProductDtoValidator _updateValidator;

    public ProductValidatorsTests()
    {
        _createValidator = new CreateProductDtoValidator();
        _updateValidator = new UpdateProductDtoValidator();
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenNameIsEmpty()
    {
        var dto = new CreateProductDto { Name = string.Empty };
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenNameExceedsMaxLength()
    {
        var dto = new CreateProductDto { Name = new string('A', 201) };
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateValidator_ShouldNotHaveError_WhenNameIsAtMaxLength()
    {
        var dto = new CreateProductDto { Name = new string('A', 200), Description = "Valid", Price = 10, QuantityInStock = 1 };
        var result = _createValidator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenPriceIsZero()
    {
        var dto = new CreateProductDto { Price = 0 };
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void CreateValidator_ShouldNotHaveError_WhenPriceIsGreaterThanZero()
    {
        var dto = new CreateProductDto { Name = "Name", Description = "Valid", Price = 0.01m, QuantityInStock = 1 };
        var result = _createValidator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenQuantityInStockIsNegative()
    {
        var dto = new CreateProductDto { QuantityInStock = -1 };
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.QuantityInStock);
    }

    [Fact]
    public void CreateValidator_ShouldNotHaveError_WhenQuantityInStockIsZero()
    {
        var dto = new CreateProductDto { Name = "Name", Description = "Valid", Price = 10, QuantityInStock = 0 };
        var result = _createValidator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.QuantityInStock);
    }

    [Fact]
    public void UpdateValidator_ShouldHaveError_WhenDescriptionIsEmpty()
    {
        var dto = new UpdateProductDto { Description = string.Empty };
        var result = _updateValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void UpdateValidator_ShouldHaveError_WhenDescriptionExceedsMaxLength()
    {
        var dto = new UpdateProductDto { Description = new string('A', 501) };
        var result = _updateValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
