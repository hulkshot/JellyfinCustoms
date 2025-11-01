using Jellyfin.Sdk.Plugins;

namespace JellyfinxStreamed
{
    public class Configuration : IPluginConfiguration
    {
        // Active domain override, empty = automatic
        public string ActiveDomain { get; set; } = string.Empty;

        // How often (in days) to check parent site validity
        public int ParentCheckIntervalDays { get; set; } = 7;

        // Enable background monitor service
        public bool EnableBackgroundMonitor { get; set; } = true;
    }
}
