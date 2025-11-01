using System.Collections.Generic;
using Jellyfin.Plugin.Sdk;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace JellyfinCustoms
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public Plugin(IApplicationPaths appPaths, IXmlSerializer xmlSerializer)
            : base(appPaths, xmlSerializer)
        {
        }

        public override string Name => "Jellyfin Customs - Streamed.pk";
        public override string Description => "Displays live sports from Streamed.pk as a dedicated library.";

        public void RegisterServices(IServiceCollection serviceCollection)
        {
            // Register our custom library so Jellyfin picks it up
            serviceCollection.AddSingleton<Library.LiveSportsLibrary>();
            serviceCollection.AddSingleton<Providers.StreamedPkProvider>();
        }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            yield return new PluginPageInfo
            {
                Name = "JellyfinCustomsConfiguration",
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.configPage.html"
            };
        }
    }
}
