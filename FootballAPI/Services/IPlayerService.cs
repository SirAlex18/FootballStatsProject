using API.Models;
using Player;

namespace API.Services;

public interface IPlayerService
{
    Task<IPlayerData?> GetPlayerStatsAsync(IPlayerInputData input);
}
