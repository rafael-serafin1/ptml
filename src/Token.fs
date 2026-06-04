namespace PTML
open System
open System.Text
open PTML.PiAttrs

module Token =
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

    type Terminal = {
        ViewWidth: int
        ViewHeight: int

        SafeWidth: int
        SafeHeight: int
    }

    let getViewport() = {
        ViewWidth = Console.WindowWidth
        ViewHeight = Console.WindowHeight

        SafeWidth = Console.WindowWidth - 1
        SafeHeight = Console.WindowHeight - 1
    }

    let getLayoutViewport() =
        let width = Console.WindowWidth
        let height = Console.WindowHeight
        let layoutWidth, layoutHeight = getLayoutSize width height
        {
            ViewWidth = layoutWidth
            ViewHeight = layoutHeight
            SafeWidth = max 0 (layoutWidth - 1)
            SafeHeight = max 0 (layoutHeight - 1)
        }

    let getOutputViewport() =
        let width = Console.WindowWidth
        let height = Console.WindowHeight
        let outputWidth, outputHeight = getOutputSize width height
        {
            ViewWidth = outputWidth
            ViewHeight = outputHeight
            SafeWidth = max 0 (outputWidth - 1)
            SafeHeight = max 0 (outputHeight - 1)
        }

    type Status =
        | Success = 0
        | Error = 1
    
    let defineStatus(S: Status): int =
        if S = Status.Success then
            0
        else
            1

    type LexToken =
        | ProcInst of string
        | Comment of string
        | StartTag of string * bool * list<string * string>  // tagName, selfClosing, attrs
        | EndTag of string
        | Text of string
