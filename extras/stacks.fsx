module Stacks
#load "../token.fsx"
open Token

[<Struct>]
type StackNode = {
    info: Token
    mutable next: StackNode option
}

(*
    Stacks são nada mais que Pilhas, ou seja, estruturas de dados do tipo LIFO (Last In First Out).
    Elas são usadas para armazenar informações temporárias, como por exemplo, as tags abertas durante
    o parsing de um documento HTML. 
    Quando encontramos uma tag de abertura, ela é empilhada, e quando encontramos uma tag de fechamento, a tag correspondente é desempilhada. Isso nos ajuda a garantir que as tags sejam fechadas na ordem correta e que a estrutura do documento seja mantida. 
    Além disso, as stacks também podem ser usadas para armazenar informações sobre o estado atual do parser, como por exemplo, o nível de aninhamento das tags ou o contexto em que estamos (dentro de uma tag específica, dentro de um atributo, etc).
*)
type Stack() =
    let mutable top : StackNode option = None
    
    member this.Push(token: Token) =
        let newNode: StackNode = {
                info = token
                next = top
            }
        top <- Some newNode

    member this.Pop() =
        match top with
        | Some node ->
            top <- node.next
            node.info

        | None ->
            failwith "Stack vazia"

    member this.Print() =
        let rec loop node =
            match node with
            | Some n ->
                printfn "%s" n.info.tag
                loop n.next
            | None -> ()

        loop top