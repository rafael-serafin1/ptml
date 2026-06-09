import * as vscode from "vscode";

interface PTMLTag {
    name: string;
    snippet: string;
    documentation: string;
}

export class PTMLCompletionProvider
    implements vscode.CompletionItemProvider {

    provideCompletionItems(
        document: vscode.TextDocument,
        position: vscode.Position
    ): vscode.CompletionItem[] {

        const line = document.lineAt(position.line).text;
        const beforeCursor = line.substring(0, position.character);

        const tagOpenMatch = beforeCursor.match(/<(\w*)$/);

        if (!tagOpenMatch) {
            return [];
        }

        const partial = tagOpenMatch[1];

        const tags: PTMLTag[] = [
            {
                name: "text",
                snippet: 'text>$0</text>',
                documentation: '<text>|</text>'
            },
            {
                name: "cell",
                snippet: 'cell>$0</cell>',
                documentation: '<cell>|</cell>'
            },
            {
                name: "box",
                snippet: 'box>$0</box>',
                documentation: '<box>|</box>'
            },
            {
                name: "block",
                snippet: 'block title="$1">$0</block>',
                documentation: '<block title="...">|</block>'
            },
            {
                name: "row",
                snippet: 'row>$0</row>',
                documentation: '<row>|</row>'
            },
            {
                name: "column",
                snippet: 'column>$0</column>',
                documentation: '<column>|</column>'
            },
            {
                name: "depth",
                snippet: 'depth index="$1">$0</depth>',
                documentation: '<depth index="...">|</depth>'
            },
            {
                name: "snippet",
                snippet: 'snippet id="$1">$0</snippet>',
                documentation: '<snippet id="...">|</snippet>'
            },
            {
                name: "frag",
                snippet: 'frag>$0</frag>',
                documentation: '<frag>|</frag>',
            },
            {
                name: "spinner",
                snippet: 'spinner type="$1" interval="$2" duration="$3" completed="$4"/>',
                documentation: '<spinner type="..." interval="..." duration="..." completed="..."/>'
            },
            {
                name: "hr",
                snippet: 'hr orientation="$1"/>',
                documentation: '<hr orientation="..."/>'
            },
        ];

        const filtered = tags.filter(t => t.name.startsWith(partial));

        return filtered.map(tag => {
            const item = new vscode.CompletionItem(
                tag.name,
                vscode.CompletionItemKind.Class
            );

            item.detail = "PTML Widget";
            item.documentation = tag.documentation;
            item.insertText = new vscode.SnippetString(tag.snippet);
            item.range = new vscode.Range(
                position.line,
                position.character - partial.length,
                position.line,
                position.character
            );

            return item;
        });
    }
}