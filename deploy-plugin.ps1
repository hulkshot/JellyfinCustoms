# Ensure we're in the project directory
$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Build the project
dotnet build "$projectDir\JellyfinCustoms.csproj" --configuration Release

# Define plugin directory (modify this path to match your Jellyfin plugins location)
$jellyfinPluginsDir = "$env:APPDATA\jellyfin\plugins\JellyfinCustoms"

# Create plugin directory if it doesn't exist
if (-not (Test-Path $jellyfinPluginsDir)) {
    New-Item -ItemType Directory -Path $jellyfinPluginsDir -Force
}

# Copy files
Copy-Item "$projectDir\bin\Release\net6.0\JellyfinCustoms.dll" $jellyfinPluginsDir
Copy-Item "$projectDir\bin\Release\net6.0\JellyfinCustoms.deps.json" $jellyfinPluginsDir
Copy-Item "$projectDir\meta.json" $jellyfinPluginsDir

Write-Host "Plugin files copied to: $jellyfinPluginsDir"
Write-Host "Restart Jellyfin for changes to take effect"