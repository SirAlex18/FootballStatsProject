using API.Integration;
using API.Mappers;
using API.Models;
using Microsoft.Extensions.Logging;
using Player;

namespace API.Services;

public class PlayerApiService : IPlayerApiService
{
    private readonly IIntegrationApi _apiClient;
    private readonly ILogger<PlayerApiService> _logger;

    public PlayerApiService(IIntegrationApi apiClient, ILogger<PlayerApiService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<IPlayerData?> GetPlayerDataAsync(IPlayerInputData input)
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
