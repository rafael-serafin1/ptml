namespace PTML
open System
open System.Text
open System.IO
open System.Diagnostics
open System.Runtime.InteropServices
open PTML.Token
open PTML.Lexer
open PTML.Parser
open PTML.Tree
open PTML.Layout
open PTML.Buffer
open PTML.Render
open PTML.Runner
open PTML.Watch
open PTML.Debug
open PTML.Messager

module Program =
    let help: string = """Comandos:
- run <PATH>   :: Roda um arquivo PTML.
- watch <PATH> :: Roda e observa mudanças e atualiza o terminal automaticamente.
- debug <PATH> :: Ferramenta para debug do pipeline (uso para desenvolvedores). 
----------------------------------------------------------------------------------
Flags:
"--help" ou "-h" -> Printar esse comando.
"--version" ou "-v" -> Printa a versão do projeto.
"--window" ou "-w" -> Força o programa a rodar em um terminal em janela do sistema com tamanho 460x200
                    """
    let version: string = "Versão do projeto :: 0.1.1"

    type Command = 
        | Run
        | Watch
        | Debug

    type Flag =
        | Help of bool
        | Version of bool
        | Window of bool

    type Config = {
        command: Command option
        filePath: string option
        flags: Flag list
    }

    let quoteCmdArg (arg: string) =
        if arg.Contains(" ") || arg.Contains("\t") || arg.Contains("\"") then
            "\"" + arg.Replace("\"", "\\\"") + "\""
        else arg

    let buildCmdLine (args: string[]) =
        args
        |> Array.map quoteCmdArg
        |> String.concat " "

    let isWindowChild() = Environment.GetEnvironmentVariable("PTML_WINDOW_CHILD") = "1"

    let openInSystemWindow argv =
        let exePath =
            let entry = System.Reflection.Assembly.GetEntryAssembly().Location
            if String.IsNullOrWhiteSpace(entry) then
                match Environment.ProcessPath with
                | null | "" -> "ptml"
                | path -> path
            else entry

        let quotedExe = quoteCmdArg exePath
        let quotedArgs = buildCmdLine argv
        let childCommand = sprintf "%s %s" quotedExe quotedArgs

        let psi = ProcessStartInfo()
        psi.FileName <- "cmd.exe"
        psi.Arguments <- sprintf "/c start \"PTML\" cmd /k \"mode con: cols=460 lines=200 && %s\"" childCommand
        psi.UseShellExecute <- false
        psi.Environment.["PTML_WINDOW_CHILD"] <- "1"
        Process.Start(psi) |> ignore
        0

    [<EntryPoint>]
    let main(argv): int =
        // força o console a usar UTF-8 para renderizar os caracteres de borda corretamente
        Console.OutputEncoding <- Encoding.UTF8
        Console.InputEncoding <- Encoding.UTF8

        let mutable config: Config = {
            command = None
            filePath = None
            flags = []
        }

        for arg in argv do 
            match arg with
            //flags
            | "--help" | "-h" -> config <- { config with flags = Help true :: config.flags }
            | "--version" | "-v" -> config <- { config with flags = Version true :: config.flags }
            | "--window" | "-w" -> config <- { config with flags = Window true :: config.flags }
            //commands
            | "run" -> config <- { config with command = Some Run }
            | "watch" -> config <- { config with command = Some Watch }
            | "debug" -> config <- { config with command = Some Debug }
            //file path
            | _ when arg.ToLower().EndsWith(".ptml") -> config <- { config with filePath = Some arg }
            | _ -> printfn "Unknown argument: %s" arg

        match config.command with
        | None -> 
            if config.flags.IsEmpty = false then
                for flag: Flag in config.flags do
                    match flag with
                    | Help h -> printfn "%s" help
                    | Version v -> printfn "%s" version
                    | Window w -> () // a flag de window é processada mais tarde, quando o programa for rodar
                ()
            else 
                PTMLMessage("No command provided. Use --help for usage information.", MessageStatus.Error)
                Environment.Exit(defineStatus(MessageStatus.Error))
        | Some _ -> ()
        
        match config.filePath with
        | None -> 
            PTMLMessage("No file path provided. Use --help for usage information.", MessageStatus.Error)
            Environment.Exit(defineStatus(MessageStatus.Error))
        | Some _ -> 
            let file = System.IO.File.Exists(config.filePath.Value)
            if file = false then
                PTMLMessage(sprintf "File not found: %s" config.filePath.Value, MessageStatus.Error)
                Environment.Exit(defineStatus(MessageStatus.Error))
            else ()

        let shouldOpenWindow =
            config.flags
            |> List.exists (fun flag -> match flag with Window true -> true | _ -> false)
            && not (isWindowChild())

        if shouldOpenWindow then
            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                ()
            else
                PTMLMessage("The --window flag is currently supported only on Windows. Continuing in the current terminal.", MessageStatus.Warning)

        let mutable S: Token.Status option = None
        match config.command with
        | Some Run -> 
            match config.filePath with
            | Some file -> 
                if shouldOpenWindow then
                    openInSystemWindow [| "run"; file |] |> ignore
                    Environment.Exit(0)
                else
                    S <- Some (run(file))
            | None -> ()
        | Some Watch -> 
            match config.filePath with
            | Some file -> 
                if shouldOpenWindow then
                    openInSystemWindow [| "watch"; file |] |> ignore
                    S <- Some Status.Success
                else
                    S <- Some (watch(file))
            | None -> ()
        | Some Debug ->
            match config.filePath with 
            | Some file -> 
                if shouldOpenWindow then
                    openInSystemWindow [| "debug"; file |] |> ignore
                    Environment.Exit(0)
                else
                    S <- Some (debug(file))
            | None -> ()
        | None -> ()

        for flag: Flag in config.flags do
            match flag with
            | Help h -> printfn "%s" help
            | Version v -> printfn "%s" version
            | Window w -> ()                                // a flag de window é processada mais tarde, quando o programa for rodar
            

        match S with
        | Some s -> Token.defineStatus(s)
        | None -> 404