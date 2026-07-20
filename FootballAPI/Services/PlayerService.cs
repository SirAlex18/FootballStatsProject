using API.Models;
using Player;

namespace API.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerApiService _apiService;
    private readonly IPlayerDbService _dbService;

    public PlayerService(IPlayerApiService apiService, IPlayerDbService dbService)
    {
        _apiService = apiService;
        _dbService = dbService;
    }

    public async Task<IPlayerData?> GetPlayerStatsAsync(IPlayerInputData input)
    {
        // 1. Check database first (cache hit)
        var cachedStat = await _dbService.GetCachedStatsAsync(input.PlayerId, input.YearOfSeason);
        if (cachedStat != null)
        {
            return cachedStat;
        }

        // 2. Fetch from API (cache miss)
        var playerStats = await _apiService.GetPlayerDataAsync(input);
        
        if (playerStats != null)
        {
            // 3. Fire-and-forget background save
            _ = _dbService.SaveStatsAsync(playerStats, input.PlayerId, input.YearOfSeason);
        }

        return playerStats;
    }

    public async Task<List<IPlayerData>> GetPlayerStatsBulkAsync(IEnumerable<string> playerIds, string season)
    {
        var results = new List<IPlayerData>();
        
        foreach (var playerId in playerIds)
        {
            try
            {
                // Check cache first for bulk requests as well
                var cachedStat = await _dbService.GetCachedStatsAsync(playerId, season);

                if (cachedStat != null)
                {
                    results.Add(cachedStat);
                    continue;
                }

                var input = new PlayerInputData { PlayerId = playerId, YearOfSeason = season };
                var stats = await _apiService.GetPlayerDataAsync(input);
                if (stats != null)
                {
                    results.Add(stats);
                    _ = _dbService.SaveStatsAsync(stats, playerId, season);
                }
                
                await Task.Delay(100); 
            }
            catch
            {
                continue;
            }
        }
        
        return results;
    }
}
