param(
  [Parameter(Mandatory=$true)]
  [string]$Server,

  [Parameter(Mandatory=$true)]
  [string]$User,

  [Parameter(Mandatory=$true)]
  [string]$PluginPath = "/var/lib/jellyfin/plugins/JellyfinCustoms"
)

$localDll = Join-Path -Path (Get-Location) -ChildPath "bin\Release\net6.0\JellyfinCustoms.dll"
$localDeps = Join-Path -Path (Get-Location) -ChildPath "bin\Release\net6.0\JellyfinCustoms.deps.json"
$localMeta = Join-Path -Path (Get-Location) -ChildPath "meta.json"
$localIcon = Join-Path -Path (Get-Location) -ChildPath "Resources\icon.png"

if (-not (Test-Path $localDll)) { throw "Release DLL not found: $localDll. Build Release first." }

Write-Host "Copying plugin files to ${User}@${Server}:$PluginPath"

# Copy files to /tmp on server
scp $localDll $localDeps $localMeta $localIcon "${User}@${Server}:/tmp/"

# Move files to plugin folder and set permissions via SSH
$sshCmd = @"
sudo mkdir -p $PluginPath
sudo mv /tmp/JellyfinCustoms.dll $PluginPath/
sudo mv /tmp/JellyfinCustoms.deps.json $PluginPath/
sudo mv /tmp/meta.json $PluginPath/
sudo mv /tmp/icon.png $PluginPath/ || true
sudo chown -R jellyfin:jellyfin $PluginPath
sudo chmod -R 755 $PluginPath
sudo systemctl restart jellyfin
"@

ssh $User@$Server $sshCmd

Write-Host "Deployment finished. Tail logs to verify: sudo journalctl -u jellyfin -f"