using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherStates;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class WatcherStateInfoService(IConcurrentCache<string, WatcherStateInfo> cache)
    : IWatcherStateInfoService
{
    public Task<IList<WatcherStateInfoDto>> GetWatcherStateInfos()
    {
        List<WatcherStateInfoDto> watcherStateInfoDtos = cache.Select(kvp => kvp.Value.ToWatcherStateInfoDto()).ToList();

        return Task.FromResult((IList<WatcherStateInfoDto>)watcherStateInfoDtos);
    }
}
