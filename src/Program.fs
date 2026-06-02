namespace PTML
open System
open System.Text
open System.IO
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
                    """
    let version: string = "Versão do projeto :: 0.1.1"

    type Command = 
        | Run
        | Watch
        | Debug

    type Flag =
        | Help of bool
        | Version of bool

    type Config = {
        command: Command option
        filePath: string option
        flags: Flag list
    }

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
            //commands
            | "run" -> config <- { config with command = Some Run }
            | "watch" -> config <- { config with command = Some Watch }
            | "debug" -> config <- { config with command = Some Debug }
            //file path
            | _ when arg.EndsWith(".ptml") -> config <- { config with filePath = Some arg }
            | _ -> printfn "Unknown argument: %s" arg

        match config.command with
        | None -> 
            if config.flags.IsEmpty = false then
                for flag: Flag in config.flags do
                    match flag with
                    | Help h -> printfn "%s" help
                    | Version v -> printfn "%s" version
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

        let mutable S: Token.Status option = None
        match config.command with
        | Some Run -> 
            match config.filePath with
            | Some file -> 
                S <- Some (run(file))
            | None -> ()
        | Some Watch -> 
            match config.filePath with
            | Some file -> 
                S <- Some (watch(file))
            | None -> ()
        | Some Debug ->
            match config.filePath with 
            | Some file -> 
                S <- Some (debug(file))
            | None -> ()
        | None -> ()

        for flag: Flag in config.flags do
            match flag with
            | Help h -> printfn "%s" help
            | Version v -> printfn "%s" version
            

        match S with
        | Some s -> Token.defineStatus(s)
        | None -> 404