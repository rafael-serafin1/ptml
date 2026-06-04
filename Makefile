#!/bin/bash

debug_script:
	dotnet fsi ./playground/debug/debug.fsx

script:
	dotnet fsi ./playground/scripting/runtime.fsx "index.ptml"

build_fs: 
	dotnet build

pub:
	dotnet publish -c Release -r win-x64 --self-contained true -o ./build

qtestr: pub run
testr: build_fs pub run
qtestw: pub watch
testw: build_fs pub watch
qtestd: pub debug
testd: build_fs pub debug

run:
	ptml run "index.ptml"
watch:
	ptml watch "index.ptml"
debug:
	ptml debug "index.ptml"

wd:
	ptml run "index.ptml" --window

window: build_fs pub wd