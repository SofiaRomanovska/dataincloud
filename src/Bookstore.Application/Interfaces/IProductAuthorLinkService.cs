using Bookstore.Application.DTOs;

namespace Bookstore.Application.Interfaces;

public interface IProductAuthorLinkService
{
    Task<ProductAuthorLinkResult> CreateAsync(CreateProductAuthorLinkDto dto, CancellationToken cancellationToken = default);
    Task<ProductAuthorLinkResult> UpdateAsync(Guid productId, UpdateProductAuthorLinkDto dto, CancellationToken cancellationToken = default);
    Task<ProductAuthorLinkResult> DeleteAsync(Guid productId, CancellationToken cancellationToken = default);
}
