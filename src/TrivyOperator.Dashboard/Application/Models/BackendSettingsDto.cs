namespace TrivyOperator.Dashboard.Application.Models;

public class BackendSettingsDto
{
    public IEnumerable<string> EnabledTrivyReports { get; init; } = [];
}
