using CatalogCloud.Application.Interfaces;

namespace CatalogCloud.API.Services;

public class EntityChangeStatisticsSubscriber : IHostedService
{
    private readonly IEntityChangeSubscriber _subscriber;
    private readonly InMemoryEntityChangeStatisticsStore _statisticsStore;
    private IDisposable? _subscription;

    public EntityChangeStatisticsSubscriber(
        IEntityChangeSubscriber subscriber,
        InMemoryEntityChangeStatisticsStore statisticsStore)
    {
        _subscriber = subscriber;
        _statisticsStore = statisticsStore;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _subscription = _subscriber.Subscribe(_statisticsStore.RecordAsync);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription?.Dispose();
        _subscription = null;
        return Task.CompletedTask;
    }
}
