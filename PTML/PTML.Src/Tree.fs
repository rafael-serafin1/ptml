namespace PTML
open System
open System.Threading

open PTML.Token
open PTML.Parser
open PTML.Spinner
open PTML.Progress
open PTML.Escape
open PTML.Overflow
open PTML.Code
open PTML.Hyper
open PTML.Char
open PTML.Global
open PTML.Snippet
open PTML.Id
open PTML.Parsers


module Tree =
    // discriminated union for semantic tree
    type Widget =
        | CharWidget of codename: CharCodename
        | HrWidget of orientation: Orientation * width: Dimension * height: Dimension
        | TextWidget of
            text      : string *
            foreground: string option *
            background: string option *
            font      : string option *
            url       : string option
        | FragWidget of
            text      : string *
            foreground: string option *
            background: string option *
            font      : string option *
            url       : string option
        | RowWidget of
            width   : Dimension *
            overflow: OverflowValues *
            border  : Border *
            gap     : int *
            align   : Align option *
            children: Widget list
        | ColumnWidget of
            width   : Dimension *
            overflow: OverflowValues *
            border  : Border *
            gap     : int *
            yAlign  : Align option *
            children: Widget list
        | DepthWidget of 
            index   : int *
            overflow: OverflowValues *
            zAlign  : Align option *
            gap     : int *
            children: Widget list
        | BoxWidget of
            width      : Dimension *
            height     : Dimension *
            overflow   : OverflowValues *
            border     : Border *
            borderColor: string option *
            align      : Align option *
            padding    : int * int *
            children: Widget list
        | BlockWidget of
            width      : Dimension *
            height     : Dimension *
            overflow   : OverflowValues *
            border     : Border *
            borderColor: string option *
            name       : string option *
            align      : Align option *
            padding    : int * int *
            children: Widget list
        | CellWidget of 
            children: Widget list
        | TerminalWidget of
            width     : Dimension *
            height    : Dimension *
            foreground: string option *
            background: string option *
            alignX    : Align option *
            alignY    : Align option *
            windowT   : string *
            tabT      : string *
            children  : Widget list
        | SpinnerWidget of
            text      : Types *
            interval  : string *
            duration  : string *
            completed : string *
            foreground: string option *
            background: string option
        | ProgressWidget of
          tp    : ProgressType *
          value : int *
          max   : int *
          width : Dimension *
          height: Dimension *
          show  : string option
        | EscapeWidget of 
            sequence: Escape.EscapeSequence * multiplier: int
        | CursorWidget of 
            shape   : Cursor.Shape * blink              : string option * color: string option * visible: string option
        | FrameWidget of
            tp      : Frames.FrameWorks *
            color   : string option *
            width   : Dimension *
            heigth  : Dimension *
            overflow: OverflowValues *
            align   : Align option *
            padding : int * int *
            children: Widget list

    ///
    let getRawText (ast: AstNode) : string =
        match ast with
        | TextNode content ->
            content
            |> List.map (fun (c: TextContent) ->
                match c with
                | RawText raw -> raw
                | Fragment(f: FragmentNode) -> f.Content)
            |> String.concat "\n"
        | Element(name, globalAttributes, attributes, children) -> ""

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
            else if text.Contains("\n") || text.Contains("\r") || text.Contains("\t") then
                let normalized =
                    text.Split([| ' '; '\n'; '\r'; '\t' |], System.StringSplitOptions.RemoveEmptyEntries)
                    |> String.concat " "

                let trimmed = normalized.Trim()
                if trimmed = "" then None else Some trimmed
            else
                Some(text.Trim())

    let private parseNodes (tokens) =
        let tryGetAttrLocal name attrs =
            attrs |> List.tryFind (fun (k, _) -> k = name) |> Option.map snd

        let fragFromChildren children =
            children
            |> List.collect (function
                | TextNode contents -> contents
                | _ -> failwith "Invalid content inside <frag>.")
            |> List.map (function
                | RawText text -> text
                | Fragment _ -> failwith "Nested <frag> is not allowed.")
            |> String.concat ""

        let buildFragmentNode attrs children = {
            Url        = tryGetAttrLocal "url" attrs
            Foreground = tryGetAttrLocal "foreground" attrs
            Background = tryGetAttrLocal "background" attrs
            Font       = tryGetAttrLocal "font" attrs
            Content    = fragFromChildren children
        }

        let rec loop tokens acc =
            match tokens with
            | [] -> List.rev acc, []
            | EndTag _ :: _ -> List.rev acc, tokens
            | Comment _ :: rest -> loop rest acc
            | ProcInst _ :: rest -> loop rest acc
            | Text text :: rest ->
                match normalizeText text with
                | Some content ->
                    let preserveLeadingSpace =
                        match acc with
                        | Element("char", _, _, _) :: _ -> text.Length > 0 && Char.IsWhiteSpace(text.[0])
                        | _ -> false

                    let preserveTrailingSpace =
                        match rest with
                        | StartTag("char", _, _) :: _ -> text.Length > 0 && Char.IsWhiteSpace(text.[text.Length - 1])
                        | _ -> false

                    let content =
                        if preserveLeadingSpace && not (content.StartsWith(" ")) then
                            " " + content
                        else
                            content

                    let content =
                        if preserveTrailingSpace && not (content.EndsWith(" ")) then
                            content + " "
                        else
                            content

                    loop rest (TextNode [ RawText content ] :: acc)
                | None -> loop rest acc
            | StartTag(tag, selfClosing, attrs) :: rest ->
                let globalAttrs, localAttrs = splitGlobalAttrs attrs

                if tag = "frag" then
                    if selfClosing then
                        let fragmentNode = buildFragmentNode localAttrs []
                        loop rest (TextNode [ Fragment fragmentNode ] :: acc)
                    else
                        let children, remaining = loop rest []

                        match remaining with
                        | EndTag endTag :: restAfter when endTag = tag ->
                            let fragmentNode = buildFragmentNode localAttrs children
                            loop restAfter (TextNode [ Fragment fragmentNode ] :: acc)
                        | EndTag endTag :: _ -> failwith $"Mismatched end tag: expected </{tag}>, found </{endTag}>"
                        | _ -> failwith $"Unclosed tag: {tag}"
                else if selfClosing then
                    loop rest (Element(tag, globalAttrs, localAttrs, []) :: acc)
                else
                    let children, remaining = loop rest []

                    match remaining with
                    | EndTag endTag :: restAfter when endTag = tag ->
                        loop restAfter (Element(tag, globalAttrs, localAttrs, children) :: acc)
                    | EndTag endTag :: _ -> failwith $"Mismatched end tag: expected </{tag}>, found </{endTag}>"
                    | _ -> failwith $"Unclosed tag: {tag}"

        loop tokens []

    let buildAst (tokens) =
        let ast, remaining = parseNodes (tokens)

        match remaining with
        | [] ->
            let resolvedAst = resolveSnippets ast
            let duplicateWarnings = collectDuplicateIdWarnings resolvedAst
            emitWarnings duplicateWarnings
            resolvedAst
        | EndTag tag :: _ -> failwith $"Unexpected closing tag: {tag}"
        | _ -> failwith "Unable to parse tokens to AST"

//#region DOM_BUILDING
    let buildDom (ast: AstNode list) : PTML.DOM.Builder.PTMLDocument =
        let inputAttributes attributes =
            attributes
            |> List.rev
            |> List.map (fun (name, value) -> name, value)
            |> Map.ofList

        let fragmentAttributes (fragment: FragmentNode) =
            [
                fragment.Foreground |> Option.map (fun value -> "foreground", value)
                fragment.Background |> Option.map (fun value -> "background", value)
                fragment.Font |> Option.map (fun value -> "font", value)
                fragment.Url |> Option.map (fun value -> "url", value)
            ]
            |> List.choose id
            |> Map.ofList

        let globalAttributes (attributes: GlobalAttributes) =
            [
                attributes.Id |> Option.map (fun value -> "id", value)
                attributes.Snippet |> Option.map (fun value -> "snippet", value)
                attributes.Content
                |> Option.map (function
                    | Clipboard -> "content", "clipboard"
                    | Primary -> "content", "primary"
                    | Secondary -> "content", "secondary")
            ]
            |> List.choose id

        let rec toInputNodes (node: AstNode) : PTML.DOM.Builder.InputNode list =
            match node with
            | TextNode contents ->
                contents
                |> List.map (function
                    | RawText text -> PTML.DOM.Builder.InputText text
                    | Fragment fragment ->
                        PTML.DOM.Builder.InputElement(
                            "frag",
                            fragmentAttributes fragment,
                            [ PTML.DOM.Builder.InputText fragment.Content ]
                        ))
            | Element(tag, globals, attributes, children) ->
                let attributes =
                    Map.ofList (globalAttributes globals @ (inputAttributes attributes |> Map.toList))

                [
                    PTML.DOM.Builder.InputElement(
                        tag,
                        attributes,
                        children |> List.collect toInputNodes
                    )
                ]

        ast
        |> List.collect toInputNodes
        |> PTML.DOM.Builder.construct

    let private astFromDom (document: PTML.DOM.Builder.PTMLDocument) =
        let getNode (id: PTML.DOM.Builder.NodeId) : PTML.DOM.Builder.PTMLNode =
            Map.tryFind id document.Nodes
            |> Option.defaultWith (fun () -> failwith $"DOM node {id} does not exist.")

        let childrenOf (node: PTML.DOM.Builder.PTMLNode) =
            node.Children |> List.map getNode

        let attributesOfNode (attributes: Map<string, string>) : GlobalAttributes * list<string * string> =
            let content =
                match Map.tryFind "content" attributes with
                | None -> None
                | Some value ->
                    Some(
                        tryMatchContentAttrValue (Some value)
                        |> Option.defaultWith (fun () -> failwith $"Invalid DOM content attribute: {value}")
                    )

            let globals = {
                Id = Map.tryFind "id" attributes
                Snippet = Map.tryFind "snippet" attributes
                Content = content
            }

            let localAttributes =
                attributes
                |> Map.remove "id"
                |> Map.remove "snippet"
                |> Map.remove "content"
                |> Map.toList

            globals, localAttributes

        let fragmentOfNode (node: PTML.DOM.Builder.PTMLNode) : FragmentNode =
            let allowedAttributes = set [ "foreground"; "background"; "font"; "url" ]
            let unsupportedAttributes =
                node.Attributes
                |> Map.keys
                |> Seq.filter (fun name -> not (Set.contains name allowedAttributes))
                |> Seq.toList

            if not (List.isEmpty unsupportedAttributes) then
                let names = String.concat ", " unsupportedAttributes
                failwith $"Invalid attribute on DOM fragment: {names}"

            let content =
                childrenOf node
                |> List.map (fun child ->
                    match child.Kind, child.Children with
                    | PTML.DOM.Builder.NodeKind.TextNode text, [] -> text
                    | _ -> failwith "DOM fragments can only contain text nodes.")
                |> String.concat ""

            {
                Url = Map.tryFind "url" node.Attributes
                Foreground = Map.tryFind "foreground" node.Attributes
                Background = Map.tryFind "background" node.Attributes
                Font = Map.tryFind "font" node.Attributes
                Content = content
            }

        let rec toAst (node: PTML.DOM.Builder.PTMLNode) : AstNode list =
            match node.Kind with
            | PTML.DOM.Builder.NodeKind.DocumentNode ->
                if not (Map.isEmpty node.Attributes) then
                    failwith "The DOM document node cannot have attributes."

                childrenOf node |> List.collect toAst
            | PTML.DOM.Builder.NodeKind.TextNode text ->
                if not (List.isEmpty node.Children) then
                    failwith "DOM text nodes cannot have children."

                [ PTML.Token.TextNode [ RawText text ] ]
            | PTML.DOM.Builder.NodeKind.ElementNode tag ->
                let childNodes = childrenOf node

                let astChildren =
                    if tag = "text" then
                        childNodes
                        |> List.collect (fun child ->
                            match child.Kind with
                            | PTML.DOM.Builder.NodeKind.ElementNode "frag" ->
                                [ PTML.Token.TextNode [ Fragment(fragmentOfNode child) ] ]
                            | PTML.DOM.Builder.NodeKind.TextNode text ->
                                if not (List.isEmpty child.Children) then
                                    failwith "DOM text nodes cannot have children."

                                [ PTML.Token.TextNode [ RawText text ] ]
                            | _ -> toAst child)
                    else
                        childNodes |> List.collect toAst

                let globals, attributes = attributesOfNode node.Attributes
                [ Element(tag, globals, attributes, astChildren) ]

        let root = getNode document.Root

        match root.Kind with
        | PTML.DOM.Builder.NodeKind.DocumentNode -> toAst root
        | _ -> failwith "The DOM root must be a document node."
//#endregion

//#region SEMANTIC_TREE
    let private buildTextNodeWidgets contents =
        contents
        |> List.collect (function
            | RawText text -> [ TextWidget(text, None, None, None, None) ]
            | Fragment fragment ->
                [
                    TextWidget(fragment.Content, fragment.Foreground, fragment.Background, fragment.Font, fragment.Url)
                ])

    let private applyTextStyle widget parentFg parentBg parentFont parentUrl =
        match widget with
        | CharWidget codename ->
            let text = string (matchCodename codename)
            TextWidget(text, parentFg, parentBg, parentFont, parentUrl)
        | EscapeWidget(seq, multi) ->
            let char = concatEscapes (seq, multi)
            TextWidget(char, None, None, None, None)
        | TextWidget(text, fg, bg, font, url) ->
            let finalFg = if fg.IsSome then fg else parentFg
            let finalBg = if bg.IsSome then bg else parentBg
            let finalFont = if font.IsSome then font else parentFont
            let finalUrl = if url.IsSome then url else parentUrl
            TextWidget(text, finalFg, finalBg, finalFont, finalUrl)
        | _ -> failwith "Invalid child inside <text>."

    let rec private mergeAdjacentTextWidgets widgets =
        match widgets with
        | TextWidget(text, fg, bg, font, url) :: TextWidget(nextText, nextFg, nextBg, nextFont, nextUrl) :: rest when
            fg = nextFg && bg = nextBg && font = nextFont && url = nextUrl
            ->
            mergeAdjacentTextWidgets (TextWidget(text + nextText, fg, bg, font, url) :: rest)
        | widget :: rest -> widget :: mergeAdjacentTextWidgets rest
        | [] -> []

    let private applyFragStyle widget parentFg parentBg parentFont parentUrl =
        match widget with
        | EscapeWidget(seq, multi) ->
            let char = concatEscapes (seq, multi)
            FragWidget(char, None, None, None, None)
        | FragWidget(text, fg, bg, font, url) ->
            let finalFg = if fg.IsSome then fg else parentFg
            let finalBg = if bg.IsSome then bg else parentBg
            let finalFont = if font.IsSome then font else parentFont
            let finalUrl = if url.IsSome then url else parentUrl
            FragWidget(text, finalFg, finalBg, finalFont, url)
        | _ -> failwith "Invalid child inside <frag>."

    let rec buildSemanticTree ast = ast |> List.collect buildWidget

    /// Builds a Widget tree from a list of AST nodes
    and buildWidget node =
        match node with
        | TextNode contents -> buildTextNodeWidgets contents
        | Element(tag, _, attrs, children) ->
            match tag with
            | "tree" ->
                if not (List.isEmpty children) then
                    failwith "Element <tree> cannot contain child elements or text."

                let path =
                    tryGetAttr "path" attrs
                    |> Option.defaultWith (fun () -> failwith "Missing required attribute for tree: path")

                let rootLimit = parseIntAttr "root-limit" 5 attrs

                let children =
                    DirTree.render path rootLimit
                    |> List.map (fun line -> TextWidget(line, None, None, None, None))

                [ ColumnWidget(Auto, OverflowValues.Break, NoBorder, 0, None, children) ]
            | "list" ->
                let style =
                    tryGetAttr "ofstyle" attrs
                    |> Option.defaultValue "bullet"
                    |> PTML.List.parseStyle

                let before = tryGetAttr "before" attrs |> Option.defaultValue ""
                let after = tryGetAttr "after" attrs |> Option.defaultValue ""
                let childrenWidgets = children |> List.collect buildWidget

                let decorateItems items =
                    items
                    |> List.mapi (fun index item ->
                        let prefix = PTML.List.formatItem style before after (index + 1)

                        RowWidget(
                            Auto,
                            OverflowValues.Break,
                            NoBorder,
                            0,
                            None,
                            [ TextWidget(prefix, None, None, None, None); item ]
                        ))

                match childrenWidgets with
                | [ ColumnWidget(width, overflow, border, gap, yAlign, items) ] ->
                    [ ColumnWidget(width, overflow, border, gap, yAlign, decorateItems items) ]
                | [ RowWidget(width, overflow, border, gap, align, items) ] ->
                    [ RowWidget(width, overflow, border, gap, align, decorateItems items) ]
                | items ->
                    [
                        RowWidget(Auto, OverflowValues.Break, NoBorder, 0, None, decorateItems items)
                    ]
            | "hr" ->
                let width =
                    tryGetAttr "width" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let height =
                    tryGetAttr "height" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let orientation: Orientation =
                    tryGetAttr "orientation" attrs
                    |> Option.map parseHr
                    |> Option.defaultValue Orientation.Horizontal

                [ HrWidget(orientation, width, height) ]
            | "text" ->
                let parentFg = tryGetAttr "foreground" attrs
                let parentBg = tryGetAttr "background" attrs
                let parentFont = tryGetAttr "font" attrs
                let url = tryGetAttr "url" attrs
                let childrenWidgets = children |> List.collect buildWidget

                childrenWidgets
                |> List.map (fun widget -> applyTextStyle widget parentFg parentBg parentFont url)
                |> mergeAdjacentTextWidgets
            | "frag" ->
                let parentFg = tryGetAttr "foreground" attrs
                let parentBg = tryGetAttr "background" attrs
                let parentFont = tryGetAttr "font" attrs
                let url = tryGetAttr "url" attrs
                let childrenWidgets = children |> List.collect buildWidget

                childrenWidgets
                |> List.map (fun widget -> applyFragStyle widget parentFg parentBg parentFont url)
            | "row" ->
                let width =
                    tryGetAttr "width" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let overflow =
                    tryGetAttr "overflow" attrs
                    |> Option.map parseOverflow
                    |> Option.defaultValue OverflowValues.Break

                let border =
                    tryGetAttr "border" attrs
                    |> Option.map parseBorder
                    |> Option.defaultValue NoBorder

                let gap = parseIntAttr "gap" 0 attrs
                let align = tryGetAttr "align" attrs |> Option.map parseAlign
                let childrenWidgets = children |> List.collect buildWidget
                [ RowWidget(width, overflow, border, gap, align, childrenWidgets) ]
            | "column" ->
                let width =
                    tryGetAttr "width" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let overflow =
                    tryGetAttr "overflow" attrs
                    |> Option.map parseOverflow
                    |> Option.defaultValue OverflowValues.Break

                let border =
                    tryGetAttr "border" attrs
                    |> Option.map parseBorder
                    |> Option.defaultValue NoBorder

                let gap = parseIntAttr "gap" 0 attrs
                let yAlign = tryGetAttr "y-align" attrs |> Option.map parseAlign
                let childrenWidgets = children |> List.collect buildWidget
                [ ColumnWidget(width, overflow, border, gap, yAlign, childrenWidgets) ]
            | "layer" ->
                let index =
                    match tryGetAttr "index" attrs with
                    | Some value ->
                        let mutable i = 0

                        if System.Int32.TryParse(value, &i) then
                            i
                        else
                            failwith $"Invalid integer value for index: {value}"
                    | None -> failwith "Missing required attribute for layer: index"

                let gap = parseIntAttr "gap" 0 attrs

                let overflow =
                    tryGetAttr "overflow" attrs
                    |> Option.map parseOverflow
                    |> Option.defaultValue OverflowValues.Break

                let zAlign = tryGetAttr "z-align" attrs |> Option.map parseAlign
                let childrenWidgets = children |> List.collect buildWidget
                [ DepthWidget(index, overflow, zAlign, gap, childrenWidgets) ]
            | "box" ->
                let width =
                    tryGetAttr "width" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let height =
                    tryGetAttr "height" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let overflow =
                    tryGetAttr "overflow" attrs
                    |> Option.map parseOverflow
                    |> Option.defaultValue OverflowValues.Break

                let border =
                    tryGetAttr "border" attrs
                    |> Option.map parseBorder
                    |> Option.defaultValue Single

                let borderColor = tryGetAttr "border-color" attrs
                let align = tryGetAttr "align" attrs |> Option.map parseAlign

                let paddingV, paddingH =
                    tryGetAttr "padding" attrs
                    |> Option.map parsePadding
                    |> Option.defaultValue (0, 0)

                let childrenWidgets = children |> List.collect buildWidget

                [
                    BoxWidget(width, height, overflow, border, borderColor, align, paddingV, paddingH, childrenWidgets)
                ]
            | "block" ->
                let width =
                    tryGetAttr "width" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let height =
                    tryGetAttr "height" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let overflow =
                    tryGetAttr "overflow" attrs
                    |> Option.map parseOverflow
                    |> Option.defaultValue OverflowValues.Break

                let border =
                    tryGetAttr "border" attrs
                    |> Option.map parseBorder
                    |> Option.defaultValue Single

                let borderColor = tryGetAttr "border-color" attrs
                let name = tryGetAttr "title" attrs
                let align = tryGetAttr "align" attrs |> Option.map parseAlign

                let paddingV, paddingH =
                    tryGetAttr "padding" attrs
                    |> Option.map parsePadding
                    |> Option.defaultValue (0, 0)

                let childrenWidgets = children |> List.collect buildWidget

                [
                    BlockWidget(
                        width,
                        height,
                        overflow,
                        border,
                        borderColor,
                        name,
                        align,
                        paddingV,
                        paddingH,
                        childrenWidgets
                    )
                ]
            | "cell" ->
                let childrenWidgets = children |> List.collect buildWidget
                [ CellWidget(childrenWidgets) ]
            | "terminal" ->
                let width =
                    tryGetAttr "width" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let height =
                    tryGetAttr "height" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let xAlign = tryGetAttr "x-align" attrs |> Option.map parseAlign
                let yAlign = tryGetAttr "y-align" attrs |> Option.map parseAlign
                let winTitle = tryGetAttr "window-title" attrs |> Option.defaultValue "PTML"
                let tabTitle = tryGetAttr "icon-name" attrs |> Option.defaultValue "ptmlexec"

                let fore = tryGetAttr "foreground" attrs
                let back = tryGetAttr "background" attrs

                let childrenWidgets = children |> List.collect buildWidget

                [
                    TerminalWidget(width, height, fore, back, xAlign, yAlign, winTitle, tabTitle, childrenWidgets)
                ]
            | "spinner" ->
                let spinnerType =
                    tryGetAttr "type" attrs
                    |> Option.map parseSpinnerType
                    |> Option.defaultValue Braille

                let interval = tryGetAttr "interval" attrs |> Option.defaultValue "250ms"
                let duration = tryGetAttr "duration" attrs |> Option.defaultValue "3000ms"

                let completed =
                    tryGetAttr "completed" attrs
                    |> Option.map normalizeSpinnerCompleted
                    |> Option.defaultValue "✓"

                let foreground = tryGetAttr "foreground" attrs
                let background = tryGetAttr "background" attrs

                [
                    SpinnerWidget(spinnerType, interval, duration, completed, foreground, background)
                ]
            | "progress" ->
                let progressType =
                    tryGetAttr "style" attrs
                    |> Option.map parseProgressType
                    |> Option.defaultValue Progress.Blocks

                let value =
                    tryGetAttr "value" attrs |> Option.defaultValue "0" |> System.Int32.Parse

                let max = tryGetAttr "max" attrs |> Option.defaultValue "100" |> System.Int32.Parse

                let width =
                    tryGetAttr "width" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let height =
                    tryGetAttr "height" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let show = tryGetAttr "show-value" attrs |> Option.defaultValue "false"
                [ ProgressWidget(progressType, value, max, width, height, Some show) ]
            | "escape" ->
                let sequence =
                    tryGetAttr "sequence" attrs
                    |> Option.map parseEscapeSequence
                    |> Option.defaultValue EscapeSequence.Break

                let multiplier = parseIntAttr "multiplier" 1 attrs
                [ EscapeWidget(sequence, multiplier) ]
            | "frame" ->
                let fw =
                    tryGetAttr "framework" attrs
                    |> Option.map Frames.parseFrameWork
                    |> Option.defaultValue Frames.FrameWorks.Photograph

                let fc = tryGetAttr "frame-color" attrs

                let width =
                    tryGetAttr "width" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let height =
                    tryGetAttr "height" attrs
                    |> Option.map parseDimension
                    |> Option.defaultValue Auto

                let overflow =
                    tryGetAttr "overflow" attrs
                    |> Option.map parseOverflow
                    |> Option.defaultValue OverflowValues.Break

                let align = tryGetAttr "align" attrs |> Option.map parseAlign

                let paddingV, paddingH =
                    tryGetAttr "padding" attrs
                    |> Option.map parsePadding
                    |> Option.defaultValue (0, 0)

                let childrenWidgets = children |> List.collect buildWidget

                [
                    FrameWidget(fw, fc, width, height, overflow, align, paddingV, paddingH, childrenWidgets)
                ]
            | "cursor" ->
                let shape =
                    tryGetAttr "shape" attrs
                    |> Option.map Cursor.parseShape
                    |> Option.defaultValue Cursor.Shape.Underline

                let color =
                    tryGetAttr "color" attrs
                    |> Option.map Cursor.parseCursorColor
                    |> Option.defaultValue "#ffffff"

                let blink = tryGetAttr "blink" attrs |> Option.defaultValue "false"
                let visible = tryGetAttr "visible" attrs |> Option.defaultValue "true"
                [ CursorWidget(shape, Some blink, Some color, Some visible) ]
            | "code" ->
                let src: string option = tryGetAttr "src" attrs

                let executeMethod =
                    tryGetAttr "execute-method" attrs
                    |> Option.defaultValue "async"
                    |> getExecutionMethod

                let content = children
                []
            | "char" ->
                let codename =
                    tryGetAttr "codename" attrs
                    |> Option.defaultWith (fun () -> failwith "Missing required attribute for char: codename")

                [ CharWidget(translateCodename codename) ]
            | _ -> failwith $"Unsupported semantic tag: {tag}"

    let buildSemanticTreeFromDom (document: PTML.DOM.Builder.PTMLDocument) =
        document |> astFromDom |> buildSemanticTree

    let semanticTreeOfTokens tokens =
        let ast = buildAst tokens
        ast |> buildDom |> buildSemanticTreeFromDom
//#endregion
