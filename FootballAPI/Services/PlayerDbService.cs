using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;
using Player;

namespace API.Services;

public class PlayerDbService : IPlayerDbService
{
    private readonly FootballStatsContext _context;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PlayerDbService> _logger;

    public PlayerDbService(FootballStatsContext context, IServiceScopeFactory scopeFactory, ILogger<PlayerDbService> logger)
    {
        _context = context;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<IPlayerData?> GetCachedStatsAsync(string playerId, string season)
    {
        var cachedStat = await _context.PlayerStats
            .FirstOrDefaultAsync(p => p.PlayerId == playerId && p.Season == season);

        if (cachedStat != null)
        {
            _logger.LogInformation("Returning cached stats for Player: {PlayerId}, Season: {Season}", playerId, season);
            return MapToIPlayerData(cachedStat);
        }

        return null;
    }

    public async Task SaveStatsAsync(IPlayerData stats, string playerId, string season)
    {
        const int maxRetries = 3;
        
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<FootballStatsContext>();
                
                var existing = await context.PlayerStats
                    .FirstOrDefaultAsync(p => p.PlayerId == playerId && p.Season == season);

                if (existing != null)
                {
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
                return;
            }
            catch (Exception ex)
            {
                if (attempt == maxRetries - 1)
                {
                    _logger.LogError(ex, "Failed to save player stats for ID: {PlayerId}, Season: {Season} after {MaxRetries} retries.", playerId, season, maxRetries);
                }
                else
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    _logger.LogWarning(ex, "DB save failed for ID: {PlayerId}, Season: {Season}. Retrying in {Delay}ms...", playerId, season, delay.TotalMilliseconds);
                    await Task.Delay(delay);
                }
            }
        }
    }

    private IPlayerData MapToIPlayerData(PlayerStat stat)
    {
        return new PlayerData
        {
            PlayerName = stat.PlayerName ?? "Unknown",
            League = stat.League ?? string.Empty,
            LeagueCountryOfOrigin = stat.LeagueCountryOfOrigin ?? string.Empty,
            Appearances = stat.Appearances,
            Minutes = stat.Minutes,
            TotalShots = stat.TotalShots,
            TotalShotsOnTarget = stat.TotalShotsOnTarget,
            Goals = stat.Goals,
            Passes = stat.Passes,
            PassAccuracy = stat.PassAccuracy,
            DribblesAttempts = stat.DribblesAttempts,
            SuccessfulDribbles = stat.SuccessfulDribbles
        };
    }
}
