using k8s.Models;
using k8s;

namespace TrivyOperator.Dashboard.Utils;

public class VarUtils
{
    public static readonly string defaultWatchersKey = "generic.Key";

    public static string GetWatchersKey(IKubernetesObject<V1ObjectMeta>? kubernetesObject) => kubernetesObject?.Namespace() ?? defaultWatchersKey;
}
