namespace PTML
open System

open PTML.Cursor
module CSI =
    let private ESC = "\x1b"
    let private BEL = "\x07"
    let private ST = "\x1b\\"

    type ClearSettings = 
    | FromCursor
    | ToCursor

    let private Clear_FromCursorToEnd   = "0J"
    let private Clear_FromStartToCursor = "1J"


//#region CURSOR 
    let ResetCursorPos() = 
        Console.SetCursorPosition(0, 0)
        
    let ClearAll (settings: ClearSettings list) =
        let clear (setting: ClearSettings) = 
            match setting with
            | FromCursor ->
                printf "%s[%s" ESC Clear_FromCursorToEnd
            | ToCursor ->
                printf "%s[%s" ESC Clear_FromStartToCursor
        
        settings
            |> List.iter (fun c -> clear c)

    let private matchAnsiIntegerEntry(cursor_shape: Shape)(cursor_blink: bool): int =
        match cursor_shape with
            | Shape.Block ->
                if cursor_blink then 1 else 2
            | Shape.Underline ->
                if cursor_blink then 3 else 4
            | Shape.Bar ->
                if cursor_blink then 5 else 6 

    let parseCursor (cursor_state: Cursor.Shape * bool) =
        match cursor_state with
        | c, p ->
            let value = matchAnsiIntegerEntry c p

            printf "%s[%d q" ESC value

            let starter = if p then 'h' else 'l'
            printf "%s[?12%c" ESC starter 

    let showCursor (should: bool) =
        let starter = if should then 'h' else 'l'
        printf "%s[?25%c" ESC starter

//#endregion