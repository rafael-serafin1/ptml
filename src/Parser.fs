namespace PTML
open PTML.Lexer
open PTML.Token

module Parser =
    type AttrValidationResult =
        | Valid
        | InvalidValue of string
        | UnknownAttribute of string

    let isGlobalAttribute(name: string): bool = 
        Set.contains name Global.globalAttributes

    let validateAttribute tag name value =
        if tag = "snippet" then
            Valid
        else
            match Map.tryFind tag Global.validAttributes with
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
                        | "multiplier" ->
                            let mutable i = 0
                            if System.Int32.TryParse(value, &i) && i > 0 then Valid
                            else InvalidValue $"Multiplier must be int and positive: {value}"
                        | "max" | "value" ->
                            let mutable i = 0
                            if System.Int32.TryParse(value, &i) then Valid
                            else InvalidValue $"Invalid value for {name}: {value}"
                        | "root-limit" ->
                            let mutable i = 0
                            if System.Int32.TryParse(value, &i) && i >= 0 then Valid
                            else InvalidValue $"Invalid root-limit: {value}"
                        | "path" when value = "" ->
                            InvalidValue "The path attribute for <tree> cannot be empty."
                        | "width" | "height" ->
                            if value <> "auto" && not (value.EndsWith "%") then
                                let mutable i = 0
                                if System.Int32.TryParse(value, &i) then Valid
                                else InvalidValue $"Invalid {name}: {value}"
                            else
                                Valid
                        | "url" -> 
                            if isSecureUrl(value) then
                                Valid
                            else
                                InvalidValue $"Invalid URL: {value}"
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
        elif isInsideTag "text" stack && tag <> "frag" && tag <> "escape" then
            failwith $"Elemento <{tag}> não é permitido dentro de <text>."

    let rec parser(tokens, stack ) =
        match tokens with
        | [] -> if stack <> [] then failwith "Unclosed tags" else ()
        | ProcInst pi :: rest ->
            let piAttrs = parsePiAttrs(pi)
            PiAttrs.parseAndApplyPiAttrs(piAttrs)
            parser(rest, stack)
        | Comment _ :: rest -> parser(rest, stack)
        | Text _ :: rest -> parser(rest, stack)
        | StartTag (tag, selfClosing, attrs) :: rest ->
            if not (Set.contains tag Global.validTags) then 
                failwith $"Invalid tag: {tag}"

            validateFragContext tag stack

            if tag <> "snippet" then
                for (name, value) in attrs do
                    match validateAttribute tag name value with
                    | Valid -> ()
                    | InvalidValue message -> failwith message
                    | UnknownAttribute _ -> failwith $"Invalid attribute for attribute '{tag}': {name}"

                if tag = "block" && not (List.exists (fun (name, _) -> name = "title") attrs) then
                    failwith "Missing required attribute for block: title"
                if tag = "layer" && not (List.exists (fun (name, _) -> name = "index") attrs) then
                    failwith "Missing required attribute for layer: index"
                if tag = "tree" && not (List.exists (fun (name, _) -> name = "path") attrs) then
                    failwith "Missing required attribute for tree: path"
                    
            if selfClosing then
                parser(rest, stack)
            else
                parser(rest, tag :: stack)
        | EndTag tag :: rest ->
            match stack with
            | top :: remaining when top = tag -> parser(rest, remaining)
            | _ -> failwith $"Mismatched end tag: {tag}"