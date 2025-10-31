using System.Collections.Generic;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;

namespace JellyfinCustoms.Library
{
    public class LiveSportsVirtualFolderProvider : BaseItemProvider
    {
        private readonly LiveSportsLibrary _liveSportsLibrary;

        public LiveSportsVirtualFolderProvider(LiveSportsLibrary liveSportsLibrary, ILogger logger)
            : base(logger)
        {
            _liveSportsLibrary = liveSportsLibrary;
        }

        public override IEnumerable<BaseItem> GetItems(BaseItem item)
        {
            // Root folder
            if (item == null)
            {
                yield return new Folder
                {
                    Name = "Live Sports",
                    Id = "live-sports-root",
                    FolderType = FolderType.Container,
                    PrimaryImagePath = "/plugins/JellyfinCustoms/icon.png"
                };
                yield break;
            }

            // If parent is the Live Sports folder, return matches
            if (item.Id == "live-sports-root")
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
