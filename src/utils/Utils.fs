namespace PTML

module Utils =
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