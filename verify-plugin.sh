#!/usr/bin/env bash
# verify-plugin.sh
# One-shot diagnostics for streamed.pk connectivity and basic plugin presence.

set -euo pipefail

PLUGIN_DIR="/var/lib/jellyfin/plugins/JellyfinCustoms"

echo "1) Plugin files (check path for your distro):"
if [ -d "$PLUGIN_DIR" ]; then
  ls -la "$PLUGIN_DIR" || true
else
  echo "  Plugin directory $PLUGIN_DIR not found. Adjust PLUGIN_DIR if needed." >&2
fi

echo
echo "2) Jellyfin service status (if systemd):"
if command -v systemctl >/dev/null 2>&1; then
  systemctl status jellyfin --no-pager || true
else
  ps aux | grep -i jellyfin || true
fi

echo
echo "3) DNS resolution checks (using Cloudflare 1.1.1.1 as in examples):"
echo "  dig +short streamed.pk @1.1.1.1"
dig +short streamed.pk @1.1.1.1 || true
echo
echo "  dig +short api.streamed.pk @1.1.1.1"
dig +short api.streamed.pk @1.1.1.1 || true

echo
echo "4) HTTP checks (fetching the live matches endpoints)."
echo "  curl -sS -m 10 -w '\nHTTP_STATUS:%{http_code}\n' https://streamed.pk/api/matches/live | sed -n '1,20p'"
curl -sS -m 10 -w "\nHTTP_STATUS:%{http_code}\n" https://streamed.pk/api/matches/live | sed -n '1,20p' || true

echo
echo "  curl -sS -m 10 -w '\nHTTP_STATUS:%{http_code}\n' https://api.streamed.pk/api/matches/live | sed -n '1,20p'"
curl -sS -m 10 -w "\nHTTP_STATUS:%{http_code}\n" https://api.streamed.pk/api/matches/live | sed -n '1,20p' || true

echo
echo "5) Logs: show last 50 lines of jellyfin log (path may vary):"
if [ -f /var/log/jellyfin/jellyfin.log ]; then
  echo "-- /var/log/jellyfin/jellyfin.log (last 50 lines) --"
  tail -n 50 /var/log/jellyfin/jellyfin.log || true
else
  echo "  /var/log/jellyfin/jellyfin.log not found; check /var/lib/jellyfin/log or journalctl." >&2
fi

echo
echo "Done. If streamed.pk resolves and returns 200 but api.streamed.pk fails, ensure plugin code uses 'streamed.pk' (not 'api.streamed.pk')."
