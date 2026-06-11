using API.Integration;
using API.Models;
using API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
Console.WriteLine("DI Container configured successfully. Ready to fetch player stats.");
