using API.Integration;
using API.Models;
using API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Player;

var services = new ServiceCollection();

// Configuration
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

services.Configure<Config>(config.GetSection("ApiConfig"));
services.AddSingleton<IConfig>(sp => sp.GetRequiredService<IOptions<Config>>().Value);

// Register Services
services.AddScoped<IIntegrationApi, IntegrationApi>();
services.AddScoped<IPlayerService, PlayerService>();

var serviceProvider = services.BuildServiceProvider();

// Example usage
using var scope = serviceProvider.CreateScope();
var playerService = scope.ServiceProvider.GetRequiredService<IPlayerService>();

// Get player ID and season from command-line arguments or use defaults
string playerId = args.Length > 0 ? args[0] : "154";
string yearOfSeason = args.Length > 1 ? args[1] : "2024";

// Basic Input Validation
if (string.IsNullOrWhiteSpace(playerId) || !int.TryParse(playerId, out _))
{
    Console.WriteLine("❌ Invalid Player ID. Please provide a numeric ID.");
    return;
}

if (string.IsNullOrWhiteSpace(yearOfSeason) || yearOfSeason.Length != 4 || !int.TryParse(yearOfSeason, out _))
{
    Console.WriteLine("❌ Invalid Season Year. Please provide a 4-digit year (e.g., 2023).");
    return;
}

var inputData = new PlayerInputData 
{ 
    PlayerId = playerId,
    YearOfSeason = yearOfSeason 
};

try
{
    Console.WriteLine($"🔍 Fetching player stats for ID: {playerId}, Season: {yearOfSeason}...");
    var playerStats = await playerService.GetPlayerStatsAsync(inputData);
    
    if (playerStats != null)
    {
        Console.WriteLine($"✅ Successfully retrieved stats for: {playerStats.PlayerName}");
        Console.WriteLine($"📊 League: {playerStats.League} ({playerStats.LeagueCountryOfOrigin})");
        Console.WriteLine($"👟 Appearances: {playerStats.Appeareances}, Goals: {playerStats.Goals}, Passes: {playerStats.Passes}");
    }
    else
    {
        Console.WriteLine("⚠️ No stats found for the provided player/season.");
    }
}
catch (ApiException apiEx)
{
    Console.WriteLine($"❌ API Error [{apiEx.ErrorCode ?? "UNKNOWN"}]: {apiEx.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
