using Model;
using API.Models;
using RestSharp;

namespace API.Integration;

public class IntegrationApi : IIntegrationApi
{
    private readonly RestClient _client;
    private readonly string _apiKey;

    public IntegrationApi(IConfig config)
    {
        _apiKey = config.ApiKey ?? throw new ArgumentException("API Key is required.");
        
        var options = new RestClientOptions(config.EndPoint ?? throw new ArgumentException("Endpoint is required."))
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        _client = new RestClient(options);
    }

    public async Task<Rootobject?> GetPlayerDataAsync(IPlayerInputData input)
    {
        var request = new RestRequest("/players", Method.Get);
        request.AddQueryParameter("id", input.PlayerId);
        request.AddQueryParameter("season", input.YearOfSeason);
        request.AddHeader("x-rapidapi-key", _apiKey);

        // Use ExecuteAsync<T> to get a RestResponse<Rootobject> which contains IsSuccessStatusCode and Data
        var response = await _client.ExecuteAsync<Rootobject>(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"API request failed with status code: {response.StatusCode}. Content: {response.Content}"
            );
        }

        return response.Data;
    }
}
