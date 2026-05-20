module Buffer

(* TERMINAL BUFFER LOGIC *)
#load "./render.fsx"
open Render

(* ENUM PARA FORMATAÇÃO DE COR *)
type ConsoleColor = 
    | None = 0
    | Black = 30
    | Red = 31
    | Green = 32
    | Gold = 33
    | Blue = 34 
    | Purple = 35
    | Cyan = 36

(* ENUM PARA FORMATAÇÃO DE FONTE *)
type ConsoleFont = 
    | None = 0
    | Bold = 1
    | Dim = 2
    | Italic = 3
    | UnderLine = 4
    | SlowBlink = 5
    | RapidBlink = 6
    | Marked = 7
    | Conceal = 8
    | StrikeThrough = 9

(* BUFFER *)
type Cell = {
    char: char
    foreground: string option
    background: string option
    font: string option
}

(* EMPTY CELL *)
let emptyCell: Cell = {
    char = ' '
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
            foreground = foreground
            background = background
            font = font
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