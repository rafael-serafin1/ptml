open System.IO

let path = "index.ptml"
let contents = File.ReadAllLines(path)

for content in contents do
    printf "%s\n" content


//
let conteudo = File.ReadAllText(path)

printfn "Conteúdo do arquivo:"
printfn "------------------------\n"
printf "%s" conteudo
