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

layout:
	dotnet fsi ./tests/layout-test/test.fsx