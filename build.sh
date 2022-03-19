#!/usr/bin/env sh
set -e
cd "$(dirname "$0")"
rm -rf site
for d in dev v*; do
    cd $d
    mkdocs build -d ../site/$d
    cd ..
done
cp home/* site
