namespace PTML

module Char =
    let validCodenames = 
        Set.ofList ["copyright"; "rights-reserved"; "trade-mark"; "service-mark"; "phonogram"; "celsius"; "fahrenheit"; "number"; "section"; "nul"; "esc"; "del"; "infinite"; "integral"; "empty"]

    type CharCodename = 
    | Copyright
    | RightsReserved
    | TradeMark
    | ServiceMark
    | Phonogram
    | Celsius
    | Fahrenheit
    | Number
    | Section
    | Nul
    | Esc
    | Del
    | Infinite
    | Integral
    | Empty
    | Object

    let translateCodename (str: string): CharCodename =
        match str.ToUpperInvariant() with
        | "COPYRIGHT"       -> Copyright
        | "RIGHTS-RESERVED" -> RightsReserved
        | "TRADE-MARK"      -> TradeMark
        | "SERVICE-MARK"    -> ServiceMark
        | "PHONOGRAM"       -> Phonogram
        | "CELSIUS"         -> Celsius
        | "FAHRENHEIT"      -> Fahrenheit
        | "NUMBER"          -> Number
        | "SECTION"         -> Section
        | "NUL"             -> Nul
        | "ESC"             -> Esc
        | "DEL"             -> Del
        | "INFINITE"        -> Infinite
        | "INTEGRAL"        -> Integral
        | "EMPTY"           -> Empty
        | "OBJECT"          -> Object
        | _ -> 
            failwith $"Unknown special character name: {str}"

    let matchCodename (codename: CharCodename): char =
        match codename with
        | Copyright         -> '©'
        | RightsReserved    -> '®'
        | TradeMark         -> '™'
        | ServiceMark       -> '℠'
        | Phonogram         -> '℗'
        | Celsius           -> '℃'
        | Fahrenheit        -> '℉'
        | Number            -> '№'
        | Section           -> '§'
        | Nul               -> '␀'
        | Esc               -> '␛'
        | Del               -> '␡'
        | Infinite          -> '∞'
        | Integral          -> '∫'
        | Empty             -> '∅'
        | Object            -> '￼'