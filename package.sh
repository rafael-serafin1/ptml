#!/bin/bash

cd ./syntax-highlighter
arg="$1"

if [ "$arg" = "pub" ]; then
    echo "Executando publish..."
    rm "*.vsix"
    npm run compile
    vsce package
    vsce publish
elif [ "$arg" = "pack" ]; then
    echo "Executando package..."
    rm "*.vsix"
    npm run compile
    vsce package
else
    echo "Uso: $0 [pub|pack]"
    exit 1
fi

cd ../
exit 0