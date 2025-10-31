using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Logging;
using JellyfinCustoms.Models;

namespace JellyfinCustoms.Providers
{
    public class StreamedPkProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _apiKey;

        public StreamedPkProvider(ILogger logger, string apiKey)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _apiKey = apiKey;
        }

        public async Task<List<VideoItem>> GetLiveMatchesAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.streamed.pk/v1/matches");
                request.Headers.Add("Authorization", $"Bearer {_apiKey}");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var matches = JsonSerializer.Deserialize<List<StreamedPkMatch>>(content);

                var items = new List<VideoItem>();
                foreach (var match in matches)
                {
                    items.Add(new VideoItem
                    {
                        Name = match.Title,
                        Id = match.MatchId,
                        MediaSources = new List<MediaSource>
                        {
                            new MediaSource
                            {
                                Path = match.StreamUrl,
                                Protocol = MediaProtocol.Http
                            }
                        },
                        DateCreated = match.StartTime
                    });
                }

                return items;
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching streamed.pk matches: " + ex.Message);
                return new List<VideoItem>();
            }
        }
    }
}
