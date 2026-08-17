namespace PTML
open System
open PTML.Diff
open PTML.Buffer
open PTML.FontCode

module DiffRenderer =
    let private ansi = "\x1b["
    let private ocs = "\x1b]"

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