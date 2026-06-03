namespace PTML
open System
open PTML.Diff
open PTML.Buffer

module DiffRenderer =
    let private ansi = "\x1b["
    let private foregroundCode = function
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
        | _ -> None

    let private backgroundCode = function
        | Some "black" -> Some "40"
        | Some "red" -> Some "41"
        | Some "green" -> Some "42"
        | Some "gold" -> Some "43"
        | Some "blue" -> Some "44"
        | Some "purple" -> Some "45"
        | Some "cyan" -> Some "46"
        | Some "white" -> Some "47"
        | _ -> None

    let private fontCode = function
        | Some "bold" -> Some "1"
        | Some "dim" -> Some "2"
        | Some "italic" -> Some "3"
        | Some "underline" -> Some "4"
        | Some "slow-blink" -> Some "5"
        | Some "rapid-blink" -> Some "6"
        | Some "reverse" -> Some "7"
        | Some "conceal" -> Some "8"
        | Some "strike-through" -> Some "9"
        | _ -> None

    let private styleCodes cell =
        [
            yield! Option.toList (foregroundCode cell.foreground)
            yield! Option.toList (backgroundCode cell.background)
            yield! Option.toList (fontCode cell.font)
        ]

    let private ansiStyle cell =
        match styleCodes cell with
        | [] -> sprintf "%s0m" ansi
        | codes -> sprintf "%s%sm" ansi (String.concat ";" codes)

    let private reset = "\x1b[0m"
    let mutable currentCursorPos: ValueTuple<int, int> option = None
    let newDiffs (diff: Diff.CellChange) =  
        async { 
            currentCursorPos <- Some (Console.GetCursorPosition())
            Console.SetCursorPosition(diff.x, diff.y)
            match diff.newCell with
            | Some n ->
                Console.Write(ansiStyle n)
                Console.Write($"{n.char}{reset}")
                match currentCursorPos with
                | Some (x, y) -> Console.SetCursorPosition(x, y)
                | None -> ()
            | None -> ()
        }
    let unrenderOldCell(X: int, Y: int) = 
        async {
            currentCursorPos <- Some (Console.GetCursorPosition())
            Console.SetCursorPosition(X, Y)
            Console.Write " "
            match currentCursorPos with
            | Some (x, y) -> Console.SetCursorPosition(x, y)
            | None -> ()
        }

    let renderDiffs (diff: Diff.CellChange) =
        match diff.oldCell, diff.newCell with 
        | Some o, None -> 
            unrenderOldCell(diff.x, diff.y) |> Async.RunSynchronously
        | None, Some n -> 
            newDiffs diff |> Async.RunSynchronously 
        | Some o, Some n -> 
            if o <> n then 
                newDiffs(diff) |> Async.RunSynchronously
            else 
                ()
        | None, None -> ()

    let renderBufferDiffs (oldBuffer: Buffer.Cell[,]) (newBuffer: Buffer.Cell[,]) =
        diffBuffers oldBuffer newBuffer
        |> List.iter renderDiffs

    let renderBuffer (buffer: Buffer.Cell[,]) =
        let height = Array2D.length1 buffer
        let width = Array2D.length2 buffer
        for y in 0 .. height - 1 do
            for x in 0 .. width - 1 do
                let cell = buffer.[y, x]
                Console.SetCursorPosition(x, y)
                if cell = emptyCell then
                    Console.Write(reset)
                    Console.Write(' ')
                else
                    Console.Write(ansiStyle cell)
                    Console.Write(cell.char)
                    Console.Write(reset)
        Console.SetCursorPosition(0, 0)