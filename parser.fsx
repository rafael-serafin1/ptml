module Parser
#load "token.fsx"
#load "lexer.fsx"
open Token

let validTags = Set.ofList ["text"; "row"; "column"; "box"; "block"]
let colorValues = Set.ofList ["none"; "black"; "red"; "green"; "gold"; "blue"; "purple"; "cyan"; "fire"; "limegreen"; "yellow"; "lightblue"; "lilac"; "crystal"; "gray"; "lightgray"; "white"]
let fontValues = Set.ofList ["none"; "bold"; "dim"; "italic"; "underline"; "slowblink"; "rapidblink"; "reverse"; "conceal"; "strikethrough"]
let overflowValues = Set.ofList ["break"; "wrap"; "cut"; "clip"]
let alignValues = Set.ofList ["start"; "center"; "end"]
let borderValues = Set.ofList ["single"; "double"; "classic"; "bold"; "strange"; "rounded"; "ascii"; "none"]
let terminalResizeValues = Set.ofList ["reflow"; "clip"; "static"]

let validAttributes = Map.ofList [
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
    "block", Map.ofList [
        "name", Set.empty
        "overflow", overflowValues
        "border", borderValues
        "width", Set.empty
        "height", Set.empty
        "border-color", colorValues
        "align", alignValues
    ]
]

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
            if tag = "block" && not (List.exists (fun (name, _) -> name = "name") attrs) then
                failwith "Missing required attribute for block: name"
        | None -> ()
        if selfClosing then
            parser(rest, stack)
        else
            parser(rest, tag :: stack)
    | EndTag tag :: rest ->
        match stack with
        | top :: remaining when top = tag -> parser(rest, remaining)
        | _ -> failwith $"Mismatched end tag: {tag}"
