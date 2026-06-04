#!/usr/bin/env pwsh

Set-Location ./syntax-highlighter
npm run compile
vsce package
Set-Location ../
# Só estou com preguiça de ficar escrevendo o caminho completo dessa pasta kkkkkkkkkk