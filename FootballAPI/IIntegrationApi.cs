using Model;
using API.Models;

namespace API.Integration;

public interface IIntegrationApi
{
    Task<Rootobject?> GetPlayerDataAsync(IPlayerInputData input);
}
