namespace API.Models;

public class Config : IConfig
{
    public string? ApiKey { get; set; }
    public string? EndPoint { get; set; }
}
