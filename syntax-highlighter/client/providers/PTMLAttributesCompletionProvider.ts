import * as vscode from "vscode";
import { align, borders, colors, encoding, fonts, NaNValuesInNumericAttrs, nums, overflow } from "./PTMLAttributesValueCompletionProvider";

interface AttributeDefinition {
    name: string;
    values?: string[];
}

const widgetAttributes: Record<string, AttributeDefinition[]> = {
    "?ptml?": [
        {
            name: "terminal-resize",
            values: ["reflow", "static", "clip"]
        },
        {
            name: "encoding",
            values: encoding
        }
    ],

    terminal: [
        {
            name: "x-align",
            values: align
        },
        {
            name: "y-align",
            values: align
        }
    ],

    box: [
        {
            name: "border",
            values: borders
        },
        {
            name: "border-color",
            values: colors
        },
        {
            name: "overflow",
            values: overflow
        },
        {
            name: "width"
        },
        {
            name: "height"
        },
        {
            name: "align",
            values: align
        }
    ],

    block: [
        {
            name: "title"
        },
        {
            name: "border",
            values: borders
        },
        {
            name: "border-color",
            values: colors
        },
        {
            name: "overflow",
            values: overflow
        },
        {
            name: "width"
        },
        {
            name: "height"
        },
        {
            name: "align",
            values: align
        }
    ],

    column: [
        {
            name: "overflow",
            values: overflow
        },
        {
            name: "gap",
            values: NaNValuesInNumericAttrs
        },
        {
            name: "y-align",
            values: align
        }
    ],

    row: [
        {
            name: "overflow",
            values: overflow
        },
        {
            name: "gap",
            values: NaNValuesInNumericAttrs
        },
        {
            name: "align",
            values: align
        }
    ],

    depth: [
        {
            name: "index",
            values: nums && NaNValuesInNumericAttrs
        },
        {
            name: "overflow",
            values: overflow
        },
        {
            name: "gap",
            values: NaNValuesInNumericAttrs
        },
        {
            name: "z-align",
            values: align
        }
    ],

    text: [
        {
            name: "foreground",
            values: colors
        },
        {
            name: "background",
            values: colors
        },
        {
            name: "font",
            values: fonts
        }
    ]
};

export class PTMLAttributeCompletionProvider
    implements vscode.CompletionItemProvider {

    provideCompletionItems(
        document: vscode.TextDocument,
        position: vscode.Position
    ): vscode.CompletionItem[] {

        const line = document.lineAt(position.line).text;
        const beforeCursor = line.substring(0, position.character);

        const tagMatch = beforeCursor.match(/<(\w+)[^>]*$/);

        if (!tagMatch) {
            return [];
        }

        const tagName = tagMatch[1];

        const attributes = widgetAttributes[tagName];

        if (!attributes) {
            return [];
        }

        const partialMatch = beforeCursor.match(/\s([\w-]*)$/);

        const partial = partialMatch?.[1] ?? "";

        return attributes
            .filter(attr =>
                attr.name.startsWith(partial)
            )
            .map(attr => {

                const item = new vscode.CompletionItem(
                    attr.name,
                    vscode.CompletionItemKind.Property
                );

                item.insertText = new vscode.SnippetString(
                    `${attr.name}="$1"`
                );

                item.detail = `${tagName} attribute`;

                item.command = {
                    command: "editor.action.triggerSuggest",
                    title: "Suggest Values"
                };

                return item;
            });
    }
}