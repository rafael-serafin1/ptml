import * as vscode from "vscode";
import { align, basicColors, boolean, borders, charcodiname, colors, completed, consoleInstance, contentHandlers, cursorColor, encoding, escapesSequence, fonts, frameworks, NaNValuesInNumericAttrs, nums, ofstyles, ori, overflow, progressType, shape, spinners } from "./PTMLAttributesValueCompletionProvider";

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

const globalAttributes3 = {
    name: "content",
    values: contentHandlers
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
        },
        {
            name: "console-instance",
            values: consoleInstance
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
        {
            name: "window-title",
            values: []
        },
        {
            name: "icon-name",
            values: []
        },
        {
            name: "foreground",
            values: basicColors
        },
        {
            name: "background",
            values: basicColors
        },
        globalAttributes,   
        globalAttributes2,
        globalAttributes3
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
        globalAttributes2,
        globalAttributes3
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
        globalAttributes2,
        globalAttributes3
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
        globalAttributes2,
        globalAttributes3
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
        globalAttributes2,
        globalAttributes3
    ],

    layer: [
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
        globalAttributes2,
        globalAttributes3
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
        {
            name: "url",
            values: [""]
        },
        globalAttributes,   
        globalAttributes2,
        globalAttributes3
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
        {
            name: "url",
            values: [""]
        },
        globalAttributes,   
        globalAttributes2,
        globalAttributes3
    ],

    snippet: [
        {
            name: "extends",
            values: []
        },
        globalAttributes,   
        globalAttributes2,
        globalAttributes3
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
        globalAttributes2,
        globalAttributes3
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
        globalAttributes2,
        globalAttributes3
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
        globalAttributes2,
        globalAttributes3
    ],
    frame: [
        {
            name: "framework",
            values: frameworks
        },
        {
            name: "frame-color",
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
        globalAttributes2,
        globalAttributes3
    ],
    cursor: [
        {
            name: "visible",
            values: boolean
        },
        {
            name: "blink",
            values: boolean
        },
        {
            name: "shape",
            values: shape
        },
        {
            name: "color",
            values: cursorColor
        },
        globalAttributes,   
        globalAttributes2,
        globalAttributes3
    ],
    tree: [
        {
            name: "src",
        },
        {
            name: "root-limit",
        },
        globalAttributes,   
        globalAttributes2,
        globalAttributes3
    ],
    list: [
        {
            name: "ofstyle",
            values: ofstyles
        },
        {
            name: "before"
        },
        {
            name: "after"
        },
        globalAttributes,   
        globalAttributes2,
        globalAttributes3
    ],
    char: [
        {
            name: "codename",
            values: charcodiname
        },
        globalAttributes,   
        globalAttributes2,
        globalAttributes3
    ]
};

export class PTMLAttributeCompletionProvider
    implements vscode.CompletionItemProvider {

    provideCompletionItems(document: vscode.TextDocument, position: vscode.Position): vscode.CompletionItem[] {
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