using k8s;
using k8s.Models;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;

public interface IKubernetesPodWatchEventHandler
{
    Task Handle(WatchEventType type, V1Pod? item);
}
