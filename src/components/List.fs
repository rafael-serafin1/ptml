namespace PTML

module List =
    type Style =
        | Unordered
        | Bullet
        | Enumerated
        | LowerAlphabet
        | UpperAlphabet

    let parseStyle = function
        | "unorder" -> Unordered
        | "bullet" -> Bullet
        | "enum" -> Enumerated
        | "lower-alphabet" -> LowerAlphabet
        | "upper-alphabet" -> UpperAlphabet
        | value -> failwith $"Invalid list style: {value}"

    let private alphabeticMarker firstLetter index =

        let rec build remaining prefix =
            let zeroBased: int = remaining - 1
            let letter: char   = char (int firstLetter + zeroBased % 26)
            let prefix: string = string letter + prefix
            let remaining: int = zeroBased / 26

            if remaining = 0 then 
                prefix
            else 
                build remaining prefix

        build index ""

    let marker style index =
        if index < 1 then
            invalidArg (nameof index) "List item indexes must be positive."

        match style with
        | Unordered -> "-"
        | Bullet -> "○"
        | Enumerated -> $"{index}."
        | LowerAlphabet -> alphabeticMarker 'a' index + "."
        | UpperAlphabet -> alphabeticMarker 'A' index + "."

    let formatItem style before after index =
        before + marker style index + after
    