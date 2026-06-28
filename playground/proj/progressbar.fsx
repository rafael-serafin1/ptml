type ProgressType = 
    | Blocks
    | Dots
    | Square
    | TinySquare
    | Rhombus

let mutable defaultframe: char = '░'
let mutable semiframe: char = '▒'
let mutable framed: char = '█'
let mutable defaultCount: int = 10

// define 
let setFrames(tp: string) =
    let tp: ProgressType = 
        match tp with
        | "blocks" -> Blocks
        | "dots" -> Dots
        | "square" -> Square
        | "tiny-square" -> TinySquare
        | "rhombus" -> Rhombus
        | _ -> failwith $"Tipagem inexistente para <progress>: '{tp}'"
    match tp with
    | Blocks -> 
        ()
    | Dots ->
        defaultframe <- '○'
        semiframe <- '◍'
        framed <- '●'
    | Square ->
        defaultframe <- '□'
        semiframe <- '◩'
        framed <- '■'
    | TinySquare ->
        defaultframe <- '▫'
        semiframe <- '◾'
        framed <- '▪'
    | Rhombus ->
        defaultframe <- '◇'
        semiframe <- '◈'
        framed <- '◆'
    ()

// retorna um array de caracteres contendo o frame sem progresso
let initialframes(w: int, value: int, max: int) =
    let certain = max + 3
    if max < 0 then 
        failwith $"Valor máximo adicionado menor que 0: '{max}'"
    else 

    let charframes = Array.create w defaultframe
    charframes

// retorna um array de caracteres contendo o progresso totalitário
let framefy(w: int, h: int, tp: string, value: int, max: int, show: string) =
    let show: bool = 
        match show with
        | "true" -> true
        | "false" -> false
        | _ -> failwith $"Entrada inesperada para atributo booleano de <progress>: '{show}'"

    if w <> 0 && w > 0 then
        defaultCount <- w
    
    setFrames(tp)
    let mutable charframes: char array = initialframes(w, value, max)

    let percentage: int = value * defaultCount / max
    let floated: float = (float (max * defaultCount) / 100.0) - (float ((max - value) * defaultCount) / 100.0)
    let diff: float = floated - float percentage
    printf "%A, %A, %A\n" percentage floated diff
    // (100 * 10 / 100) - ((100 - 45) * 10 / 100) = 4.5
    for i = 0 to percentage - 1 do
        if diff = 0.5 && i = percentage - 1 then
            charframes[i] <- semiframe
        else 
            charframes[i] <- framed

    if show then
        let maxed: string = " " + (string) (value * 100 / max) + "%"
        let maxedtoarray = maxed.ToCharArray()
        let newcharframes: char array = Array.append charframes maxedtoarray

        System.String(newcharframes)
    else
        System.String(charframes)


let str = framefy(20, 1, "rhombus", 5, 10, "true")
printf "%s\n" str

let numberLength (number: int): int = 
    let stringfy = (string number)
    let size = stringfy.Length
    size
printf "Number: %i\n" (numberLength(10))