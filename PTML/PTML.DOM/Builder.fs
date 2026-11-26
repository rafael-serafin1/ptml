namespace PTML.DOM

module Builder =
    type NodeId = int

    type InputNode =
        | InputElement  of tag_name: string * attributes: Map<string, string> * children: InputNode list
        | InputText     of text    : string

    type NodeKind =
        | DocumentNode
        | ElementNode   of tag_name: string
        | TextNode      of text    : string

    type PTMLNode = {
        Id        : NodeId
        Kind      : NodeKind
        Attributes: Map<string, string>
        Children  : NodeId list
    }

    type PTMLDocument = {
        Root : NodeId
        Nodes: Map<NodeId, PTMLNode>
    }

    let construct (nodes: InputNode list) : PTMLDocument =
        let mutable nextId = 1
        let mutable builtNodes = Map.empty

        let autoIncrement() =
            nextId <- nextId + 1

        let rec addNode(kind: NodeKind)(attributes: Map<string,string>)(children: InputNode list): int =
            let id: int = nextId
            autoIncrement()

            let childIds = children |> List.map addInputNode
            let node = {
                Id         = id
                Kind       = kind
                Attributes = attributes
                Children   = childIds
            }

            builtNodes <- Map.add id node builtNodes 
            id

        and addInputNode input =
            match input with
            | InputElement(tagName, attributes, children) ->
                addNode (ElementNode tagName) attributes children
            | InputText text ->
                addNode (TextNode text) Map.empty []

        let root = addNode DocumentNode Map.empty nodes
        { Root = root; Nodes = builtNodes }
