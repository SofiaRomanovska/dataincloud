using CatalogCloud.Domain.Entities;

namespace CatalogCloud.Domain.Interfaces;

public interface IProductAuthorLinkRepository
{
    Task<ProductAuthorLink?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductAuthorLink>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpsertAsync(ProductAuthorLink link, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid productId, CancellationToken cancellationToken = default);
}
