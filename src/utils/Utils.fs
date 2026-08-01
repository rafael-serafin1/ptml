namespace PTML
open System

module Utils =
    type Grounds = 
    | Foreground
    | Background

    let mutable shouldWindow: bool = false
    
    type Classify = 
    | even = 0
    | odd = 1

    let isEven(value: int): bool =
        if (value % 2) = 0 then true
        else false

    let classify_value(value: int, classe: Classify): bool = 
        if classe = Classify.even then
            isEven(value)
        else 
            not(isEven(value))

    // retorna quantas casas, antes da virgula, um numero tem
    let numberLength (number: int): int = 
        let stringfy = (string number)
        let size = stringfy.Length
        size

    // faz a regra de 3 para achar a porcentagem
    let regrade3(cem: int, num: int): float =
        (float) (num * 100 / cem)

    let hexadecimal(hex: string, ground: Grounds): string = 
        if hex.Length <> 7 || hex[0] <> '#' then
            failwith $"Invalid hexadecimal format: {hex}" 
        let r = Convert.ToInt32(hex.Substring(1, 2), 16)
        let g = Convert.ToInt32(hex.Substring(3, 2), 16)
        let b = Convert.ToInt32(hex.Substring(5, 2), 16)
        
        let mutable status = 0
        match ground with
        | Foreground -> 
            status <- 38
        | Background ->
            status <- 48
            
        let result = sprintf "%d;2;%d;%d;%d" status r g b
        result