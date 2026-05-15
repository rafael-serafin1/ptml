module ListOrdered
[<Struct>]
type List = {
    info: string
    next: List option
}

type ListOrdered() =
    let mutable list: List option = None

    member private this.InsertOrdered(current, str: string) =
        let rec insert nodeOption =
            match nodeOption with
            | None ->
                Some { info = str; next = None }
            | Some node when node.info.Equals(str) ->
                failwith $"Tag já registrada: {str}"
            | Some node when System.String.CompareOrdinal(str, node.info) < 0 ->
                Some { info = str; next = nodeOption }
            | Some node ->
                Some { info = node.info; next = insert node.next }
        insert current

    member this.Add(str: string) =
        list <- this.InsertOrdered (list, str)
