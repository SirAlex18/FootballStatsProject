using API.Data;
using API.Integration;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Player;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Configuration is automatically loaded from appsettings.json by the Web SDK
builder.Services.Configure<Config>(builder.Configuration.GetSection("ApiConfig"));
builder.Services.AddSingleton<IConfig>(sp => sp.GetRequiredService<IOptions<Config>>().Value);

// Register EF Core with PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<FootballStatsContext>(options =>
    options.UseNpgsql(connectionString));

// CORS Configuration for React Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register Services
builder.Services.AddScoped<IIntegrationApi, IntegrationApi>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

var app = builder.Build();

app.UseCors("AllowReactApp");

// Health & Readiness Endpoints for Docker/K8s Probes
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.MapGet("/ready", async (FootballStatsContext context) =>
{
    try
    {
        await context.Database.CanConnectAsync();
        return Results.Ok(new { status = "ready" });
    }
    catch
    {
        return Results.StatusCode(503);
    }
});

// API Endpoint for Frontend - Single Player
app.MapGet("/api/players/{id}/{season}", async (string id, string season, IPlayerService playerService) =>
{
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

// API Endpoint for Frontend - Bulk Players (POST with JSON body)
app.MapPost("/api/players/bulk", async ([FromBody] BulkPlayerRequest request, IPlayerService playerService) =>
{
    if (request?.Ids == null || !request.Ids.Any())
        return Results.BadRequest("Missing 'Ids' in request body.");

    if (string.IsNullOrWhiteSpace(request.Season) || request.Season.Length != 4 || !int.TryParse(request.Season, out _))
        return Results.BadRequest("Invalid Season Year. Must be a 4-digit year.");

    try
    {
        var playerStatsList = await playerService.GetPlayerStatsBulkAsync(request.Ids, request.Season);
        return Results.Ok(playerStatsList);
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
});

app.Run();

public record BulkPlayerRequest(IEnumerable<string> Ids, string Season);
