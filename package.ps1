#!/usr/bin/env pwsh

param([string]$Command)

Set-Location ./syntax-highlighter

if ($Command -eq "pub") {
    Write-Host "Executando publish..."
    Remove-Item "*.vsix"
    npm run compile
    vsce package
    vsce publish
}
elseif ($Command -eq "pack") {
    Write-Host "Executando package..."
    Remove-Item "*.vsix"
    npm run compile
    vsce package
}
else {
    Write-Host "Uso: .\package.ps1 [pub|pack]"
    exit 1
}

Set-Location ../
exit 0