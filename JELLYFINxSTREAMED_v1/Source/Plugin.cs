using Jellyfin.Sdk.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JellyfinxStreamed
{
    public class JellyfinxStreamedPlugin : BasePlugin<JellyfinxStreamed.Configuration>
    {
        public override string Name => "JELLYFINxSTREAMED";
        public override string Description => "Adds Live Sports integration via Streamed API (streamed.pk / strmd.link)";
        public override string Author => "hulkshot";

        public JellyfinxStreamedPlugin() { }

        // Optional: register services here
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHttpClient("StreamedApi");
            services.AddSingleton<Providers.StreamedPkProvider>();
        }
    }
}
