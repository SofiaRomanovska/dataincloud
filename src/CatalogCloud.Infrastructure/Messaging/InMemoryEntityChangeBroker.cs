using System.Collections.Concurrent;
using CatalogCloud.Application.Interfaces;
using CatalogCloud.Application.Messaging;

namespace CatalogCloud.Infrastructure.Messaging;

public class InMemoryEntityChangeBroker : IEntityChangePublisher, IEntityChangeSubscriber
{
    private readonly ConcurrentDictionary<Guid, Func<EntityChangeMessage, CancellationToken, Task>> _handlers = new();

    public async Task PublishAsync(EntityChangeMessage message, CancellationToken cancellationToken = default)
    {
        foreach (var handler in _handlers.Values.ToArray())
        {
            await handler(message, cancellationToken);
        }
    }

    public IDisposable Subscribe(Func<EntityChangeMessage, CancellationToken, Task> handler)
    {
        var subscriptionId = Guid.NewGuid();
        _handlers[subscriptionId] = handler;

        return new Subscription(_handlers, subscriptionId);
    }

    private sealed class Subscription : IDisposable
    {
        private readonly ConcurrentDictionary<Guid, Func<EntityChangeMessage, CancellationToken, Task>> _handlers;
        private readonly Guid _subscriptionId;
        private bool _disposed;

        public Subscription(
            ConcurrentDictionary<Guid, Func<EntityChangeMessage, CancellationToken, Task>> handlers,
            Guid subscriptionId)
        {
            _handlers = handlers;
            _subscriptionId = subscriptionId;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _handlers.TryRemove(_subscriptionId, out _);
            _disposed = true;
        }
    }
}
