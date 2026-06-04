#!/bin/powershell

npm run compile
vsce package
vsce publish 