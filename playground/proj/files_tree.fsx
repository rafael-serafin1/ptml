open System.IO

let rec printTree indent (path: string) =
    printfn "%s%s" indent (Path.GetFileName(path))

    for dir in Directory.GetDirectories(path) do
        printTree (indent + "│   ") dir

    for file in Directory.GetFiles(path) do
        printfn "%s├── %s" (indent + "│   ") (Path.GetFileName(file))

printTree "" @"C:\Users\Usuario\Documents\rafael\projeto\ptml"