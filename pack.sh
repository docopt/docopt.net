#!/usr/bin/env bash
set -e
cd "$(dirname "$0")"
dotnet pwsh -NoProfile -File "$(basename "$0").ps1" "$@"
