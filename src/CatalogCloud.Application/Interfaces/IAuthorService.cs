using CatalogCloud.Application.DTOs;

namespace CatalogCloud.Application.Interfaces;

public interface IAuthorService
{
    Task<AuthorDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuthorDto> CreateAsync(CreateAuthorDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(string id, UpdateAuthorDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
