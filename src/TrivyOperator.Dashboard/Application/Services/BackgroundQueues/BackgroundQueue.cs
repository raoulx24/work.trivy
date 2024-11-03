using k8s;
using k8s.Models;
using Microsoft.Extensions.Options;
using System.Threading.Channels;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Options;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.BackgroundQueues;

public class BackgroundQueue<TKubernetesObject> : IBackgroundQueue<TKubernetesObject>
    where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
{
    private readonly ILogger<BackgroundQueue<TKubernetesObject>> logger;
    private readonly Channel<IWatcherEvent<TKubernetesObject>> queue;

    public BackgroundQueue(IOptions<BackgroundQueueOptions> options, ILogger<BackgroundQueue<TKubernetesObject>> logger)
    {
        this.logger = logger;
        BoundedChannelOptions boundedChannelOptions = new(options.Value.Capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
        };
        queue = Channel.CreateBounded<IWatcherEvent<TKubernetesObject>>(boundedChannelOptions);
        logger.LogDebug("Started BackgroundQueue for {kubernetesObjectType}.", typeof(TKubernetesObject).Name);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(IWatcherEvent<TKubernetesObject> watcherEvent)
    {
        ArgumentNullException.ThrowIfNull(watcherEvent, nameof(watcherEvent));

        logger.LogDebug(
            "Queueing Event {watcherEventType} - {kubernetesObjectType} - {kubernetesObjectName}",
            watcherEvent.WatcherEventType,
            typeof(TKubernetesObject).Name,
            watcherEvent.KubernetesObject.Metadata?.Name);
        await queue.Writer.WriteAsync(watcherEvent);
    }

    public async ValueTask<IWatcherEvent<TKubernetesObject>> DequeueAsync(CancellationToken cancellationToken)
    {
        IWatcherEvent<TKubernetesObject> watcherEvent = await queue.Reader.ReadAsync(cancellationToken);
        logger.LogDebug(
            "Dequeued Event {watcherEventType} - {kubernetesObjectType} - {kubernetesObjectName}",
            watcherEvent.WatcherEventType,
            typeof(TKubernetesObject).Name,
            watcherEvent.KubernetesObject.Metadata?.Name);

        return watcherEvent;
    }
}
