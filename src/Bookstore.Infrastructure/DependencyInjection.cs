using Bookstore.Domain.Interfaces;
using Bookstore.Infrastructure.Data;
using Bookstore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Bookstore.Infrastructure;

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

        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddSingleton<IProductAuthorLinkRepository, BlobProductAuthorLinkRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
