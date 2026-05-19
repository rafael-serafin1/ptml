open System.IO
open System

let path = "index.ptml"
let conteudo = File.ReadAllText(path)

let cmdw = Console.WindowWidth
let cmdh= Console.WindowHeight

printfn "Conteúdo do arquivo:"
printfn "------------------------\n"
printf "%s" conteudo
printfn "\n%d x %d" cmdw cmdh
