namespace PTML
open System
open System.Text
open PTML.Buffer
open System.Threading
open PTML.Spinner
open PTML.State

module Output =
    let private escape = "\x1b"
    let private resetCode = sprintf "%s[0m" escape
    let private bell = "\x07"

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
        | Some "overline" -> Some "53"
        | Some "double-underline" -> Some "21"
        | _ -> None

    // OSC 8 ; ; URL BEL -> abre um hyperlink; o mesmo OSC com URL vazia fecha
    let private urlCode = function
        | Some (url: string) -> Some(sprintf "%s]8;;%s%s" escape url bell)
        | None -> None

    let private urlCloseCode = sprintf "%s]8;;%s" escape bell
 
    // escreve o char da célula, envolvendo-o em OSC 8 (hyperlink) quando cell.url = Some url
    let private appendCellChar (sb: StringBuilder) (cell: Cell) =
        match urlCode cell.url with
        | Some openCode ->
            sb.Append(openCode) |> ignore
            sb.Append(cell.char) |> ignore
            sb.Append(urlCloseCode) |> ignore
        | None ->
            sb.Append(cell.char) |> ignore

    // OSC 12 ;color BEL -> define a cor do cursor de texto
    let private cursorColorCode (color: string option) =
        color
        |> Option.map (fun hex -> sprintf "%s]12;%s%s" escape hex bell)

    // ***shape*** + ***blink***: DECSCUSR combina os dois num único parâmetro
    // 1/2 = block (piscando/estático), 3/4 = underline, 5/6 = bar
    let private cursorShapeParam (shape: Cursor.Shape) (blinking: bool) =
        match shape, blinking with
        | Cursor.Block, true -> "1"
        | Cursor.Block, false -> "2"
        | Cursor.Underline, true -> "3"
        | Cursor.Underline, false -> "4"
        | Cursor.Bar, true -> "5"
        | Cursor.Bar, false -> "6"

    let private cursorShapeCode (shape: Cursor.Shape) (blink: string option) =
        let blinking =
            match blink with
            | Some "false" -> false
            | _ -> true // padrão do terminal é piscando
        Some(sprintf "%s[%s q" escape (cursorShapeParam shape blinking))

    // ***visible***: DECTCEM, só emite algo se o atributo foi declarado
    let private cursorVisibilityCode (visible: string option) =
        match visible with
        | Some "false" -> Some(sprintf "%s[?25l" escape)
        | Some "true" -> Some(sprintf "%s[?25h" escape)
        | _ -> None

    // Junta tudo que o <cursor> estiliza numa única sequência a ser escrita
    let private ansiCursorStyle (cursor: Cursor) =
        [ cursorShapeCode cursor.sh cursor.blk
          cursorColorCode cursor.clr
          cursorVisibilityCode cursor.v ]
        |> List.choose id
        |> String.concat ""

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
        || Option.isSome (urlCode cell.url)

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
                if shouldRenderCell cell || Option.isSome cell.cursor then
                    match cell.spinner, cell.cursor with
                    | Some c, cc -> ()
                    | None, Some cc ->
                        sb.Append(cursorTo x y) |> ignore
                        sb.Append(ansiCursorStyle cc) |> ignore
                    | None, None ->
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
                        appendCellChar sb cell

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
                        match cell.cursor with
                        | Some cursor ->
                            sb.Append(cursorTo x y) |> ignore
                            sb.Append(ansiCursorStyle cursor) |> ignore
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
                                appendCellChar sb cell
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
                                appendCellChar sb cell

                        lock Console.Out (fun () ->
                            Console.Out.Write(sb.ToString())
                        )
                    ))
            |]

        threads |> Array.iter (fun t -> t.Start())
        threads |> Array.iter (fun t -> t.Join())

    (* ===================== BUFFER STATE ===================== *)
    // A Output é quem de fato escreve no terminal físico, então é o lugar
    // certo pra saber "o terminal mudou de tamanho desde a última vez que
    // eu escrevi nele?" e reagir sozinha — sem depender de quem chama
    // lembrar de checar isso a cada frame.

    // O que a Output acredita estar na tela agora (último frame desenhado).
    let mutable private terminalState: State.BufferState option = None

    // Tamanho físico atual do terminal. Terminais sem TTY (pipe, CI, etc.)
    // lançam exceção ao ler WindowWidth/WindowHeight; nesse caso caímos
    // pro próprio tamanho do buffer que está sendo desenhado.
    let private physicalDimensions (fallbackWidth: int) (fallbackHeight: int) =
        try
            Console.WindowWidth, Console.WindowHeight
        with _ ->
            fallbackWidth, fallbackHeight

    let private ensureState width height =
        match terminalState with
        | Some s -> s
        | None ->
            let s = State.createState width height
            terminalState <- Some s
            s
            
    let getState () = terminalState
    let invalidateState () = terminalState <- None
    let private fullRedraw (buffer: Cell[,]) =
        Console.Clear()
        writeAnsiBuffer buffer

    // Trava de segurança: se o usuário estiver arrastando a borda da janela
    // continuamente, não queremos entrar num loop de redraw infinito.
    let private maxRedrawAttempts = 3
    let render (buffer: Cell[,]) =
        let bufferHeight = Array2D.length1 buffer
        let bufferWidth = Array2D.length2 buffer

        let rec attempt (n: int) =
            let state = ensureState bufferWidth bufferHeight
            let (widthBefore, heightBefore) = physicalDimensions bufferWidth bufferHeight
            let needsFullRedraw =
                state.firstRender || State.hasResized state widthBefore heightBefore

            if needsFullRedraw then
                fullRedraw buffer
            else
                writeAnsiBuffer buffer

            let (widthAfter, heightAfter) = physicalDimensions bufferWidth bufferHeight
            let changedDuringWrite =
                widthAfter <> widthBefore || heightAfter <> heightBefore

            if changedDuringWrite && n < maxRedrawAttempts then
                // o terminal mudou de tamanho embaixo da gente: descarta o
                // estado conhecido e tenta desenhar de novo já ciente disso
                terminalState <- Some(State.invalidate state widthAfter heightAfter)
                attempt (n + 1)
            else
                let (finalWidth, finalHeight) = physicalDimensions bufferWidth bufferHeight
                let synced, _ = State.sync state buffer finalWidth finalHeight
                terminalState <- Some synced

        attempt 1