namespace PTML
open System
open System.Text
open PTML.Buffer
open System.Threading
open PTML.Spinner

module Output =
    let private escape = "\x1b"
    let private resetCode = sprintf "%s[0m" escape

    let private cursorTo x y =
        sprintf "%s[%d;%dH" escape (y + 1) (x + 1)

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

    let private styleCodes (cell: Cell) =
        [ 
        yield! Option.toList (foregroundCode cell.foreground)
        yield! Option.toList (backgroundCode cell.background)
        yield! Option.toList (fontCode cell.font) ]

    let private ansiStyle cell =
        match styleCodes cell with
        | [] -> None
        | codes -> Some(sprintf "%s[%sm" escape (String.concat ";" codes))

    let private shouldRenderCell (cell: Cell) =
        cell.char <> ' '
        || Option.isSome (foregroundCode cell.foreground)
        || Option.isSome (backgroundCode cell.background)
        || Option.isSome (fontCode cell.font)

    let private shouldRenderSpinner(cell: Cell) =
        cell.char <> ' '
        && Option.isSome (cell.spinner)
        || Option.isSome (foregroundCode cell.foreground)
        || Option.isSome (backgroundCode cell.background)
        || Option.isSome (fontCode cell.font)
    let bufferToAnsi (buffer: Cell[,]) =
        let height = Array2D.length1 buffer
        let width = Array2D.length2 buffer
        let sb = StringBuilder()
        let mutable currentStyle = ""

        for y in 0 .. height - 1 do
            for x in 0 .. width - 1 do
                let cell = buffer.[y, x]
                if shouldRenderCell cell then
                    match cell.spinner with
                    | Some c -> ()
                    | None ->
                        sb.Append(cursorTo x y) |> ignore
                        match ansiStyle cell with
                        | Some style when style <> currentStyle ->
                            if currentStyle <> "" then
                                sb.Append(resetCode) |> ignore
                            sb.Append(style) |> ignore
                            currentStyle <- style
                        | None when currentStyle <> "" ->
                            sb.Append(resetCode) |> ignore
                            currentStyle <- ""
                        | _ -> ()
                        sb.Append(cell.char) |> ignore

        if currentStyle <> "" then
            sb.Append(resetCode) |> ignore
        sb.ToString()

    // escreve um por um normalmente
    let printAnsiBuffer (buffer: Cell[,]) =
        Console.Write(bufferToAnsi buffer)

    // concatena tudo em uma string e escreve man
    let writeAnsiBuffer (buffer: Cell[,]) =
        let height = Array2D.length1 buffer

        let lines =
            Array.Parallel.init height (fun y ->
                let sb = StringBuilder()

                let width = Array2D.length2 buffer
                let mutable currentStyle = ""

                for x in 0 .. width - 1 do
                    let cell = buffer.[y, x]
                    match cell.spinner with
                    | Some c ->
                        if shouldRenderSpinner cell then
                            sb.Append(cursorTo x y) |> ignore
                            let t =
                                Thread(ThreadStart(fun () ->
                                    Spinner.drawSpinner(
                                        c.tp,
                                        x,
                                        y,
                                        c.interval,
                                        c.dur,
                                        c.complete
                                    )
                                ))
                            t.Start()
                            match ansiStyle cell with
                            | Some style when style <> currentStyle ->
                                if currentStyle <> "" then
                                    sb.Append(resetCode) |> ignore

                                sb.Append(style) |> ignore
                                currentStyle <- style

                            | None when currentStyle <> "" ->
                                sb.Append(resetCode) |> ignore
                                currentStyle <- ""

                            | _ -> ()
                    | None ->
                        if shouldRenderCell cell then
                            sb.Append(cursorTo x y) |> ignore
                            match ansiStyle cell with
                            | Some style when style <> currentStyle ->
                                if currentStyle <> "" then
                                    sb.Append(resetCode) |> ignore
                                sb.Append(style) |> ignore
                                currentStyle <- style
                            | None when currentStyle <> "" ->
                                sb.Append(resetCode) |> ignore
                                currentStyle <- ""
                            | _ -> ()
                            sb.Append(cell.char) |> ignore
                if currentStyle <> "" then
                    sb.Append(resetCode) |> ignore
                sb.ToString()
            )
        Console.Out.Write(String.Concat(lines))

    // usa threads para escrever cada linhas da matriz
    let writeAll(buffer: Cell[,]) = 
        let height = Array2D.length1 buffer

        let threads =
            [|
                for y in 0 .. height - 1 ->
                    Thread(ThreadStart(fun () ->
                        let sb = StringBuilder()

                        let width = Array2D.length2 buffer

                        for x in 0 .. width - 1 do
                            let cell = buffer.[y,x]

                            if shouldRenderCell cell then
                                sb.Append(cursorTo x y) |> ignore
                                sb.Append(cell.char) |> ignore

                        lock Console.Out (fun () ->
                            Console.Out.Write(sb.ToString())
                        )
                    ))
            |]

        threads |> Array.iter (fun t -> t.Start())
        threads |> Array.iter (fun t -> t.Join())