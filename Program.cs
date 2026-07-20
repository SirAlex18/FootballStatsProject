using API.Data;
using API.Integration;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Entity Framework Core with PostgreSQL
builder.Services.AddDbContext<FootballStatsContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IIntegrationApi, IntegrationApi>();
builder.Services.AddSingleton<IConfig>(sp => new Config(builder.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
