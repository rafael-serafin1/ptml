#!/bin/bash

cd ./syntax-highlighter
npm run compile
vsce package
cd ../