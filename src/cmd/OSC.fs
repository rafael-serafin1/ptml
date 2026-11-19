namespace PTML
open System
open System.Text
open PTML.Global

module OSC =
    let ESC_escape = "\x1b"
    let OSC_escape = "\x07"
    let getOSC_entry (num: int) =
        $"{ESC_escape}]{num};"

    let private buildString(str1: string) (str2: string) (compl: string): string  =
        let joined = str1 + compl + str2
        joined

//#region TERMINAL TITLES

    let changeWindowTitle (newTitle: string) = 
        Console.Write (buildString (getOSC_entry 2) OSC_escape newTitle)

    let changeIconName (newTitle: string) =
        Console.Write (buildString (getOSC_entry 1) OSC_escape newTitle)

//#endregion


//#region CURSOR

    // OSC 12 ;color BEL -> define a cor do cursor de texto
    let cursorColorCode (color: string option) =
        color
        |> Option.map (fun hex -> sprintf "%s]12;%s%s" ESC_escape hex OSC_escape)

//#endregion


//#region URL
    
    let urlCloseCode = sprintf "%s]8;;%s" ESC_escape OSC_escape

    // OSC 8 ; ; URL BEL -> abre um hyperlink; o mesmo OSC com URL vazia fecha
    let urlCode = function
        | Some (url: string) -> Some(sprintf "%s]8;;%s%s" ESC_escape url OSC_escape)
        | None -> None

//#endregion


//#region OSC_52
// é necessário implementar a execução dessa função no programa principal

    let private buildOSC52(str1: string)(char: char) = 
        sprintf "%s]%d;%c;%s%s" OSC_escape 52 char str1 ESC_escape

    let private toBase64 (text: string) =
        Convert.ToBase64String(Encoding.UTF8.GetBytes text)

    let copyToClipboard(text: string) =
        let encoded = toBase64 text
        let osc_str = buildOSC52 encoded 'c'
        Console.Write osc_str
        Console.SetCursorPosition(0, 0)

    let copyToPrimary(text: string) =
        let encoded = toBase64 text
        let osc_str = buildOSC52 encoded 'p'
        Console.Write osc_str
        Console.SetCursorPosition(0, 0)

    let copyToSecondary(text: string) = 
        let encoded = toBase64 text
        let osc_str = buildOSC52 encoded 'q'
        Console.Write osc_str
        Console.SetCursorPosition(0, 0)

    let handleContent(content: ContentAttrValue)(text: string) = 
        match content with
        | Clipboard -> copyToClipboard text
        | Primary   -> copyToPrimary   text
        | Secondary -> copyToSecondary text

//#endregion