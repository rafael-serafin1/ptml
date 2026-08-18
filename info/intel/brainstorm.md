## Procedural Instruction

`<?ptml encoding="UTF-8" terminal-resize="reflow"?>` <br />
Serve como Instrução Procedural, além de garantir que ao terminal resize o retained-mode rendering vai ser corretamente aplicado. <br />
Valores possíveis para atributo ***terminal-resize***:  
```
reflow              (valor padrão)
clip
static
```

>> Elementos com título `IGNORE POR ENQUANTO` significa que sua lógica ainda estão sendo desenvolvida corretamente.

---
## Elemento Comentário `<!-- -->`
São ignorados pelo parser. Servem para comentar código.

---
## Categorização de Elementos PTML
Os elementos PTML são categorizados em dois tipos, concreto e abstrato. Elementos concretos são aqueles que desenham/escrevem alguma no terminal. Já os elementos abstratos são aqueles que não desenham, mas definem o fluxo e direção do conteúdo de forma expressiva.

Sendo assim, atualmente os concretos são:
```ptml
<frag>
<text>
<box>
<block>
<spinner>
<hr>
<progress>
<frame>
---> Em desenvolvimento
<tree>
<toast>
<input>
<entity>
<bind>
<graphs>
```

Agora, os abstratos são:
```ptml
<row>
<column>
<layer>
<terminal>
<cell>
<snippet>
<escape>
<cursor>
---> Em desenvolvimento
<timeline>
<carousel>  
<slide>
<code>
<function>
```

---
## Atributos Globais
São aqueles que qualquer elemento tem disponível para usar. Sendo eles:

***id***:
Atributo que aplica um identificador único ao elemento.

***snippet***:
*LEIA `<snippet>` PARA MELHOR CONTEXTO* 
Atributo que todos os elementos possuem e que faz a adição dos atributos resumidos no elemento `<snippet>`.
> OBS.1: Se o `<snippet>` tiver atributos que o elemento não possui, exemplo `padding` para `<text>`, a execução não será interrompida, mas um aviso será gerado no terminal para deixar claro que aquele atributo não existe para tal elemento.

--- 
## Atributos Compartilhados

Aqui é citado atributos que são compartilhados por mais de 2 elementos simultaneamente e que possuem lógica de funcionamento igual.

### Atributo ***width/height***:
Compartilhado entre os elementos
```
<box>
<block>
<hr>
<bind>
<progress>
<frame>
```

Corresponde a largura e altura do componente. Seus valores são numéricos inteiros ou específicos. Em caso de não declaração, o valor padrão é `auto` (renderiza no tamanho necessário para confortar o texto). Valores não-numéricos específicos:
```
auto                                    (renderiza do tamanho necessário)
Nº%                                     (valor associado ao elemento-pai substituindo Nº por número --> percentage. Exemplo: 40%)
```

Em caso de não possuir um elemento-pai, a porcentagem será tirada do tamanho total do terminal.
Exemplo:
- Input
```ptml
<?ptml enconding="UTF-8" terminal-resize="reflow"?>
<box width="50%" height="50%"></box>                 <!-- ocupa 50% do tamanho total do terminal, já que não possui elemento-pai -->
```

### Atributo ***overflow***:
Compartilhado entre os elementos
```
<row>
<column>
<layer>             (antigo <depth>)
<box>
<block>
<input>
<frame>
```

Atributo que define o comportamento do container em caso de overflow de conteúdo. Caso não seja explicitado o resoluto em caso de overflow, o valor por padrão é o `break` (quebra o conteudo em uma nova linha). Valores possíveis:
```                                 
break                               (quebra em qualquer caractere)
wrap                                (quebra respeitando palavras)
cut                                 (corta texto bruto) 
clip                                (recorta área renderizada final)
```

Caso de containeres compostos<br />
Exemplo:
- Input
```ptml
<row overflow="wrap">
    <box width="20"/>
    <box width="20"/>
</row>
```

> Obs: O uso do `overflow` é diretamente ligado ao `width/height`. Casos:
```
se `width` = 'auto' e `height` != 'auto' --> overflow vai ser considerado apenas para o `height`
se `width` != 'auto' e `height` = 'auto' --> overflow vai ser considerado apenas para o `width`
se `width` = 'auto' e `height` = 'auto' --> overflow vai ser considerado apenas para ambos
se `width` != 'auto' e `height` != 'auto' --> overflow é totalmente desconsiderado
```

### Atributo ***padding***:
Atributo que define o espaço entre a borda e o conteúdo, sendo seu valor padrão 0. Compartilhado entre os elementos
```
<box>
<block>
<frame>
```

- Input 
```ptml
<box padding="1">
    <text>Olá</text>
</box>
```

- Output
```cmd
┌─────┐
│     │
│ Olá │
│     │
└─────┘
```

> Como é possível ver, a distância do conteúdo para as bordas verticais e horizontais são de 1 assim como foi referenciado no atributo padding.
> No entanto, é possível definir o padding vertical e horizontal através de valores separados por 'x'. O número que antecede o caractere define o padding vertical, enquanto que o número que precede o caractere 'x' define o padding horizontal. 

Exemplo:
- Input
```ptml
<box padding="1x0">
    <text>Olá</text>
</box>
```

- Output
```cmd
┌───┐
│   │
│Olá│
│   │
└───┘
```

> Em caso de ter um valor negativo para o padding, um erro vai aparecer na tela e a execução será forçada a parar.

### OBS:
Textos crus existem, mas eles não causam inutilização do elemento `<text>` pelo fato de eles não possuírem outra forma de serem estilizados, mas no fim eles acabam virando nó padrão de `<text>`.

### Atributo ***url***:
Atributo responsável por aderir uma url de um site ao texto. Compartilhado entre os elementos:
```
<text>
<frag>
```

Seu uso é simples:

- Input
```ptml
<text url="https://github.com/rafael-serafin1">Github</text>
```

- Output
```
Github  
```

---
### Elemento `<text>`

`<text></text>`
Faz o display de seu conteúdo no terminal com estilização. 

Exemplo:
- Input
```ptml
<text>Hello World!</text>
```

- Output
```terminal
Hello World!
```
 
Através do atributo `foreground=` e `background=`, pode-se escolher valores pré-determinados para a cor final do texto.

Exemplo:
- Input
```ptml
<text foreground="red">Este texto está vermelho!</text>
<text background="cyan">O fundo desse texto está na cor ciano.</text>
<text foreground="black" background="white">Ambos foreground e background estão coloridos nesse.</text>
```

- Output
```terminal
Este texto está vermelho!
O fundo desse texto está na cor ciano.
Ambos foreground e background estão coloridos nesse.
```

#### *Valores possíveis para o atributo `foreground` (o mesmo vale para `background`, ambas tem suporte a cores hexadecimais):*
```
none -- [0m
black -- [30m
red -- [31m
green -- [32m
gold -- [33m
blue -- [34m
purple -- [35m
cyan -- [36m
fire -- [1;31m
limegreen -- [1;32m
yellow -- [1;33m
lightblue -- [1;34m
lilac -- [1;35m
crystal -- [1;36m
gray -- [1;30m
lightgray -- [1;37m
```
**tenha em mente que essa tabela pode sofrer alterações e que são as cores esperadas para o renderer**

#### *O atributo `font` faz a estilização da fonte através de valores possíveis descritos abaixo:*
```
reset
bold
dim
italic
underline
slow-blink
rapid-blink
reverse                 (marked)
conceal                 (hidden)
strike-through
overline
double-underline
```

### OBS:
Caracteres Unicode complexos como emojis por enquanto serão ignorados e apenas seu código será colocado na tela.

---
## Elemento `<frag>`

`<frag></frag>`
Elemento que representa um fragmento de um texto (`<text>`). Sua função é estilizar partes específicas do texto para vários mótivos como ênfase e destaque.

Exemplo:
- Before
```ptml
<row>
    <text>Príons são </text>
    <text foreground="gray" font="bold">proteínas infecciosas</text>
    <text>.</text>
</row>
```

- After
```ptml
<text>Príons são <frag foreground="gray" font="bold">proteínas infecciosas</frag>.</text>
```

### **Atributos**:

#### *Valores possíveis para o atributo `foreground` (o mesmo vale para `background`, ambas tem suporte a cores hexadecimais):*
```
none -- [0m
black -- [30m
red -- [31m
green -- [32m
gold -- [33m
blue -- [34m
purple -- [35m
cyan -- [36m
fire -- [1;31m
limegreen -- [1;32m
yellow -- [1;33m
lightblue -- [1;34m
lilac -- [1;35m
crystal -- [1;36m
gray -- [1;30m
lightgray -- [1;37m
```
**tenha em mente que essa tabela pode sofrer alterações e que são as cores esperadas para o renderer**

#### *O atributo `font` faz a estilização da fonte através de valores possíveis descritos abaixo:*
```
reset
bold
dim
italic
underline
slow-blink
rapid-blink
reverse                 (marked)
conceal                 (hidden)
strike-through
overline
double-underline
```

--- 
## Elemento `<escape>`

`<escape />`
Elemento que define uma escape sequence.

### **Atributos:**

***sequence***:
Define qual escape sequence vai ser usada. Valores possíveis:
```
break                               ('\n', valor padrão)
horizontal-tab                      ('\t')
audible-bell                        ('\a', som de sino audível?)
backspace                           ('\b')
form-feed                           ('\f', move o cursor para o começo da próxima página lógica )
carriage-return                     ('\r', move o cursor para o começo da linha )
vertical-tab                        ('\v')
```

Exemplo:
- Input
```
<text>Lista:</text>
<escape sequence="horizontal-tab" />
<text>- 69 Ketchups</text>
```

- Output
```
List:
    - 69 Ketchups
```

***multiplier***:
Multiplica a quantidade de quebra de linhas pelo número.

Exemplo:
- Input
```ptml
<text>Hello</text>
<escape sequence="break" multiplier="2" />
<text>World</text>
```

- Output
```
Hello\n
\n
World
```

---
## Elemento `<cursor>`

`<cursor />`
Elemento abstrato cujo propósito é estilizar o cursor.

### **Atributos**:

***shape***:
Define o formato do cursor.
```
block
bar
underline
```

***blink***:
Atributo booleano que define se o cursor vai ficar piscando ou não.

***color***:
Define a cor do cursor. Aceita qualquer valor hexadecimal. Valores não-hexadecimais aceitos também:
```
red     (traduz para #f00)
yellow  (traduz para #ff0)
white   (traduz para #fff)
black   (traduz para #000)
teal    (traduz para #0ff)
blue    (traduz para #00f)
pink    (traduz para #f0f)
green   (traduz para #0f0)
purple  (traduz para #50f)
```

***visible***:
Define se o cursor é visível ou não. Valor booleano (true || false).

---
## Elemento `<hr>`

`<hr />`
Elemento que com a função de separar elementos no terminal.

Exemplo:
- Input
```ptml
<column>
    <text>Acima</text>
    <hr orientation="horizontal" />
    <text>Abaixo</text>
</column>
```

- Output
```cmd
Acima
──────────────
Abaixo
```

### **Atributos**:

***orientation***:
Define a orientação da barreira. Valores possíveis:
```
vertical
horizontal
```

---
## `<hr>` VS `<cell>`

```
<hr>                            | <cell>
+ calculo interno mais leve     | - calculo interno mais pesado
- sem continuidade de bordas    | + continuidade de layout de bordas
```
> Legenda: '+' significa pro e '-' significa contra.

---
## Elemento `<row>`

`<row></row>`
Faz juz a uma linha. Seus filhos são distribuídos horizontalmente.

Exemplo:
- Input
```ptml
<row>
    <text foreground="red">Red</text>
    <text>John</text>
</row>
```

- Output
```cmd
RedJohn
```

### **Atributos:**

***gap***:
Define o espaçamento entre um filho e outro no layout. O valor deve ser numérico e inteiro.

- Input
```ptml
<row gap="1">
    <text foreground="red">Red</text>
    <text>John</text>
</row>
```

- Output
```terminal
Red John
```

***align***:
Atributo que alinha um conteúdo horizontalmente pela largura disponível do container pai. Valores possíveis:
```
start               (valor padrão/default)
center 
end
```

Exemplo:
- Input
```ptml
<row gap="1" align="center">
    <text foreground="red">Red</text>
    <text>John</text>
</row>
```

- Output (exemplo em terminal 12cols)
```terminal
|  Red John  |
```

---
## Elemento `<column>`

`<column></column>` 
Faz juz a uma coluna. Seus filhos são distribuídos verticalmente.

Exemplo:
- Input
```ptml
<column>
    <text>A</text>
    <text>B</text>
</column>
```

- Output
```terminal
A
B
```

### **Atributos:**

***gap***:
Define o espaçamento entre um filho e outro no layout. O valor deve ser numérico e inteiro.

Exemplo:
- Input
```ptml
<column gap="1">
    <text foreground="red">Red</text>
    <text>John</text>
</column>
```

- Output
```terminal
Red

John
```

***y-align***:
Atributo que alinha um conteúdo verticalmente pela altura disponível do container pai. Valores possíveis:
```
start               (valor padrão/default)
center 
end
```

Exemplo:
- Input
```ptml
<column gap="1" y-align="end">
    <text foreground="red">Red</text>
    <text>John</text>
</column>
```

- Output (exemplo em terminal 5cols)
```terminal
|  Red|
|     |
| John|
```

---
## Elemento `<layer>`

`<layer></layer>`
Faz juz a profundidade. Seus filhos são distribuídos por índice da 3º dimensão espacial.

Exemplo .1:
- Input
```ptml
<layer index="0">
    <box>
        <text>Hello World!</text>
    </box>
</layer>
<layer index="-1">
    <box>
        <text>GoodBye World!</text>
    </box>
</layer>
<layer index="-2">
    <box>
        <text>Hello Again!</text>
    </box>
</layer>
```

- Output
```cmd
┌────────────┐
│┌───────────┴┐
└┤┌───────────┴┐
 └┤Hello World!│
  └────────────┘
```

### **Disclaimer** --> o conteúdo dos índices -1 e -2 não foram perdido, apenas sobrescrito, ainda é possível acessar eles.
### **OBS** N1--> Em caso de o usuário utilizar outros valores para representar a superfície (0), um aviso aparecerá no terminal, mas isso não impedirá a execução do código.

Exemplo .(1.5):
- Input
```ptml
<layer index="0">
    <box>
        <text>Hello World!</text>
    </box>
    <box>
        <text>GoodBye World!</text>
    </box>
</layer>
```

- Output
```cmd
Erro: não pode haver dois filhos com índice de mesmo valor!
```

Exemplo .2:
- Input
```ptml
<layer index="0">
    <column>
        <row>
            <box border="single">
                <text foreground="gold">Hello World!</text>
            </box>
            <box border="single">
                <text>GoodBye World!</text>
            </box>
        </row>
        <row>
            <box border="single">
                <text foreground="gold">Hello World!</text>
            </box>
            <box border="single">
                <text>GoodBye World!</text>
            </box>
        </row>
    </column>
</layer>
<layer index="-1">
    <column>
        <row>
            <box border="single">
                <text foreground="gold">Hello World!</text>
            </box>
            <box border="single">
                <text>GoodBye World!</text>
            </box>
        </row>
        <row>
            <box border="single">
                <text foreground="gold">Hello World!</text>
            </box>
            <box border="single">
                <text>GoodBye World!</text>
            </box>
        </row>
    </column>
</layer>
```

- Output
```cmd
┌────────────┐  ┌──────────────┐
│┌───────────┴┐ │┌─────────────┴┐
└┤Hello World!│ └┤GoodBye World!│
 └────────────┘  └──────────────┘
┌────────────┐  ┌──────────────┐ 
│┌───────────┴┐ │┌─────────────┴┐
└┤Hello World!│ └┤GoodBye World!│
 └────────────┘  └──────────────┘
```

### **Atributos**:

***index*** (obrigatório):
Define o índice de profundidade do elemento-filho.

Exemplo:
- Input
```ptml
<layer index="0">
    <column>
        <box>
            <text>Hello World!</text>
        </box>
    </column>
</layer>
<layer index="-1">
    <column>
        <box>
            <text>GoodBye World!</text>
        </box>
    </column>
</layer>
```

- Output
```cmd
┌───────────┐
│┌──────────┴─┐
└┤Hello World!│
 └────────────┘
```

***z-align***:
Atributo que alinha um conteúdo dimensionalmente pela profundidade disponível do container pai. Valores possíveis:
```
start       (valor default)
center 
end         (valor default APENAS para o cenário descrito na observação N2)
```

Exemplo
- Input
```ptml
<layer index="0" z-align="center">
    <column>
        <text>Hello World!</text>
    </column>
</layer>
<layer index="-1" z-align="center">
    <column>
        <text>Bye World!</text>
    </column>
</layer>
```

- Output
```cmd
  ┌──────────┐
 ┌┴──────────┴┐
 │Hello World!│
 └────────────┘
```


***gap***:
Define o espaçamento entre um filho e outro no layout. O valor deve ser numérico e inteiro.

Exemplo:
- Input
```ptml
<layer index="0" gap="1">
    <column>
        <text>Hello World!</text>
    </column>
</layer>
<layer index="-1" gap="1">
    <column>
        <text>Bye World!</text>
    </column>
</layer>
```

- Output
```cmd
┌────────────┐  
│Bye World!  │        
└┬───────────┴┐ 
 │Hello World!│ 
 └────────────┘ 
```

### **OBS** N2 --> Caso o elemento `<layer>` tenha gap igual ou maior que 1 ***E*** o conteúdo do elemento `<column>`, de índice menor que a da superfície, for maior que o conteúdo do elemento da superfície, um aviso deve ser gerado no terminal e a coluna deve ser exibida da seguinte forma:

- Input 
```ptml
<layer gap="1">
    <column index="0">
        <text>Hello World!</text>
    </column>
    <column index="-1">
        <text>GoodBye World!</text>
    </column>
</layer>
```

- Output
```cmd
 ┌──────────────┐  
 │GoodBye World!│        
┌┴───────────┬──┘
│Hello World!│ 
└────────────┘ 
```

---
## Elemento `<box>`

`<box></box>`
Define um bloco dentro do terminal.

Exemplo:
- Input
```ptml
<box border="single" width="10" height="5">
    <text>Hello World!</text>
</box>
```

- Output
```terminal
┌────────┐
│Hello Wo│
│rld!    │
│        │
└────────┘
```
> não parece mas tanto a altura quando a largura tem o mesmo número de caracteres (5).

### **Atributos:**

***border***:
É um renderer preset que define como a borda vai ser. Em caso de não declaração, o valor padrão é `single`. Valores possíveis:
```
single              (┌ ┐ └ ┘ ─ │)
double              (╔ ╗ ╚ ╝ ═ ║)
bold                (┏ ┓ ┗ ┛ ━ ┃)
strange             (╒ ╕ ╘ ╛ ═ │) 
classic             (┍ ┑ ┕ ┙ ─ │)
rounded             (╭ ╮ ╰ ╯ ─ │)
ascii               (+ - |)
borderless          (tem borda, mas esta invisível)
none
```

***border-color***:
Define a cor da borda. Os valores possíveis são os mesmos do `<text>`.

***align***:
Atributo que alinha um conteúdo horizontalmente pela largura disponível do container pai. Valores possíveis:
```
start               (valor padrão/default)
center 
end
```

--- 
## Elemento `<block>`

`<block></block>`
Define um bloco nomeado através de um atributo obrigatório chamado `title`. A diferença chave entre `<box>` e `<block>` é que o conteúdo de `<block>` é tratado como dele, assim poderá no futuro colocar várias seções de botões rádio que não vão conflitarem entre si.

Exemplo:
- Input
```ptml
<block title="Status">
    <column>
        <text>CPU  ███████░░ 73%</text>
        <text>RAM  ████░░░░ 41%</text>
        <text>NET  ▲ 12MB/s</text>
    </column>
</block>
```

- Output
```cmd
┌──Status───────────┐
│ CPU  ███████░░ 73%│
│ RAM  ████░░░░ 41% │
│ NET  ▲ 12MB/s     │
└───────────────────┘
```

### **Atributos**:

***title***:
Define o nome do bloco, sendo um atributo obrigatório de ter na declaração, mas seu valor pode ser nulo.
```ptml
<block title="">
    <text>Hello World!</text>
</block>
```

***border***:
É um renderer preset que define como a borda vai ser. Em caso de não declaração, o valor padrão é `single`. Valores possíveis:
```
single              (┌ ┐ └ ┘ ─ │)
double              (╔ ╗ ╚ ╝ ═ ║)
bold                (┏ ┓ ┗ ┛ ━ ┃)
strange             (╒ ╕ ╘ ╛ ═ │) 
classic             (┍ ┑ ┕ ┙ ─ │)
rounded             (╭ ╮ ╰ ╯)
ascii               (+ - |)
borderless          (tem borda, mas esta invisível)
none
```

***border-color***:
Define a cor da borda. Os valores possíveis são os mesmos do `<text>`.

***align***:
Atributo que alinha um conteúdo horizontalmente pela largura disponível do container pai. Valores possíveis:
```
start               (valor padrão/default)
center 
end
```

Exemplo:
- Input
```ptml
<block title="Name" align="start">
    <text foreground="red">Red</text>
    <text>John</text>
</block>
```

- Output (exemplo em terminal 12cols)
```terminal
┌ Name ───┐
│Red John │
└─────────┘
```

--- 
## Elemento `<terminal>`

`<terminal></terminal>`
Referencia ao terminal, servindo como um viewport root.

### **Atributos**:

***x-align/y-align***:
Atributo que alinha um conteúdo horizontalmente/verticalmente pela largura/altura do terminal respectivamente. Valores possíveis:
```
start
center
end
```

---
## Elemento `<cell>`

`<cell></cell>`
Faz a grid cell divindo o elemento-pai concreto (elemento-pai que desenha no CMD (block/box). Column e Row são apenas elementos de display, como seus filhos serão dispostos) conforme a quantidade de seus irmãos. 

Exemplo .1 Sem Cell:
- Input
```ptml
<block title="Cardapio">
    <column>
        <row>
            <text>Tilapia Cozida</text>
        </row>
        <row>
            <text>Pao de Batata</text>
        </row>
    </column>
</block>
```

- Output
```cmd
┌ Cardapio ──────┐
│ Tilapia Cozida │
│ Pao de Batata  │
└────────────────┘
```

Exemplo .1 Com Cell:
- Input
```ptml
<block title="Cardapio">
    <column>
        <row>
            <cell>
                <text>Tilapia Cozida</text>
            </cell>
        </row>
        <row>
            <cell>
                <text>Pao de Batata</text>
            </cell>
        </row>
    </column>
</block>
```

- Output
```cmd
┌ Cardapio ──────┐
│ Tilapia Cozida │
├────────────────┤
│ Pao de Batata  │
└────────────────┘
```

Exemplo .2 Sem Cell:
- Input
```ptml
<block title="Cardapio">
    <row gap="1">
        <cell>
            <text>Tilapia Cozida</text>
        </cell>
        <cell>
            <text>Pao de Batata</text>
        </cell>
    </row>
</block>
```

- Output
```cmd
┌ Cardapio ──────────────────┐
│Tilapia Cozida Pao de Batata|
└────────────────────────────┘
```

Exemplo .2 Com Cell:
- Input
```ptml
<block title="Cardapio">
    <row>
        <cell>
            <text>Tilapia Cozida</text>
        </cell>
        <cell>
            <text>Pao de Batata</text>
        </cell>
    </row>
</block>
```

- Output
```cmd
┌ Cardapio ────┬─────────────┐
│Tilapia Cozida│Pao de Batata|
└──────────────┴─────────────┘
```

Exemplo .3:
- Input
```ptml
<block title="Cardapio">
    <column>
        <row>
            <cell>
                <text>Tilapia Cozida</text>
            </cell>
            <cell>
                <text>Pao de Batata</text>
            </cell>
        </row>
    </column>
    <column>
        <row>
            <cell>
                <text>Tilapia, Ervas.</text>
            </cell>
            <cell>
                <text>Pao, Batata.</text>
            </cell>
        </row>
    </column>
</block>
```

- Output
```cmd
┌ Cardapio ──────┬────────────────┐
│ Tilapia Cozida │ Tilapia, Ervas.│
├────────────────┼────────────────┤         
│ Pao de Batata  │ Pao, Batata.   │
└────────────────┴────────────────┘
```

> OBS: Como toda célula, a divisão é feita de forma proporcional, ou seja, nunca se divide por números ímpares.

Exemplo 1:
- Input
```ptml
<box>
    <cell></cell>
</box>
```

- Output
```
┌┬┐
││|
└┴┘
```

Exemplo 2:
- Input
```ptml
<box>
    <cell></cell>
    <cell></cell>
</box>
```

- Output
```
┌┬┐
││|
└┴┘
```
> Sim, o mesmo output do anterior

Exemplo 2:
- Input
```ptml
<box>
    <cell></cell>
    <cell></cell>
    <cell></cell>
</box>
```

- Output
```
┌┬┐
│││
├┼┤         
│││
└┴┘
```

---
## Elemento `<spinner>`

`<spinner></spinner>` ou `<spinner />`
Elemento que cria um spinner no terminal.

Exemplo:
- Usage
```ptml
<row>
    <spinner type="ascii" interval="269ms" duration="3laps" completed="check"/>
</row>
```

### **Atributos**:

***type***:
Define o tipo de `<spinner>` a ser usado. Seu valor padrão é `braille`. Valores possíveis:
```
braille         (⠋ ⠙ ⠹ ⠸ ⠼ ⠴ ⠦ ⠧ ⠇ ⠏)
dots            (⣾ ⣽ ⣻ ⢿ ⡿ ⣟ ⣯ ⣷)
waiting         (. .. ... ....)                                     ! AGORA FUNCIONA
burger          (- = ≡)
beam            (= == ===) -- ([=  ] [== ] [===] [ ==] [  =])       ! NÃO FUNCIONA PORQUE O PRIMEIRO FRAME POSSUI 5 CARACTERES
ascii           (| / - \)
circle          (◐ ◓ ◑ ◒)
square          (◰ ◳ ◲ ◱)
moon            (◜ ◝ ◞ ◟)
arrow           (← ↖ ↑ ↗ → ↘ ↓ ↙)
bounce          (⠁ ⠂ ⠄ ⠂)
fill            (▁ ▂ ▃ ▄ ▅ ▆ ▇ █)
```

***interval***:
Define o intervalo de tempo entre um frame e outro da animação em ms (milissegundos). Seu valor padrão é definido como **250ms**. 
Seu valor não pode ser negativo ou nulo. Em caso de ser negativo, haverá a conversão para positivo e um aviso será emitido no terminal. Já ao ser nulo, o intervalo é definido para o valor padrão e um aviso também é emitido no terminal.

***duration***:
Define por quanto tempo o `<spinner>` vai ficar girando em ms (milissegundos). Após o tempo expirar, o `<spinner>` para e é substituido pelo caractere `✓` por padrão. Valores negativos farão com que o `<spinner>` não pare de girar. Valor padrão está definido para 3000ms. Unidades além do ms que podem ser usadas:
```
ms              (milissegundos)
s               (segundos)
laps             (voltas)
```

```
Lap é calculado pela multiplicação da quantidade de frames pelo intervalo. Então se um tipo de spinner tem 4 frames a conta seria:
    qtd_frames * interval_int = total_duration
    4 * 250ms = 1000ms
```

***completed***:
Define o que deve fazer ao ser completado. Por padrão, é colocado o caractere `✓` (alias para `check`). Alias possíveis:
```
check                           (✓)
error                           (✖)
star                            (✱)
cog                             (⚙)
bright                          (✦)
```

***foreground/background***:
Define a cor da fonte/fundo. Seu valores possíveis estão definidos no elemento `<text>`.

### IGNORE POR ENQUANTO!
***until-task***: 
Define que o spinner vai continuar girando até que uma função termine de ser executada.

---
## Elemento `<snippet>`

`<snippet></snippet>`
É um elemento usado para salvar configurações de atributos para serem usados depois, evitando repetições. O atributo `id` é indispensável para criar um `<snippet>`.

Exemplo:
- Declaration
```pmtl
<snippet id="warning-text">
    foreground="black"
    background="red"
    font="bold"
</snippet>
```

- Usage
```ptml
<text snippet="warning-text">!! Erro !!</text>
```

Exemplo .2:
- Declaration
```pmtl
<snippet id="warning-text">
    foreground="black"
    background="red"
    font="bold"
</snippet>
```

- Usage
```ptml
<box snippet="warning-text">
    <text>!! ERRO !!</text>
</box>
```

> Esse exemplo gera um aviso no terminal, sobre os atributos não pertencerem ao box, mas isso não impede a geração do UI, apenas impede a estilização através do `<snippet>`.

### **Atributos**:

***id***:
Atributo que define um crachá especial para o snippet. Atributo obrigatório de ter.

***extends***: 
Atributo que herda atribuições de outras tags `<snippet>` através de seus Id's.

Exemplo:
- Declaration
```ptml
<snippet id="danger">
    foreground="red"
</snippet>

<snippet id="fatal" extends="danger">
    font="bold"
</snippet>
```

***snippet***:
Atributo que todos os elementos possuem e que faz a adição dos atributos resumidos no elemento `<snippet>`.
> OBS.1: Se o `<snippet>` tiver atributos que o elemento não possui, exemplo `padding` para `<text>`, a execução não será interrompida, mas um aviso será gerado no terminal para deixar claro que aquele atributo não existe para tal elemento.

---
### IGNORE POR ENQUANTO!
## Elemento `<list>`

`<list></list>`
Elemento usado para descrever listas.

### **Atributos**:

***ofstyle***:
Define como a lista será escrita.
```
unorder                     (não ordernada -, -, -)
order                       (ordenada ○, ○, ○)
enum                        (enumerada ex: 1,2,3)
lower-alphabet              (ex: a, b, c)
upper-alphabet              (ex: A, B, C)
```

***oftype***:
Define o comportamento dos items da lista. Valores possíveis:
```
text                        (valor padrão, apenas texto)
radio-collection            (os itens se comportam como itens de escolha única)
checklist                   (os itens se comportam como itens de múltipla escolha)
```

***before/after***:
Descreve o que deve vir antes ou depois do caractere de lista.

Exemplo:
- Input
```ptml
<column>
    <text>How to use `HTML`: </text>
    <list oftype="enum" before="Step " after=" " items-behaviour="text">
        <column>
            <item>Create a HTML file.</item>
            <item>Configure DOM.</item>
            <item>Open file on browser.</item>
        </column>
    </list>
</column>
```

- Output
```
How to use `HTML`: 
Step 1. Create a HTML file.
Step 2. Configure DOM.
Step 3. Open file on browser.
```

---
### IGNORE POR ENQUANTO!
## Elemento `<code>`

`<code></code>`
Elemento usado para inferir ou referenciar scripts executáveis na linguagem F#.

### **Atributos**:

***src***:
Define o caminho para um arquivo de script externo.

***execute***:
Define como o script externo deve ser carregado e executado. Valores possíveis:
```
defer               (em paralelo, mas apenas se o PTML ja foi totalmente processado)
async               (de forma assíncrona)
```

---
### IGNORE POR ENQUANTO!
## Elemento `<button>`

`<button />`
Elemento com handling de evento para clique.

Exemplo:
- Input
```ptml
<button handler="" />
```

### **Atributos**:

***type***:
Define o tipo de botão. Valores possíveis:
```

```

--- 
### IGNORE POR ENQUANTO!
## Elemento `<input>`

`<input></input>` ou `<input />`
Elemento que recebe valores em formato de string.

Exemplo:
- Input
```ptml
<input type="button" event="single-click" placeholder="Click here!" width="28" />
```

- Output
```cmd
[Click here!               ]
```

### **Atributos**:

***event***:
Define o tipo de evento que o input vai ser ativado por. Valores possíveis:
```
clicks                          (aceita qualquer tipo de clique, podendo ser tratado durante o `handler`)
single-click
double-click
hold-click
...
```

***placeholder***:
Coloca um texto explícito em formatação DIM dentro do input. Tem como valor default, um caractere escondido.

---
### IGNORE POR ENQUANTO!
## Elemento `<entity>`

`<entity></entity>` ou `<entity />`
Representa uma entidade no terminal.

### **Atributos**:

***name***:
Define o nome da entidade.

---
### IGNORE POR ENQUANTO!
## Elemento `<bind>`

`<bind></bind>` ou `<bind />`

### **Atributos**:

***from***:
Define de onde aponta.

***to***:
Define para onde aponta.

***linkage***:
Define o tipo de ligação que as entidades terão. Valores possíveis:
```
arrow                   (e1 ----> e2)
mutual-arrow            (e1 <---> e2)
relationship            (e1 --<...>-- e2)
```

***relation***:
Descreve a relação entre as duas entidades.

---
### IGNORE POR ENQUANTO!
## Elemento `<tree>`

`<tree />`  
Elemento que desenha uma árvore de diretórios no terminal.

### **Atributos**:

***path***:
Caminho relativo ou absoluto da raiz.

***root-limit***:
Limite de ramificações totais da árvore final.

---
### IGNORE POR ENQUANTO!
## Elemento `<graphs>`

`<graphs></graphs>` ou `<graphs />`
Representa um plano cartesiano de coordenadas.

### **Atributos**:

***x-coordinates/y-coordinates***:
Define o valor limite para o crescimento do plano.

Exemplo.1:
- Input
```ptml
<graphs x-coordinates="10"/>
```

- Output
```cmd
   y
   ↑
   │ 
   │
   │
   │
   ┼────────────┼→ x
  0             10
```

Exemplo.2:
- Input
```ptml
<graphs x-coordinates="15" y-coordinates="30" />
```

- Output
```cmd
   y
   ↑
30 ┼
   │
   │
   │
   ┼────────────┼→ x
  0             15
``` 

***scale***:
Escala o tamanho do gráfico conforme o número entrado. Valores possíveis:
```
auto                                 (renderiza do tamanho necessário)
Nº%                                  (valor associado ao elemento-pai substituindo Nº por número --> percentage. Exemplo: 40%)
```

---
### IGNORE POR ENQUANTO!
## Elemento `<function>`

`<function></function>` ou `<function />`
Elemento que descreve uma função matemática para planos cartesianos.

Exemplos de valores a serem usados em `<function>`
```
x
Nx
x^N
x*N
x/N
x-N
x^N + N*x + C
log(2, x)
log(10, x)
ln(x)
...
```

> **x** representa as posições marcadas no eixo X do gráfico.
> **C** significa constante e deve ser trocada por um número qualquer 

Exemplo: 
- Input
```ptml
<graphs scale="auto" x-coordinates="15">
    <function>x^2 + 5x + 3</function>
</graphs>
```

- Output
```cmd
     y
     ↑
     │           ○
     │       ○
     │   ○ 
     ┼───┼───┼───┼→ x
    0    5    10  15
```

> O gráfico é definido pelo elemento `<graphs>`, mas os pontos/curvas dentro dele são definidos pelo elemento `<function>`

---
## Elemento `<progress>`

`<progress />`
Elemento que cria uma barra de progresso no terminal.

Exemplo:
- Input
```ptml
<row gap="1">
    <text>Progresso:</text>
    <progress value="50" max="100"/>
</row>
```

- Output
```
Progresso: █████░░░░░ 50%
```

### **Atributos:**

***style***:
Estilização da barra de progresso. Valores possíveis:
```
blocks                      (█ ▒ ░ - valor padrão)
dots                        (● ◍ ○)
square                      (■ ◩ □)
tiny-square                 (▪ ◾ ▫)
rhombus                     (◆ ◈ ◇)
```

***max***:
Valor máximo do progresso. Valor padrão 100.

***value***:
Valor atual do progresso. Valor padrão 0.

***show-value***:
Valor booleano para mostrar ou não porcentagem. Valor possíveis:
```
true
false               (valor padrão)
```
 
--- 
## Elemento `<frame>`

`<frame></frame>`
Elemento concreto com foco em enquadrar o conteúdo dentro.

Exemplo:
- Input
```ptml
<frame>
    <text>Hello World!</text>
</frame>
```

- Output
```
⌜            ⌝
 Hello World!
⌞            ⌟
```

### **Atributos:**

***framework***;
Estilização do enquadramento. Valores possíveis:
```
bold                (▛ ▜ ▙ ▟)
pixels              (▞ ▚ ▚ ▞)
cubic               (▅ ▅ ▅ ▅)
point               (▘ ▝ ▖ ▗)
border              (╭ ╮ ╰ ╯)
picture             (◜ ◝ ◟ ◞)
photograph          (⌜⌝ ⌞⌟ valor padrão)
pythagoras          (◤ ◥ ◣ ◢)
arrow               (↘ ↙ ↗ ↖)
ascii               (/ \ \ /)
``` 

***frame-color***:
Define a cor do enquadramento. Valores possíveis:
```
none 
black 
red 
green 
gold 
blue 
purple
cyan 
fire
limegreen 
yellow 
lightblue 
lilac 
crystal 
gray 
lightgray
```

***align***:
Atributo que alinha um conteúdo horizontalmente pela largura disponível do container pai. Valores possíveis:
```
start               (valor padrão/default)
center 
end
```

Exemplo:
- Input
```ptml
<frame framework="photograph" width="10" align="center">
    <row gap="1">
        <text foreground="red">Red</text>
        <text>John</text>
    </row>
</frame>
```

- Output (exemplo em terminal 12cols)
```terminal
⌜          ⌝
  Red John  
⌞          ⌟
```

---
### IGNORE POR ENQUANTO!!   
## Elemento `<include>`

`<include />`
Elemento que renderiza o conteúdo de um arquivo PTML alvo.

### **Atributos**:

***src***:
Define a fonte do conteúdo.

***implement-rule***:
Define oque será incluído ao arquivo. Valores possíveis:
```
snippet-only
...
```

---
### IGNORE POR ENQUANTO!!
## Elemento `<carousel>` 

`<carousel></carousel>`
Elemento que define um carrossel de slides que avança o `<slide>` ou `<layer>` conforme o sinal recebido. Sempre começa com o primeiro filho declarado no PTML, não é possível combinar layers com slides.

### **Atributos**:

***mf-signal***:
Define o sinal para avançar o slide.

***sb-signal***:
Define o sinal para recuar o slide.

Valores possíveis para ambos:
```
keybind->
keybind-<
keybind-D
keybind-A
crtl+l
crtl+n
alt+c
alt+v
```

---
### IGNORE POR ENQUANTO!!
## Elemento `<slide>`

`<slide></slide>`
Elemento que define o comportamento dos filhos como sendo de um slide. Desenha apenas quando o `<carousel>` avança para seu slide.

---
### IGNORE POR ENQUANTO!!
## Elemento `<timeline>`

`<timeline></timeline>`
Elemento abstrato que define uma linha temporal entre seus elementos filhos.

Exemplo:
- Input
```ptml
<timeline>
    <text>Starte</text>
    <text>Downloading</text>
    <text>Completed</text>
</timeline>
```

- Output
```cmd
● Started
│
● Downloading
│
● Completed
```

---
### IGNORE POR ENQUANTO!!
## Elemento `<toast>`

`<toast></toast>`
Elemento concreto que cria um texto temporário na tela.

### **Atributos**:

***duration***:
Duração extendida do texto em tela, Valor padrão '500ms'.

---
### IGNORE POR ENQUANTO!!
## Elemento `<tabs>`

`<tabs></tabs>`
Elemento que define um atalho de navegação para páginas de terminal.

Exemplo:
- Input
```ptml
<tabs selected="1" selected-color="#0f0">
    <tab>Home</tab>
    <tab>About</tab>
    <tab>Contact</tab>
</tabs>
```

- Output
```cmd
- Home - | About | Contact
```

> Pressione 'TAB'
- Output
```cmd
Home | - About - | Contact
```

---
### IGNORE POR ENQUANTO!
## Elemento `<modal>`

`<modal></modal>`
Elemento abstrato que define um modal popup.

