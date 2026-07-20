using API.Models;
using Player;

namespace API.Services;

public interface IPlayerApiService
{
    Task<IPlayerData?> GetPlayerDataAsync(IPlayerInputData input);
}
