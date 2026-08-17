namespace PTML

module FontCode = 
    let private escape = "\x1b"
    let private resetCode = sprintf "%s[0m" escape
    let private bell = "\x07"

    (* CHAR ANSI ESCAPE MESS *)
    let foregroundCode = function
        | Some "black" -> Some "30"
        | Some "red" -> Some "31"
        | Some "green" -> Some "32"
        | Some "gold" -> Some "33"
        | Some "blue" -> Some "34"
        | Some "purple" -> Some "35"
        | Some "cyan" -> Some "36"
        | Some "white" -> Some "37"
        | Some "fire" -> Some "1;31"
        | Some "limegreen" -> Some "1;32"
        | Some "yellow" -> Some "1;33"
        | Some "lightblue" -> Some "1;34"
        | Some "lilac" -> Some "1;35"
        | Some "crystal" -> Some "1;36"
        | Some "gray" -> Some "1;30"
        | Some "lightgray" -> Some "1;37"
        | s -> 
            match s with
            | Some hex -> 
                Some (Utils.hexadecimal(hex, Utils.Foreground))
            | None -> None

    let backgroundCode = function
        | Some "black" -> Some "40"
        | Some "red" -> Some "41"
        | Some "green" -> Some "42"
        | Some "gold" -> Some "43"
        | Some "blue" -> Some "44"
        | Some "purple" -> Some "45"
        | Some "cyan" -> Some "46"
        | Some "white" -> Some "47"
        | s ->            
            match s with 
            | Some hex ->
                Some (Utils.hexadecimal(hex, Utils.Background))
            | None -> None

    let fontCode = function
        | Some "bold" -> Some "1"
        | Some "dim" -> Some "2"
        | Some "italic" -> Some "3"
        | Some "underline" -> Some "4"
        | Some "slow-blink" -> Some "5"
        | Some "rapid-blink" -> Some "6"
        | Some "reverse" -> Some "7"
        | Some "conceal" -> Some "8"
        | Some "strike-through" -> Some "9"
        | Some "overline" -> Some "53"
        | Some "double-underline" -> Some "21"
        | _ -> None

    (* CURSOR STYLE MESS *)
    let cursorShapeParam (shape: Cursor.Shape) (blinking: bool) =
        match shape, blinking with
        | Cursor.Block, true -> "1"
        | Cursor.Block, false -> "2"
        | Cursor.Underline, true -> "3"
        | Cursor.Underline, false -> "4"
        | Cursor.Bar, true -> "5"
        | Cursor.Bar, false -> "6"

    let cursorShapeCode (shape: Cursor.Shape) (blink: string option) =
        let blinking =
            match blink with
            | Some "false" -> false
            | _ -> true // padrão do terminal é piscando
        Some(sprintf "%s[%s q" escape (cursorShapeParam shape blinking))
    
    let cursorVisibilityCode(visible: string option) =
        match visible with
        | Some "false" -> Some(sprintf "%s[?25l" escape)
        | Some "true" -> Some(sprintf "%s[?25h" escape)
        | _ -> None