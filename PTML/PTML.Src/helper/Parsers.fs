namespace PTML
open System

open PTML.Global
open PTML.Spinner
open PTML.Progress
open PTML.Escape

module Parsers =
//#region  TRY_GET_ATTR
    let tryGetAttr name attrs =
        attrs |> List.tryFind (fun (k, _) -> k = name) |> Option.map snd
//#endregion

    let parseIntAttr name defaultValue attrs =
        match tryGetAttr name attrs with
        | Some value ->
            let strValue: string = value
            let mutable i: int = 0

            if System.Int32.TryParse(strValue, &i) then
                i
            else
                failwith $"Invalid integer value for {name}: {value}"
        | None -> defaultValue

    let parseDimension value =
        if value = "auto"       then Auto
        elif value.EndsWith "%" then
            let numStr: string = value.[0 .. value.Length - 2]
            let mutable i: int = 0

            if System.Int32.TryParse(numStr, &i) then
                Percent i
            else
                failwith $"Invalid percentage: {value}"
        else
            let strValue: string = value
            let mutable i: int = 0

            if System.Int32.TryParse(strValue, &i) then
                Fixed i
            else
                failwith $"Invalid dimension: {value}"

//#region ALIGN / OVERFLOW / ESCAPE
    let parseAlign =
        function
        | "start"   -> Start
        | "center"  -> Center
        | "end"     -> End
        | value -> failwith $"Invalid align value: {value}"

    let parseOverflow = Overflow.parseOverflow

    let parseEscapeSequence =
        function
        | "new-line"
        | "break"           -> EscapeSequence.Break
        | "horizontal-tab"  -> HorizontalTab
        | "vertical-tab"    -> VerticalTab
        | "audible-bell"    -> AudibleBell
        | "backspace"       -> BackSpace
        | "form-feed"       -> FormFeed
        | "carriage-return" -> CarriageReturn
        | value -> failwith $"Tipagem inexistente para <escape>: '{value}'"
//#endregion

//#region PADDING
    let parsePadding (value: string) =
        let parsePart (part: string) =
            let mutable i: int = 0

            if System.Int32.TryParse(part, &i) && i >= 0 then
                i
            else
                failwith $"Invalid padding value: {value}"

        let parts: string[] = value.Split([| 'x' |])

        match parts with
        | [| part |] -> let p = parsePart part in (p, p)
        | [| vertical; horizontal |] -> (parsePart vertical, parsePart horizontal)
        | _ -> failwith $"Invalid padding format: {value}"
//#endregion

//#region HR
    let parseHr =
        function
        | "vertical"    -> Vertical
        | "horizontal"  -> Horizontal
        | value -> failwith $"Invalid orientation value: {value}"
//#endregion

//#region BORDER
    let parseBorder =
        function
        | "single"      -> Single
        | "double"      -> Double
        | "classic"     -> Classic
        | "bold"        -> Bold
        | "strange"     -> Strange
        | "rounded"     -> Rounded
        | "ascii"       -> Border.Ascii
        | "none"        -> NoBorder
        | "borderless"  -> Borderless
        | value -> failwith $"Invalid border value: {value}"
//#endregion

//#region SPINNER
    let parseSpinnerType =
        function
        | "braille" -> Braille
        | "dots"    -> Spinner.Dots
        | "waiting" -> Waiting
        | "burger"  -> Burger
        | "beam"    -> Beam
        | "ascii"   -> Spinner.Ascii
        | "circle"  -> Circle
        | "square"  -> Spinner.Square
        | "moon"    -> Moon
        | "arrow"   -> Arrow
        | "bounce"  -> Bounce
        | "fill"    -> Fill
        | value -> failwith $"Invalid spinner type: {value}"

    let normalizeSpinnerCompleted value =
        if String.IsNullOrWhiteSpace(value) then
            "✓"
        else
        match value with
        | "check"   -> "✓"
        | "error"   -> "✖"
        | "star"    -> "✱"
        | "cog"     -> "⚙"
        | "bright"  -> "✦"
        | _         -> value
//#endregion

    let parseProgressType =
        function
        | "blocks"      -> Blocks
        | "dots"        -> Dots
        | "square"      -> Square
        | "tiny-square" -> TinySquare
        | "rhombus"     -> Rhombus
        | value -> failwith $"Tipagem inexistente para <progress>: '{value}'"
