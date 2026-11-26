namespace PTML
open System
open System.Text
open PTML.Global
open PTML.Utils

module OSC =
    let private ESC = "\x1b"
    let private BEL = "\x07"
    let private ST = "\x1b\\"

    let getOSC_entry (num: int) =
        $"{ESC}]{num};"

    let private buildString(str1: string) (str2: string) (compl: string): string  =
        let joined = str1 + compl + str2
        joined

//#region TERMINAL TITLES

    let changeWindowTitle (newTitle: string) = 
        Console.Write (buildString (getOSC_entry 2) BEL newTitle)

    let changeIconName (newTitle: string) =
        Console.Write (buildString (getOSC_entry 1) BEL newTitle)

//#endregion


//#region CURSOR

    // OSC 12 ;color BEL -> define a cor do cursor de texto
    let cursorColorCode (color: string option) =
        color
        |> Option.map (fun hex -> sprintf "%s]12;%s%s" ESC hex BEL)

//#endregion


//#region URL
    
    let urlCloseCode = sprintf "%s]8;;%s" ESC BEL

    // OSC 8 ; ; URL BEL -> abre um hyperlink; o mesmo OSC com URL vazia fecha
    let urlCode = function
        | Some (url: string) -> Some(sprintf "%s]8;;%s%s" ESC url BEL)
        | None -> None

//#endregion


//#region OSC_52
// é necessário implementar a execução dessa função no programa principal

    let private buildOSC52(str1: string)(char: char) = 
        sprintf "%s]%d;%c;%s%s" ESC 52 char str1 ST

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

//#region TERMINAL FORE/BACKGROUND

    let private terminalForeground (color: string) = 
        $"{ESC}]10;{color}{BEL}"

    let private terminalBackground (color: string)  = 
        $"{ESC}]11;{color}{BEL}"

    let terminalColorChange(color: string)(ground: Grounds) = 
        let curPos = Console.GetCursorPosition()

        match ground with
        | Foreground -> 
            Console.Write(terminalForeground color)
        | Background ->
            Console.Write(terminalBackground color)

        match curPos with
        | l, t -> Console.SetCursorPosition(l, t)

//#endregion