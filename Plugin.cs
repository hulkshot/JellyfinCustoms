using System;
using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using JellyfinCustoms.Library;

namespace JellyfinCustoms
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        private readonly ILogger<Plugin> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private LiveSportsLibrary _liveLibrary;
        public static Plugin Instance { get; private set; } = null!;

        public Plugin(
            IApplicationPaths applicationPaths,
            IXmlSerializer xmlSerializer,
            ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory,
            Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache,
            IServiceScopeFactory scopeFactory)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            _logger = loggerFactory.CreateLogger<Plugin>();
            _httpClientFactory = httpClientFactory;

            // Initialize services
            _liveLibrary = new LiveSportsLibrary(loggerFactory, scopeFactory);
            _liveLibrary.StartBackgroundRefresh();

            // Register virtual folder provider
            var virtualFolderProvider = new LiveSportsVirtualFolderProvider(_liveLibrary, _logger);
            // TODO: Use proper DI registration in Jellyfin 10.8+
            // Server.PluginManager.RegisterProvider(virtualFolderProvider);
        }

        public override string Name => "Jellyfin Customs - Streamed.pk";
        public override string Description => "Streams live sports from streamed.pk directly in Jellyfin.";
        public override Guid Id => new Guid("c5f7eb0b-35de-4b89-94e8-1c66f3ea1c0c");

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "streamedpk",
                    // Embedded resource lives under the Resources folder
                    EmbeddedResourcePath = GetType().Namespace + ".Resources.configPage.html",
                    EnableInMainMenu = true,
                    MenuSection = "server",
                    DisplayName = "Streamed.pk"
                }
            };
        }

    }
}
