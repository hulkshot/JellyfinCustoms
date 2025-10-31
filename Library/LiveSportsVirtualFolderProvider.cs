using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;

namespace JellyfinCustoms.Library
{
    public class LiveSportsVirtualFolderProvider
    {
        private readonly LiveSportsLibrary _liveSportsLibrary;
        private readonly ILogger _logger;

        public LiveSportsVirtualFolderProvider(LiveSportsLibrary liveSportsLibrary, ILogger logger)
        {
            _liveSportsLibrary = liveSportsLibrary;
            _logger = logger;
        }

        public IEnumerable<BaseItem> GetItems(BaseItem item)
        {
            // Root folder
            if (item == null)
            {
                yield return new CollectionFolder
                {
                    Name = "Live Sports",
                    Id = Guid.Parse("live-sports-root"),
                    Path = "/plugins/JellyfinCustoms/live-sports"
                };
                yield break;
            }

            // If parent is the Live Sports folder, return matches
            if (item.Id == Guid.Parse("live-sports-root"))
            {
                var matches = _liveSportsLibrary.GetCurrentMatches();
                foreach (var match in matches)
                {
                    yield return match;
                }
            }
        }
    }
}
