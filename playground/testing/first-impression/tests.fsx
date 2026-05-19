// alguns tipos precisam de sufixo/prefixo
// uint precisa de sufixo 'u' bem merda.
// sbyte que corresponde a 8-bits integer precisa de sufixo 'y'
// byte que corresponde a 8-bits natural precisa de sufixo 'uy'
let mutable mutableValue: string = "This is a mutable value"   // esse valor pode ser alterado
let natural: uint32 = 10u   // precisa do sufixo 'u'

[<Literal>]
let literalValue = "This is a literal value"        // esse valor é constante

let mutable array: int array = [|1; 2; 3|]
let bool: bool = true

if mutableValue = "This is a mutable value" then
    printfn "The mutable value is: %s" mutableValue
else 
    printf "The mutable value is: %s" mutableValue