using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Plugins;
using JellyfinCustoms.Library;

namespace JellyfinCustoms
{
    public class Plugin : BasePlugin<PluginConfiguration>
    {
        public static Plugin Instance { get; private set; }

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public override string Name => "Jellyfin Customs - Streamed.pk";
        public override string Description => "Streams live sports from streamed.pk directly in Jellyfin.";

        public override void Run()
        {
            base.Run();

            // Start background refresh
            var liveLibrary = new LiveSportsLibrary(Logger, Configuration.ApiKey);
            liveLibrary.StartBackgroundRefresh();

            // Register virtual folder provider
            var virtualFolderProvider = new LiveSportsVirtualFolderProvider(liveLibrary, Logger);
            MediaBrowser.Controller.Library.Providers.BaseItemProvider.RegisterProvider(virtualFolderProvider);
        }

    }
}
