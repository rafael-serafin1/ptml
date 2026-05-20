#load "../scripting/output.fsx"
open System
open System.IO
open System.Threading
open Lexer          (*LEXER*)
open Parser         (*PARSER*)
open Tree           (*AST & SEMANTIC TREE*)
open Layout         (*LAYOUT PASS*)
open Render         (*TREE RENDER*)
open Buffer         (*TERMINAL BUFFER*)
open Diff           (*DIFF ENGINE*)
open Output        
open Diffrenderer

type Status =
    | Success = 0
    | Failure = 1

let rec readWhenReady path retries =
    try
        File.ReadAllText(path)
    with
    | :? IOException ->
        if retries <= 0 then
            reraise()
        Thread.Sleep(50)
        readWhenReady path (retries - 1)

// buffer anterior
let mutable previousBuffer = 
    createBuffer (getViewport().SafeWidth) (getViewport().SafeHeight)

let asyncSetting(terminal: TerminalViewport, path) = 
    async {
        let input: string = readWhenReady path 10
        let tokens = lex input 0 []
        parser(tokens, [])

        let ast: AstNode list = buildAst(tokens)
        let semantic: Widget list = buildSemanticTree(ast)
        let layout = layoutTree semantic
        let renderOps = renderTree layout

        let buffer = processRenderTree renderOps terminal.SafeWidth terminal.SafeHeight

        Diff.diffBuffers previousBuffer buffer
            |> List.iter renderDiffs

        previousBuffer <- buffer
    }
let setWatcher(path: string) =
    let terminal: TerminalViewport = getViewport()
    let mutable fullPath = Path.GetFullPath(path)   // ele morre aqui
    if fullPath = "" then   
        fullPath <- "../" + path
    else    
    let directory = Path.GetDirectoryName(fullPath)
    let fileName = Path.GetFileName(fullPath)

    let watcher = new FileSystemWatcher()
    watcher.Path <- directory
    watcher.Filter <- fileName
    watcher.NotifyFilter <- NotifyFilters.LastWrite

    asyncSetting(terminal, path) |> Async.RunSynchronously

    watcher.Changed.Add(fun _ ->
        asyncSetting(terminal, path) |> Async.RunSynchronously
    )
    watcher.EnableRaisingEvents <- true
    Console.ReadLine() |> ignore

let run(path: string): Status =
    let terminal = getViewport()

    let input: string = File.ReadAllText(path)
    let tokens = lex input 0 []
    parser(tokens, [])

    let ast: AstNode list = buildAst(tokens)
    let semantic: Widget list = buildSemanticTree(ast)

    let layout = layoutTree semantic
    let renderOps = renderTree layout

    let buffer = processRenderTree renderOps (terminal.ViewWidth) (terminal.ViewHeight)
    Console.Write("\x1b[2J\x1b[H")
    Output.printAnsiBuffer(buffer)
    printfn ""
    Status.Success

let watch(path: string): Status = 
    let mutable S: Status = run(path)
    setWatcher(path) 
    S

let path = "./index.ptml"
let S: Status = watch(path)