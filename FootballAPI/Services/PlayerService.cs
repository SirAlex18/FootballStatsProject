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
}
