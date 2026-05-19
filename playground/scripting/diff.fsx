(* DIFF ENGINE LOGIC *)
#load "buffer.fsx"
open Buffer

type CellChange = {
    x: int
    y: int
    oldCell: Cell
    newCell: Cell
}

let private cellEquals a b =
    a.char = b.char
    && a.foreground = b.foreground
    && a.background = b.background
    && a.font = b.font

let diffBuffers (oldBuffer: Cell[,]) (newBuffer: Cell[,]) : CellChange list =
    let height = max (Array2D.length1 oldBuffer) (Array2D.length1 newBuffer)
    let width = max (Array2D.length2 oldBuffer) (Array2D.length2 newBuffer)

    [ for y in 0 .. height - 1 do
        for x in 0 .. width - 1 do
            let oldCell =
                if x < Array2D.length2 oldBuffer && y < Array2D.length1 oldBuffer then
                    oldBuffer.[y, x]
                else
                    emptyCell

            let newCell =
                if x < Array2D.length2 newBuffer && y < Array2D.length1 newBuffer then
                    newBuffer.[y, x]
                else
                    emptyCell

            if not (cellEquals oldCell newCell) then
                yield {
                    x = x
                    y = y
                    oldCell = oldCell
                    newCell = newCell
                } 
    ]

let diffToLines diffs =
    diffs
    |> List.map (fun diff ->
        sprintf "(%d,%d): '%s' -> '%s'" diff.x diff.y diff.oldCell.char diff.newCell.char)

let printDiffs oldBuffer newBuffer =
    diffBuffers oldBuffer newBuffer
    |> diffToLines
    |> List.iter (printfn "%s")