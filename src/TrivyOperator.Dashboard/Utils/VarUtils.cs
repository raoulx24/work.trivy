using k8s;
using k8s.Models;

namespace TrivyOperator.Dashboard.Utils;

public class VarUtils
{
    public const string DefaultCacheRefreshKey = "generic.Key";

    public static string GetCacheRefreshKey(IKubernetesObject<V1ObjectMeta>? kubernetesObject) =>
        kubernetesObject?.Namespace() ?? DefaultCacheRefreshKey;
}
