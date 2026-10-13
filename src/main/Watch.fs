namespace PTML

open System
open System.IO
open System.Threading
open PTML.Token
open PTML.Lexer
open PTML.Parser
open PTML.Tree
open PTML.Layout
open PTML.Buffer
open PTML.Render
open PTML.Depth
open PTML.DiffRenderer
open PTML.ErrorHandle

module Watch =
    let rec readWhenReady (path: string) (retries: int) =
        let mutable lastError: IOException option = None
        let contents =
            try
                let firstRead = File.ReadAllText(path)
                Thread.Sleep(25)
                let secondRead = File.ReadAllText(path)
                if firstRead = secondRead then Some secondRead else None
            with
            | :? IOException as ex ->
                lastError <- Some ex
                None

        match contents with
        | Some text -> text
        | None when retries > 0 ->
            Thread.Sleep(50)
            readWhenReady path (retries - 1)
        | None ->
            match lastError with
            | Some ex -> raise ex
            | None -> raise (IOException($"The PTML file changed while it was being read: {path}"))

    let private samePath (left: string) (right: string) =
        let comparison =
            if OperatingSystem.IsWindows() then
                StringComparison.OrdinalIgnoreCase
            else
                StringComparison.Ordinal

        String.Equals(Path.GetFullPath(left), right, comparison)

    let setWatcher (path: string) =
        let fullPath = Path.GetFullPath(path)
        let directory = Path.GetDirectoryName(fullPath)
        let fileName = Path.GetFileName(fullPath)
        let renderLock = obj()
        let timerLock = obj()
        let shutdown = new CancellationTokenSource()
        let mutable previousBuffer: Cell[,] option = None
        let mutable previousSize: int * int = (0, 0)
        let mutable needsFullRender = true
        let mutable lastError = ""

        use watcher = new FileSystemWatcher(directory, "*")
        watcher.NotifyFilter <-
            NotifyFilters.LastWrite
            ||| NotifyFilters.FileName
            ||| NotifyFilters.Size
            ||| NotifyFilters.CreationTime
        watcher.InternalBufferSize <- 64 * 1024

        let renderCurrent () =
            try
                let input = readWhenReady fullPath 10
                if String.IsNullOrEmpty(input) then
                    failwith "PTML File is empty."

                let tokens = lex input 0 []
                parser(tokens, [])

                let ast: AstNode list = buildAst(tokens)
                let semantic: Widget list = buildSemanticTree(ast)
                let layout = layoutTree semantic
                let filteredLayout, depthLayers = Depth.extractDepthLayers layout
                let renderOps = renderTree filteredLayout
                let terminal = getOutputViewport()
                let baseBuffer =
                    processRenderTree renderOps terminal.SafeWidth terminal.SafeHeight
                let buffer = Depth.composeDepthLayers baseBuffer depthLayers
                let size = (Array2D.length2 buffer, Array2D.length1 buffer)

                Spinner.stopAll()

                let canApplyDiff =
                    not needsFullRender
                    && previousSize = size
                    && Option.isSome previousBuffer

                if canApplyDiff then
                    Console.SetCursorPosition(0, 0)
                    DiffRenderer.renderBufferDiffs previousBuffer.Value buffer
                else
                    Console.Clear()
                    DiffRenderer.renderBuffer buffer

                previousBuffer <- Some buffer
                previousSize <- size
                needsFullRender <- false
                lastError <- ""

                let startsSpinnerAt x y spinner =
                    let firstFrameWidth = max 1 (Spinner.firstFrame spinner.tp).Length
                    let mutable runStart = x
                    while runStart > 0 && buffer[y, runStart - 1].spinner = Some spinner do
                        runStart <- runStart - 1
                    (x - runStart) % firstFrameWidth = 0

                for y = 0 to Array2D.length1 buffer - 1 do
                    for x = 0 to Array2D.length2 buffer - 1 do
                        match buffer[y, x].spinner with
                        | Some spinner when startsSpinnerAt x y spinner ->
                            Spinner.threadDraw(
                                spinner.tp,
                                x,
                                y,
                                spinner.interval,
                                spinner.dur,
                                spinner.complete
                            )
                        | _ -> ()
            with ex ->
                Spinner.stopAll()
                ErrorHandle.renderError ex.Message
                lastError <- ex.Message
                needsFullRender <- true

        let timer =
            new Timer(
                TimerCallback(fun _ ->
                    lock renderLock (fun () ->
                        if not shutdown.IsCancellationRequested then
                            renderCurrent())),
                null,
                Timeout.Infinite,
                Timeout.Infinite
            )

        let scheduleRender () =
            lock timerLock (fun () ->
                if not shutdown.IsCancellationRequested then
                    timer.Change(120, Timeout.Infinite) |> ignore)

        let isTarget (eventPath: string) = samePath eventPath fullPath

        watcher.Changed.Add(fun event ->
            if isTarget event.FullPath then scheduleRender())
        watcher.Created.Add(fun event ->
            if isTarget event.FullPath then scheduleRender())
        watcher.Deleted.Add(fun event ->
            if isTarget event.FullPath then scheduleRender())
        watcher.Renamed.Add(fun event ->
            if isTarget event.FullPath || isTarget event.OldFullPath then
                scheduleRender())
        watcher.Error.Add(fun event ->
            lock renderLock (fun () ->
                if not shutdown.IsCancellationRequested then
                    let message = event.GetException().Message
                    if message <> lastError then
                        ErrorHandle.renderError message
                        lastError <- message
                    needsFullRender <- true)
            scheduleRender())

        let mutable cursorHidden = false
        try
            Console.CursorVisible <- false
            cursorHidden <- true
            watcher.EnableRaisingEvents <- true
            lock renderLock renderCurrent
            Console.ReadLine() |> ignore
        finally
            lock timerLock (fun () ->
                shutdown.Cancel()
                watcher.EnableRaisingEvents <- false
                timer.Dispose())

            lock renderLock (fun () ->
                Spinner.stopAll()
                if cursorHidden then
                    Console.CursorVisible <- true)

            shutdown.Dispose()

    let watch (path: string) : Status =
        if Console.IsOutputRedirected || Console.IsInputRedirected then
            Console.Error.WriteLine("The PTML watcher requires an interactive terminal.")
            Status.Error
        else
            try
                setWatcher path
                Status.Success
            with
            | :? IOException as ex ->
                Console.Error.WriteLine($"Unable to start the PTML watcher: {ex.Message}")
                Status.Error
