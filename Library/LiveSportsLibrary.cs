using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Data.Entities;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;
using JellyfinCustoms.Providers;

namespace JellyfinCustoms.Library
{
    public class LiveSportsLibrary : IServerEntryPoint
    {
        private readonly ILogger<LiveSportsLibrary> _logger;
        private readonly ILibraryManager _libraryManager;
        private readonly StreamedPkProvider _provider;
        private Timer _timer;

        public LiveSportsLibrary(ILogger<LiveSportsLibrary> logger, ILibraryManager libraryManager, StreamedPkProvider provider)
        {
            _logger = logger;
            _libraryManager = libraryManager;
            _provider = provider;
        }

        public Task RunAsync()
        {
            _logger.LogInformation("[StreamedPk] LiveSportsLibrary starting background refresh...");
            _timer = new Timer(async _ => await RefreshMatches(), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            _timer?.Dispose();
            return Task.CompletedTask;
        }

        private async Task RefreshMatches()
        {
            try
            {
                var matches = await _provider.GetLiveMatchesAsync();
                if (matches == null || matches.Count == 0)
                {
                    _logger.LogWarning("[StreamedPk] No live matches found.");
                    return;
                }

                _logger.LogInformation("[StreamedPk] {Count} live matches found.", matches.Count);

                // Create (or update) a virtual library folder
                var root = _libraryManager.RootFolder;
                var liveSportsFolder = root.GetVirtualFolders().Find(x => x.Name == "Live Sports");
                if (liveSportsFolder == null)
                {
                    liveSportsFolder = new Folder
                    {
                        Name = "Live Sports",
                        DisplayMediaType = MediaType.Video
                    };
                    root.AddVirtualChild(liveSportsFolder);
                    _logger.LogInformation("[StreamedPk] Created new Live Sports virtual folder.");
                }

                // Clear existing items
                foreach (var child in liveSportsFolder.Children)
                    liveSportsFolder.RemoveChild(child);

                // Add new items
                foreach (var match in matches)
                {
                    var item = new Video
                    {
                        Name = match.Title,
                        Path = match.StreamUrl,
                        Overview = match.Description ?? "Live sports stream",
                        IsVirtualItem = true
                    };
                    liveSportsFolder.AddVirtualChild(item);
                    _logger.LogInformation("[StreamedPk] Added match: {0}", match.Title);
                }

                _logger.LogInformation("[StreamedPk] Live Sports library refreshed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StreamedPk] Error refreshing live matches.");
            }
        }
    }
}
