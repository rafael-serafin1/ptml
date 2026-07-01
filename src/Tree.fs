namespace PTML
open System
open PTML.Token
open PTML.Parser
open PTML.Spinner
open PTML.Progress
open PTML.Escape

module Tree =
    type GlobalAttributes = {
        Id : string option
        Snippet : string option
    }

    type SnippetDefinition = {
        Id : string
        Extends : string option
        GlobalAttributes : GlobalAttributes
        Attributes : list<string * string>
    }

    type SnippetRegistry = Map<string, SnippetDefinition>

    type ValidationWarning ={
        Message : string
    }

    type ValidationError = {
        Message : string
    }

    type FragmentNode = {
            Foreground : string option
            Background : string option
            Font : string option
            Content : string
        }

    type TextContent =
        | RawText of string
        | Fragment of FragmentNode

    type AstNode =
        | Element of string * GlobalAttributes * list<string * string> * AstNode list
        | TextNode of TextContent list

    type Dimension =
        | Auto
        | Percent of int
        | Fixed of int

    type CellOrientation =
        | Horizontal
        | Vertical

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
        | Borderless
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

    type Orientation =
    | Vertical
    | Horizontal

    // discriminated union for semantic tree
    type Widget =
        | HrWidget of orientation: Orientation * width: Dimension * height: Dimension
        | TextWidget of text:string * foreground:string option * background:string option * font:string option
        | FragWidget of text:string * foreground:string option * background:string option * font:string option
        | RowWidget of width:Dimension * border:Border * gap:int * align:Align option * children:Widget list
        | ColumnWidget of width:Dimension * border:Border * gap:int * yAlign:Align option * children:Widget list
        | DepthWidget of index:int * zAlign: Align option * gap: int * children: Widget list
        | BoxWidget of width:Dimension * height:Dimension * border:Border * borderColor:string option * align:Align option * padding:int * int * children:Widget list
        | BlockWidget of width:Dimension * height:Dimension * border:Border * borderColor:string option * name:string option * align:Align option * padding:int * int * children:Widget list
        | CellWidget of children: Widget list
        | TerminalWidget of width: Dimension * height: Dimension * alignX: Align option * alignY: Align option * children: Widget list
        | SpinnerWidget of text:Types * interval: string * duration: string * completed: string * foreground:string option * background:string option
        | ProgressWidget of tp: ProgressType * value: int * max: int * width: Dimension * height: Dimension * show: string option
        | EscapeWidget of sequence: Escape.EscapeSequence * multiplier: int

    ///
    /// AST BUILDING
    /// 
    let private normalizeText (text: string) =
        if text = "" then
            None
        else
            let allWhitespace = text |> Seq.forall Char.IsWhiteSpace
            if allWhitespace then
                if text.Contains("\n") || text.Contains("\r") then
                    None
                else
                    Some text
            else
                if text.Contains("\n") || text.Contains("\r") || text.Contains("\t") then
                    let normalized =
                        text.Split([| ' '; '\n'; '\r'; '\t' |], System.StringSplitOptions.RemoveEmptyEntries)
                        |> String.concat " "
                    let trimmed = normalized.Trim()
                    if trimmed = "" then None else Some trimmed
                else
                    Some text

    let private emptyGlobalAttributes = { Id = None; Snippet = None }

    let private splitGlobalAttrs attrs =
        let id = attrs |> List.tryFind (fun (name, _) -> name = "id") |> Option.map snd
        let snippet = attrs |> List.tryFind (fun (name, _) -> name = "snippet") |> Option.map snd
        let filtered = attrs |> List.filter (fun (name, _) -> name <> "id" && name <> "snippet")
        ({ Id = id; Snippet = snippet }, filtered)

    let private parseNodes(tokens) =
        let tryGetAttrLocal name attrs =
            attrs |> List.tryFind (fun (k, _) -> k = name) |> Option.map snd

        let fragFromChildren children =
            children
            |> List.collect (function
                | TextNode contents -> contents
                | _ -> failwith "Invalid content inside <frag>."
            )
            |> List.map (function
                | RawText text -> text
                | Fragment _ -> failwith "Nested <frag> is not allowed."
            )
            |> String.concat ""

        let buildFragmentNode attrs children = {
            Foreground = tryGetAttrLocal "foreground" attrs
            Background = tryGetAttrLocal "background" attrs
            Font = tryGetAttrLocal "font" attrs
            Content = fragFromChildren children
        }

        let rec loop tokens acc =
            match tokens with
            | [] -> List.rev acc, []
            | EndTag _ :: _ -> List.rev acc, tokens
            | Comment _ :: rest -> loop rest acc
            | ProcInst _ :: rest -> loop rest acc
            | Text text :: rest ->
                match normalizeText text with
                | Some content -> loop rest (TextNode [RawText content] :: acc)
                | None -> loop rest acc
            | StartTag (tag, selfClosing, attrs) :: rest ->
                let globalAttrs, localAttrs = splitGlobalAttrs attrs
                if tag = "frag" then
                    if selfClosing then
                        let fragmentNode = buildFragmentNode localAttrs []
                        loop rest (TextNode [Fragment fragmentNode] :: acc)
                    else
                        let children, remaining = loop rest []
                        match remaining with
                        | EndTag endTag :: restAfter when endTag = tag ->
                            let fragmentNode = buildFragmentNode localAttrs children
                            loop restAfter (TextNode [Fragment fragmentNode] :: acc)
                        | EndTag endTag :: _ ->
                            failwith $"Mismatched end tag: expected </{tag}>, found </{endTag}>"
                        | _ ->
                            failwith $"Unclosed tag: {tag}"
                else
                    if selfClosing then
                        loop rest (Element(tag, globalAttrs, localAttrs, []) :: acc)
                    else
                        let children, remaining = loop rest []
                        match remaining with
                        | EndTag endTag :: restAfter when endTag = tag ->
                            loop restAfter (Element(tag, globalAttrs, localAttrs, children) :: acc)
                        | EndTag endTag :: _ ->
                            failwith $"Mismatched end tag: expected </{tag}>, found </{endTag}>"
                        | _ ->
                            failwith $"Unclosed tag: {tag}"
        loop tokens []

    let private emitWarning (warning: ValidationWarning) =
        System.Console.Error.WriteLine($"[warning] {warning.Message}")

    let private emitWarnings warnings =
        warnings |> List.rev |> List.iter emitWarning

    let private parseSnippetBodyAttrs children : list<string * string> =
        let rawText =
            children
            |> List.choose (function
                | TextNode contents ->
                    let text =
                        contents
                        |> List.choose (function
                            | RawText text -> Some text
                            | Fragment _ -> None)
                        |> String.concat ""
                    Some text
                | _ -> None)
            |> String.concat " "

        let tokens : string[] = rawText.Split([| ' '; '\n'; '\r'; '\t' |], System.StringSplitOptions.RemoveEmptyEntries)
        let tryParseToken (token: string) : (string * string) option =
            let eqIdx = token.IndexOf('=')
            if eqIdx > 0 && token.Length > eqIdx + 2 && token.[eqIdx + 1] = '"' && token.[token.Length - 1] = '"' then
                let name = token.[0..eqIdx - 1]
                let value = token.[eqIdx + 2..token.Length - 2]
                Some(name, value)
            else
                None
        tokens |> Array.choose tryParseToken |> Array.toList

    let private dedupeAttrsKeepLast attrs : list<string * string> =
        let folder (seen, result) (name, value) =
            if Set.contains name seen then
                (seen, result)
            else
                (Set.add name seen, (name, value) :: result)

        attrs
        |> List.rev
        |> List.fold folder (Set.empty, [])
        |> snd

    let private mergeAttributeLists baseAttrs overrides : list<string * string> =
        let overrideNames = overrides |> List.map fst |> Set.ofList
        let filteredAttrs =
            baseAttrs
            |> List.filter (fun (name, _) -> not (Set.contains name overrideNames))
        filteredAttrs @ overrides

    let private collectSnippetDefinitions ast : SnippetRegistry * ValidationError list =
        let rec loop nodes (registry: SnippetRegistry) (errors: ValidationError list) : SnippetRegistry * ValidationError list =
            match nodes with
            | [] -> registry, errors
            | TextNode _ :: rest -> loop rest registry errors
            | Element("snippet", globalAttrs, attrs, children) :: rest ->
                let bodyAttrs = parseSnippetBodyAttrs children
                let extends = attrs |> List.tryFind (fun (name, _) -> name = "extends") |> Option.map snd
                let localAttrs = attrs |> List.filter (fun (name, _) -> name <> "extends")
                let definitionAttrs = dedupeAttrsKeepLast (localAttrs @ bodyAttrs)
                match globalAttrs.Id with
                | Some id when id <> "" ->
                    if Map.containsKey id registry then
                        let duplicateSnippetError: ValidationError = { Message = $"Duplicate snippet id '{id}'" }
                        loop rest registry (duplicateSnippetError :: errors)
                    else
                        let definition = { Id = id; Extends = extends; GlobalAttributes = globalAttrs; Attributes = definitionAttrs }
                        loop rest (Map.add id definition registry) errors
                | _ ->
                    let missingSnippetIdError: ValidationError = { Message = "Snippet declaration must have a valid id." }
                    loop rest registry (missingSnippetIdError :: errors)
            | Element(_, _, _, children) :: rest ->
                let registry, errors = loop children registry errors
                loop rest registry errors
        loop ast Map.empty []

    let private mergeSnippetAttributes tag snippetAttrs explicitAttrs : list<string * string> * ValidationWarning list =
        let explicitNames = explicitAttrs |> List.map fst |> Set.ofList
        let folder (mergedAttrs, warnings) (name, value) =
            if Set.contains name explicitNames then
                mergedAttrs, warnings
            else
                match validateAttribute tag name value with
                | Valid -> (name, value) :: mergedAttrs, warnings
                | InvalidValue message -> failwith message
                | UnknownAttribute _ ->
                    let warning: ValidationWarning = { Message = $"O atributo '{name}' não existe para o elemento {tag}." }
                    mergedAttrs, warning :: warnings
        let initialState: list<string * string> * ValidationWarning list = ([], [])
        let merged, warnings = List.fold folder initialState snippetAttrs
        (List.rev merged) @ explicitAttrs, warnings

    let private resolveSnippets ast =
        let snippetRegistry, registryErrors = collectSnippetDefinitions ast
        if registryErrors <> [] then
            let messages = registryErrors |> List.rev |> List.map (fun e -> e.Message) |> String.concat "; "
            failwith messages

        let rec resolveSnippetAttributes snippetId visited : list<string * string> =
            if Set.contains snippetId visited then
                failwith $"Circular snippet inheritance detected for '{snippetId}'."
            else
                match Map.tryFind snippetId snippetRegistry with
                | None -> failwith $"Snippet '{snippetId}' não encontrado."
                | Some snippetDefinition ->
                    let parentAttrs =
                        match snippetDefinition.Extends with
                        | Some parentId -> resolveSnippetAttributes parentId (Set.add snippetId visited)
                        | None -> []
                    dedupeAttrsKeepLast (mergeAttributeLists parentAttrs snippetDefinition.Attributes)

        let rec resolveNode node =
            match node with
            | TextNode _ as textNode -> Some textNode
            | Element("snippet", _, _, _) ->
                // Snippet declarations are only a parser-time construct and do not survive into the final AST.
                None
            | Element(tag, globalAttrs, attrs, children) ->
                let resolvedChildren = children |> List.choose resolveNode
                match globalAttrs.Snippet with
                | Some snippetId ->
                    let resolvedSnippetAttrs = resolveSnippetAttributes snippetId Set.empty
                    let mergedAttrs, warnings = mergeSnippetAttributes tag resolvedSnippetAttrs attrs
                    emitWarnings warnings
                    let mergedGlobals = {
                        Id = globalAttrs.Id
                        Snippet = None
                    }
                    Some(Element(tag, mergedGlobals, mergedAttrs, resolvedChildren))
                | None -> Some(Element(tag, globalAttrs, attrs, resolvedChildren))

        ast
        |> List.choose resolveNode

    let private collectDuplicateIdWarnings ast : ValidationWarning list =
        let rec loop nodes (seen: Set<string>) (warnings: ValidationWarning list) : ValidationWarning list =
            match nodes with
            | [] -> warnings
            | TextNode _ :: rest -> loop rest seen warnings
            | Element("snippet", _, _, children) :: rest ->
                loop (children @ rest) seen warnings
            | Element(_, globalAttrs, _, children) :: rest ->
                let warnings =
                    match globalAttrs.Id with
                    | Some id when id <> "" && Set.contains id seen ->
                        let duplicateWarning: ValidationWarning = { Message = $"ID duplicado '{id}' encontrado." }
                        duplicateWarning :: warnings
                    | _ -> warnings
                let seen =
                    match globalAttrs.Id with
                    | Some id when id <> "" -> Set.add id seen
                    | _ -> seen
                loop (children @ rest) seen warnings
        loop ast Set.empty []

    let buildAst(tokens) =
        let ast, remaining = parseNodes(tokens)
        match remaining with
        | [] ->
            let resolvedAst = resolveSnippets ast
            let duplicateWarnings = collectDuplicateIdWarnings resolvedAst
            emitWarnings duplicateWarnings
            resolvedAst
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

    let private parsePadding (value: string) =
        let parsePart (part: string) =
            let mutable i : int = 0
            if System.Int32.TryParse(part, &i) && i >= 0 then i
            else failwith $"Invalid padding value: {value}"
        let parts : string[] = value.Split([| 'x' |])
        match parts with
        | [| part |] -> let p = parsePart part in (p, p)
        | [| vertical; horizontal |] -> (parsePart vertical, parsePart horizontal)
        | _ -> failwith $"Invalid padding format: {value}"

    let private parseHr = function
        | "vertical" -> Vertical
        | "horizontal" -> Horizontal
        | value -> failwith $"Invalid orientation value: {value}"

    let private parseBorder = function
        | "single" -> Single
        | "double" -> Double
        | "classic" -> Classic
        | "bold" -> Bold
        | "strange" -> Strange
        | "rounded" -> Rounded
        | "ascii" -> Border.Ascii
        | "none" -> NoBorder
        | "borderless" -> Borderless
        | value -> failwith $"Invalid border value: {value}"

    let private parseSpinnerType = function
        | "braille" -> Braille
        | "dots" -> Spinner.Dots
        | "waiting" -> Waiting
        | "burger" -> Burger
        | "beam" -> Beam
        | "ascii" -> Spinner.Ascii
        | "circle" -> Circle
        | "square" -> Spinner.Square
        | "moon" -> Moon
        | "arrow" -> Arrow
        | "bounce" -> Bounce
        | value -> failwith $"Invalid spinner type: {value}"

    let private parseProgressType = function
        | "blocks" -> Blocks
        | "dots" -> Dots
        | "square" -> Square
        | "tiny-square" -> TinySquare
        | "rhombus" -> Rhombus
        | value -> failwith $"Tipagem inexistente para <progress>: '{value}'"

    let private normalizeSpinnerCompleted value =
        if String.IsNullOrWhiteSpace(value) then
            "✓"
        else
        match value with
        | "check" -> "✓"
        | "error" -> "✖"
        | "star" -> "✱"
        | "cog" -> "⚙"
        | "bright" -> "✦"
        | _ -> value
        
    let private parseEscapeSequence = function
        | "break" -> EscapeSequence.Break
        | "horizontal-tab" -> HorizontalTab
        | "vertical-tab" -> VerticalTab
        | "audible-bell" -> AudibleBell
        | "backspace" -> BackSpace
        | "form-feed" -> FormFeed
        | "carriage-return" -> CarriageReturn
        | value -> failwith $"Tipagem inexistente para <escape>: '{value}'"

    let mutable previousTag: AstNode option = None

    let private buildTextNodeWidgets contents =
        contents
        |> List.collect (function
            | RawText text -> [ TextWidget(text, None, None, None) ]
            | Fragment fragment -> [ TextWidget(fragment.Content, fragment.Foreground, fragment.Background, fragment.Font) ]
        )

    let private applyTextStyle widget parentFg parentBg parentFont =
        match widget with
        | EscapeWidget(seq, multi) ->
            let char = concatEscapes(seq, multi)
            TextWidget(char, None, None, None)
        | TextWidget(text, fg, bg, font) ->
            let finalFg = if fg.IsSome then fg else parentFg
            let finalBg = if bg.IsSome then bg else parentBg
            let finalFont = if font.IsSome then font else parentFont
            TextWidget(text, finalFg, finalBg, finalFont)
        | _ -> failwith "Invalid child inside <text>."

    let private applyFragStyle widget parentFg parentBg parentFont =
        match widget with
        | EscapeWidget(seq, multi) ->
            let char = concatEscapes(seq, multi)
            FragWidget(char, None, None, None)
        | FragWidget(text, fg, bg, font) ->
            let finalFg = if fg.IsSome then fg else parentFg
            let finalBg = if bg.IsSome then bg else parentBg
            let finalFont = if font.IsSome then font else parentFont
            FragWidget(text, finalFg, finalBg, finalFont)
        | _ -> failwith "Invalid child inside <frag>."

    let rec buildSemanticTree ast =
        ast |> List.collect buildWidget

    /// Builds a Widget tree from a list of AST nodes
    and buildWidget node =
        match node with
        | TextNode contents -> buildTextNodeWidgets contents
        | Element(tag, _, attrs, children) ->
            match tag with
            | "hr" -> 
                let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let height = tryGetAttr "height" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let orientation: Orientation = tryGetAttr "orientation" attrs |> Option.map parseHr |> Option.defaultValue Orientation.Horizontal
                [ HrWidget(orientation, width, height) ]
            | "text" ->
                let parentFg = tryGetAttr "foreground" attrs
                let parentBg = tryGetAttr "background" attrs
                let parentFont = tryGetAttr "font" attrs
                let childrenWidgets = children |> List.collect buildWidget
                childrenWidgets
                |> List.map (fun widget -> applyTextStyle widget parentFg parentBg parentFont)
            | "frag" ->
                let parentFg = tryGetAttr "foreground" attrs
                let parentBg = tryGetAttr "background" attrs
                let parentFont = tryGetAttr "font" attrs
                let childrenWidgets = children |> List.collect buildWidget
                childrenWidgets
                |> List.map (fun widget -> applyFragStyle widget parentFg parentBg parentFont)
            | "row" ->
                let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let border = tryGetAttr "border" attrs |> Option.map parseBorder |> Option.defaultValue NoBorder
                let gap = parseIntAttr "gap" 0 attrs
                let align = tryGetAttr "align" attrs |> Option.map parseAlign
                let childrenWidgets = children |> List.collect buildWidget
                [ RowWidget(width, border, gap, align, childrenWidgets) ]
            | "column" ->
                let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let border = tryGetAttr "border" attrs |> Option.map parseBorder |> Option.defaultValue NoBorder
                let gap = parseIntAttr "gap" 0 attrs
                let yAlign = tryGetAttr "y-align" attrs |> Option.map parseAlign
                let childrenWidgets = children |> List.collect buildWidget
                [ ColumnWidget(width, border, gap, yAlign, childrenWidgets) ]
            | "layer" ->
                let index =
                    match tryGetAttr "index" attrs with
                    | Some value ->
                        let mutable i = 0
                        if System.Int32.TryParse(value, &i) then i
                        else failwith $"Invalid integer value for index: {value}"
                    | None -> failwith "Missing required attribute for depth: index"
                let gap = parseIntAttr "gap" 0 attrs
                let zAlign = tryGetAttr "z-align" attrs |> Option.map parseAlign
                let childrenWidgets = children |> List.collect buildWidget
                [ DepthWidget(index, zAlign, gap, childrenWidgets) ]
            | "box" ->
                let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let height = tryGetAttr "height" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let border = tryGetAttr "border" attrs |> Option.map parseBorder |> Option.defaultValue Single
                let borderColor = tryGetAttr "border-color" attrs
                let align = tryGetAttr "align" attrs |> Option.map parseAlign
                let paddingV, paddingH = tryGetAttr "padding" attrs |> Option.map parsePadding |> Option.defaultValue (0, 0)
                let childrenWidgets = children |> List.collect buildWidget
                [ BoxWidget(width, height, border, borderColor, align, paddingV, paddingH, childrenWidgets) ]
            | "block" ->
                let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let height = tryGetAttr "height" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let border = tryGetAttr "border" attrs |> Option.map parseBorder |> Option.defaultValue Single
                let borderColor = tryGetAttr "border-color" attrs
                let name = tryGetAttr "title" attrs
                let align = tryGetAttr "align" attrs |> Option.map parseAlign
                let paddingV, paddingH = tryGetAttr "padding" attrs |> Option.map parsePadding |> Option.defaultValue (0, 0)
                let childrenWidgets = children |> List.collect buildWidget
                [ BlockWidget(width, height, border, borderColor, name, align, paddingV, paddingH, childrenWidgets) ]
            | "cell" ->
                let childrenWidgets = children |> List.collect buildWidget
                [ CellWidget(childrenWidgets) ]
            | "terminal" ->
                let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let height = tryGetAttr "height" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let xAlign = tryGetAttr "x-align" attrs |> Option.map parseAlign
                let yAlign = tryGetAttr "y-align" attrs |> Option.map parseAlign
                let childrenWidgets = children |> List.collect buildWidget
                [ TerminalWidget(width, height, xAlign, yAlign, childrenWidgets) ]
            | "spinner" ->
                let spinnerType = tryGetAttr "type" attrs |> Option.map parseSpinnerType |> Option.defaultValue Braille
                let interval = tryGetAttr "interval" attrs |> Option.defaultValue "250ms"
                let duration = tryGetAttr "duration" attrs |> Option.defaultValue "3000ms"
                let completed = tryGetAttr "completed" attrs |> Option.map normalizeSpinnerCompleted |> Option.defaultValue "✓"
                let foreground = tryGetAttr "foreground" attrs
                let background = tryGetAttr "background" attrs
                [ SpinnerWidget(spinnerType, interval, duration, completed, foreground, background) ]
            | "progress" ->
                let progressType = tryGetAttr "style" attrs |> Option.map parseProgressType |> Option.defaultValue Progress.Blocks
                let value = tryGetAttr "value" attrs |> Option.defaultValue "0" |> System.Int32.Parse
                let max = tryGetAttr "max" attrs |> Option.defaultValue "100" |> System.Int32.Parse
                let width = tryGetAttr "width" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let height = tryGetAttr "height" attrs |> Option.map parseDimension |> Option.defaultValue Auto
                let show = tryGetAttr "show-value" attrs |> Option.defaultValue "false"
                [ ProgressWidget(progressType, value, max, width, height, Some show) ]
            | "escape" ->
                let sequence = tryGetAttr "sequence" attrs |> Option.map parseEscapeSequence |> Option.defaultValue EscapeSequence.Break
                let multiplier = parseIntAttr "multiplier" 1 attrs
                [ EscapeWidget(sequence, multiplier) ]
            | _ ->
                failwith $"Unsupported semantic tag: {tag}"

    let semanticTreeOfTokens tokens =
        tokens |> buildAst |> buildSemanticTree