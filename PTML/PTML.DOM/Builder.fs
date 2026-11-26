namespace PTML.DOM

// Document Object Model
// Interface que vai servir como forma de representar a estrutura de um documento PTML na forma de uma árvore de objetos.
// Assim como o HTML possui o DOM nos navegadores que permite que o Javascript acesse e manipule dinamicamente o conteúdo do mesmo
// O objetivo desse módulo é implementar o DOM com foco em fazer com que o PTML seja manipulado de forma dinâmica por scripts escritos em Lua com o MoonSharp

// O DOM deve vir antes da construção da árvore
// Ele deve conter identidade, hierarquia, atributos, eventos e mutações
// Identidade não deve ser mutável, mas se eu quiser mudar o texto de um elemento ou os filhos de um elemento, então eles devem ser mutáveis via MoonSharp
module Builder =
    type NodeId = int

    type NodeKind =
    | ElementNode of tag_name: string
    | TextNode    of text: string

    type PTMLNode = {
        Id: NodeId
        Kind: NodeKind
        Attributes: Map<string, string>
        Children: NodeId list
    }

    type PTMLDocument = {
        Root: NodeId
        Nodes: Map<NodeId, PTMLNode> option
    }

    let construct(): PTMLDocument =
        {
            Root  = 1
            Nodes = None
        }