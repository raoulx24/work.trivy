using TrivyOperator.Dashboard.Application.Models;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;

public interface IWatcherStateInfoService
{
    Task<IList<WatcherStateInfoDto>> GetWatcherStateInfos();
}
