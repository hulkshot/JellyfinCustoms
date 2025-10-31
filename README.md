# JellyfinCustoms - Streamed.pk Live Sports

This plugin exposes a virtual "Live Sports" folder in Jellyfin populated from streamed.pk.

Quick install (Linux)

1. Build release:

```bash
dotnet build --configuration Release
```

2. Copy files to your Jellyfin plugins folder (example path):

```bash
sudo mkdir -p /var/lib/jellyfin/plugins/JellyfinCustoms
sudo cp bin/Release/net6.0/JellyfinCustoms.dll /var/lib/jellyfin/plugins/JellyfinCustoms/
sudo cp bin/Release/net6.0/JellyfinCustoms.deps.json /var/lib/jellyfin/plugins/JellyfinCustoms/
sudo cp meta.json /var/lib/jellyfin/plugins/JellyfinCustoms/
sudo cp Resources/icon.png /var/lib/jellyfin/plugins/JellyfinCustoms/
```

3. Set ownership and restart Jellyfin:

```bash
sudo chown -R jellyfin:jellyfin /var/lib/jellyfin/plugins/JellyfinCustoms
sudo systemctl restart jellyfin
```

4. Verify plugin is loaded and browse Admin → Plugins → Streamed.pk

Notes

- The provider fetches live matches from streamed.pk and attempts to select a playable stream (prefers HD). Some embed URLs may not be directly playable by Jellyfin clients; additional extraction may be required for certain sources.
- Matches and stream responses are cached for 60 seconds to reduce API load.

If you need help extracting direct HLS URLs from an embed page, paste a sample embed URL and I can inspect it.
