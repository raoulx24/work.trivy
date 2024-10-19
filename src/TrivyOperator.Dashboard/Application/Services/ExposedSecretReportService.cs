using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ExposedSecretReportService(IConcurrentCache<string, IList<ExposedSecretReportCr>> cache, ILogger<ExposedSecretReportService> logger)
{

}
