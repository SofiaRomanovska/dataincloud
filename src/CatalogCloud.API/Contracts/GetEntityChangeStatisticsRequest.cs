using CatalogCloud.Application.Enums;

namespace CatalogCloud.API.Contracts;

public class GetEntityChangeStatisticsRequest
{
    public CatalogEntityType? EntityType { get; init; }

    public EntityChangeOperation? Operation { get; init; }
}
