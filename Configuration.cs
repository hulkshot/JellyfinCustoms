using MediaBrowser.Model.Plugins;
using System;

namespace JellyfinCustoms
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        // No configuration required for streamed.pk by default. Keep the class
        // so Jellyfin can still store any future settings without breaking.
    }
}
