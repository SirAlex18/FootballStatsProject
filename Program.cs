using API.Integration;
using API.Models;
using API.Services;
using Microsoft.Extensions.Options;
using Player;

var builder = WebApplication.CreateBuilder(args);

// Configuration is automatically loaded from appsettings.json by the Web SDK
builder.Services.Configure<Config>(builder.Configuration.GetSection("ApiConfig"));
builder.Services.AddSingleton<IConfig>(sp => sp.GetRequiredService<IOptions<Config>>().Value);

// Register Services
builder.Services.AddScoped<IIntegrationApi, IntegrationApi>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

var app = builder.Build();

// API Endpoint for Frontend - Single Player
app.MapGet("/api/players/{id}/{season}", async (string id, string season, IPlayerService playerService) =>
{
    // Input Validation
    if (string.IsNullOrWhiteSpace(id) || !int.TryParse(id, out _))
        return Results.BadRequest("Invalid Player ID. Must be numeric.");
    
    if (string.IsNullOrWhiteSpace(season) || season.Length != 4 || !int.TryParse(season, out _))
        return Results.BadRequest("Invalid Season Year. Must be a 4-digit year.");

    var inputData = new PlayerInputData 
    { 
        PlayerId = id,
        YearOfSeason = season 
    };

    try
    {
        var playerStats = await playerService.GetPlayerStatsAsync(inputData);
        
        if (playerStats != null)
            return Results.Ok(playerStats);
            
        return Results.NotFound("No stats found for the provided player/season.");
    }
    catch (ApiException apiEx)
    {
        return Results.Problem(
            detail: apiEx.Message,
            statusCode: apiEx.StatusCode ?? 500,
            extensions: new Dictionary<string, object?> { ["errorCode"] = apiEx.ErrorCode }
        );
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
});

// API Endpoint for Frontend - Bulk Players
app.MapGet("/api/players/bulk", async (string? ids, string season, IPlayerService playerService) =>
{
    if (string.IsNullOrWhiteSpace(ids))
        return Results.BadRequest("Missing 'ids' query parameter. Format: ?ids=1,2,3");

    if (string.IsNullOrWhiteSpace(season) || season.Length != 4 || !int.TryParse(season, out _))
        return Results.BadRequest("Invalid Season Year. Must be a 4-digit year.");

    var playerIdList = ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    
    try
    {
        var playerStatsList = await playerService.GetPlayerStatsBulkAsync(playerIdList, season);
        return Results.Ok(playerStatsList);
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
});

app.Run();
