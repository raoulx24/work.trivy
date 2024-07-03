using k8s;
using System.Threading.Channels;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;

public class BackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject> :
    IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
        where TKubernetesObject : IKubernetesObject
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>
{
    private readonly Channel<TKubernetesWatcherEvent> queue;

    public BackgroundQueue(int capacity)
    {
        // Capacity should be set based on the expected application load and
        // number of concurrent threads accessing the queue.            
        // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
        // which completes only when space became available. This leads to backpressure,
        // in case too many publishers/calls start accumulating.
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        queue = Channel.CreateBounded<TKubernetesWatcherEvent>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(TKubernetesWatcherEvent watcherEvent)
    {
        if (watcherEvent == null)
        {
            throw new ArgumentNullException(nameof(watcherEvent));
        }

        await queue.Writer.WriteAsync(watcherEvent);
    }

    public async ValueTask<TKubernetesWatcherEvent> DequeueAsync(CancellationToken cancellationToken)
    {
        var watcherEvent = await queue.Reader.ReadAsync(cancellationToken);

        return watcherEvent;
    }
}
