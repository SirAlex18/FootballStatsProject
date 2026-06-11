using Model;
using Player;

namespace API.Mappers;

public static class PlayerDataMapper
{
    public static IPlayerData MapToPlayerData(Response? response, Statistic? statistic)
    {
        if (response == null || statistic == null)
            return new PlayerData();

        var player = response.Player;
        var name = player != null ? $"{player.Firstname} {player.Lastname}".Trim() : "Unknown";

        var leagueName = statistic.League?.Name ?? string.Empty;
        var leagueCountry = statistic.League?.Country ?? string.Empty;

        return new PlayerData
        {
            PlayerName = name,
            League = leagueName,
            LeagueCountryOfOrigin = leagueCountry,
            Appearences = statistic.Games?.Appearences,
            Minutes = statistic.Games?.Minutes,
            TotalShots = statistic.Shots?.Total,
            TotalShotsOnTarget = statistic.Shots?.On,
            Goals = statistic.Goals?.Total,
            Passes = statistic.Passes?.Total,
            PassAccuracy = statistic.Passes?.Accuracy,
            DribblesAttempts = statistic.Dribbles?.Attempts,
            SuccesfulDribbles = statistic.Dribbles?.Success
        };
    }
}
