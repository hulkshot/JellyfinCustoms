JELLYFINxSTREAMED - Source + Prebuilt ZIP placeholder
===============================================

Structure:
- Source/ : Visual Studio / VSCode plugin project (ready-to-build targeting .NET 9)
- DLL/    : Place compiled plugin DLL here (empty placeholder in this package)

Important notes:
- This project targets .NET 9.0 (net9.0). Ensure you have the matching SDK (dotnet --list-sdks).
- Jellyfin server version used should be 10.11.1 (or matching packages). Package references use Jellyfin.Sdk 10.11.1.
- Because NuGet sources for Jellyfin packages may be unavailable in some environments, you can build Jellyfin from source and reference local DLLs, or add local .nupkg files to a local NuGet source and run 'dotnet restore' with that source enabled.

How to build (typical):
1. cd Source
2. dotnet restore
3. dotnet build -c Release

If restore cannot find Jellyfin packages (common), options:
A) Build jellyfin server locally and copy the required assemblies from its bin folder into a 'localrefs' folder, then modify the .csproj to reference them via HintPath.
B) Download the specific .nupkg files and add them to a local NuGet folder:
   dotnet nuget add source /path/to/local/nuget -n local-jellyfin
   Place *.nupkg there, then dotnet restore --source local-jellyfin
C) Use the prebuilt Jellyfin server's /usr/lib/jellyfin/bin DLLs as references when building on the server (adjust .csproj to Reference those files).

Deploying compiled plugin to Jellyfin (server):
1. Stop jellyfin service: sudo systemctl stop jellyfin
2. Create plugin folder: sudo mkdir -p /var/lib/jellyfin/plugins/JELLYFINxSTREAMED
3. Copy the compiled DLL and any runtimes folder into that plugin folder. Usually you copy the single DLL that contains plugin types.
4. Start jellyfin: sudo systemctl start jellyfin
5. Check logs (journalctl -u jellyfin) for plugin load messages.

Why the DLL directory is empty in this ZIP:
- I cannot compile a .NET 9 assembly here that is guaranteed compatible with your server runtime and native libs (Skia, platform-specific native binaries). The DLL/ folder is a placeholder; once you build on your machine or provide a compiled DLL I can include it in a follow-up.

If you want, upload a compiled JellyfinCustoms.dll (built on your target machine) and I will package it into the DLL/ folder and regenerate the ZIP.

Enjoy — JELLYFINxSTREAMED
