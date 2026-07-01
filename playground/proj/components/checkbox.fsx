(* EXEMPLO DE CHECK BOX EM TERMINAL *)
open System

let selectedIndex = new ResizeArray<int>()
let checke: string = "[x] "

let MostrarOpcoes(str: string array, index: int) =
    Console.Clear()
    Console.WriteLine("Selecione uma opção: \n")

    let mutable i = 0
    Console.CursorVisible <- false
    for i = 0 to (str.Length - 1) do
        if i = index && not (selectedIndex.Contains(i)) then
            Console.ForegroundColor <- ConsoleColor.Yellow
            Console.Write checke
        elif i = index && selectedIndex.Contains(i) then
            Console.ForegroundColor <- ConsoleColor.DarkRed
            Console.Write checke
        elif selectedIndex.Contains(i) then
            Console.ForegroundColor <- ConsoleColor.Green
            Console.Write checke
        else
            Console.ForegroundColor <- ConsoleColor.Gray
            Console.Write("[ ] ")
        Console.WriteLine(str[i])
    Console.ResetColor()

let opcoes: string array = [|"Comprar"; "Vender"; "Perecer"|]
let mutable index: int = 0
let mutable tecla: ConsoleKey option = None
let mutable count: int = 0

while tecla <> Some (ConsoleKey.Backspace) do
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
        elif t = ConsoleKey.Enter then
            if not (selectedIndex.Contains(index)) then 
                selectedIndex.Add(index)
            elif selectedIndex.Contains(index) then
                selectedIndex.Remove(index) |> ignore
    | None -> ()
    count <- count + 1


Console.Clear()
Console.CursorVisible <- true
Console.WriteLine($"Você selecionou: {opcoes[index]}")
Console.WriteLine("---------------------------------")
let items = selectedIndex.ToArray() |> Array.toList |> List.iter (fun c -> (Console.WriteLine($"Opção selecionada: {opcoes[c]}")))
