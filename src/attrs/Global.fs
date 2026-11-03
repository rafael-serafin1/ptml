namespace PTML

module Global =
    let validTags = 
        Set.ofList ["frag"; "text"; "row"; "column"; "layer"; "box"; "block"; "terminal"; "cell"; "snippet"; "spinner"; "hr"; "progress"; "escape"; "frame"; "cursor"; "tree"; "list"; "code"]
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
    ]
    
    let globalAttributes = Set.ofList ["id"; "snippet"]