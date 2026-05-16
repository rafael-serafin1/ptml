### PTML

`<?ptml encoding="UTF-8" terminal-resize="calculate"!>` <br />
Serve como Instrução Procedural, além de garantir que ao terminal resize o retained-mode rendering vai ser corretamente aplicado. <br />
Valores possíveis para atributo ***terminal-resize***:  
```
reflow              (valor padrão)
clip
static
```

---
### Elemento Comentário `<!-- -->`
São ignorados pelo parser. Servem para comentar código.

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
0 -- reset
1 -- bold
2 -- dim
3 -- italic
4 -- underline
5 -- slow blink
6 -- rapid blink
7 -- reverse                 (marked)
8 -- conceal                 (hidden)
9 -- strikethrough
```

### OBS:
Caracteres Unicode complexos como emojis por enquanto serão ignorados e apenas seu código será colocado na tela.

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

> Sem padding por enquanto.

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
rounded             (╭ ╮ ╰ ╯)
ascii               (+ - |)
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
<!ptml enconding="UTF-8" terminal-resize="reflow"?>
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

### OBS:
Textos crus existem, mas eles não causam inutilização do elemento `<text>` pelo fato de eles não possuírem outra forma de serem estilizados, mas no fim eles acabam virando nó padrão de `<text>`.