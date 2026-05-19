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

module Program =
    type Command = 
        | Run
        | Watch

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
            //file path
            | _ when arg.EndsWith(".ptml") -> config <- { config with filePath = Some arg }
            | _ -> printfn "Unknown argument: %s" arg

        match config.command with
        | None -> 
            printfn "No command provided. Use --help for usage information."
            Environment.Exit(defineStatus(Status.Error))
        | Some _ -> ()
        
        match config.filePath with
        | None -> 
            printfn "No file path provided. Use --help for usage information."
            Environment.Exit(defineStatus(Status.Error))
        | Some _ -> ()

        let mutable S: Status option = None
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
        | None -> ()
        0