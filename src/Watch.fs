namespace PTML
open System
open System.Text
open System.Threading
open System.IO
open PTML.Token
open PTML.Lexer
open PTML.Parser
open PTML.Tree
open PTML.Layout
open PTML.Buffer
open PTML.Render
open PTML.Buffer
open PTML.DiffRenderer

module Watch =
    let rec readWhenReady path retries =
        try
            File.ReadAllText(path)
        with
        | :? IOException ->
            if retries <= 0 then
                reraise()

            Thread.Sleep(50)
            readWhenReady path (retries - 1)

    let asyncSetting(terminal: Terminal, path) = 
        async {
            let input: string = readWhenReady path 10
            let tokens = lex input 0 []
            parser(tokens, [])

            let ast: AstNode list = buildAst(tokens)
            let semantic: Widget list = buildSemanticTree(ast)
            let layout = layoutTree semantic
            let renderOps = renderTree layout
            let buffer = processRenderTree renderOps terminal.SafeWidth terminal.SafeWidth
            let emptyBuffer = createBuffer terminal.SafeWidth terminal.SafeWidth

            Diff.diffBuffers emptyBuffer buffer 
                |> List.iter (DiffRenderer.renderDiffs)
        }

    let setWatcher(path: string) =
        let terminal: Terminal = getViewport()

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

        let input: string = File.ReadAllText(path)
        let tokens = lex input 0 []
        parser(tokens, [])

        let ast: AstNode list = buildAst(tokens)
        let semantic: Widget list = buildSemanticTree(ast)
        let layout = layoutTree semantic
        let renderOps = renderTree layout
        let buffer = processRenderTree renderOps terminal.SafeWidth terminal.SafeWidth
        let emptyBuffer = createBuffer terminal.SafeWidth terminal.SafeWidth

        watcher.Changed.Add(fun _ ->
            asyncSetting(terminal, path) |> Async.RunSynchronously
        )

        watcher.EnableRaisingEvents <- true
        Console.ReadLine() |> ignore

    let watch(path: string): Status = 
        let mutable S: Status = Runner.run(path)
        setWatcher(path) 
        S

