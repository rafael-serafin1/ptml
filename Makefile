#!/bin/bash

run:
	dotnet fsi runtime.fsx

test:
	dotnet fsi ./tests/first-impression/tests.fsx
	dotnet fsi ./tests/first-impression/recursive.fsx
	dotnet fsi ./tests/first-impression/class.fsx

layout:
	dotnet fsi ./tests/layout-test/test.fsx