#!/usr/bin/env sh
set -e
if [ -z "$1" ]; then
  echo "Usage: $(basename "$0") <version>"
  exit 1
fi
cd "$(dirname "$0")"
ver="v$1"
mkdir "$ver"
cp mkdocs.yml "$ver"
cp -a docs "$ver/docs"
rm "$ver/docs/versions.json"
rm -rf "$ver/docs/dev"
