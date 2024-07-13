using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services;

public class NamespaceService(IConcurrentCache<string, IList<V1Namespace>> cache)
    : INamespaceService
{
    public Task<List<string>> GetKubernetesNamespaces()
    {
        List<V1Namespace> allNamespaces = [];
        if (cache.TryGetValue(VarUtils.defaultCacherRefreshKey, out IList<V1Namespace>? namespaces))
        {
            allNamespaces.AddRange(namespaces);
        }

        return Task.FromResult(allNamespaces.Select(x => x.Metadata!.Name).ToList());
    }
}
