using Microsoft.Extensions.Configuration;

namespace API.Models;

public class Config : IConfig
{
    public string? ApiKey { get; set; }
    public string? EndPoint { get; set; }

    public Config(IConfiguration configuration)
    {
        ApiKey = configuration["RapidAPI:Key"];
        EndPoint = configuration.GetValue<string>("ApiSettings:EndPoint") ?? "https://v3.football.api-sports.io";
    }
}
