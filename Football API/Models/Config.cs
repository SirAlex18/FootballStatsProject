namespace API.Models
{
    public class Config : IConfig
    {
        public string? EndPoint { get; set; }
        public string? ApiKey { get; set; }
    }
}
