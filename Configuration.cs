using MediaBrowser.Model.Plugins;
using System;

namespace JellyfinCustoms
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string ApiKey { get; set; } = "";
    }
}
