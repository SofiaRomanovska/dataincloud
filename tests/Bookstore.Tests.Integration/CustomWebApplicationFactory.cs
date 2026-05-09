using Bookstore.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

namespace Bookstore.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer;
    private readonly MongoDbContainer _mongoContainer;
    private readonly string _blobContainerPath;

    public CustomWebApplicationFactory()
    {
        _dbContainer = new MsSqlBuilder()
            .Build();
        _mongoContainer = new MongoDbBuilder()
            .Build();
        _blobContainerPath = Path.Combine(Path.GetTempPath(), $"bookstore-blob-tests-{Guid.NewGuid():N}");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MongoDb:ConnectionString"] = _mongoContainer.GetConnectionString(),
                ["MongoDb:DatabaseName"] = $"BookstoreAuthorsTests_{Guid.NewGuid():N}",
                ["MongoDb:AuthorsCollectionName"] = "authors",
                ["BlobCache:ContainerPath"] = _blobContainerPath,
                ["BlobCache:ProductAuthorLinksBlobName"] = "product-author-links.json"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_dbContainer.GetConnectionString());
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _mongoContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await _mongoContainer.DisposeAsync();

        if (Directory.Exists(_blobContainerPath))
        {
            Directory.Delete(_blobContainerPath, recursive: true);
        }
    }
}
