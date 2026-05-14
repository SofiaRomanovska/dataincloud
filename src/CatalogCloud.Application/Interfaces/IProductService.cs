using CatalogCloud.Application.DTOs;

namespace CatalogCloud.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
