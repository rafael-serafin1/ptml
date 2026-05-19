namespace PTML
open PTML.Buffer

module Diff = 
    type CellChange = {
        x: int
        y: int
        oldCell: Cell
        newCell: Cell
    }

    let private cellEquals (a: Cell) (b: Cell) =
        a.char = b.char
        && a.foreground = b.foreground
        && a.background = b.background
        && a.font = b.font

    let diffBuffers (oldBuffer: Cell[,]) (newBuffer: Cell[,]) : CellChange list =
        let oldHeight = Array2D.length1 oldBuffer
        let oldWidth = Array2D.length2 oldBuffer

        let newHeight = Array2D.length1 newBuffer
        let newWidth = Array2D.length2 newBuffer

        let height = max oldHeight newHeight
        let width = max oldWidth newWidth

        [
            for y in 0 .. height - 1 do
                for x in 0 .. width - 1 do

                    let oldCell =
                        if y < oldHeight && x < oldWidth then
                            oldBuffer.[y, x]
                        else
                            emptyCell

                    let newCell =
                        if y < newHeight && x < newWidth then
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
            sprintf "(%d,%d): '%c' -> '%c'" diff.x diff.y diff.oldCell.char diff.newCell.char)

    let printDiffs oldBuffer newBuffer =
        diffBuffers oldBuffer newBuffer
        |> diffToLines
        |> List.iter (printfn "%s")