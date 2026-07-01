namespace PTML

module Escape =
    type EscapeSequence =
    | Break
    | HorizontalTab
    | VerticalTab
    | BackSpace
    | AudibleBell
    | FormFeed
    | CarriageReturn

    let chars(seq: EscapeSequence): string =
        match seq with
        | Break -> "\n"
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
    