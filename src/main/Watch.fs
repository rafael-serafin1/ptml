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
open PTML.Depth
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
        createBuffer (getOutputViewport().SafeWidth) (getOutputViewport().SafeHeight)
    let mutable firstRender = true
    let asyncSetting(terminal: Terminal, path) = 
        async {
            let input: string = readWhenReady path 10
            let tokens = lex input 0 []
            parser(tokens, [])

            let ast: AstNode list = buildAst(tokens)
            let semantic: Widget list = buildSemanticTree(ast)
            let layout = layoutTree semantic
            let filteredLayout, depthLayers = Depth.extractDepthLayers layout
            let renderOps = renderTree filteredLayout

            let baseBuffer = processRenderTree renderOps terminal.SafeWidth terminal.SafeHeight
            let buffer = Depth.composeDepthLayers baseBuffer depthLayers

            if Utils.shouldWindow = false then
                Console.WindowWidth <- 219
                Console.WindowHeight <- 55

            if firstRender then
                Console.Clear()
                DiffRenderer.renderBuffer buffer
                firstRender <- false
            else
                DiffRenderer.renderBufferDiffs previousBuffer buffer
            
            let bufferHeight = buffer.GetLength(0)
            let bufferWidth = buffer.GetLength(1)
            for y = 0 to bufferHeight - 1 do
                for x = 0 to bufferWidth - 1 do
                    let cell = buffer[y, x]
                    match cell.spinner with
                    | Some c -> 
                        Spinner.threadDraw(c.tp, x, y, c.interval, c.dur, c.complete)
                    | None -> ()

            previousBuffer <- buffer
        }

    // previous error message
    let mutable msn: string = ""

    // impede que dois renders rodem ao mesmo tempo (FileSystemWatcher pode
    // disparar eventos em threads do pool concorrentemente) e centraliza
    // o tratamento de erro para o render inicial e para os re-renders
    let renderLock = obj()
    let runRender(terminal: Terminal, path: string) =
        lock renderLock (fun () ->
            try
                asyncSetting(terminal, path) |> Async.RunSynchronously
                ErrorHandle.clearError msn      // clear previous error message from 'with'
            with ex ->
                ErrorHandle.renderError (ex.Message)
                msn <- ex.Message
        )

    let setWatcher(path: string) =
        let mutable terminal: Terminal = getOutputViewport()
        let mutable fullPath = Path.GetFullPath(path) 
        if fullPath = "" then                           // this is inconsistent, but works ¯\_(ツ)_/¯
            fullPath <- "../" + path

        let directory = Path.GetDirectoryName(fullPath)
        let fileName = Path.GetFileName(fullPath)

        // setting watcher 
        let watcher = new FileSystemWatcher()
        watcher.Path <- directory
        watcher.Filter <- fileName
        watcher.NotifyFilter <- 
            NotifyFilters.LastWrite
            ||| NotifyFilters.FileName
            ||| NotifyFilters.Size
            ||| NotifyFilters.CreationTime
        watcher.InternalBufferSize <- 64 * 1024        // reduz risco de overflow em rajadas de eventos

        // primeiro render também precisa ficar protegido: um arquivo já
        // salvo com erro de sintaxe não pode derrubar o processo inteiro
        runRender(terminal, path)

        // muitos editores (VSCode, JetBrains, Vim, etc.) disparam VÁRIOS
        // eventos "Changed" para um único save (escrita + flush + metadata),
        // e alguns salvam via arquivo temporário + rename, o que dispara
        // "Renamed"/"Created" em vez de "Changed". Sem agrupar (debounce)
        // essas rajadas, o watcher tenta re-renderizar várias vezes em
        // paralelo, lendo o arquivo pela metade e corrompendo o diff.
        let debounceMs = 120
        let mutable debounceTimer: Timer = null
        let scheduleRender () =
            lock renderLock (fun () ->
                if not (isNull debounceTimer) then
                    debounceTimer.Dispose()
                debounceTimer <-
                    new Timer(
                        (fun _ ->
                            terminal <- getOutputViewport()   // update terminal size
                            runRender(terminal, path)),
                        null, debounceMs, Timeout.Infinite)
            )

        // do smthng when file is changed
        watcher.Changed.Add(fun _ -> scheduleRender())
        watcher.Renamed.Add(fun _ -> scheduleRender())
        watcher.Created.Add(fun _ -> scheduleRender())

        // se o watcher perder eventos por overflow do buffer interno,
        // isso falha silenciosamente sem esse handler
        watcher.Error.Add(fun e ->
            ErrorHandle.renderError (e.GetException().Message)
        )

        watcher.EnableRaisingEvents <- true
        Console.ReadLine() |> ignore

    let watch(path: string): Status = 
        Console.CursorVisible <- false
        setWatcher(path) 
        Status.Success

