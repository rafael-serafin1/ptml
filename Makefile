#!/bin/bash

debug_script:
	dotnet fsi ./playground/debug/debug.fsx

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
	ptml run "examples/depth.ptml"

depth: 
	ptml run "examples/depth.ptml"

test_snippet: dobuild
	ptml run "examples/snippet.ptml"

snippet:
	ptml run "examples/snippet.ptml"