namespace PTML
open System
open System.Text
open PTML.Global
open PTML.Utils
open PTML.OSC
open PTML.Token
open PTML.Parser

module Snippet =
    let emitWarning (warning: ValidationWarning) =
        System.Console.Error.WriteLine($"[warning] {warning.Message}")

    let emitWarnings warnings =
        warnings |> List.rev |> List.iter emitWarning

//#region ATTRS_DEDUPE
    let private dedupeAttrsKeepLast attrs : list<string * string> =
        let folder (seen, result) (name, value) =
            if Set.contains name seen then
                (seen, result)
            else
                (Set.add name seen, (name, value) :: result)

        attrs |> List.rev |> List.fold folder (Set.empty, []) |> snd

    let private mergeAttributeLists baseAttrs overrides : list<string * string> =
        let overrideNames = overrides |> List.map fst |> Set.ofList

        let filteredAttrs =
            baseAttrs
            |> List.filter (fun (name, _) -> not (Set.contains name overrideNames))

        filteredAttrs @ overrides
//#endregion

//#region SNIPPET_PARSING
    let parseSnippetBodyAttrs children : list<string * string> =
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

        let tokens: string[] =
            rawText.Split([| ' '; '\n'; '\r'; '\t' |], System.StringSplitOptions.RemoveEmptyEntries)

        let tryParseToken (token: string) : (string * string) option =
            let eqIdx = token.IndexOf('=')

            if
                eqIdx > 0
                && token.Length > eqIdx + 2
                && token.[eqIdx + 1] = '"'
                && token.[token.Length - 1] = '"'
            then
                let name = token.[0 .. eqIdx - 1]
                let value = token.[eqIdx + 2 .. token.Length - 2]
                Some(name, value)
            else
                None

        tokens |> Array.choose tryParseToken |> Array.toList

    let collectSnippetDefinitions ast : SnippetRegistry * ValidationError list =
        let rec loop
            nodes
            (registry: SnippetRegistry)
            (errors: ValidationError list)
            : SnippetRegistry * ValidationError list =
            match nodes with
            | [] -> registry, errors
            | TextNode _ :: rest -> loop rest registry errors
            | Element("snippet", globalAttrs, attrs, children) :: rest ->
                let bodyAttrs = parseSnippetBodyAttrs children

                let extends =
                    attrs |> List.tryFind (fun (name, _) -> name = "extends") |> Option.map snd

                let localAttrs = attrs |> List.filter (fun (name, _) -> name <> "extends")
                let definitionAttrs = dedupeAttrsKeepLast (localAttrs @ bodyAttrs)

                match globalAttrs.Id with
                | Some id when id <> "" ->
                    if Map.containsKey id registry then
                        let duplicateSnippetError: ValidationError =
                            {
                                Message = $"Duplicate snippet id '{id}'"
                            }

                        loop rest registry (duplicateSnippetError :: errors)
                    else
                        let definition =
                            {
                                Id = id
                                Extends = extends
                                GlobalAttributes = globalAttrs
                                Attributes = definitionAttrs
                            }

                        loop rest (Map.add id definition registry) errors
                | _ ->
                    let missingSnippetIdError: ValidationError =
                        {
                            Message = "Snippet declaration must have a valid id."
                        }

                    loop rest registry (missingSnippetIdError :: errors)
            | Element(_, _, _, children) :: rest ->
                let registry, errors = loop children registry errors
                loop rest registry errors

        loop ast Map.empty []
//#endregion

//#region SNIPPET_ATTRS
    let mergeSnippetAttributes tag snippetAttrs explicitAttrs : list<string * string> * ValidationWarning list =
        let explicitNames = explicitAttrs |> List.map fst |> Set.ofList

        let folder (mergedAttrs, warnings) (name, value) =
            if Set.contains name explicitNames then
                mergedAttrs, warnings
            else
                match validateAttribute tag name value with
                | Valid -> (name, value) :: mergedAttrs, warnings
                | InvalidValue message -> failwith message
                | UnknownAttribute _ ->
                    let warning: ValidationWarning =
                        {
                            Message = $"O atributo '{name}' não existe para o elemento {tag}."
                        }

                    mergedAttrs, warning :: warnings

        let initialState: list<string * string> * ValidationWarning list = ([], [])
        let merged, warnings = List.fold folder initialState snippetAttrs
        (List.rev merged) @ explicitAttrs, warnings

    let resolveSnippets ast =
        let snippetRegistry, registryErrors = collectSnippetDefinitions ast

        if registryErrors <> [] then
            let messages =
                registryErrors
                |> List.rev
                |> List.map (fun e -> e.Message)
                |> String.concat "; "

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
                    let mergedGlobals = { Id = globalAttrs.Id; Snippet = None; Content = globalAttrs.Content }
                    
                    Some(Element(tag, mergedGlobals, mergedAttrs, resolvedChildren))
                | None -> Some(Element(tag, globalAttrs, attrs, resolvedChildren))

        ast |> List.choose resolveNode

//#endregion