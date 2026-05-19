#!/bin/bash


debug:
	dotnet fsi ./playground/debug/debug.fsx

script:
	dotnet fsi ./playground/scripting/runtime.fsx "index.ptml"

pub:
	cd ./src
	dotnet build
	dotnet publish -c Release -r win-x64 --self-contained true -o ./build

test:
	./build/ptml run "index.ptml"