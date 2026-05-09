using Bookstore.Application.DTOs;
using Bookstore.Application.Interfaces;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Interfaces;

namespace Bookstore.Application.Services;

public class ProductAuthorLinkService : IProductAuthorLinkService
{
    private readonly IProductRepository _productRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IProductAuthorLinkRepository _linkRepository;

    public ProductAuthorLinkService(
        IProductRepository productRepository,
        IAuthorRepository authorRepository,
        IProductAuthorLinkRepository linkRepository)
    {
        _productRepository = productRepository;
        _authorRepository = authorRepository;
        _linkRepository = linkRepository;
    }

    public async Task<ProductAuthorLinkResult> CreateAsync(CreateProductAuthorLinkDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.ProductId == Guid.Empty)
        {
            throw new ArgumentException("Product id cannot be empty.", nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(dto.AuthorId))
        {
            throw new ArgumentException("Author id cannot be empty.", nameof(dto));
        }

        var existenceStatus = await CheckEntitiesExistAsync(dto.ProductId, dto.AuthorId, cancellationToken);
        if (existenceStatus != ProductAuthorLinkStatus.Success)
        {
            return ProductAuthorLinkResult.Failure(existenceStatus);
        }

        var link = new ProductAuthorLink
        {
            ProductId = dto.ProductId,
            AuthorId = dto.AuthorId,
            CachedAt = DateTime.UtcNow
        };

        await _linkRepository.UpsertAsync(link, cancellationToken);
        return ProductAuthorLinkResult.Success(MapToDto(link));
    }

    public async Task<ProductAuthorLinkResult> UpdateAsync(Guid productId, UpdateProductAuthorLinkDto dto, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product id cannot be empty.", nameof(productId));
        }

        if (string.IsNullOrWhiteSpace(dto.AuthorId))
        {
            throw new ArgumentException("Author id cannot be empty.", nameof(dto));
        }

        var existenceStatus = await CheckEntitiesExistAsync(productId, dto.AuthorId, cancellationToken);
        if (existenceStatus != ProductAuthorLinkStatus.Success)
        {
            return ProductAuthorLinkResult.Failure(existenceStatus);
        }

        var existingLink = await _linkRepository.GetByProductIdAsync(productId, cancellationToken);
        if (existingLink == null)
        {
            return ProductAuthorLinkResult.Failure(ProductAuthorLinkStatus.LinkNotFound);
        }

        existingLink.AuthorId = dto.AuthorId;
        existingLink.CachedAt = DateTime.UtcNow;

        await _linkRepository.UpsertAsync(existingLink, cancellationToken);
        return ProductAuthorLinkResult.Success(MapToDto(existingLink));
    }

    public async Task<ProductAuthorLinkResult> DeleteAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product id cannot be empty.", nameof(productId));
        }

        var existingLink = await _linkRepository.GetByProductIdAsync(productId, cancellationToken);
        if (existingLink == null)
        {
            return ProductAuthorLinkResult.Failure(ProductAuthorLinkStatus.LinkNotFound);
        }

        await _linkRepository.DeleteAsync(productId, cancellationToken);
        return ProductAuthorLinkResult.Success();
    }

    private async Task<ProductAuthorLinkStatus> CheckEntitiesExistAsync(
        Guid productId,
        string authorId,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
        {
            return ProductAuthorLinkStatus.ProductNotFound;
        }

        var author = await _authorRepository.GetByIdAsync(authorId, cancellationToken);
        if (author == null)
        {
            return ProductAuthorLinkStatus.AuthorNotFound;
        }

        return ProductAuthorLinkStatus.Success;
    }

    private static ProductAuthorLinkDto MapToDto(ProductAuthorLink link)
    {
        return new ProductAuthorLinkDto
        {
            ProductId = link.ProductId,
            AuthorId = link.AuthorId,
            CachedAt = link.CachedAt
        };
    }
}
