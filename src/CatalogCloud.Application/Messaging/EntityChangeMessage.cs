using CatalogCloud.Application.Enums;

namespace CatalogCloud.Application.Messaging;

public sealed record EntityChangeMessage(
    CatalogEntityType EntityType,
    EntityChangeOperation Operation,
    string EntityId,
    DateTime OccurredAtUtc);
