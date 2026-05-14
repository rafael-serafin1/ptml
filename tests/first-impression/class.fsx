type Class() =
    member this.Method() =
        printfn "Hello, World!\n"

let object: Class = Class()
object.Method()