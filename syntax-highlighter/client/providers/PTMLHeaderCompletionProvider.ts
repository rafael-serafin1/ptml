import * as vscode from "vscode";

export class PTMLHeaderCompletionProvider
    implements vscode.CompletionItemProvider {

    provideCompletionItems(
        document: vscode.TextDocument,
        position: vscode.Position
    ): vscode.CompletionItem[] {

        const line = document.lineAt(position.line).text;
        const beforeCursor = line.substring(0, position.character);

        if (!beforeCursor.endsWith("??")) {
            return [];
        }

        const item = new vscode.CompletionItem(
            "PTML Header",
            vscode.CompletionItemKind.Snippet
        );

        item.detail = "Insert PTML document header";

        item.documentation =
            '<?ptml encoding="UTF-6" terminal-resize="static"?>';

        item.insertText = new vscode.SnippetString(
            '<?ptml encoding="UTF-6" terminal-resize="static"?>'
        );

        item.range = new vscode.Range(
            position.line,
            position.character - 2,
            position.line,
            position.character
        );

        return [item];
    }
}