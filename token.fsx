module Token

[<Struct>]
type Attr = {
    attr_name: string
    boolean_attr: bool
    attr_value: string
}

[<Struct>]
type Pair = {
    first: int
    second: int
}

[<Struct>]
type Token = {
    tag: string
    position: Pair
    self_closing_tag: bool
    attrs: Attr array option
    children: Token[] option
}


type LexToken =
    | ProcInst of string
    | Comment of string
    | StartTag of string * bool * list<string * string>  // tagName, selfClosing, attrs
    | EndTag of string
    | Text of string
