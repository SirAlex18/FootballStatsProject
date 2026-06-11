namespace API.Models
{
    public interface IConfig
    {
        string? ApiKey { get; set; }
        string? EndPoint { get; set; }
    }
}