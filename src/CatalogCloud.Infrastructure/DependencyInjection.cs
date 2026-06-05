using CatalogCloud.Domain.Interfaces;
using CatalogCloud.Infrastructure.Data;
using CatalogCloud.Infrastructure.Messaging;
using CatalogCloud.Infrastructure.Repositories;
using CatalogCloud.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CatalogCloud.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<MongoDbOptions>(configuration.GetSection(MongoDbOptions.SectionName));
        services.Configure<BlobCacheOptions>(configuration.GetSection(BlobCacheOptions.SectionName));
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });
        services.AddScoped(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.DatabaseName);
        });

        services.AddSingleton<InMemoryEntityChangeBroker>();
        services.AddSingleton<IEntityChangePublisher>(serviceProvider =>
            serviceProvider.GetRequiredService<InMemoryEntityChangeBroker>());
        services.AddSingleton<IEntityChangeSubscriber>(serviceProvider =>
            serviceProvider.GetRequiredService<InMemoryEntityChangeBroker>());

        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddSingleton<IProductAuthorLinkRepository, BlobProductAuthorLinkRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }

    public static async Task ApplyInfrastructureMigrationsAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
