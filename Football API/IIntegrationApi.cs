using API.Models;
using Player;

namespace IntegrationToApi.Football_API
{
    public interface IIntegrationApi
    {
        Task<List<PlayerData>> GetFootballStats(Config conf, PlayerInputData input);
    }
}