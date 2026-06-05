namespace CatalogCloud.API.Contracts;

public class EntityChangeStatisticsResponse
{
    public DateTime StartedAtUtc { get; init; }

    public IReadOnlyCollection<EntityChangeStatisticItemResponse> Items { get; init; } = Array.Empty<EntityChangeStatisticItemResponse>();
}

public class EntityChangeStatisticItemResponse
{
    public string EntityType { get; init; } = string.Empty;

    public string Operation { get; init; } = string.Empty;

    public int Count { get; init; }
}
