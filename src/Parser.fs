namespace PTML
open PTML.Lexer
open PTML.Token

module Parser =
    let validTags = Set.ofList ["frag"; "text"; "row"; "column"; "layer"; "box"; "block"; "terminal"; "cell"; "snippet"; "spinner"; "hr"; "progress"]
    let colorValues = Set.ofList ["none"; "black"; "red"; "green"; "gold"; "blue"; "purple"; "cyan"; "fire"; "limegreen"; "yellow"; "lightblue"; "lilac"; "crystal"; "gray"; "lightgray"; "white"]
    let fontValues = Set.ofList ["none"; "bold"; "dim"; "italic"; "underline"; "slow-blink"; "rapid-blink"; "reverse"; "conceal"; "strike-through"]
    let overflowValues = Set.ofList ["break"; "wrap"; "cut"; "clip"]
    let alignValues = Set.ofList ["start"; "center"; "end"]
    let borderValues = Set.ofList ["single"; "double"; "classic"; "bold"; "strange"; "rounded"; "ascii"; "none"; "borderless"]
    let terminalResizeValues = Set.ofList ["reflow"; "clip"; "static"]
    let orientation = Set.ofList ["vertical"; "horizontal"]
    let progresstype = Set.ofList ["blocks"; "square"; "dots"; "tiny-square"; "rhombus"]
    let boolean = Set.ofList ["true"; "false"]


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
            "foreground", colorValues
            "background", colorValues
            "font", fontValues
        ]   
        "text", Map.ofList [
            "foreground", colorValues
            "background", colorValues
            "font", fontValues
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
            "z-align", alignValues
            "gap", Set.empty
        ]
        "box", Map.ofList [
            "overflow", overflowValues
            "border", borderValues
            "width", Set.empty
            "height", Set.empty
            "border-color", colorValues
            "align", alignValues
            "padding", Set.empty
        ]
        "block", Map.ofList [
            "title", Set.empty
            "overflow", overflowValues
            "border", borderValues
            "width", Set.empty
            "height", Set.empty
            "border-color", colorValues
            "align", alignValues
            "padding", Set.empty
        ]
        "terminal", Map.ofList [
            "x-align", alignValues
            "y-align", alignValues
        ]
        "cell", Map.ofList [];      // no attrs for now
        "spinner", Map.ofList [
            "type", Set.ofList ["braille"; "dots"; "waiting"; "burger"; "beam"; "ascii"; "circle"; "square"; "moon"; "arrow"; "bounce"]
            "interval", Set.empty
            "duration", Set.empty
            "completed", Set.empty
            "foreground", colorValues
            "background", colorValues
        ]
    ]

    let globalAttributes = Set.ofList ["id"; "snippet"]

    type AttrValidationResult =
        | Valid
        | InvalidValue of string
        | UnknownAttribute of string

    let isGlobalAttribute name = Set.contains name globalAttributes

    let validateAttribute tag name value =
        if tag = "snippet" then
            Valid
        else
            match Map.tryFind tag validAttributes with
            | Some attrMap ->
                match Map.tryFind name attrMap with
                | Some valueSet ->
                    if valueSet <> Set.empty then
                        if not (Set.contains value valueSet) then
                            InvalidValue $"Invalid value for {name}: {value}"
                        else
                            Valid
                    else
                        match name with
                        | "index" ->
                            let mutable i = 0
                            if System.Int32.TryParse(value, &i) then Valid
                            else InvalidValue $"Index must be signed int: {value}"
                        | "gap" ->
                            let mutable i = 0
                            if System.Int32.TryParse(value, &i) then Valid
                            else InvalidValue $"Gap must be int: {value}"
                        | "max" | "value" ->
                            let mutable i = 0
                            if System.Int32.TryParse(value, &i) then Valid
                            else InvalidValue $"Invalid value for {name}: {value}"
                        | "width" | "height" ->
                            if value <> "auto" && not (value.EndsWith "%") then
                                let mutable i = 0
                                if System.Int32.TryParse(value, &i) then Valid
                                else InvalidValue $"Invalid {name}: {value}"
                            else
                                Valid
                        | "padding" ->
                            let parts : string[] = value.Split([| 'x' |])
                            let parsePaddingPart (part: string) =
                                let mutable i = 0
                                if System.Int32.TryParse(part, &i) && i >= 0 then Some i
                                else None
                            match parts with
                            | [| part |] when parsePaddingPart part |> Option.isSome -> Valid
                            | [| vertical; horizontal |] when parsePaddingPart vertical |> Option.isSome && parsePaddingPart horizontal |> Option.isSome -> Valid
                            | _ -> InvalidValue $"Invalid padding format: {value}"
                        | _ -> Valid
                | None ->
                    if isGlobalAttribute name then
                        Valid
                    else
                        UnknownAttribute $"Invalid attribute for {tag}: {name}"
            | None ->
                if isGlobalAttribute name then
                    Valid
                else
                    UnknownAttribute $"Invalid attribute for {tag}: {name}"

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

    let negative (number: int): bool =
        if number <= 0 then true
        else false 

    let private isInsideTag tag stack =
        List.contains tag stack

    let private validateFragContext tag stack =
        if tag = "frag" then
            if not (isInsideTag "text" stack) then
                failwith "Elemento <frag> só pode existir dentro de <text>."
            if isInsideTag "frag" stack then
                failwith "Elemento <frag> não pode conter outro <frag>."
        elif isInsideTag "text" stack && tag <> "frag" then
            failwith $"Elemento <{tag}> não é permitido dentro de <text>."

    let rec parser(tokens, stack ) =
        match tokens with
        | [] -> if stack <> [] then failwith "Unclosed tags" else ()
        | ProcInst pi :: rest ->
            let piAttrs = parsePiAttrs pi
            PiAttrs.parseAndApplyPiAttrs piAttrs
            parser(rest, stack)
        | Comment _ :: rest -> parser(rest, stack)
        | Text _ :: rest -> parser(rest, stack)
        | StartTag (tag, selfClosing, attrs) :: rest ->
            if not (Set.contains tag validTags) then failwith $"Invalid tag: {tag}"
            validateFragContext tag stack
            if tag <> "snippet" then
                for (name, value) in attrs do
                    match validateAttribute tag name value with
                    | Valid -> ()
                    | InvalidValue message -> failwith message
                    | UnknownAttribute _ -> failwith $"Invalid attribute for attribute '{tag}': {name}"
                if tag = "block" && not (List.exists (fun (name, _) -> name = "title") attrs) then
                    failwith "Missing required attribute for block: title"
                if tag = "depth" && not (List.exists (fun (name, _) -> name = "index") attrs) then
                    failwith "Missing required attribute for depth: index"
            if selfClosing then
                parser(rest, stack)
            else
                parser(rest, tag :: stack)
        | EndTag tag :: rest ->
            match stack with
            | top :: remaining when top = tag -> parser(rest, remaining)
            | _ -> failwith $"Mismatched end tag: {tag}"