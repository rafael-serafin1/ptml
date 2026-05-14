// commom attribute values
type AttributeValue =
    | String of string
    | Integer of int
    | Percentage of int
    | Boolean of bool

type Attribute = {
    Name: string
    Value: AttributeValue
}

// Terminal Attribute
type TerminalResizeMode =
    | Reflow
    | Clip
    | Static

type ProcessingInstruction = {
    Encoding: string option
    TerminalResize: TerminalResizeMode
}

// text node
type RawTextNode = {
    Content: string
}

// comment node
type CommentNode = {
    Content: string
}

// generic element node
type ElementNode = {
    Tag: string
    Attributes: Attribute list
    Children: ASTNode list
}

and ASTNode =
    | Element of ElementNode
    | RawText of RawTextNode
    | Comment of CommentNode

type PTMLDocument = {
    Instruction: ProcessingInstruction option
    Children: ASTNode list
}