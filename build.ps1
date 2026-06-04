#!/usr/bin/env pwsh

Set-Location ./src
dotnet build
dotnet publish -c Release -r linux-x64 --self-contained true -o ./build
Set-Location ../build
Set-Variable path=%path%;
Where-Object ptml
ptml --help