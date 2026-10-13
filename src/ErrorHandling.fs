namespace PTML
open System

module ErrorHandle =
    let mutable private errorLine: int option = None

    let renderError (msg: string) =
        let struct (cursorX, cursorY) = Console.GetCursorPosition()
        let width = max 1 Console.WindowWidth
        let height = max 1 Console.WindowHeight
        let line = min (height - 1) (cursorY + 1)
        let visibleMessage =
            if msg.Length > width then msg.Substring(0, width)
            else msg.PadRight(width)

        Console.SetCursorPosition(0, line)
        Console.Write($"\x1b[2K\x1b[31m{visibleMessage}\x1b[0m")
        Console.SetCursorPosition(min cursorX (width - 1), min cursorY (height - 1))
        errorLine <- Some line
    
    let clearError (_message: string) =
        match errorLine with
        | Some line ->
            let struct (cursorX, cursorY) = Console.GetCursorPosition()
            let width = max 1 Console.WindowWidth
            let height = max 1 Console.WindowHeight
            Console.SetCursorPosition(0, min line (height - 1))
            Console.Write(String.replicate width " ")
            Console.SetCursorPosition(min cursorX (width - 1), min cursorY (height - 1))
            errorLine <- None
        | None -> ()