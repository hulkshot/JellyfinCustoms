using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Logging;
using JellyfinCustoms.Models;
using JellyfinCustoms.Providers;

namespace JellyfinCustoms.Library
{
    public class LiveSportsLibrary
    {
        private readonly ILogger _logger;
        private readonly StreamedPkProvider _provider;
        private readonly List<VideoItem> _currentMatches = new List<VideoItem>();
        private readonly object _lock = new object();

        public LiveSportsLibrary(ILogger logger, string apiKey)
        {
            _logger = logger;
            _provider = new StreamedPkProvider(logger, apiKey);
        }

        public void StartBackgroundRefresh()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await RefreshMatches();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error refreshing Live Sports Library: " + ex.Message);
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1));
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

            _logger.Info($"Live Sports Library refreshed: {_currentMatches.Count} matches found.");
        }

        // Optional method to expose matches to other parts of Jellyfin
        public List<VideoItem> GetCurrentMatches()
        {
            lock (_lock)
            {
                return new List<VideoItem>(_currentMatches);
            }
        }
    }
}
