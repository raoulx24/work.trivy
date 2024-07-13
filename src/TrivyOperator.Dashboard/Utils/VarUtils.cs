using k8s.Models;
using k8s;

namespace TrivyOperator.Dashboard.Utils;

public class VarUtils
{
    public static readonly string defaultCacherRefreshKey = "generic.Key";

    public static string GetCacherRefreshKey(IKubernetesObject<V1ObjectMeta>? kubernetesObject) => kubernetesObject?.Namespace() ?? defaultCacherRefreshKey;
}
