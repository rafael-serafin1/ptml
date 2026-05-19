#!/bin/bash

run:
	dotnet fsi runtime.fsx $(ARGS)

debug:
	dotnet fsi debug.fsx

test:
	dotnet fsi runtime.fsx 

#ignore these
play:
	dotnet fsi ./playground/argv/args.fsx $(ARGS)
	dotnet fsi ./playground/terminal/terminal.fsx

layout:
	dotnet fsi ./tests/layout-test/test.fsx