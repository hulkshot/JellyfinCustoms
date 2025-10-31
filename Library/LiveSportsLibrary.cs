using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using JellyfinCustoms.Models;
using JellyfinCustoms.Providers;

namespace JellyfinCustoms.Library
{
    public class LiveSportsLibrary : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly StreamedPkProvider _provider;
        private readonly List<BaseItem> _currentMatches = new List<BaseItem>();
        private readonly object _lock = new object();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public LiveSportsLibrary(ILogger logger, IHttpClientFactory httpClientFactory, Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _provider = new StreamedPkProvider(httpClientFactory, logger, cache);
        }

        public void StartBackgroundRefresh()
        {
            Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        await RefreshMatches();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error refreshing Live Sports Library");
                    }

                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1), _cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            });
        }

        private async Task RefreshMatches()
        {
            var matches = await _provider.GetLiveMatchesAsync();

            lock (_lock)
            {
                _currentMatches.Clear();
                _currentMatches.AddRange(matches);
            }

            _logger.LogInformation("Live Sports Library refreshed: {Count} matches found", _currentMatches.Count);
        }

        // Optional method to expose matches to other parts of Jellyfin
        public List<BaseItem> GetCurrentMatches()
        {
            lock (_lock)
            {
                return new List<BaseItem>(_currentMatches);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
