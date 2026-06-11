using API.Data;
using API.Integration;
using API.Mappers;
using API.Models;
using Microsoft.Extensions.Logging;
using Player;

namespace API.Services;

public class PlayerService : IPlayerService
{
    private readonly IIntegrationApi _apiClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(IIntegrationApi apiClient, IServiceScopeFactory scopeFactory, ILogger<PlayerService> logger)
    {
        _apiClient = apiClient;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<IPlayerData?> GetPlayerStatsAsync(IPlayerInputData input)
    {
        // 1. Fetch immediately from API (fast path)
        var playerStats = await FetchFromApiAsync(input);
        
        if (playerStats != null)
        {
            // 2. Fire-and-forget background save (does not block response)
            _ = SaveToDatabaseAsync(playerStats, input.PlayerId, input.YearOfSeason);
        }

        return playerStats;
    }

    private async Task<IPlayerData?> FetchFromApiAsync(IPlayerInputData input)
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

    private async Task SaveToDatabaseAsync(IPlayerData stats, string playerId, string season)
    {
        try
        {
            // Create a new scope for the background task to safely access DbContext
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FootballStatsContext>();
            
            var existing = await context.PlayerStats
                .FirstOrDefaultAsync(p => p.PlayerId == playerId && p.Season == season);

            if (existing != null)
            {
                // Update existing record
                existing.PlayerName = stats.PlayerName ?? existing.PlayerName;
                existing.League = stats.League ?? existing.League;
                existing.LeagueCountryOfOrigin = stats.LeagueCountryOfOrigin ?? existing.LeagueCountryOfOrigin;
                existing.Appearances = stats.Appearances ?? existing.Appearances;
                existing.Minutes = stats.Minutes ?? existing.Minutes;
                existing.TotalShots = stats.TotalShots ?? existing.TotalShots;
                existing.TotalShotsOnTarget = stats.TotalShotsOnTarget ?? existing.TotalShotsOnTarget;
                existing.Goals = stats.Goals ?? existing.Goals;
                existing.Passes = stats.Passes ?? existing.Passes;
                existing.PassAccuracy = stats.PassAccuracy ?? existing.PassAccuracy;
                existing.DribblesAttempts = stats.DribblesAttempts ?? existing.DribblesAttempts;
                existing.SuccessfulDribbles = stats.SuccessfulDribbles ?? existing.SuccessfulDribbles;
                existing.FetchedAt = DateTime.UtcNow;
            }
            else
            {
                // Insert new record
                await context.PlayerStats.AddAsync(new PlayerStat
                {
                    PlayerId = playerId,
                    Season = season,
                    PlayerName = stats.PlayerName ?? "Unknown",
                    League = stats.League ?? string.Empty,
                    LeagueCountryOfOrigin = stats.LeagueCountryOfOrigin ?? string.Empty,
                    Appearances = stats.Appearances,
                    Minutes = stats.Minutes,
                    TotalShots = stats.TotalShots,
                    TotalShotsOnTarget = stats.TotalShotsOnTarget,
                    Goals = stats.Goals,
                    Passes = stats.Passes,
                    PassAccuracy = stats.PassAccuracy,
                    DribblesAttempts = stats.DribblesAttempts,
                    SuccessfulDribbles = stats.SuccessfulDribbles,
                    FetchedAt = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log silently to avoid crashing the HTTP request
            _logger.LogError(ex, "Failed to save player stats for ID: {PlayerId}, Season: {Season}", playerId, season);
        }
    }

    public async Task<List<IPlayerData>> GetPlayerStatsBulkAsync(IEnumerable<string> playerIds, string season)
    {
        var results = new List<IPlayerData>();
        
        foreach (var playerId in playerIds)
        {
            try
            {
                var input = new PlayerInputData { PlayerId = playerId, YearOfSeason = season };
                var stats = await FetchFromApiAsync(input);
                if (stats != null)
                {
                    results.Add(stats);
                    _ = SaveToDatabaseAsync(stats, playerId, season); // Fire-and-forget for bulk too
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
