<#
Build helper (PowerShell)

Creates a temporary NuGet.Config from environment variables or parameters,
runs `dotnet restore` with that config, then `dotnet build -c Release`.

Usage:
  # Using environment variables
  $env:NUGET_FEED_URL = 'https://nuget.pkg.github.com/OWNER/index.json'
  $env:NUGET_USERNAME = 'USERNAME'
  $env:NUGET_PASSWORD = 'PERSONAL_ACCESS_TOKEN'
  pwsh ./scripts/build-with-auth.ps1

  # Or pass as parameters (safer for ephemeral use):
  pwsh ./scripts/build-with-auth.ps1 -FeedUrl 'https://...' -Username 'user' -Password 'token'

Note: This script writes a temporary NuGet.Config next to the script and deletes it after.
Do NOT commit credentials to the repo.
#>

[CmdletBinding()]
param(
    [string]$FeedUrl = $env:NUGET_FEED_URL,
    [string]$Username = $env:NUGET_USERNAME,
    [string]$Password = $env:NUGET_PASSWORD
)

if (-not $FeedUrl -or -not $Username -or -not $Password) {
    Write-Error "Missing credentials. Provide FeedUrl, Username and Password via parameters or environment variables."
    exit 2
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$tempConfig = Join-Path $scriptDir 'NuGet.Config.temp'

$xml = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="authenticated" value="$FeedUrl" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <authenticated>
      <add key="Username" value="$Username" />
      <add key="ClearTextPassword" value="$Password" />
    </authenticated>
  </packageSourceCredentials>
</configuration>
"@

try {
    Write-Host "Writing temporary NuGet.Config -> $tempConfig"
    [System.IO.File]::WriteAllText($tempConfig, $xml)

    Push-Location $PSScriptRoot
    Write-Host "Restoring packages using temporary NuGet.Config..."
    dotnet restore --configfile $tempConfig

    Write-Host "Building solution (Release)..."
    dotnet build "${PSScriptRoot}/../JellyfinCustoms.sln" -c Release
    $buildExit = $LASTEXITCODE
    if ($buildExit -ne 0) {
        Write-Error "dotnet build failed with exit code $buildExit"
        exit $buildExit
    }
    Write-Host "Build succeeded. Artifacts are in bin/Release."
} finally {
    if (Test-Path $tempConfig) {
        Write-Host "Removing temporary NuGet.Config"
        Remove-Item $tempConfig -Force
    }
    Pop-Location
}
