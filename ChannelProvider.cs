using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JellyfinCustoms.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace JellyfinCustoms
{
    public class ChannelProvider
    {
        private readonly string baseUrl = "https://streamed.pk/api/matches";

        public async Task<List<ChannelItem>> GetChannelsAsync()
        {
            var client = new RestClient();
            var channels = new List<ChannelItem>();

            // Attempt to dynamically get categories (sports)
            var categoriesRequest = new RestRequest(baseUrl, Method.Get);
            var categoriesResponse = await client.ExecuteAsync(categoriesRequest);

            if (!categoriesResponse.IsSuccessful)
                return channels;

            var categoriesJson = JsonConvert.DeserializeObject<JObject>(categoriesResponse.Content);

            var categories = categoriesJson.Properties().Select(p => p.Name).ToList();

            foreach (var category in categories)
            {
                var url = $"{baseUrl}/{category}";
                var request = new RestRequest(url, Method.Get);
                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                    continue;

                var matches = JsonConvert.DeserializeObject<List<JObject>>(response.Content);
                foreach (var match in matches)
                {
                    var matchId = match["id"]?.ToString();
                    var title = match["title"]?.ToString();
                    var logo = match["logo"]?.ToString();
                    var sources = match["sources"] as JArray;

                    if (matchId == null || sources == null || sources.Count == 0)
                        continue;

                    var source = sources[0];
                    var sourceId = source["id"]?.ToString();
                    var sourceName = source["source"]?.ToString();

                    if (sourceId == null || sourceName == null)
                        continue;

                    var streamRequest = new RestRequest($"https://streamed.pk/api/stream/{sourceName}/{sourceId}", Method.Get);
                    var streamResponse = await client.ExecuteAsync(streamRequest);

                    if (!streamResponse.IsSuccessful)
                        continue;

                    var streamJson = JsonConvert.DeserializeObject<JObject>(streamResponse.Content);
                    var streamUrl = streamJson["stream"]?.ToString();

                    if (string.IsNullOrEmpty(streamUrl))
                        continue;

                    channels.Add(new ChannelItem
                    {
                        Id = matchId,
                        Title = title,
                        Stream = streamUrl,
                        Logo = logo,
                        Category = category.First().ToString().ToUpper() + category.Substring(1)
                    });
                }
            }

            return channels;
        }
    }
}
