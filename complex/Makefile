#!/bin/make

test:
	g++ TestingBehaviour.cpp components/Button.cpp components/Input.cpp -o ./output/test.exe
	./output/test

memory:
	g++ -g -fsanitize=address -o ./output/memory TestingBehaviour.cpp components/Button.cpp components/Input.cpp

dll:
	g++ -shared NativeComponentsLib.cpp \
	-o ./output/NativeComponentsLib.dll \
	-Wl,--out-implib,./output/NativeComponentsLib.dll.a