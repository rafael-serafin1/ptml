import * as vscode from "vscode";

export const align = [
    "left",
    "center",
    "right"
];

export const borders = [
    "single",
    "double",
    "bold",
    "strange",
    "classic",
    "rounded",
    "ascii",
    "none"
]

export const colors = [
    "white",
    "lightgray",   
    "gray",   
    "black",
    "red",
    "fire",
    "green",
    "limegreen",   
    "yellow",   
    "gold",
    "blue",
    "cyan",
    "purple",
    "lilac",   
    "crystal",
    "lightblue"
];

export const fonts = [
    "none",
    "bold",
    "dim",
    "italic",
    "underline",
    "slow-blink",
    "rapid-blink",
    "reverse",
    "conceal",
    "strike-through"
]

export const overflow = [
    "break",
    "wrap",
    "cut",
    "clip"
]

export const NaNValuesInNumericAttrs = [
    "auto",
    "N%"
]
export const nums = [
    "0",
    "-1",
    "-2"
]

export const encoding = [
    "UTF-8",
    "UTF-16",
    "UTF-32"
]

export class PTMLAttributeValueCompletionProvider
    implements vscode.CompletionItemProvider {

    private readonly attributeValues: Record<string, string[]> = {
        "terminal-resize": [
            "reflow",
            "static",
            "clip"
        ],
        border: borders,
        font: fonts,
        foreground: colors,
        background: colors,
        "border-color": colors,

        align: align,
        "x-align": align,
        "y-align": align,
        "z-align": align,

        overflow: overflow,

        width: NaNValuesInNumericAttrs,
        height: NaNValuesInNumericAttrs,
        index: nums && NaNValuesInNumericAttrs
    };

    provideCompletionItems(document: vscode.TextDocument, position: vscode.Position): vscode.CompletionItem[] {

        const line = document.lineAt(position.line).text;
        const beforeCursor = line.substring(0, position.character);

        for (const [attribute, values] of Object.entries(this.attributeValues)) {

            const regex = new RegExp(`${attribute}="[^"]*$`);

            if (regex.test(beforeCursor)) {

                return values.map(value => {

                    const item = new vscode.CompletionItem(
                        value,
                        vscode.CompletionItemKind.EnumMember
                    );

                    item.insertText = value;
                    item.detail = `${attribute} value`;

                    return item;
                });
            }
        }

        return [];
    }
}