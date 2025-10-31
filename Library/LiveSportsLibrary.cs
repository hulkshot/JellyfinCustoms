using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using JellyfinCustoms.Models;
using JellyfinCustoms.Providers;

namespace JellyfinCustoms.Library
{
    public class LiveSportsLibrary : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly List<BaseItem> _currentMatches = new List<BaseItem>();
        private readonly object _lock = new object();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public LiveSportsLibrary(ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<LiveSportsLibrary>();
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public void StartBackgroundRefresh()
        {
            Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        // Create a service scope per refresh to avoid using disposed services
                        using var scope = _scopeFactory.CreateScope();
                        await RefreshMatches(scope.ServiceProvider);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error refreshing Live Sports Library");
                    }

                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1), _cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            });
        }

        private async Task RefreshMatches(IServiceProvider services)
        {
            try
            {
                // Resolve per-scope services and construct a provider for this run
                var httpClientFactory = services.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory
                    ?? throw new InvalidOperationException("IHttpClientFactory not available in scope");
                var cache = services.GetService(typeof(Microsoft.Extensions.Caching.Memory.IMemoryCache)) as Microsoft.Extensions.Caching.Memory.IMemoryCache
                    ?? throw new InvalidOperationException("IMemoryCache not available in scope");

                var providerLoggerFactory = services.GetService(typeof(ILoggerFactory)) as ILoggerFactory ?? _loggerFactory;
                var providerLogger = providerLoggerFactory.CreateLogger<StreamedPkProvider>();

                var provider = new StreamedPkProvider(httpClientFactory, providerLogger, cache);

                var matches = await provider.GetLiveMatchesAsync();

                lock (_lock)
                {
                    _currentMatches.Clear();
                    _currentMatches.AddRange(matches);
                }

                _logger.LogInformation("Live Sports Library refreshed: {Count} matches found", _currentMatches.Count);
            }
            catch (ObjectDisposedException ex)
            {
                // Host is likely shutting down; stop further background refreshes quietly
                _logger.LogWarning("Background refresh aborted due to disposed service: {Message}", ex.Message);
                try { _cts.Cancel(); } catch { }
            }
        }

        // Optional method to expose matches to other parts of Jellyfin
        public List<BaseItem> GetCurrentMatches()
        {
            lock (_lock)
            {
                return new List<BaseItem>(_currentMatches);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
