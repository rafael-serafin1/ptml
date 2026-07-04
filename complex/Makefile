#!/bin/make

test:
	g++ NativeComponentsLib.cpp -o ./output/test.exe
	./output/test

dll:
	g++ -shared NativeComponentsLib.cpp \
	-o ./output/NativeComponentsLib.dll \
	-Wl,--out-implib,./output/NativeComponentsLib.dll.a