using System.Collections.Concurrent;
using CatalogCloud.API.Contracts;
using CatalogCloud.Application.Enums;
using CatalogCloud.Application.Messaging;

namespace CatalogCloud.API.Services;

public class InMemoryEntityChangeStatisticsStore
{
    private readonly ConcurrentDictionary<StatisticKey, int> _statistics = new();

    public InMemoryEntityChangeStatisticsStore()
    {
        StartedAtUtc = DateTime.UtcNow;
    }

    public DateTime StartedAtUtc { get; }

    public Task RecordAsync(EntityChangeMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = new StatisticKey(message.EntityType, message.Operation);
        _statistics.AddOrUpdate(key, 1, static (_, count) => count + 1);

        return Task.CompletedTask;
    }

    public EntityChangeStatisticsResponse Get(CatalogEntityType? entityType, EntityChangeOperation? operation)
    {
        var items = _statistics
            .Where(x => (!entityType.HasValue || x.Key.EntityType == entityType.Value)
                && (!operation.HasValue || x.Key.Operation == operation.Value))
            .OrderBy(x => x.Key.EntityType)
            .ThenBy(x => x.Key.Operation)
            .Select(x => new EntityChangeStatisticItemResponse
            {
                EntityType = x.Key.EntityType.ToString(),
                Operation = x.Key.Operation.ToString(),
                Count = x.Value
            })
            .ToArray();

        return new EntityChangeStatisticsResponse
        {
            StartedAtUtc = StartedAtUtc,
            Items = items
        };
    }

    private readonly record struct StatisticKey(CatalogEntityType EntityType, EntityChangeOperation Operation);
}
