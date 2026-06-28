namespace PTML

module Utils =
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