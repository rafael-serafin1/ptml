(*DIFF RENDERER*)
(*ALTERA AS CÉLULAS EM CASO DE MUDANÇA NO ARQUIVO, DETECTADO PELO DIFF ENGINE*)
#load "diff.fsx"
open Diff
open System
open System.Text


let renderDiffs(diffs: CellChange list) =
    for diff in diffs do 
        Console.SetCursorPosition(diff.x, diff.y)
        Console.Write(diff.newCell.char)