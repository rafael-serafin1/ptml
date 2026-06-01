#!/bin/bash


debug_script:
	dotnet fsi ./playground/debug/debug.fsx

script:
	dotnet fsi ./playground/scripting/runtime.fsx "index.ptml"

build_fs: 
	dotnet build

pub:
	dotnet publish -c Release -r win-x64 --self-contained true -o ./build

all: build_fs pub

testr_rapid: pub run
testr: build_fs pub run
testw_rapid: pub watch
testw: build_fs pub watch
testd_rapid: pub debug
testd: build_fs pub debug

run:
	ptml run "index.ptml"
watch:
	ptml watch "index.ptml"
debug:
	ptml debug "cell_test.ptml"