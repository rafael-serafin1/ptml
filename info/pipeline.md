PTML Source

↓

Lexer
“quebra em tokens”

↓

Parser
“entende estrutura”

↓

AST
“representação sintática”

↓

Semantic Tree
“widgets reais”

↓

Layout Pass
“calcula posições e tamanhos”

↓

Render Tree
“operações gráficas”

↓

Terminal Buffer
“estado completo da tela”

↓

Terminal State 
"estado do terminal"

↓

Diff Engine
“detecta mudanças e renderiza”

↓

ANSI Output
“escreve no terminal”

No comando `run`, quando as operações renderizadas excedem o viewport, o buffer virtual cresce para preservar o conteúdo e as linhas são escritas em fluxo normal, permitindo que o terminal quebre linhas largas e role verticalmente. Quando o conteúdo cabe no viewport, a renderização em tela cheia existente é mantida.
