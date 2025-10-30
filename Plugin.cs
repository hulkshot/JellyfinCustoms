using MediaBrowser.Common.Plugins;

namespace JellyfinCustoms
{
    public class Plugin : BasePlugin
    {
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
        }

        public override string Name => "JellyfinCustoms";
        public override string Description => "Adds live sports streams from streamed.pk dynamically.";
    }
}
