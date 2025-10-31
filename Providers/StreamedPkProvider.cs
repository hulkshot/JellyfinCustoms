using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using JellyfinCustoms.Models;
using System.Linq;

// Lightweight stream DTO matching docs
internal class StreamInfo
{
    public string Id { get; set; } = string.Empty;
    public int StreamNo { get; set; }
    public string? Language { get; set; }
    public bool Hd { get; set; }
    public string EmbedUrl { get; set; } = string.Empty;
    public string? Source { get; set; }
}

namespace JellyfinCustoms.Providers
{
    public class StreamedPkProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private const string BaseApiUrl = "https://streamed.pk/api";

        public StreamedPkProvider(IHttpClientFactory httpClientFactory, ILogger logger, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<BaseItem>> GetLiveMatchesAsync()
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                // Use cache for match list (short TTL)
                var matches = await _cache.GetOrCreateAsync("streamedpk_matches_live", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
                    _logger.LogInformation("Fetching live matches from streamed.pk API");

                    var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseApiUrl}/matches/live");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var list = JsonSerializer.Deserialize<List<StreamedPkMatch>>(content);
                    return list ?? new List<StreamedPkMatch>();
                });

                var items = new List<BaseItem>();
                if (matches == null) return items;

                foreach (var match in matches)
                {
                    // Find a stream URL for this match. Match contains a 'sources' array
                    string? streamUrl = null;

                    if (match.Sources != null)
                    {
                        // Try each source in order until we get streams
                        foreach (var src in match.Sources)
                        {
                            try
                            {
                                // Check cache for this source/id combination
                                var cacheKey = $"streamedpk_stream_{src.Source}_{src.Id}";
                                var streams = await _cache.GetOrCreateAsync(cacheKey, async e =>
                                {
                                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
                                    var streamReq = new HttpRequestMessage(HttpMethod.Get, $"{BaseApiUrl}/stream/{src.Source}/{src.Id}");
                                    var streamResp = await client.SendAsync(streamReq);
                                    if (!streamResp.IsSuccessStatusCode) return new List<StreamInfo>();

                                    var streamContent = await streamResp.Content.ReadAsStringAsync();
                                    var sList = JsonSerializer.Deserialize<List<StreamInfo>>(streamContent);
                                    return sList ?? new List<StreamInfo>();
                                });

                                if (streams == null || streams.Count == 0) continue;
                                var chosen = streams.FirstOrDefault(s => s.Hd) ?? streams[0];
                                streamUrl = chosen.EmbedUrl;
                                _logger.LogInformation("Selected stream for match {MatchTitle}: {EmbedUrl}", match.Title, streamUrl);
                                break;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogDebug(ex, "Failed to get streams for source {Source}/{Id}", src.Source, src.Id);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(streamUrl))
                    {
                        // Skip matches with no playable stream
                        continue;
                    }

                    var video = new Video
                    {
                        Name = match.Title,
                            // Use a deterministic GUID generated from the match's id so
                            // the item keeps a stable id across refreshes
                            Id = CreateDeterministicGuid(match.Id),
                        Path = streamUrl,
                        DateCreated = DateTimeOffset.FromUnixTimeMilliseconds(match.Date).UtcDateTime
                    };

                    items.Add(video);
                }

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching streamed.pk matches");
                return new List<BaseItem>();
            }
        }

        private static Guid CreateDeterministicGuid(string input)
        {
            // Create an MD5 hash of the input and use it as GUID bytes
            using var md5 = System.Security.Cryptography.MD5.Create();
            var bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input ?? string.Empty));
            return new Guid(bytes);
        }
    }
}
