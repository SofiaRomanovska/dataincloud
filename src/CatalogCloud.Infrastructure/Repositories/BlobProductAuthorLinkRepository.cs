using System.Text.Json;
using CatalogCloud.Domain.Entities;
using CatalogCloud.Domain.Interfaces;
using CatalogCloud.Infrastructure.Data;
using Microsoft.Extensions.Options;

namespace CatalogCloud.Infrastructure.Repositories;

public class BlobProductAuthorLinkRepository : IProductAuthorLinkRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly BlobCacheOptions _options;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public BlobProductAuthorLinkRepository(IOptions<BlobCacheOptions> options)
    {
        _options = options.Value;
    }

    public async Task<ProductAuthorLink?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var links = await ReadAllInternalAsync(cancellationToken);
        return links.FirstOrDefault(x => x.ProductId == productId);
    }

    public async Task<IEnumerable<ProductAuthorLink>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await ReadAllInternalAsync(cancellationToken);
    }

    public async Task UpsertAsync(ProductAuthorLink link, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var links = await ReadAllUnlockedAsync(cancellationToken);
            links.RemoveAll(x => x.ProductId == link.ProductId);
            links.Add(link);
            await WriteAllUnlockedAsync(links, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task DeleteAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var links = await ReadAllUnlockedAsync(cancellationToken);
            links.RemoveAll(x => x.ProductId == productId);
            await WriteAllUnlockedAsync(links, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<List<ProductAuthorLink>> ReadAllInternalAsync(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            return await ReadAllUnlockedAsync(cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<List<ProductAuthorLink>> ReadAllUnlockedAsync(CancellationToken cancellationToken)
    {
        var blobPath = GetBlobPath();
        if (!File.Exists(blobPath))
        {
            return new List<ProductAuthorLink>();
        }

        await using var stream = File.OpenRead(blobPath);
        var links = await JsonSerializer.DeserializeAsync<List<ProductAuthorLink>>(stream, JsonOptions, cancellationToken);
        return links ?? new List<ProductAuthorLink>();
    }

    private async Task WriteAllUnlockedAsync(List<ProductAuthorLink> links, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_options.ContainerPath);

        var blobPath = GetBlobPath();
        var tempPath = $"{blobPath}.{Guid.NewGuid():N}.tmp";

        await using (var stream = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(stream, links.OrderBy(x => x.ProductId), JsonOptions, cancellationToken);
        }

        File.Move(tempPath, blobPath, true);
    }

    private string GetBlobPath()
    {
        return Path.Combine(_options.ContainerPath, _options.ProductAuthorLinksBlobName);
    }
}
