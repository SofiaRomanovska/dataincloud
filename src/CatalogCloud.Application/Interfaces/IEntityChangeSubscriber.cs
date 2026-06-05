using CatalogCloud.Application.Messaging;

namespace CatalogCloud.Application.Interfaces;

public interface IEntityChangeSubscriber
{
    IDisposable Subscribe(Func<EntityChangeMessage, CancellationToken, Task> handler);
}
