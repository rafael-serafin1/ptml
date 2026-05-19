namespace PTML
open PTML.Token

module Tree =
    type AstNode =
        | Element of string * list<string * string> * AstNode list
        | TextNode of string

    type Dimension =
        | Auto
        | Percent of int
        | Fixed of int

    type Align =
        | Start
        | Center
        | End

    type Border =
        | Single
        | Double
        | Classic
        | Bold
        | Strange
        | Rounded
        | Ascii
        | NoBorder

    type Colors =
        | None = 0
        | Black = 30
        | Red = 31
        | Green = 32
        | Gold = 33
        | Blue = 34
        | Purple = 35
        | Cyan = 36
        | White = 37

    type Fonts =
        | None = 0
        | Bold = 1
        | Dim = 2
        | Italic = 3
        | UnderLine = 4
        | SlowBlink = 5
        | RapidBlink = 6
        | Marked = 7
        | Conceal = 8
        | StrikeThrough = 9

    // discriminated union for semantic tree
    type Widget =
        | TextWidget of text:string * foreground:string option * background:string option * font:string option
        | RowWidget of width:Dimension * border:Border * gap:int * align:Align option * children:Widget list
        | ColumnWidget of width:Dimension * border:Border * gap:int * yAlign:Align option * children:Widget list
        | BoxWidget of width:Dimension * height:Dimension * border:Border * borderColor:string option * align:Align option * children:Widget list
        | BlockWidget of width:Dimension * height:Dimension * border:Border * borderColor:string option * name:string option * align:Align option * children:Widget list
        | TerminalWidget of width: Dimension * height: Dimension * alignX: Align option * alignY: Align option * children: Widget list

    ///
    /// AST BUILDING
    /// 
    let private normalizeText (text: string) =
        let trimmed = text.Trim()
        if trimmed = "" then None else Some trimmed

    let private parseNodes(tokens) =
        let rec loop tokens acc =
            match tokens with
            | [] -> List.rev acc, []
            | EndTag _ :: _ -> List.rev acc, tokens
            | Comment _ :: rest -> loop rest acc
            | ProcInst _ :: rest -> loop rest acc
            | Text text :: rest ->
                match normalizeText text with
                | Some content -> loop rest (TextNode content :: acc)
                | None -> loop rest acc
            | StartTag (tag, selfClosing, attrs) :: rest ->
                if selfClosing then
                    loop rest (Element(tag, attrs, []) :: acc)
                else
                    let children, remaining = loop rest []
                    match remaining with
                    | EndTag endTag :: restAfter when endTag = tag ->
                        loop restAfter (Element(tag, attrs, children) :: acc)
                    | EndTag endTag :: _ ->
                        failwith $"Mismatched end tag: expected </{tag}>, found </{endTag}>"
                    | _ ->
                        failwith $"Unclosed tag: {tag}"
        loop tokens []

    let buildAst(tokens) =
        let ast, remaining = parseNodes(tokens)
        match remaining with
        | [] -> ast
        | EndTag tag :: _ -> failwith $"Unexpected closing tag: {tag}"
        | _ -> failwith "Unable to parse tokens to AST"


    //
    // SEMANTIC TREE BUILDING
    //
    let private tryGetAttr name attrs =
        attrs |> List.tryFind (fun (k, _) -> k = name) |> Option.map snd

    let private parseIntAttr name defaultValue attrs =
        match tryGetAttr name attrs with
        | Some value ->
            let strValue : string = value
            let mutable i : int = 0
            if System.Int32.TryParse(strValue, &i) then i else failwith $"Invalid integer value for {name}: {value}"
        | None -> defaultValue

    let private parseDimension value =
        if value = "auto" then Auto
        elif value.EndsWith("%") then
            let numStr : string = value.[0..value.Length - 2]
            let mutable i : int = 0
            if System.Int32.TryParse(numStr, &i) then Percent i else failwith $"Invalid percentage: {value}"
        else
            let strValue : string = value
            let mutable i : int = 0
            if System.Int32.TryParse(strValue, &i) then Fixed i else failwith $"Invalid dimension: {value}"

    let private parseAlign = function
        | "start" -> Start
        | "center" -> Center
        | "end" -> End
        | value -> failwith $"Invalid align value: {value}"

    let private parseBorder = function
        | "single" -> Single
        | "double" -> Double
        | "classic" -> Classic
        | "bold" -> Bold
        | "strange" -> Strange
        | "rounded" -> Rounded
        | "ascii" -> Ascii
        | "none" -> NoBorder
        | value -> failwith $"Invalid border value: {value}"

    let private buildTextContent children =
        children
        |> List.choose (function
            | TextWidget(text, _, _, _) -> Some text
            | _ -> None)
        |> String.concat ""

    /// builder
    let rec buildSemanticTree ast =
        ast |> List.map buildWidget

    /// Builds a Widget tree from a list of AST nodes
    and buildWidget node =
        match node with
        | TextNode text -> TextWidget(text, None, None, None)
        | Element(tag, attrs, children) ->
            let childrenWidgets = buildSemanticTree children
            match tag with
            | "text" ->
                let fg = tryGetAttr "foreground" attrs
                let bg = tryGetAttr "background" attrs
                let font = tryGetAttr "font" attrs
                match childrenWidgets with
                | [TextWidget(text, _, _, _)] -> TextWidget(text, fg, bg, font)
                | _ -> TextWidget(buildTextContent childrenWidgets, fg, bg, font)
            | "row" ->
                let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let border = tryGetAttr "border" attrs |> Option.map parseBorder |> Option.defaultValue NoBorder
                let gap = parseIntAttr "gap" 0 attrs
                let align = tryGetAttr "align" attrs |> Option.map parseAlign
                RowWidget(width, border, gap, align, childrenWidgets)
            | "column" ->
                let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let border = tryGetAttr "border" attrs |> Option.map parseBorder |> Option.defaultValue NoBorder
                let gap = parseIntAttr "gap" 0 attrs
                let yAlign = tryGetAttr "y-align" attrs |> Option.map parseAlign
                ColumnWidget(width, border, gap, yAlign, childrenWidgets)
            | "box" ->
                let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let height = tryGetAttr "height" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let border = tryGetAttr "border" attrs |> Option.map parseBorder |> Option.defaultValue Single
                let borderColor = tryGetAttr "border-color" attrs
                let align = tryGetAttr "align" attrs |> Option.map parseAlign
                BoxWidget(width, height, border, borderColor, align, childrenWidgets)
            | "block" ->
                let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let height = tryGetAttr "height" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let border = tryGetAttr "border" attrs |> Option.map parseBorder |> Option.defaultValue Single
                let borderColor = tryGetAttr "border-color" attrs
                let name = tryGetAttr "title" attrs
                let align = tryGetAttr "align" attrs |> Option.map parseAlign
                BlockWidget(width, height, border, borderColor, name, align, childrenWidgets)
            | "terminal" ->
                let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let height = tryGetAttr "height" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let xAlign = tryGetAttr "x-align" attrs |> Option.map parseAlign
                let yAlign = tryGetAttr "y-align" attrs |> Option.map parseAlign
                TerminalWidget(width, height, xAlign, yAlign, childrenWidgets)
            | _ ->
                failwith $"Unsupported semantic tag: {tag}"

    let semanticTreeOfTokens tokens =
        tokens |> buildAst |> buildSemanticTree