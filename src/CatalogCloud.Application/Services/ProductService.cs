using CatalogCloud.Application.DTOs;
using CatalogCloud.Application.Interfaces;
using CatalogCloud.Domain.Entities;
using CatalogCloud.Domain.Interfaces;

namespace CatalogCloud.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Product id cannot be empty.", nameof(id));
        }

        var product = await _repository.GetByIdAsync(id, cancellationToken);
        if (product == null) return null;

        return MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), page, "Page must be greater than zero.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "Page size must be greater than zero.");
        }

        var products = await _repository.GetPagedAsync(page, pageSize, cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = new Product(dto.Name, dto.Description, dto.Price, dto.QuantityInStock);

        await _repository.AddAsync(product, cancellationToken);

        return MapToDto(product);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Product id cannot be empty.", nameof(id));
        }

        var product = await _repository.GetByIdAsync(id, cancellationToken);
        if (product == null) return false;

        product.UpdateDetails(dto.Name, dto.Description, dto.Price, dto.QuantityInStock);

        await _repository.UpdateAsync(product, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Product id cannot be empty.", nameof(id));
        }

        var product = await _repository.GetByIdAsync(id, cancellationToken);
        if (product == null) return false;

        product.Delete();
        await _repository.UpdateAsync(product, cancellationToken);
        return true;
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            QuantityInStock = product.QuantityInStock,
            CreatedAt = product.CreatedAt
        };
    }
}
