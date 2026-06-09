#!/bin/bash

# Receitas para build

pack: 
	dotnet build
	dotnet publish -c Release -r win-x64 --self-contained true -o ./build

pack-win: pack
	.\package.ps1 pack

pack-linux: pack
	.\package.sh pack

build-win:
	.\build.ps1

build-linux:
	.\build.sh
	


# Receitas de teste para desenvolvimento
# Ignore
script:
	dotnet fsi ./playground/scripting/runtime.fsx "index.ptml"

build_fs: 
	dotnet build
pub:
	dotnet publish -c Release -r win-x64 --self-contained true -o ./build
dobuild: build_fs pub


qtestr: pub run
testr: dobuild run
qtestw: pub watch
testw: dobuild watch
qtestd: pub debug
testd: dobuild debug


run:
	ptml run "index.ptml"
watch:
	ptml watch "index.ptml"
debug:
	ptml debug "index.ptml"

wd:
	ptml run "index.ptml" --window

window: dobuild wd


test_depth: dobuild
	ptml run "tests/depth.ptml"
test_snippet: dobuild
	ptml run "tests/snippet.ptml"
test_spinner: dobuild
	ptml run "tests/spinner.ptml"
