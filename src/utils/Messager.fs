namespace PTML

module Messager =
    type MessageStatus =
        | Success = 0
        | Warning = 1
        | Error = 2

    let defineStatus(status: MessageStatus): int =
        match status with
        | MessageStatus.Success -> 0
        | MessageStatus.Warning -> 1
        | MessageStatus.Error -> 2
        | _ -> 0

    let PTMLMessage(message: string, status: MessageStatus): unit =
        let prefix: string = 
            match status with
            | MessageStatus.Success -> "[SUCCESS]"
            | MessageStatus.Warning -> "[WARNING]"
            | MessageStatus.Error -> "[ERROR]"
            | _ -> "[INFO]"
        
        printfn "%s %s" prefix message