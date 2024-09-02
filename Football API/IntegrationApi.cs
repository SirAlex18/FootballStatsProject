using RestSharp;
using Player;
using Model;
using API.Models;

namespace IntegrationToApi.Football_API
{
    public class IntegrationApi : IIntegrationApi
    {
        /// <summary>
        /// Will get football stats for the player of your choice and decide the season as well.
        /// </summary>
        public async Task<List<PlayerData>> GetFootballStats(Config conf, PlayerInputData input)
        {
            var listOfFootballStats = new List<PlayerData>();

            var endpoint = "/players?id=" + input.PlayerId + @"&season=" + input.YearOfSeason;

            var apiClient = new RestClient(conf.EndPoint!);
            var req = new RestRequest(endpoint);
            req.AddHeader("x-apisports-key", conf.ApiKey!);

            var resp = await apiClient.GetAsync<Rootobject>(req);
            var content = resp!.Response;

            foreach (var item in content!)
            {
                listOfFootballStats.Add(new PlayerData
                {
                    PlayerName = item.Player!.Name,
                    League = item.Statistics!.Select(x => x.League!.Name).FirstOrDefault(),
                    LeagueCountryOfOrigin = item.Statistics!.Select(x => x.League!.Country).FirstOrDefault(),
                    Appearences = item.Statistics!.Select(x => x.Games!.Appearences).FirstOrDefault(),
                    Minutes = item.Statistics!.Select(x => x.Games!.Minutes).FirstOrDefault(),
                    TotalShots = item.Statistics!.Select(x => x.Shots!.Total).FirstOrDefault(),
                    TotalShotsOnTarget = item.Statistics!.Select(x => x.Shots!.On).FirstOrDefault(),
                    Goals = item.Statistics!.Select(x => x.Goals!.Total).FirstOrDefault(),
                    Passes = item.Statistics!.Select(x => x.Passes!.Total).FirstOrDefault(),
                    PassAccuracy = item.Statistics!.Select(x => x.Passes!.Accuracy).FirstOrDefault(),
                    DribblesAttempts = item.Statistics!.Select(x => x.Dribbles!.Attempts).FirstOrDefault(),
                    SuccesfulDribbles = item.Statistics!.Select(x => x.Dribbles!.Success).FirstOrDefault()
                });
            }

            return listOfFootballStats;
        }
    }
}