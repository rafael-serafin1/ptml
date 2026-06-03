#!/bin/bash
# Build and install ptml

cd ./src
dotnet build
dotnet publish -c Release -r win-x64 --self-contained true -o ./build
chmod +x ptml
sudo mv ptml /usr/local/bin/
which ptml 
ptml --help