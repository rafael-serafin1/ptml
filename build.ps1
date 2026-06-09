#!/usr/bin/env pwsh

Set-Location ./src
dotnet build
dotnet publish -c Release -r win-x64 --self-contained true -o ./build
Set-Location ../build
Set-Variable path=%path%;
Where-Object ptml
Set-Location ../
ptml --help --version
ptml run "index.ptml" --window