module Parser
#load "token.fsx"
#load "lexer.fsx"
open System
open Token

let validTags = Set.ofList ["text"; "row"; "column"; "box"]
let colorValues = Set.ofList ["none"; "black"; "red"; "green"; "gold"; "blue"; "purple"; "cyan"; "fire"; "limegreen"; "yellow"; "lightblue"; "lilac"; "crystal"; "gray"; "lightgray"]
let overflowValues = Set.ofList ["break"; "wrap"; "cut"; "clip"]
let alignValues = Set.ofList ["start"; "center"; "end"]
let borderValues = Set.ofList ["single"; "double"; "bold"; "rounded"; "ascii"; "none"]
let terminalResizeValues = Set.ofList ["reflow"; "clip"; "static"]

let validAttributes = Map.ofList [
    "text", Map.ofList [
        "foreground", colorValues
        "background", colorValues
    ]
    "row", Map.ofList [
        "overflow", overflowValues
        "gap", Set.empty // special
        "align", alignValues
    ]
    "column", Map.ofList [
        "overflow", overflowValues
        "gap", Set.empty
        "y-align", alignValues
    ]
    "box", Map.ofList [
        "overflow", overflowValues
        "border", borderValues
        "width", Set.empty
        "height", Set.empty
        "border-color", colorValues
        "align", alignValues
    ]
]

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
    | Bold
    | Rounded
    | Ascii
    | NoBorder

type Widget =
    | TextWidget of text:string * foreground:string option * background:string option
    | RowWidget of width:Dimension * border:Border * gap:int * align:Align option * children:Widget list
    | ColumnWidget of width:Dimension * border:Border * gap:int * yAlign:Align option * children:Widget list
    | BoxWidget of width:Dimension * height:Dimension * border:Border * borderColor:string option * align:Align option * children:Widget list

let parsePiAttrs (pi: string) =
    let content = pi.[6..pi.Length - 3]
    let parts = content.Split(' ') |> Array.filter (fun s -> s <> "")
    let mutable attrs = []
    for part in parts do
        let eqIdx = part.IndexOf('=')
        if eqIdx > 0 then
            let name = part.[0..eqIdx - 1]
            let value = part.[eqIdx + 2..part.Length - 2]
            attrs <- (name, value) :: attrs
    attrs |> List.rev

let rec parser(tokens, stack ) =
    match tokens with
    | [] -> if stack <> [] then failwith "Unclosed tags" else ()
    | ProcInst pi :: rest ->
        let piAttrs = parsePiAttrs pi
        for (name, value) in piAttrs do
            match name with
            | "encoding" -> ()
            | "terminal-resize" -> if not (Set.contains value terminalResizeValues) then failwith $"Invalid terminal-resize: {value}"
            | _ -> failwith $"Invalid pi attribute: {name}"
        parser(rest, stack)
    | Comment _ :: rest -> parser(rest, stack)
    | Text _ :: rest -> parser(rest, stack)
    | StartTag (tag, selfClosing, attrs) :: rest ->
        if not (Set.contains tag validTags) then failwith $"Invalid tag: {tag}"
        match Map.tryFind tag validAttributes with
        | Some attrMap ->
            for (name, value) in attrs do
                match Map.tryFind name attrMap with
                | Some valueSet ->
                    if valueSet <> Set.empty then
                        if not (Set.contains value valueSet) then failwith $"Invalid value for {name}: {value}"
                    else
                        match name with
                        | "gap" -> 
                            let mutable i = 0
                            if not (System.Int32.TryParse(value, &i)) then failwith $"Gap must be int: {value}"
                        | "width" | "height" ->
                            if value <> "auto" && not (value.EndsWith "%") then
                                let mutable i = 0
                                if not (System.Int32.TryParse(value, &i)) then failwith $"Invalid {name}: {value}"
                        | _ -> ()
                | None -> failwith $"Invalid attribute for {tag}: {name}"
        | None -> ()
        if selfClosing then
            parser(rest, stack)
        else
            parser(rest, tag :: stack)
    | EndTag tag :: rest ->
        match stack with
        | top :: remaining when top = tag -> parser(rest, remaining)
        | _ -> failwith $"Mismatched end tag: {tag}"

let private normalizeText (text: string) =
    let trimmed = text.Trim()
    if trimmed = "" then None else Some trimmed

let private parseNodes tokens =
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

let buildAst tokens =
    let ast, remaining = parseNodes tokens
    match remaining with
    | [] -> ast
    | EndTag tag :: _ -> failwith $"Unexpected closing tag: {tag}"
    | _ -> failwith "Unable to parse tokens to AST"

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
    | "bold" -> Bold
    | "rounded" -> Rounded
    | "ascii" -> Ascii
    | "none" -> NoBorder
    | value -> failwith $"Invalid border value: {value}"

let private buildTextContent children =
    children
    |> List.choose (function
        | TextWidget(text, _, _) -> Some text
        | _ -> None)
    |> String.concat ""

let rec buildSemanticTree ast =
    ast |> List.map buildWidget

and buildWidget node =
    match node with
    | TextNode text -> TextWidget(text, None, None)
    | Element(tag, attrs, children) ->
        let childrenWidgets = buildSemanticTree children
        match tag with
        | "text" ->
            let fg = tryGetAttr "foreground" attrs
            let bg = tryGetAttr "background" attrs
            match childrenWidgets with
            | [TextWidget(text, _, _)] -> TextWidget(text, fg, bg)
            | _ -> TextWidget(buildTextContent childrenWidgets, fg, bg)
        | "row" ->
            let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue (Fixed 10)
            let border = tryGetAttr "border" attrs |> Option.map parseBorder |> Option.defaultValue Single
            let gap = parseIntAttr "gap" 0 attrs
            let align = tryGetAttr "align" attrs |> Option.map parseAlign
            RowWidget(width, border, gap, align, childrenWidgets)
        | "column" ->
            let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue (Fixed 10)
            let border = tryGetAttr "border" attrs |> Option.map parseBorder |> Option.defaultValue Single
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
        | _ ->
            failwith $"Unsupported semantic tag: {tag}"

let semanticTreeOfTokens tokens =
    tokens |> buildAst |> buildSemanticTree