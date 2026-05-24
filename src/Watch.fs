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
open PTML.ErrorHandle

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

    // antigo buffer
    let mutable previousBuffer = 
        createBuffer (getViewport().SafeWidth) (getViewport().SafeHeight)
    let asyncSetting(terminal: Terminal, path) = 
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
                |> List.iter (DiffRenderer.renderDiffs)
            
            previousBuffer <- buffer
        }

    // previous error message
    let mutable msn: string = ""
    let setWatcher(path: string) =
        let terminal: Terminal = getViewport()

        let mutable fullPath = Path.GetFullPath(path) 
        if fullPath = "" then                           // this is inconsistent, but works ¯\_(ツ)_/¯
            fullPath <- "../" + path
        else    
        let directory = Path.GetDirectoryName(fullPath)
        let fileName = Path.GetFileName(fullPath)

        // setting watcher 
        let watcher = new FileSystemWatcher()
        watcher.Path <- directory
        watcher.Filter <- fileName
        watcher.NotifyFilter <- NotifyFilters.LastWrite

        asyncSetting(terminal, path) |> Async.RunSynchronously

        // do smthng when file is changed
        watcher.Changed.Add(fun _ ->
            try
                asyncSetting(terminal, path) |> Async.RunSynchronously
                ErrorHandle.clearError msn      // clear previous error message from 'with'
            with ex ->
                ErrorHandle.renderError (ex.Message)
                msn <- ex.Message
        )

        watcher.EnableRaisingEvents <- true
        Console.ReadLine() |> ignore

    let watch(path: string): Status = 
        let mutable S: Status = Runner.run(path)
        if S = Status.Error then
            Status.Error
        else
            setWatcher(path) 
            S

