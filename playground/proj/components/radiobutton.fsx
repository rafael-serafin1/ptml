(* EXEMPLO DE LISTA DE RADIO BUTTONS EM TERMINAL *)
open System

let MostrarOpcoes(str: string array, index: int) =
    Console.Clear()
    Console.WriteLine("Selecione uma opção: \n")

    let mutable i = 0
    Console.CursorVisible <- false
    for i = 0 to (str.Length - 1) do
        if i = index then
            Console.ForegroundColor <- ConsoleColor.Green
            Console.Write("[X] ")
        else
            Console.ForegroundColor <- ConsoleColor.Gray
            Console.Write("[ ] ")
        Console.WriteLine(str[i])
    Console.ResetColor()

let opcoes: string array = [|"Comprar"; "Vender"; "Perecer"|]
let mutable index: int = 0
let mutable tecla: ConsoleKey option = None

while tecla <> Some (ConsoleKey.Enter) do
    MostrarOpcoes(opcoes, index)
    tecla <- Some (Console.ReadKey(true).Key)

    match tecla with
    | Some t ->
        if t = ConsoleKey.UpArrow then
            if index = 0 then
                index <- opcoes.Length - 1
            else 
                index <- index - 1
        elif t = ConsoleKey.DownArrow then
            if index = (opcoes.Length - 1) then
                index <- 0
            else
                index <- index + 1
    | None -> ()

Console.Clear()
Console.CursorVisible <- true
Console.WriteLine($"Você selecionou: {opcoes[index]}")

        