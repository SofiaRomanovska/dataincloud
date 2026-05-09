using CatalogCloud.Application.DTOs;
using CatalogCloud.Application.Services;
using CatalogCloud.Domain.Entities;
using CatalogCloud.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CatalogCloud.Tests.Unit;

public class ProductAuthorLinkServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly Mock<IProductAuthorLinkRepository> _linkRepositoryMock;
    private readonly ProductAuthorLinkService _sut;

    public ProductAuthorLinkServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _linkRepositoryMock = new Mock<IProductAuthorLinkRepository>();
        _sut = new ProductAuthorLinkService(
            _productRepositoryMock.Object,
            _authorRepositoryMock.Object,
            _linkRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenProductAndAuthorExist()
    {
        var productId = Guid.NewGuid();
        var authorId = "author-id";
        SetupExistingProduct(productId);
        SetupExistingAuthor(authorId);

        var result = await _sut.CreateAsync(new CreateProductAuthorLinkDto { ProductId = productId, AuthorId = authorId });

        result.Status.Should().Be(ProductAuthorLinkStatus.Success);
        result.Link.Should().NotBeNull();
        result.Link!.ProductId.Should().Be(productId);
        result.Link.AuthorId.Should().Be(authorId);
        _linkRepositoryMock.Verify(x => x.UpsertAsync(It.IsAny<ProductAuthorLink>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnProductNotFound_WhenProductDoesNotExist()
    {
        var result = await _sut.CreateAsync(new CreateProductAuthorLinkDto
        {
            ProductId = Guid.NewGuid(),
            AuthorId = "author-id"
        });

        result.Status.Should().Be(ProductAuthorLinkStatus.ProductNotFound);
        _authorRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _linkRepositoryMock.Verify(x => x.UpsertAsync(It.IsAny<ProductAuthorLink>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnAuthorNotFound_WhenAuthorDoesNotExist()
    {
        var productId = Guid.NewGuid();
        SetupExistingProduct(productId);

        var result = await _sut.CreateAsync(new CreateProductAuthorLinkDto { ProductId = productId, AuthorId = "missing" });

        result.Status.Should().Be(ProductAuthorLinkStatus.AuthorNotFound);
        _linkRepositoryMock.Verify(x => x.UpsertAsync(It.IsAny<ProductAuthorLink>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentException_WhenProductIdIsEmpty()
    {
        Func<Task> act = async () => await _sut.CreateAsync(new CreateProductAuthorLinkDto
        {
            ProductId = Guid.Empty,
            AuthorId = "author-id"
        });

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnLinkNotFound_WhenLinkDoesNotExist()
    {
        var productId = Guid.NewGuid();
        var authorId = "author-id";
        SetupExistingProduct(productId);
        SetupExistingAuthor(authorId);

        var result = await _sut.UpdateAsync(productId, new UpdateProductAuthorLinkDto { AuthorId = authorId });

        result.Status.Should().Be(ProductAuthorLinkStatus.LinkNotFound);
        _linkRepositoryMock.Verify(x => x.UpsertAsync(It.IsAny<ProductAuthorLink>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccess_WhenLinkAndEntitiesExist()
    {
        var productId = Guid.NewGuid();
        var authorId = "new-author-id";
        _linkRepositoryMock.Setup(x => x.GetByProductIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductAuthorLink { ProductId = productId, AuthorId = "old-author-id" });
        SetupExistingProduct(productId);
        SetupExistingAuthor(authorId);

        var result = await _sut.UpdateAsync(productId, new UpdateProductAuthorLinkDto { AuthorId = authorId });

        result.Status.Should().Be(ProductAuthorLinkStatus.Success);
        result.Link!.AuthorId.Should().Be(authorId);
        _linkRepositoryMock.Verify(x => x.UpsertAsync(It.Is<ProductAuthorLink>(l => l.AuthorId == authorId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnAuthorNotFound_WhenNewAuthorDoesNotExist()
    {
        var productId = Guid.NewGuid();
        _linkRepositoryMock.Setup(x => x.GetByProductIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductAuthorLink { ProductId = productId, AuthorId = "old-author-id" });
        SetupExistingProduct(productId);

        var result = await _sut.UpdateAsync(productId, new UpdateProductAuthorLinkDto { AuthorId = "missing" });

        result.Status.Should().Be(ProductAuthorLinkStatus.AuthorNotFound);
        _linkRepositoryMock.Verify(x => x.UpsertAsync(It.IsAny<ProductAuthorLink>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnLinkNotFound_WhenLinkDoesNotExist()
    {
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        result.Status.Should().Be(ProductAuthorLinkStatus.LinkNotFound);
        _linkRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenLinkExists()
    {
        var productId = Guid.NewGuid();
        _linkRepositoryMock.Setup(x => x.GetByProductIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductAuthorLink { ProductId = productId, AuthorId = "author-id" });

        var result = await _sut.DeleteAsync(productId);

        result.Status.Should().Be(ProductAuthorLinkStatus.Success);
        _linkRepositoryMock.Verify(x => x.DeleteAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentException_WhenProductIdIsEmpty()
    {
        Func<Task> act = async () => await _sut.DeleteAsync(Guid.Empty);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("productId");
    }

    private void SetupExistingProduct(Guid productId)
    {
        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Id = productId, Name = "Product" });
    }

    private void SetupExistingAuthor(string authorId)
    {
        _authorRepositoryMock.Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Author { Id = authorId, FullName = "Author" });
    }
}
