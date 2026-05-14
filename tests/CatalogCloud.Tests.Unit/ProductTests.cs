using CatalogCloud.Domain.Entities;
using FluentAssertions;

namespace CatalogCloud.Tests.Unit;

public class ProductTests
{
    [Fact]
    public void Constructor_ShouldCreateActiveProduct_WhenValuesAreValid()
    {
        var product = new Product("Product", "Description", 10, 5);

        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be("Product");
        product.Description.Should().Be("Description");
        product.Price.Should().Be(10);
        product.QuantityInStock.Should().Be(5);
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        product.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void UpdateDetails_ShouldChangeProductDetails_WhenValuesAreValid()
    {
        var product = new Product("Product", "Description", 10, 5);

        product.UpdateDetails("Updated", "Updated description", 20, 7);

        product.Name.Should().Be("Updated");
        product.Description.Should().Be("Updated description");
        product.Price.Should().Be(20);
        product.QuantityInStock.Should().Be(7);
    }

    [Theory]
    [InlineData("", "Description", 10, 5)]
    [InlineData("Product", "", 10, 5)]
    public void Constructor_ShouldRejectMissingRequiredText(string name, string description, decimal price, int quantityInStock)
    {
        var act = () => new Product(name, description, price, quantityInStock);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_ShouldRejectInvalidPrice(decimal price)
    {
        var act = () => new Product("Product", "Description", price, 5);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("price");
    }

    [Fact]
    public void Constructor_ShouldRejectNegativeQuantity()
    {
        var act = () => new Product("Product", "Description", 10, -1);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("quantityInStock");
    }

    [Fact]
    public void Delete_ShouldMarkProductAsDeleted()
    {
        var product = new Product("Product", "Description", 10, 5);

        product.Delete();

        product.IsDeleted.Should().BeTrue();
    }
}
