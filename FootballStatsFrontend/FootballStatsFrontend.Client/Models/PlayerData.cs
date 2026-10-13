using Player;
using System.Text.Json.Serialization;

namespace FootballStats.Client.Player
{
    public class PlayerData : IPlayerData
    {
        [JsonPropertyName("playerName")]
        public string? PlayerName { get; set; }
        
        [JsonPropertyName("league")]
        public string? League { get; set; }
        
        [JsonPropertyName("leagueCountryOfOrigin")]
        public string? LeagueCountryOfOrigin { get; set; }
        
        [JsonPropertyName("appearences")]
        public int? Appearences { get; set; }
        
        [JsonPropertyName("minutes")]
        public int? Minutes { get; set; }
        
        [JsonPropertyName("totalShots")]
        public int? TotalShots { get; set; }
        
        [JsonPropertyName("totalShotsOnTarget")]
        public int? TotalShotsOnTarget { get; set; }
        
        [JsonPropertyName("goals")]
        public int? Goals { get; set; }
        
        [JsonPropertyName("passes")]
        public int? Passes { get; set; }
        
        [JsonPropertyName("passAccuracy")]
        public int? PassAccuracy { get; set; }
        
        [JsonPropertyName("dribblesAttempts")]
        public int? DribblesAttempts { get; set; }
        
        [JsonPropertyName("successfulDribbles")]
        public int? SuccessfulDribbles { get; set; }
    }
}
