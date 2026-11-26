namespace PTML
open PTML.Token
open PTML.Global

module Id =
    let collectDuplicateIdWarnings ast : ValidationWarning list =
        let rec loop nodes (seen: Set<string>) (warnings: ValidationWarning list) : ValidationWarning list =
            match nodes with
            | [] -> warnings
            | TextNode _ :: rest -> loop rest seen warnings
            | Element("snippet", _, _, children) :: rest -> loop (children @ rest) seen warnings
            | Element(_, globalAttrs, _, children) :: rest ->
                let warnings =
                    match globalAttrs.Id with
                    | Some id when id <> "" && Set.contains id seen ->
                        let duplicateWarning: ValidationWarning = {
                            Message = $"ID duplicado '{id}' encontrado."
                        }

                        duplicateWarning :: warnings
                    | _ -> warnings

                let seen =
                    match globalAttrs.Id with
                    | Some id when id <> "" -> Set.add id seen
                    | _ -> seen

                loop (children @ rest) seen warnings

        loop ast Set.empty []