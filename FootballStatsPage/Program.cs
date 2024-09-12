using API.Models;
using FootballStatsPage.Components;
using IntegrationToApi.Football_API;
using Model;
using Player;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IIntegrationApi, IntegrationApi>();
builder.Services.AddSingleton<IConfig, Config>();
builder.Services.AddSingleton<IPlayerInputData, PlayerInputData>();
builder.Services.AddSingleton<IPlayerData, PlayerData>();
builder.Services.AddSingleton<ApiData>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
