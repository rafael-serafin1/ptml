#!/bin/bash

cd ./syntax-highlighter
arg="$1"

if [ "$arg" = "publish" ]; then
    echo "Executando publish..."
    rm "*.vsix"
    npm run compile
    vsce package
    vsce publish
elif [ "$arg" = "package" ]; then
    echo "Executando package..."
    rm "*.vsix"
    npm run compile
    vsce package
else
    echo "Uso: $0 [publish|package]"
    exit 1
fi

cd ../
exit 0