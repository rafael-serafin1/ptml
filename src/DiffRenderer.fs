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

    let renderDiffs (diff: Diff.CellChange) =
        Console.SetCursorPosition(diff.x, diff.y)

        Console.Write(ansiStyle diff.newCell)
        Console.Write(diff.newCell.char)