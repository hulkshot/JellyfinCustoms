#!/usr/bin/env bash
set -euo pipefail

# deploy-to-linux.sh
# Safe, atomic deploy script for the JellyfinCustoms plugin.
# Usage: run this from the repo root or any location; script finds repo root by its own location.

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO_ROOT"

PLUGIN_NAME="JellyfinCustoms"
TARGET_FRAMEWORK="net9.0"
CONFIGURATION="Release"
PLUGINS_DIR="/var/lib/jellyfin/plugins"
BACKUP_DIR="${PLUGINS_DIR}-backups"

echo "[deploy] Repo root: $REPO_ROOT"

BRANCH="main"

if [ "$#" -ge 1 ]; then
  BRANCH="$1"
fi

echo "[deploy] Using git branch: $BRANCH"

# ensure we have a clean working tree or warn
if [ -n "$(git status --porcelain)" ]; then
  echo "[deploy] Warning: working tree is not clean. Stashing changes before pull."
  git stash --include-untracked
  STASHED=1
else
  STASHED=0
fi

git fetch origin "$BRANCH"
git checkout "$BRANCH"
git pull --ff-only origin "$BRANCH"

echo "[deploy] Building project"
dotnet clean -c "$CONFIGURATION"
dotnet restore
dotnet build -c "$CONFIGURATION" -v minimal

ARTIFACT_DLL="${REPO_ROOT}/bin/${CONFIGURATION}/${TARGET_FRAMEWORK}/${PLUGIN_NAME}.dll"
ARTIFACT_DEPS="${REPO_ROOT}/bin/${CONFIGURATION}/${TARGET_FRAMEWORK}/${PLUGIN_NAME}.deps.json"

if [ ! -f "$ARTIFACT_DLL" ]; then
  echo "[deploy] Build artifact not found: $ARTIFACT_DLL"
  exit 2
fi

TMPDIR=$(mktemp -d /tmp/${PLUGIN_NAME}-deploy-XXXX)
cleanup() {
  rm -rf "$TMPDIR" || true
}
trap cleanup EXIT

echo "[deploy] Preparing temporary deployment directory $TMPDIR"
mkdir -p "$TMPDIR"
cp "$ARTIFACT_DLL" "$TMPDIR/"
if [ -f "$ARTIFACT_DEPS" ]; then
  cp "$ARTIFACT_DEPS" "$TMPDIR/"
fi
# copy metadata and resources if present
if [ -f "$REPO_ROOT/meta.json" ]; then
  cp "$REPO_ROOT/meta.json" "$TMPDIR/"
fi
if [ -d "$REPO_ROOT/Resources" ]; then
  cp -r "$REPO_ROOT/Resources" "$TMPDIR/"
fi

echo "[deploy] Stopping jellyfin service"
sudo systemctl stop jellyfin

# determine plugin destination folder
EXISTING_DIR=$(ls -d ${PLUGINS_DIR}/${PLUGIN_NAME}_* 2>/dev/null || true)
if [ -n "$EXISTING_DIR" ]; then
  DEST_DIR="$EXISTING_DIR"
else
  DEST_DIR="${PLUGINS_DIR}/${PLUGIN_NAME}_$(date +%s)"
fi

echo "[deploy] Backing up existing plugin (if present) and moving new files into place"
sudo mkdir -p "$BACKUP_DIR"
if [ -d "$EXISTING_DIR" ]; then
  echo "[deploy] Moving existing plugin $EXISTING_DIR to backups"
  sudo mv "$EXISTING_DIR" "$BACKUP_DIR/${PLUGIN_NAME}_$(date +%s)"
fi

echo "[deploy] Moving new plugin into $DEST_DIR"
sudo mv "$TMPDIR" "$DEST_DIR"

echo "[deploy] Setting ownership to jellyfin:jellyfin"
sudo chown -R jellyfin:jellyfin "$DEST_DIR"

echo "[deploy] Starting jellyfin service"
sudo systemctl start jellyfin

echo "[deploy] Deployment complete. To watch logs run: sudo journalctl -u jellyfin -f"

if [ "$STASHED" -eq 1 ]; then
  echo "[deploy] Restoring stashed changes"
  git stash pop || true
fi

exit 0
