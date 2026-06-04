namespace PTML

open System
open System.Text

module PiAttrs =
    type EncodingMode =
        | UTF8
        | UTF16
        | UTF32
        | ASCII

    type TerminalResizeMode =
        | Reflow
        | Clip
        | Static

    type PiAttr =
        | Encoding of EncodingMode
        | TerminalResize of TerminalResizeMode

    type PiSettings = {
        encoding: EncodingMode
        terminalResize: TerminalResizeMode
    }

    let private defaultSettings = {
        encoding = UTF8
        terminalResize = Reflow
    }

    let mutable private currentSettings : PiSettings = defaultSettings
    let mutable private savedViewport : (int * int) option = None

    let private normalizeAttrValue (value: string) = value.Trim()

    let private parseEncodingMode (value: string) =
        match normalizeAttrValue(value).ToUpperInvariant() with
        | "UTF-8" | "UTF8" -> UTF8
        | "UTF-16" | "UTF16" -> UTF16
        | "UTF-32" -> UTF32
        | "ASCII" -> ASCII
        | invalid -> failwith $"Invalid encoding: {invalid}. Supported values are UTF-8, UTF-16, ASCII."

    let private parseTerminalResizeMode (value: string) =
        match normalizeAttrValue(value).ToLowerInvariant() with
        | "reflow" -> Reflow
        | "clip" -> Clip
        | "static" -> Static
        | invalid -> failwith $"Invalid terminal-resize: {invalid}. Supported values are reflow, clip, static."

    let private applyEncodingMode = function
        | UTF8 -> Encoding.UTF8
        | UTF16 -> Encoding.Unicode
        | UTF32 -> Encoding.UTF32
        | ASCII -> Encoding.ASCII

    let private updateConsoleEncoding mode =
        let enc = applyEncodingMode mode
        Console.OutputEncoding <- enc
        Console.InputEncoding <- enc

    let private ensureSavedViewport width height =
        match savedViewport with
        | Some size -> size
        | None ->
            let size = (width, height)
            savedViewport <- Some size
            size

    let parseSettings (attrs: list<string * string>) =
        attrs
        |> List.fold (fun settings (name, value) ->
            match name with
            | "encoding" -> { settings with encoding = parseEncodingMode value }
            | "terminal-resize" -> { settings with terminalResize = parseTerminalResizeMode value }
            | invalid -> failwith $"Invalid pi attribute: {invalid}"
        ) currentSettings

    let applySettings settings =
        currentSettings <- settings
        updateConsoleEncoding settings.encoding

    let parseAndApplyPiAttrs attrs =
        let settings = parseSettings attrs
        applySettings settings

    let getTerminalResizeMode() = currentSettings.terminalResize

    let getLayoutSize (width: int) (height: int) =
        match currentSettings.terminalResize with
        | Reflow -> width, height
        | Clip -> ensureSavedViewport width height
        | Static -> ensureSavedViewport width height

    let getOutputSize (width: int) (height: int) =
        match currentSettings.terminalResize with
        | Reflow -> width, height
        | Clip -> width, height
        | Static -> ensureSavedViewport width height
