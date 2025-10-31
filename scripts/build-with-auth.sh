#!/usr/bin/env bash
set -euo pipefail

# Build helper (bash)
# Creates a temporary NuGet.Config from environment variables, runs restore and build,
# then removes the temporary config.

FEED_URL="${NUGET_FEED_URL:-}"
USERNAME="${NUGET_USERNAME:-}"
PASSWORD="${NUGET_PASSWORD:-}"

if [ -z "$FEED_URL" ] || [ -z "$USERNAME" ] || [ -z "$PASSWORD" ]; then
  echo "Missing credentials. Provide NUGET_FEED_URL, NUGET_USERNAME and NUGET_PASSWORD environment variables." >&2
  exit 2
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TEMP_CONFIG="$SCRIPT_DIR/NuGet.Config.temp"

cat > "$TEMP_CONFIG" <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="authenticated" value="$FEED_URL" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <authenticated>
      <add key="Username" value="$USERNAME" />
      <add key="ClearTextPassword" value="$PASSWORD" />
    </authenticated>
  </packageSourceCredentials>
</configuration>
EOF

cleanup() {
  rm -f "$TEMP_CONFIG" || true
}
trap cleanup EXIT

echo "Using temporary NuGet.Config at $TEMP_CONFIG"
pushd "$SCRIPT_DIR" >/dev/null

echo "Restoring packages..."
dotnet restore --configfile "$TEMP_CONFIG"

echo "Building solution (Release)..."
dotnet build "${SCRIPT_DIR}/../JellyfinCustoms.sln" -c Release

echo "Build finished. Artifacts are in bin/Release."
popd >/dev/null
