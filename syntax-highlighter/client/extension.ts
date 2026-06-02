import * as path from "path"
import * as vscode from "vscode"
import { PTMLCompletionProvider } from "./providers/PTMLCompletionProvider";
import { PTMLAttributeValueCompletionProvider } from "./providers/PTMLAttributesValueCompletionProvider";
import { PTMLAttributeCompletionProvider } from "./providers/PTMLAttributesCompletionProvider";

import {
    LanguageClient,
    ServerOptions,
    LanguageClientOptions
} from "vscode-languageclient/node"
import { PTMLHeaderCompletionProvider } from "./providers/PTMLHeaderCompletionProvider";

let client: LanguageClient

export function activate(context: vscode.ExtensionContext) {
    const disposable = vscode.commands.registerCommand(
        'ptml.run',
        async () => {

            const editor = vscode.window.activeTextEditor;

            if (!editor) {
                return;
            }

            await editor.document.save();

            const file = editor.document.fileName;

            const terminal = vscode.window.createTerminal(
                "ptml run code"
            );

            terminal.show();
            terminal.sendText(
                `ptml run "${file}"`
            );
        }
    );

    const watch = vscode.commands.registerCommand(
        'ptml.watch',
        async () => {
            const editor = vscode.window.activeTextEditor;
            if (!editor) return;

            await editor.document.save();
            const file = editor.document.fileName;
            const terminal = vscode.window.createTerminal(
                "ptml watch code"
            )
            terminal.show();
            terminal.sendText(
                `ptml watch "${file}"`
            );
        }
    );

    const provider = vscode.languages.registerCompletionItemProvider(
        "ptml",
        new PTMLCompletionProvider(),
        "<",
        "a","b","c","d","e","f","g","h","i","j",
        "k","l","m","n","o","p","q","r","s","t",
        "u","v","w","x","y","z"
    );
        
    context.subscriptions.push(provider);
    context.subscriptions.push(disposable);
    context.subscriptions.push(watch);
    context.subscriptions.push(
        vscode.languages.registerCompletionItemProvider(
            "ptml",
            new PTMLAttributeValueCompletionProvider(),
            '"'
        )
    );
    context.subscriptions.push(
        vscode.languages.registerCompletionItemProvider(
            "ptml",
            new PTMLAttributeCompletionProvider(),
            " ",
            "-"
        )
    );
    context.subscriptions.push(
        vscode.languages.registerCompletionItemProvider(
            "ptml",
            new PTMLHeaderCompletionProvider(),
            "?"
        )
    );

    const serverExe = context.asAbsolutePath(
        path.join("server", "ptml-lsp.exe")
    )

    const serverOptions: ServerOptions = {
        run: {
            command: serverExe
        },
        debug: {
            command: serverExe
        }
    }

    const clientOptions: LanguageClientOptions = {
        documentSelector: [
            { scheme: "file", language: "ptml" }
        ]
    }

    client = new LanguageClient(
        "ptml",
        "PTML Language Server",
        serverOptions,
        clientOptions
    )

    //client.start()
}

export function deactivate() {}