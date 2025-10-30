using System.Collections.Generic;
using System.Threading.Tasks;
using JellyfinCustoms.Models;
using Newtonsoft.Json;
using RestSharp;

namespace JellyfinCustoms
{
    public class ChannelProvider
    {
        private readonly string apiUrl = "https://yourapi.example.com/streams"; // Replace with your API

        public async Task<List<ChannelItem>> GetChannelsAsync()
        {
            //Replace apiUrl in ChannelProvider.cs with your API endpoint.
            var client = new RestClient(apiUrl);
            var request = new RestRequest("", Method.Get);

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                return new List<ChannelItem>();

            var channels = JsonConvert.DeserializeObject<List<ChannelItem>>(response.Content);
            return channels ?? new List<ChannelItem>();
        }
    }
}
