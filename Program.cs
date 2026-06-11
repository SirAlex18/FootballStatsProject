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
string playerId = args.Length > 0 ? args[0] : "12345";
string yearOfSeason = args.Length > 1 ? args[1] : "2023";

var inputData = new PlayerInputData 
{ 
    PlayerId = playerId,
    YearOfSeason = yearOfSeason 
};

try
{
    Console.WriteLine($"Fetching player stats for ID: {playerId}, Season: {yearOfSeason}...");
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
catch (Exception ex)
{
    Console.WriteLine($"❌ Error fetching player stats: {ex.Message}");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
