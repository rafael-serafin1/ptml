#!/bin/bash


debug:
	dotnet fsi ./playground/debug/debug.fsx

script:
	dotnet fsi ./playground/scripting/runtime.fsx "index.ptml"

pub:
	dotnet build
	dotnet publish -c Release -r win-x64 --self-contained true -o ./build

testr: pub run
testw: pub watch

run:
	./build/ptml run "index.ptml"
watch:
	./build/ptml watch "index.ptml"