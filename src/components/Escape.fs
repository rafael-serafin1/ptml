namespace PTML

module Escape =
    let private tabSize = 4
    type EscapeSequence =
    | NewLine
    | Break
    | HorizontalTab
    | VerticalTab
    | BackSpace
    | AudibleBell
    | FormFeed
    | CarriageReturn

    let chars(seq: EscapeSequence): string =
        match seq with
        | NewLine | Break -> "\n"
        | HorizontalTab -> "\t"
        | VerticalTab -> "\v"
        | BackSpace -> "\b"
        | AudibleBell -> "\a"
        | FormFeed -> "\f"
        | CarriageReturn -> "\r"

    let concatEscapes(seq: EscapeSequence, multi: int): string =
        let mutable str: string = ""
        for i = 0 to (multi - 1) do
            str <- str + chars(seq)
        str
    
    let resolveEscapeMetrics(sequence: EscapeSequence, multiplier: int) =
        match sequence with
        | EscapeSequence.NewLine
        | EscapeSequence.Break
        | EscapeSequence.VerticalTab
        | EscapeSequence.FormFeed -> (0, max 1 multiplier)
        | EscapeSequence.HorizontalTab -> (tabSize * multiplier, 0)
        | EscapeSequence.CarriageReturn
        | EscapeSequence.BackSpace
        | EscapeSequence.AudibleBell -> (0, 0)

    let calculateTextMetrics (text: string) =
        let mutable maxWidth = 0
        let mutable currentWidth = 0
        let mutable lines = 0
        let updateLine () =
            if currentWidth > maxWidth then maxWidth <- currentWidth
            currentWidth <- 0
        for ch in text do
            match ch with
            | '\n'
            | '\v'
            | '\f' ->
                updateLine ()
                lines <- lines + 1
            | '\r' -> currentWidth <- 0
            | '\t' ->
                let spaces = tabSize - (currentWidth % tabSize)
                currentWidth <- currentWidth + spaces
            | '\b' -> currentWidth <- max 0 (currentWidth - 1)
            | _ -> currentWidth <- currentWidth + 1
        updateLine ()
        (maxWidth, lines)