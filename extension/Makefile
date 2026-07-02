pack:
	npm run compile
	vsce package

compile:
	npm run compile

cp: compile pack

npmW:
	npm run watch

cw: compile watch

package:
	npm run compile
	vsce package
	code --install-extension ptml-language-1.9.0.vsix