#!/bin/bash
set -euo pipefail

# Build release
dotnet build --configuration Release

PLUGIN_NAME="JellyfinCustoms"
# Adjust PLUGIN_PATH if your Jellyfin uses /usr/lib/jellyfin/plugins
PLUGIN_PATH="/var/lib/jellyfin/plugins/$PLUGIN_NAME"

sudo mkdir -p "$PLUGIN_PATH"

# Copy required files
sudo cp "bin/Release/net6.0/$PLUGIN_NAME.dll" "$PLUGIN_PATH/"
sudo cp "bin/Release/net6.0/$PLUGIN_NAME.deps.json" "$PLUGIN_PATH/"
sudo cp "meta.json" "$PLUGIN_PATH/"
# Copy icon so Jellyfin can display it in UI
sudo cp "Resources/icon.png" "$PLUGIN_PATH/"

# Set permissions
sudo chown -R jellyfin:jellyfin "$PLUGIN_PATH"
sudo chmod -R 755 "$PLUGIN_PATH"

echo "Copied plugin to $PLUGIN_PATH"
echo "Restart Jellyfin: sudo systemctl restart jellyfin"