using CatalogCloud.Application.DTOs;
using CatalogCloud.Application.Services;
using CatalogCloud.Domain.Entities;
using CatalogCloud.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CatalogCloud.Tests.Unit;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _sut = new ProductService(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProductDto_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product("Test Product", "Description", 10, 5);
        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        _productRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        Func<Task> act = async () => await _sut.GetByIdAsync(Guid.Empty);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("id");
    }

    [Fact]
    public async Task GetPagedAsync_ShouldThrowArgumentOutOfRangeException_WhenPageIsInvalid()
    {
        Func<Task> act = async () => await _sut.GetPagedAsync(0, 10);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("page");
    }

    [Fact]
    public async Task GetPagedAsync_ShouldThrowArgumentOutOfRangeException_WhenPageSizeIsInvalid()
    {
        Func<Task> act = async () => await _sut.GetPagedAsync(1, 0);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("pageSize");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedProductDto()
    {
        // Arrange
        var dto = new CreateProductDto { Name = "New Product", Description = "Description", Price = 10, QuantityInStock = 5 };

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateProductDto { Name = "Updated", Description = "Description", Price = 10, QuantityInStock = 1 };
        _productRepositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.UpdateAsync(id, dto);

        // Assert
        result.Should().BeFalse();
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        var dto = new UpdateProductDto { Name = "Updated", Description = "Description", Price = 10, QuantityInStock = 1 };

        Func<Task> act = async () => await _sut.UpdateAsync(Guid.Empty, dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("id");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenProductIsUpdated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var product = new Product("Old Name", "Description", 10, 5);
        var dto = new UpdateProductDto { Name = "New Name", Description = "Description", Price = 20, QuantityInStock = 12 };
        
        _productRepositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.UpdateAsync(id, dto);

        // Assert
        result.Should().BeTrue();
        product.Name.Should().Be(dto.Name);
        product.QuantityInStock.Should().Be(dto.QuantityInStock);
        _productRepositoryMock.Verify(x => x.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        Func<Task> act = async () => await _sut.DeleteAsync(Guid.Empty);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("id");
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenProductExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var product = new Product("Product", "Description", 10, 5);
        
        _productRepositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
        product.IsDeleted.Should().BeTrue();
        _productRepositoryMock.Verify(x => x.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }
}
