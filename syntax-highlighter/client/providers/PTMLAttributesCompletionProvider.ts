import * as vscode from "vscode";
import { align, boolean, borders, colors, completed, encoding, escapesSequence, fonts, NaNValuesInNumericAttrs, nums, ori, overflow, progressType, spinners } from "./PTMLAttributesValueCompletionProvider";

interface AttributeDefinition {
    name: string;
    values?: string[];
}

const globalAttributes = {
    name: "id",
    values: []
}

const globalAttributes2 = {
    name: "snippet",
    values: []
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
    "ptml": [
        {
            name: "terminal-resize",
            values: ["reflow", "static", "clip"]
        },
        {
            name: "encoding",
            values: encoding
        },
    ],

    terminal: [
        {
            name: "x-align",
            values: align
        },
        {
            name: "y-align",
            values: align
        },
        globalAttributes,   
        globalAttributes2
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
        },
        {
            name: "padding",
            values: NaNValuesInNumericAttrs
        },
        globalAttributes,   
        globalAttributes2
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
        },
        {
            name: "padding",
            values: NaNValuesInNumericAttrs
        },
        globalAttributes,   
        globalAttributes2
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
        },
        globalAttributes,   
        globalAttributes2
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
        },
        globalAttributes,   
        globalAttributes2
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
        },
        globalAttributes,   
        globalAttributes2
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
        },
        globalAttributes,   
        globalAttributes2
    ],

    frag: [
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
        },
        globalAttributes,   
        globalAttributes2
    ],

    snippet: [
        {
            name: "extends",
            values: []
        },
        globalAttributes,   
        globalAttributes2
    ],

    spinner: [
        {
            name: "type",
            values: spinners
        },
        {
            name: "interval",
            values: []
        },
        {
            name: "duration",
            values: []
        },
        {
            name: "completed",
            values: completed
        },
        globalAttributes,
        globalAttributes2
    ],
    hr: [
        {
            name: "orientation",
            values: ori
        },
        {
            name: "width",
        },
        {
            name: "height"
        },
        globalAttributes,   
        globalAttributes2
    ],
    progress: [
        {
            name: "style",
            values: progressType
        },
        {
            name: "width",
        },
        {
            name: "height"
        },
        {
            name: "value",
        }, 
        {
            name: "max"
        },
        {
            name: "show-value",
            values: boolean
        },
        globalAttributes,   
        globalAttributes2
    ],
    escape: [
        {
            name: "sequence",
            values: escapesSequence
        },
        {
            name: "multiplier"
        },
        globalAttributes,
        globalAttributes2
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

        const tagMatch = beforeCursor.match(/<\??([a-zA-Z][\w-]*)[^>]*$/);

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