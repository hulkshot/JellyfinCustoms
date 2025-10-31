using System;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using Microsoft.Extensions.Logging;
using JellyfinCustoms.Models;

namespace JellyfinCustoms.Providers
{
    public class StreamedPkProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly string _apiKey;

        public StreamedPkProvider(IHttpClientFactory httpClientFactory, ILogger logger, string apiKey)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _apiKey = apiKey;
        }

        public async Task<List<BaseItem>> GetLiveMatchesAsync()
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.streamed.pk/v1/matches");
                request.Headers.Add("Authorization", $"Bearer {_apiKey}");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var matches = JsonSerializer.Deserialize<List<StreamedPkMatch>>(content);

                var items = new List<BaseItem>();
                foreach (var match in matches)
                {
                    items.Add(new Video
                    {
                        Name = match.Title,
                        Id = Guid.Parse(match.MatchId),
                        Path = match.StreamUrl,
                        DateCreated = match.StartTime
                    });
                }

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching streamed.pk matches");
                return new List<BaseItem>();
            }
        }
    }
}
