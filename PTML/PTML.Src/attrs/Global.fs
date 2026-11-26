namespace PTML
open PTML.Utils

module Global =
//#region POSSIBLE_VALUES
    let validTags = 
        Set.ofList ["frag"; "text"; "row"; "column"; "layer"; "box"; "block"; "terminal"; "cell"; "snippet"; "spinner"; "hr"; "progress"; "escape"; "frame"; "cursor"; "tree"; "list"; "code"; "char"]
    let colorValues = 
        Set.ofList ["none"; "black"; "red"; "green"; "gold"; "blue"; "purple"; "cyan"; "fire"; "limegreen"; "yellow"; "lightblue"; "lilac"; "crystal"; "gray"; "lightgray"; "white"]
    let fontValues = Set.ofList ["none"; "bold"; "dim"; "italic"; "underline"; "slow-blink"; "rapid-blink"; "reverse"; "conceal"; "strike-through"; "double-underline"; "overline"]
    let overflowValues = Set.ofList ["break"; "wrap"; "cut"; "clip"]
    let alignValues = Set.ofList ["start"; "center"; "end"]
    let borderValues = Set.ofList ["single"; "double"; "classic"; "bold"; "strange"; "rounded"; "ascii"; "none"; "borderless"]
    let terminalResizeValues = Set.ofList ["reflow"; "clip"; "static"]
    let orientation = Set.ofList ["vertical"; "horizontal"]
    let progresstype = Set.ofList ["blocks"; "square"; "dots"; "tiny-square"; "rhombus"]
    let boolean = Set.ofList ["true"; "false"]
    let frameworks = Set.ofList ["bold"; "pixels"; "cubic"; "point"; "border"; "picture"; "photograph"; "pythagoras"; "arrow"; "ascii"]
//#endregion

//#region VALIDATION_MAP
    let validAttributes = Map.ofList [
        "hr", Map.ofList [
            "orientation", orientation
            "width", Set.empty
            "height", Set.empty
        ]
        "progress", Map.ofList [
            "style", progresstype
            "max", Set.empty
            "value", Set.empty
            "show-value", boolean
            "width", Set.empty
            "height", Set.empty
        ]
        "frag", Map.ofList [
            "foreground", Set.empty
            "background", Set.empty
            "font", fontValues
            "url", Set.empty
        ]   
        "text", Map.ofList [
            "foreground", Set.empty
            "background", Set.empty
            "font", fontValues
            "url", Set.empty
        ]
        "row", Map.ofList [
            "overflow", overflowValues
            "gap", Set.empty // special
            "align", alignValues
        ]
        "column", Map.ofList [
            "index", Set.empty
            "overflow", overflowValues
            "gap", Set.empty
            "y-align", alignValues
        ]
        "layer", Map.ofList [
            "index", Set.empty
            "overflow", overflowValues
            "z-align", alignValues
            "gap", Set.empty
        ]
        "box", Map.ofList [
            "overflow", overflowValues
            "border", borderValues
            "width", Set.empty
            "height", Set.empty
            "border-color", Set.empty
            "align", alignValues
            "padding", Set.empty
        ]
        "block", Map.ofList [
            "title", Set.empty
            "overflow", overflowValues
            "border", borderValues
            "width", Set.empty
            "height", Set.empty
            "border-color", Set.empty
            "align", alignValues
            "padding", Set.empty
        ]
        "terminal", Map.ofList [
            "x-align", alignValues
            "y-align", alignValues
            "window-title", Set.empty
            "icon-name", Set.empty
            "foreground", Set.empty
            "background", Set.empty
        ]
        "cell", Map.ofList [];      // no attrs for now
        "spinner", Map.ofList [
            "type", Set.ofList ["braille"; "dots"; "waiting"; "burger"; "beam"; "ascii"; "circle"; "square"; "moon"; "arrow"; "bounce"; "fill"]
            "interval", Set.empty
            "duration", Set.empty
            "completed", Set.empty
            "foreground", Set.empty
            "background", Set.empty
        ]
        "escape", Map.ofList [
            "sequence", Set.ofList ["new-line"; "break"; "horizontal-tab"; "vertical-tab"; "audible-bell"; "backspace"; "form-feed"; "carriage-return"]
            "multiplier", Set.empty
        ]
        "frame", Map.ofList [
            "framework", frameworks
            "frame-color", Set.empty
            "overflow", overflowValues
            "width", Set.empty
            "height", Set.empty
            "align", alignValues
            "padding", Set.empty
        ]
        "cursor", Map.ofList [
            "shape", Set.ofList ["block"; "bar"; "underline"]
            "blink", boolean
            "visible", boolean
            "color", Set.empty
        ]
        "tree", Map.ofList [
            "path", Set.empty
            "root-limit", Set.empty
        ]
        "list", Map.ofList [
            "ofstyle", Set.ofList ["unorder"; "bullet"; "enum"; "lower-alphabet"; "upper-alphabet"]
            "before", Set.empty
            "after", Set.empty
        ]
        "code", Map.ofList [
            "src", Set.empty
            "execute-method", Set.ofList ["async"; "defer"]
        ]
        "char", Map.ofList ["codename", PTML.Char.validCodenames]
    ]
//#endregion

//#region TYPES
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
        | Overline = 10
        | DoubleUnderLine = 11

    type Orientation =
        | Vertical
        | Horizontal
//#endregion

//#region GLOBAL_ATTRS
    let globalAttributes = Set.ofList ["id"; "snippet"; "content"]

    type ContentAttrValue =
    | Clipboard
    | Primary
    | Secondary

    type GlobalAttributes = {
        Id: string option
        Snippet: string option
        Content: ContentAttrValue option
    }

    type SnippetDefinition = {
        Id              : string
        Extends         : string option
        GlobalAttributes: GlobalAttributes
        Attributes      : list<string * string>
    }

    type SnippetRegistry   = Map<string, SnippetDefinition>
    type ValidationWarning = { Message: string }
    type ValidationError   = { Message: string }

    let emptyGlobalAttributes(): GlobalAttributes  = 
        { Id = None; Snippet = None; Content = None }

    let tryMatchContentAttrValue(key: string option): ContentAttrValue option = 
        match key with
        | None             -> None
        | Some (s: string) ->
            match s.ToLower() with
            | "clipboard" -> Some Clipboard
            | "primary"   -> Some Primary
            | "secondary" -> Some Secondary
            | _           -> None

    ///<summary>Divide os atributos registrados de uma tag em atributos locais e globais</summary>
    ///<return>Uma lista contento uma estrutura de atributos considerados globais e atributos locais (atributos da própria tag)</return>
    let splitGlobalAttrs attrs =
        let id = 
            attrs |> List.tryFind (fun (name, _) -> name = "id") |> Option.map snd

        let snippet =
            attrs |> List.tryFind (fun (name, _) -> name = "snippet") |> Option.map snd

        let content = 
            attrs |> List.tryFind (fun (name, _) -> name = "content") |> Option.map snd

        if content <> None then
            Utils.shouldExecuteContent true

        let filtered =
            attrs |> List.filter (fun (name, _) -> name <> "id" && name <> "snippet" && name <> "content")

        ({ Id = id; Snippet = snippet; Content = tryMatchContentAttrValue(content) }, filtered)
//#endregion