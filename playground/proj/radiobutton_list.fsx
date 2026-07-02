open System

let MenuRadioButton(titulo: string, str: string array): int =   
    if (str = null || str.Length = 0) then
        failwith "Opções vazias."

    Console.CursorVisible <- false
    let mutable key: ConsoleKey = ConsoleKey.A
    let mutable index: int = 0
    
    while key <> ConsoleKey.Enter do
        Console.Clear();
        Console.WriteLine(titulo);
        Console.WriteLine();

        for i = 0 to str.Length - 1 do
            let mutable indicator: string = ""
            if i = index then
                indicator <- "[x] "
            else 
                indicator <- "[ ] "
            Console.WriteLine($"{indicator} {str[i]}")

        key <- Console.ReadKey(true).Key

        if key = ConsoleKey.UpArrow then
            if index = 0 then
                index <- str.Length - 1
            else
                index <- index - 1

        if key = ConsoleKey.DownArrow then
            if index = str.Length - 1 then
                index <- 0
            else
                index <- index + 1
    index

let opcoes: string array = [| "Opção A"; "Opção B"; "Opção C" |]
let escolhido: int = MenuRadioButton("Escolha uma opção:", opcoes)
Console.Clear()
Console.CursorVisible <- true
Console.WriteLine($"Você selecionou: {opcoes[escolhido]}")