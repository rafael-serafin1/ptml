namespace PTML
open System
open System.Text
open System.Threading
open System.IO
open PTML.Token

module ErrorHandle = 
    let cursorPos: ValueTuple<int, int> = Console.GetCursorPosition()
    let renderError (msg: string) =
        match cursorPos with
        | (x, y) -> Console.SetCursorPosition(x, y + 1)

        let clean = msg.PadRight(getOutputViewport().SafeWidth)

        Console.Write($"\x1b[31m{clean}\x1b[0m")
        match cursorPos with
        | (x, y) -> Console.SetCursorPosition(x, y)
    
    let clearError(msn: string) =
        if msn = "" then
            ()
        else 
            match cursorPos with
            | (x, y) ->
                Console.SetCursorPosition(0, y + 1)
                Console.Write(String.replicate msn.Length " ")
                Console.SetCursorPosition(x, y)