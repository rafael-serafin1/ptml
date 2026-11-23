namespace PTML
open System

module CSI =
    let private ESC = "\x1b"
    let private BEL = "\x07"
    let private ST = "\x1b\\"

    type ClearSettings = 
    | FromCursor
    | ToCursor

    let private Clear_FromCursorToEnd   = "0J"
    let private Clear_FromStartToCursor = "1J"


//#region 
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

//#endregion