using CatalogCloud.Domain.Entities;
using CatalogCloud.Domain.Interfaces;
using CatalogCloud.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CatalogCloud.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly IMongoCollection<Author> _authors;

    public AuthorRepository(IMongoDatabase database, IOptions<MongoDbOptions> options)
    {
        _authors = database.GetCollection<Author>(options.Value.AuthorsCollectionName);
    }

    public async Task<Author?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _authors.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Author>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _authors.Find(_ => true)
            .SortBy(x => x.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Author author, CancellationToken cancellationToken = default)
    {
        await _authors.InsertOneAsync(author, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Author author, CancellationToken cancellationToken = default)
    {
        await _authors.ReplaceOneAsync(x => x.Id == author.Id, author, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _authors.DeleteOneAsync(x => x.Id == id, cancellationToken);
    }
}
