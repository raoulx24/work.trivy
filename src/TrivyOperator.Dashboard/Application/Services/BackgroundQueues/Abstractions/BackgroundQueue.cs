using k8s;
using k8s.Models;
using Microsoft.Extensions.Options;
using System.Threading.Channels;
using TrivyOperator.Dashboard.Application.Services.Options;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;

public class BackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject> :
    IBackgroundQueue<TKubernetesObject>
        where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>
{
    private readonly Channel<TKubernetesWatcherEvent> queue;
    protected ILogger<BackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>> logger { get; init; }
    private IOptions<BackgroudQueueOptions> options { get; init; }

    public BackgroundQueue(IOptions<BackgroudQueueOptions> options, ILogger<BackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>> logger)
    {
        this.options = options;
        this.logger = logger;
        BoundedChannelOptions boundedChannelOptions = new(options.Value.Capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        queue = Channel.CreateBounded<TKubernetesWatcherEvent>(boundedChannelOptions);
        logger.LogDebug("Started BackgoundQueue for {kubernetesObjectType}.", typeof(TKubernetesObject).Name);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(IKubernetesWatcherEvent<TKubernetesObject> watcherEvent)
    {
        if (watcherEvent == null)
        {
            throw new ArgumentNullException(nameof(watcherEvent));
        }

        // TODO clarify if it is ok, more logging
        TKubernetesWatcherEvent kubernetesWatcherEvent = (TKubernetesWatcherEvent)watcherEvent;
        if (kubernetesWatcherEvent == null)
        {
            throw new ArgumentException(nameof(watcherEvent));
        }
        logger.LogDebug("Queueing Event {watcherEventType} - {kubernetesObjectType} - {kubernetesObjectName}",
            watcherEvent.WatcherEvent, typeof(TKubernetesObject).Name, watcherEvent.KubernetesObject?.Metadata?.Name);
        await queue.Writer.WriteAsync(kubernetesWatcherEvent);
    }

    public async ValueTask<IKubernetesWatcherEvent<TKubernetesObject>> DequeueAsync(CancellationToken cancellationToken)
    {
        var watcherEvent = await queue.Reader.ReadAsync(cancellationToken);
        logger.LogDebug("Dequeued Event {watcherEventType} - {kubernetesObjectType} - {kubernetesObjectName}",
            watcherEvent.WatcherEvent, typeof(TKubernetesObject).Name, watcherEvent.KubernetesObject?.Metadata?.Name);

        return watcherEvent;
    }
}
