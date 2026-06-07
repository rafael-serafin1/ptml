namespace PTML
open PTML.Render
open PTML.Spinner

module Buffer =
    (* BUFFER *)
    type Spinner = {
        tp: Types
        interval: string
        dur: string
        complete: string
    }
    type Cell = {
        char: char
        spinner: Spinner option
        foreground: string option
        background: string option
        font: string option
    }

    (* EMPTY CELL *)
    let emptyCell: Cell = {
        char = ' '
        spinner = None
        foreground = None
        background = None
        font = None
    }

    (* BUFFER INITIALIZATION *)
    let createBuffer width height =
        Array2D.create height width emptyCell

    (* SET CELL IN BUFFER *)
    let setCell buffer x y char foreground background font =
        if x >= 0 && x < Array2D.length2 buffer && y >= 0 && y < Array2D.length1 buffer then
            buffer.[y, x] <- {
                char = char
                spinner = None
                foreground = foreground
                background = background
                font = font
            }
    
    let setSpinnerCell(buffer, typ, x, y, inter, dur, complete, fg, bg)=
        if x >= 0 && x < Array2D.length2 buffer && y >= 0 && y < Array2D.length1 buffer then
            let char = (char)(firstFrame typ)
            buffer.[y, x] <- {
                char = char
                spinner = Some {
                    tp = typ
                    interval = inter
                    dur = dur
                    complete = complete
                }
                foreground = fg
                background = bg
                font = None
            }

    (* STEP 1: CLEAR BUFFER *)
    (* Limpa todas as células do buffer, preenchendo-as com células vazias *)
    let clearBuffer buffer =
        let height = Array2D.length1 buffer
        let width = Array2D.length2 buffer
        for y in 0 .. height - 1 do
            for x in 0 .. width - 1 do
                buffer.[y, x] <- emptyCell
        buffer

    (* STEP 2: RENDER OPERATIONS TO BUFFER *)
    (* Renderiza cada operação (DrawChar) do RenderTree diretamente no buffer *)
    (* Pega cada DrawChar e coloca cada caractere da string em células consecutivas *)
    let renderToBuffer buffer renderOps =
        renderOps |> List.iter (fun op ->
            match op with
            | Render.DrawChar(text, x, y, fg, bg, font) ->
                text
                |> Seq.iteri (fun offset ch ->
                    setCell buffer (x + offset) y (char ch) fg bg font)
            | Render.DrawSpinner(tp, x, y, inter, dur, complete, fg, bg) -> 
                tp 
                |> firstFrame
                |> Seq.iteri (fun offset ch -> 
                    setSpinnerCell(buffer, tp, (x + offset), y, inter, dur, complete, fg, bg))
        )
        buffer

    (* STEP 3: FINALIZE BUFFER *)
    (* Buffer final pronto com o estado completo da tela *)
    let finalizeBuffer(buffer) = buffer

    (* PIPELINE: CLEAR -> RENDER -> FINALIZE *)
    (* Função principal que executa os 3 passos *)
    (* Input: RenderTree operations, largura e altura do terminal *)
    (* Output: Buffer Cell[,] pronto para ser exibido *)
    let processRenderTree renderOps width height =
        let buffer = createBuffer width height
        buffer
            |> clearBuffer
            |> fun buf -> renderToBuffer buf renderOps
            |> finalizeBuffer