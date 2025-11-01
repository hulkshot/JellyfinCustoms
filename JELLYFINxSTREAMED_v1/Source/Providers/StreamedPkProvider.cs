using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;

namespace JellyfinxStreamed.Providers
{
    public class StreamedPkProvider
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<StreamedPkProvider> _logger;
        private readonly IMemoryCache _cache;

        public StreamedPkProvider(IHttpClientFactory httpFactory, ILogger<StreamedPkProvider> logger, IMemoryCache cache)
        {
            _httpFactory = httpFactory;
            _logger = logger;
            _cache = cache;
        }

        // Example method: fetch matches list from streamed.pk (public API)
        public async Task<List<object>> GetLiveMatchesAsync(string domain = "https://streamed.pk")
        {
            try
            {
                var client = _httpFactory.CreateClient("StreamedApi");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("JELLYFINxSTREAMED/1.0");
                var resp = await client.GetAsync($"{domain}/api/matches"); // placeholder path
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                var doc = JsonSerializer.Deserialize<List<object>>(json);
                return doc ?? new List<object>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch matches");
                return new List<object>();
            }
        }
    }
}
