#!/bin/bash

# Build the project
dotnet build --configuration Release

# Plugin name and version
PLUGIN_NAME="JellyfinCustoms"
PLUGIN_VERSION="1.0.0"

# Common Jellyfin plugin paths (uncomment the one you use)
PLUGIN_PATH="/var/lib/jellyfin/plugins/$PLUGIN_NAME"
#PLUGIN_PATH="/usr/lib/jellyfin/plugins/$PLUGIN_NAME"

# Create plugin directory
sudo mkdir -p "$PLUGIN_PATH"

# Copy plugin files
sudo cp "bin/Release/net6.0/$PLUGIN_NAME.dll" "$PLUGIN_PATH/"
sudo cp "bin/Release/net6.0/$PLUGIN_NAME.deps.json" "$PLUGIN_PATH/"
sudo cp "meta.json" "$PLUGIN_PATH/"

# Set correct permissions
sudo chown -R jellyfin:jellyfin "$PLUGIN_PATH"
sudo chmod -R 755 "$PLUGIN_PATH"

echo "Plugin files copied to: $PLUGIN_PATH"
echo "Restart Jellyfin for changes to take effect: sudo systemctl restart jellyfin"