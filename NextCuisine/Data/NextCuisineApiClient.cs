
namespace NextCuisine.Data
{
    public class NextCuisineApiClient
    {
        private readonly string uri = "http://nextcuisine-dev.us-east-1.elasticbeanstalk.com";
        public readonly HttpClient httpClient;

        public NextCuisineApiClient()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(uri);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<HttpResponseMessage> SendJsonAsync(HttpMethod method, object data, string route)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
            req.Content = JsonContent.Create(data);
            var apiResp = await httpClient.SendAsync(req);
            return apiResp;
        }
    }
}
