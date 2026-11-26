namespace PTML

open System
open PTML.Diff
open PTML.Buffer
open PTML.FontCode

module DiffRenderer =
    let private escape = "\x1b"
    let private bell = "\x07"
    let private reset = "\x1b[0m"
    let private resetCursorStyle = "\x1b[0 q\x1b]112\x07\x1b[?25h"

    let private styleCodes (cell: Cell) =
        [
            yield! Option.toList (foregroundCode cell.foreground)
            yield! Option.toList (backgroundCode cell.background)
            yield! Option.toList (fontCode cell.font)
        ]

    let private ansiStyle (cell: Cell) =
        match styleCodes cell with
        | [] -> reset
        | codes -> sprintf "%s[%sm" escape (String.concat ";" codes)

    let private cursorStyle (cell: Cell) =
        cell.cursor
        |> Option.map (fun cursor ->
            [
                cursorShapeCode cursor.sh cursor.blk
                cursor.clr |> Option.map (fun color -> sprintf "%s]12;%s%s" escape color bell)
                cursorVisibilityCode cursor.v
            ]
            |> List.choose id
            |> String.concat "")

    let private writeCell (x: int) (y: int) (cell: Cell) =
        Console.SetCursorPosition(x, y)

        match cursorStyle cell with
        | Some style -> Console.Write(style)
        | None ->
            Console.Write(ansiStyle cell)
            match cell.url with
            | Some url ->
                Console.Write(sprintf "%s]8;;%s%s" escape url bell)
                Console.Write(cell.char)
                Console.Write(sprintf "%s]8;;%s" escape bell)
            | None -> Console.Write(cell.char)
            Console.Write(reset)

    let private hasCursor (buffer: Cell[,]) =
        let mutable found = false
        for y = 0 to Array2D.length1 buffer - 1 do
            for x = 0 to Array2D.length2 buffer - 1 do
                if Option.isSome buffer[y, x].cursor then
                    found <- true
        found

    let renderDiffs (diff: CellChange) =
        match diff.newCell with
        | Some cell -> writeCell diff.x diff.y cell
        | None ->
            Console.SetCursorPosition(diff.x, diff.y)
            Console.Write(' ')

    let renderBufferDiffs (oldBuffer: Cell[,]) (newBuffer: Cell[,]) =
        diffBuffers oldBuffer newBuffer
        |> List.iter renderDiffs
        if hasCursor oldBuffer && not (hasCursor newBuffer) then
            Console.Write(resetCursorStyle)
        Console.SetCursorPosition(0, 0)

    let renderBuffer (buffer: Cell[,]) =
        let height = Array2D.length1 buffer
        let width = Array2D.length2 buffer

        for y in 0 .. height - 1 do
            for x in 0 .. width - 1 do
                writeCell x y buffer[y, x]

        if not (hasCursor buffer) then
            Console.Write(resetCursorStyle)
        Console.SetCursorPosition(0, 0)
