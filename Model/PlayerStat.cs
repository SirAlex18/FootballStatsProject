namespace Model;

public class PlayerStat
{
    public int Id { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public string League { get; set; } = string.Empty;
    public string LeagueCountryOfOrigin { get; set; } = string.Empty;
    public int? Appearances { get; set; }
    public int? Minutes { get; set; }
    public int? TotalShots { get; set; }
    public int? TotalShotsOnTarget { get; set; }
    public int? Goals { get; set; }
    public int? Passes { get; set; }
    public int? PassAccuracy { get; set; }
    public int? DribblesAttempts { get; set; }
    public int? SuccessfulDribbles { get; set; }
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
}
