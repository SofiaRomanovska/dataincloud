using CatalogCloud.Application.DTOs;
using CatalogCloud.Application.Enums;
using CatalogCloud.Application.Interfaces;
using CatalogCloud.Application.Messaging;
using CatalogCloud.Domain.Entities;
using CatalogCloud.Domain.Interfaces;

namespace CatalogCloud.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _repository;
    private readonly IEntityChangePublisher _entityChangePublisher;

    public AuthorService(IAuthorRepository repository, IEntityChangePublisher entityChangePublisher)
    {
        _repository = repository;
        _entityChangePublisher = entityChangePublisher;
    }

    public async Task<AuthorDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Author id cannot be empty.", nameof(id));
        }

        var author = await _repository.GetByIdAsync(id, cancellationToken);
        if (author == null) return null;

        return MapToDto(author);
    }

    public async Task<IEnumerable<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var authors = await _repository.GetAllAsync(cancellationToken);
        return authors.Select(MapToDto);
    }

    public async Task<AuthorDto> CreateAsync(CreateAuthorDto dto, CancellationToken cancellationToken = default)
    {
        var author = new Author(
            dto.FullName,
            dto.Biography,
            dto.BirthDate,
            dto.PublishedBooksCount,
            dto.IsActive!.Value);

        await _repository.AddAsync(author, cancellationToken);
        await PublishChangeAsync(author.Id, EntityChangeOperation.Created, cancellationToken);

        return MapToDto(author);
    }

    public async Task<bool> UpdateAsync(string id, UpdateAuthorDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Author id cannot be empty.", nameof(id));
        }

        var author = await _repository.GetByIdAsync(id, cancellationToken);
        if (author == null) return false;

        author.UpdateDetails(
            dto.FullName,
            dto.Biography,
            dto.BirthDate,
            dto.PublishedBooksCount,
            dto.IsActive!.Value);

        await _repository.UpdateAsync(author, cancellationToken);
        await PublishChangeAsync(author.Id, EntityChangeOperation.Updated, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Author id cannot be empty.", nameof(id));
        }

        var author = await _repository.GetByIdAsync(id, cancellationToken);
        if (author == null) return false;

        await _repository.DeleteAsync(id, cancellationToken);
        return true;
    }

    private static AuthorDto MapToDto(Author author)
    {
        return new AuthorDto
        {
            Id = author.Id,
            FullName = author.FullName,
            Biography = author.Biography,
            BirthDate = author.BirthDate,
            PublishedBooksCount = author.PublishedBooksCount,
            IsActive = author.IsActive,
            CreatedAt = author.CreatedAt
        };
    }

    private Task PublishChangeAsync(string entityId, EntityChangeOperation operation, CancellationToken cancellationToken)
    {
        return _entityChangePublisher.PublishAsync(
            new EntityChangeMessage(CatalogEntityType.Author, operation, entityId, DateTime.UtcNow),
            cancellationToken);
    }
}
