using CatalogCloud.Application.Messaging;

namespace CatalogCloud.Application.Interfaces;

public interface IEntityChangePublisher
{
    Task PublishAsync(EntityChangeMessage message, CancellationToken cancellationToken = default);
}
