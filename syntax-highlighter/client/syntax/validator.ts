import * as vscode from "vscode";
import * as sax from "sax";

export function validateSyntax(document: vscode.TextDocument): vscode.Diagnostic[] {

    const diagnostics: vscode.Diagnostic[] = [];

    const parser = sax.parser(true, {
        trim: false,
        normalize: false
    });

    parser.onerror = (err: any) => {
        const line = parser.line;
        const column = parser.column;

        diagnostics.push(new vscode.Diagnostic(
                new vscode.Range(line, column, line, column + 1),
                err.message,
                vscode.DiagnosticSeverity.Error
            )
        );

        parser.resume();
    };

    parser.write(document.getText()).close();
    return diagnostics;
}