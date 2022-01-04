#!/usr/bin/env bash
set -e
cd "$(dirname "$0")"
dotnet pwsh -NoProfile -File pack.ps1 "$@"
