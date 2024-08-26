using Player;

namespace IntegrationToApi.Football_API
{
    public interface IIntegrationApi
    {
        Task<List<PlayerData>> GetFootballStats(string key, string Id, string season);
    }
}