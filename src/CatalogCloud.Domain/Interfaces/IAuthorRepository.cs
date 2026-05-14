using CatalogCloud.Domain.Entities;

namespace CatalogCloud.Domain.Interfaces;

public interface IAuthorRepository
{
    Task<Author?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Author>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Author author, CancellationToken cancellationToken = default);
    Task UpdateAsync(Author author, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
