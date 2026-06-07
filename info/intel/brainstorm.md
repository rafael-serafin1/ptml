## Procedural Instruction

`<?ptml encoding="UTF-8" terminal-resize="calculate"?>` <br />
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
<input>
<output>
<entity>
<bind>
<graphs>
```

Agora, os abstratos são:
```ptml
<row>
<column>
<depth>
<terminal>
<cell>
<snippet>
<code>
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

#### *Valores possíveis para o atributo `foreground` (o mesmo vale para `background`):*
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

#### *Valores possíveis para o atributo `foreground` (o mesmo vale para `background`):*
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
```

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

***overflow***:
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

- Output
```terminal

```

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

***overflow***:
Atributo que define o comportamento do container em caso de overflow de conteúdo. Caso não seja explicitado o resoluto em caso de overflow, o valor por padrão é o `break` (quebra o conteudo em uma nova linha). Valores possíveis:
```
break                               (quebra em qualquer caractere)
wrap                                (quebra respeitando palavras)
cut                                 (corta texto bruto)
clip                                (recorta área renderizada final.)
```

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
## Elemento `<depth>`

`<depth></depth>`
Faz juz a profundidade. Seus filhos são distribuídos por índice da 3º dimensão espacial.

Exemplo .1:
- Input
```ptml
<depth index="0">
    <box>
        <text>Hello World!</text>
    </box>
</depth>
<depth index="-1">
    <box>
        <text>GoodBye World!</text>
    </box>
</depth>
<depth index="-2">
    <box>
        <text>Hello Again!</text>
    </box>
</depth>
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
<depth index="0">
    <box>
        <text>Hello World!</text>
    </box>
    <box>
        <text>GoodBye World!</text>
    </box>
</depth>
```

- Output
```cmd
Erro: não pode haver dois filhos com índice de mesmo valor!
```

Exemplo .2:
- Input
```ptml
<depth index="0">
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
</depth>
<depth index="-1">
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
</depth>
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
<depth index="0">
    <column>
        <box>
            <text>Hello World!</text>
        </box>
    </column>
</depth>
<depth index="-1">
    <column>
        <box>
            <text>GoodBye World!</text>
        </box>
    </column>
</depth>
```

- Output
```cmd
┌───────────┐
│┌──────────┴─┐
└┤Hello World!│
 └────────────┘
```

***overflow***:
Atributo que define o comportamento do container em caso de overflow de conteúdo. Caso não seja explicitado o resoluto em caso de overflow, o valor por padrão é o `break` (quebra o conteudo em uma nova linha). Valores possíveis:
```
break                               (quebra em qualquer caractere)
wrap                                (quebra respeitando palavras)
cut                                 (corta texto bruto)
clip                                (recorta área renderizada final.)
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
<depth index="0" z-align="center">
    <column>
        <text>Hello World!</text>
    </column>
</depth>
<depth index="-1" z-align="center">
    <column>
        <text>Bye World!</text>
    </column>
</depth>
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
<depth index="0" gap="1">
    <column>
        <text>Hello World!</text>
    </column>
</depth>
<depth index="-1" gap="1">
    <column>
        <text>Bye World!</text>
    </column>
</depth>
```

- Output
```cmd
┌────────────┐  
│Bye World!  │        
└┬───────────┴┐ 
 │Hello World!│ 
 └────────────┘ 
```

### **OBS** N2 --> Caso o elemento `<depth>` tenha gap igual ou maior que 1 ***E*** o conteúdo do elemento `<column>`, de índice menor que a da superfície, for maior que o conteúdo do elemento da superfície, um aviso deve ser gerado no terminal e a coluna deve ser exibida da seguinte forma:

- Input 
```ptml
<depth gap="1">
    <column index="0">
        <text>Hello World!</text>
    </column>
    <column index="-1">
        <text>GoodBye World!</text>
    </column>
</depth>
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

***overflow***:
Atributo que define o comportamento do container em caso de overflow de conteúdo. Caso não seja explicitado o resoluto em caso de overflow, o valor por padrão é o `break` (quebra o conteudo em uma nova linha). Valores possíveis:
```
break                               (quebra em qualquer caractere)
wrap                                (quebra respeitando palavras)
cut                                 (corta texto bruto)
clip                                (recorta área renderizada final.)
```

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

***width/height***:
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

***border-color***:
Define a cor da borda. Os valores possíveis são os mesmos do `<text>`.

***align***:
Atributo que alinha um conteúdo horizontalmente pela largura disponível do container pai. Valores possíveis:
```
start               (valor padrão/default)
center 
end
```

***padding***:
Atributo que define o espaço entre a borda e o conteúdo, sendo seu valor padrão 0.

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

***overflow***:
Atributo que define o comportamento do container em caso de overflow de conteúdo. Caso não seja explicitado o resoluto em caso de overflow, o valor por padrão é o `break` (quebra o conteudo em uma nova linha). Valores possíveis:
```
break                               (quebra em qualquer caractere)
wrap                                (quebra respeitando palavras)
cut                                 (corta texto bruto)
clip                                (recorta área renderizada final.)
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

***width/height***:
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

***padding***:
Atributo que define o espaço entre a borda e o conteúdo, sendo seu valor padrão 0.

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
waiting         (. .. ... ....)
beam            (= == ===) -- ([=  ] [== ] [===] [ ==] [  =])
ascii           (| / - \)
circle          (◐ ◓ ◑ ◒)
square          (◰ ◳ ◲ ◱)
moon            (◜ ◝ ◞ ◟)
arrow           (← ↖ ↑ ↗ → ↘ ↓ ↙)
bounce          (⠁ ⠂ ⠄ ⠂)
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
Define o que deve fazer ao ser completado. Por padrão, é colocado o caractere `✓` (alias para `check`).

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

***extend***: 
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
## Elemento `<input>`

`<input></input>` ou `<input />`
Elemento que recebe valores e/ou escuta eventos.

Exemplo:
- Input
```ptml
<input type="button" event="single-click" handler="handleClick()" placeholder="Click here!" />
```

- Output
```cmd
┌───────────┐
│Click here!|
└───────────┘
```

### **Atributos**:

***overflow***:
Atributo que define o comportamento do container em caso de overflow de conteúdo. Caso não seja explicitado o resoluto em caso de overflow, o valor por padrão é o `clip`. Valores possíveis:
``` 
clip                                (recorta área renderizada final)
break                               (quebra em qualquer caractere)
wrap                                (quebra respeitando palavras)
cut                                 (corta texto bruto)
```

***type***:
Define o tipo do input. Valores possíveis:
```
scan
button
radio-button
check-box
...
```

***event***:
Define o tipo de evento que o input vai ser ativado por. Valores possíveis:
```
clicks                          (aceita qualquer tipo de clique, podendo ser tratado durante o `handler`)
single-click
double-click
hold-click
...
```

***handler***:
Define a função que será executada ao detectar que o evento foi chamado. A função deve ser declarada dentro do escopo do PTML através do elemento `<code></code>`.

***placeholder***:
Coloca um texto explícito em formatação DIM dentro do input. Tem como valor default, um caractere escondido.

---
### IGNORE POR ENQUANTO!
## Elemento `<output>`

`<output></output>` ou `<output/>`
É um campo específico onde o valor retornado do elemento `<input />` será mostrado.

---
## Elemento `<entity>`

`<entity></entity>` ou `<entity />`
Representa uma entidade no terminal.

### **Atributos**:

***name***:
Define o nome da entidade.

---
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

`<tree></tree>`


---
### IGNORE POR ENQUANTO!
## Elemento `<graphs>`

`<graphs></graphs>` ou `<graphs />`
Representa um plano cartesiano de coordenadas.

### **Atributos**:

***width/height***:
Corresponde a largura e altura do componente. Seus valores são numéricos inteiros ou específicos. Em caso de não declaração, o valor padrão é `auto` (renderiza no tamanho necessário para confortar o texto). Valores não-numéricos específicos:
```
auto                                    (renderiza do tamanho necessário)
Nº%                                     (valor associado ao elemento-pai substituindo Nº por número --> percentage. Exemplo: 40%)
```

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