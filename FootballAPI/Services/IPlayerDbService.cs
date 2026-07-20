using Player;

namespace API.Services;

public interface IPlayerDbService
{
    Task<IPlayerData?> GetCachedStatsAsync(string playerId, string season);
    Task SaveStatsAsync(IPlayerData stats, string playerId, string season);
}
