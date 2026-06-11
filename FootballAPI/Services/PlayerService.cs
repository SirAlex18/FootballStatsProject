using API.Integration;
using API.Mappers;
using API.Models;
using Player;

namespace API.Services;

public class PlayerService : IPlayerService
{
    private readonly IIntegrationApi _apiClient;

    public PlayerService(IIntegrationApi apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IPlayerData?> GetPlayerStatsAsync(IPlayerInputData input)
    {
        var apiResponse = await _apiClient.GetPlayerDataAsync(input);
        
        if (apiResponse?.Response == null || apiResponse.Response.Count == 0)
            return null;

        var responseItem = apiResponse.Response[0];
        var statistic = responseItem.Statistics?.FirstOrDefault();
        
        if (statistic == null)
            return null;

        return PlayerDataMapper.MapToPlayerData(responseItem, statistic);
    }

    public async Task<List<IPlayerData>> GetPlayerStatsBulkAsync(IEnumerable<string> playerIds, string season)
    {
        var results = new List<IPlayerData>();
        
        foreach (var playerId in playerIds)
        {
            try
            {
                var input = new PlayerInputData { PlayerId = playerId, YearOfSeason = season };
                var stats = await GetPlayerStatsAsync(input);
                if (stats != null)
                    results.Add(stats);
                
                // Small delay to respect external API rate limits during bulk requests
                await Task.Delay(100); 
            }
            catch
            {
                // Skip failed requests for this player and continue processing others
                continue;
            }
        }
        
        return results;
    }
}
