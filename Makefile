#!/bin/bash
BUILD_PATH = ./build

PUBLISH_FLAGS = --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o
PUBLISH_CONFIG = Release -r

ifeq ($(OS), Windows_NT)
	RELEASE_TARGET=win-x64
else
	RELEASE_TARGET=linux-x64
endif


pack:
ifeq ($(arg), PACK)
	py package.py pack
else ifeq ($(arg), PUB)
	py package.py pub
endif


# Receitas para build
os:
	echo $(OS)

b:
	dotnet build

all: b
	dotnet publish PTML/PTML.Src/PTML.Src.fsproj -c $(PUBLISH_CONFIG) $(RELEASE_TARGET) $(PUBLISH_FLAGS) $(BUILD_PATH)
	

build_ptml:
	python build.py


# Receitas de teste para desenvolvimento
# Ignore
build_fs: 
	dotnet build

pub:
	dotnet publish -c $(PUBLISH_CONFIG) $(RELEASE_TARGET) $(PUBLISH_FLAGS) $(BUILD_PATH)

dobuild: build_fs pub