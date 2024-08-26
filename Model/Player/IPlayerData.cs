namespace Player
{
    public interface IPlayerData
    {
        int? Appearences { get; set; }
        int? DribblesAttempts { get; set; }
        int? Goals { get; set; }
        string? League { get; set; }
        string? LeagueCountryOfOrigin { get; set; }
        int? Minutes { get; set; }
        int? PassAccuracy { get; set; }
        int? Passes { get; set; }
        string? PlayerName { get; set; }
        int? SuccesfulDribbles { get; set; }
        int? TotalShots { get; set; }
        int? TotalShotsOnTarget { get; set; }
    }
}