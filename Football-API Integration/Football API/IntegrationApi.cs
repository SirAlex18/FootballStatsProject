using RestSharp;

namespace IntegrationToApi.Football_API
{
    public class IntegrationApi
    {
        /// <summary>
        /// Will get all stats of Oscar season 2023
        /// </summary>
        public void GetOscarsStats(string key, string Id, string season)
        {
            var apiClient = new RestClient(@"https://v3.football.api-sports.io");
            var req = new RestRequest("/players?id=" + Id + @"&season=" + season);
            req.AddHeader("x-apisports-key", key);

            var resp = apiClient.Execute(req);
        }
    }
}
