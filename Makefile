#!/bin/bash

run:
	dotnet fsi runtime.fsx

debug:
	dotnet fsi debug.fsx

test:
	dotnet fsi runtime.fsx

#ignore these
testing:
	dotnet fsi ./tests/first-impression/tests.fsx
	dotnet fsi ./tests/first-impression/recursive.fsx
	dotnet fsi ./tests/first-impression/class.fsx

layout:
	dotnet fsi ./tests/layout-test/test.fsx