#!/bin/bash
set -e

export PATH="/usr/share/dotnet:$PATH"
export DOTNET_ROOT="/usr/share/dotnet"

echo "=== Starting API Backend on http://0.0.0.0:5222 ==="
cd /workspace/API
dotnet run --urls "http://0.0.0.0:5222" &
API_PID=$!

trap "kill $API_PID 2>/dev/null || true" EXIT

# Wait for API to be ready
echo "Waiting for API to be ready..."
until curl -s http://127.0.0.1:5222/swagger/v1/swagger.json > /dev/null 2>&1 || curl -s http://127.0.0.1:5222/ > /dev/null 2>&1; do
  sleep 1
done
echo "API is ready!"

echo "=== Starting FinalProject Web UI on http://0.0.0.0:5000 ==="
cd /workspace/FinalProject
exec dotnet run --urls "http://0.0.0.0:5000"