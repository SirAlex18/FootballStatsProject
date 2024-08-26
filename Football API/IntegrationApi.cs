using RestSharp;
using Player;
using Model;

namespace IntegrationToApi.Football_API
{
    public class IntegrationApi
    {
        /// <summary>
        /// Will get all stats of Oscar season 2023
        /// </summary>
        public void GetFootballStats(string key, string Id, string season)
        {
            var listOfFootballStats = new List<PlayerData>();

            var apiClient = new RestClient(@"https://v3.football.api-sports.io");
            var req = new RestRequest("/players?id=" + Id + @"&season=" + season);
            req.AddHeader("x-apisports-key", key);

            var resp = apiClient.Get<Rootobject>(req);
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
        }
    }
}