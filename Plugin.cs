using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Plugins;

namespace JellyfinCustoms
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public override string Name => "JellyfinCustoms";
        public override string Description => "Pulls channels from your authorized API and exposes them as Live TV.";

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
        }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "customs",
                    EmbeddedResourcePath = GetType().Namespace + ".Web.config.html",
                    IsMainPage = true
                }
            };
        }
    }
}
