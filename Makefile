#!/bin/bash


debug:
	dotnet fsi ./playground/debug/debug.fsx

script:
	dotnet fsi ./playground/scripting/runtime.fsx "index.ptml"

build_fs: 
	dotnet build

pub:
	dotnet publish -c Release -r win-x64 --self-contained true -o ./build

testr_rapid: pub run
testr: build_fs pub run
testw_rapid: pub watch
testw: build_fs pub watch

run:
	ptml run "index.ptml"
watch:
	ptml watch "index.ptml"