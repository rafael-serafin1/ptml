# PTML (Portable Terminal Markup Language)

PTML é uma linguagem de marcação declarativa criada para construir interfaces de terminal complexas de forma estruturada, legível e expressiva.

Sua proposta é trazer para o terminal uma experiência semelhante ao que HTML + CSS oferecem para interfaces web gráficas, permitindo definir layouts, componentes, estilos, animações (mesmo que ainda não funcionam corretamente) e relacionamentos através de uma sintaxe simples baseada em XML.

---

## O que o PTML resolve?

Criar interfaces de terminal normalmente exige:

- Manipulação manual de ANSI Escape Codes
- Controle de posicionamento caractere por caractere
- Gerenciamento complexo de layouts
- Código procedural difícil de manter

O PTML abstrai esses problemas através de uma linguagem declarativa.

Em vez de:

```fs
printf "\u001b[31mHello World\u001b[0m"
```

Você escreve:

```ptml
<text foreground="red">
    Hello World
</text>
```

---
# Instalação

## Executável 
Para usar a versão mais recente do projeto, siga os seguintes passos para começar:

### **Passo 1**:
Clone o repositório.
```sh
git clone "https://github.com/rafael-serafin1/ptml.git" 
```

### **Passo 2**:
Adicionar o executável ao PATH.
```sh
.\build.ps1                 # se estiver no windows
.\build.sh                  # se estiver no linux
```

### **Passo 3**:
Teste o executável.
```bash
ptml --version
```

## Extensão (Visual Studio Code)
Instalação da extensão em desenvolvimento, siga os passos:

### **Passo 1**:
Execute o script.
```sh
.\package.ps1                 # se estiver no windows
.\package.sh                  # se estiver no linux
```

### **Passo 2**:
Clique com o botão direito do mouse no arquivo **"*.vsix"** gerado dentro da pasta **"./syntax-highlighter"** e clique em **"Install Extension VSIX"**.

---

# Principais Recursos

## Layouts

Organização de conteúdo através de:

- `<row>` → distribuição horizontal
- `<column>` → distribuição vertical
- `<layer>` → profundidade (camadas)
- `<cell>` → grids e divisões

Exemplo:

```ptml
<row gap="1">
    <text>A</text>
    <text>B</text>
</row>
```

Resultado:

```txt
A B
```

---

## Componentes Visuais

PTML possui elementos próprios para UI de terminal:

- `<text>`
- `<frag>`
- `<box>`
- `<block>`
- `<hr>`
- `<spinner>`

Exemplo:

```ptml
<block title="Status">
    <text>Servidor Online</text>
</block>
```

Resultado:

```txt
┌─Status──────────┐
│ Servidor Online │
└─────────────────┘
```

---

## Estilização

Suporte para:

- Cores
- Backgrounds
- Fontes ANSI
- Negrito
- Itálico
- Underline
- Blink
- Strike-through

Exemplo:

```ptml
<text foreground="limegreen" font="bold">
    Sistema Operacional
</text>
```

---

## Componentização

Através de `<snippet>` é possível reutilizar estilos.

```ptml
<snippet id="warning">
    foreground="red"
    font="bold"
</snippet>

<text snippet="warning">
    Erro crítico
</text>
```

---

## Elementos Interativos

O projeto já prevê suporte para:

```ptml
<input />
<output />
<code />
```

Permitindo interfaces interativas e integração com scripts F#.

---

# Conceitos Fundamentais

O PTML divide seus elementos em duas categorias.

## Elementos Concretos

Renderizam algo no terminal.

Exemplos:

```ptml
<text>
<box>
<block>
<spinner>
<hr>
```

---

## Elementos Abstratos

Controlam apenas o layout e o fluxo.

Exemplos:

```ptml
<row>
<column>
<layer>
<cell>
<terminal>
```

---

# Primeiro Contato Ideal

A melhor forma de aprender PTML não é tentando usar todos os componentes.

Comece apenas com:

### 1. Texto

```ptml
<text>Hello World</text>
```

---

### 2. Layout Vertical

```ptml
<column>
    <text>Item 1</text>
    <text>Item 2</text>
</column>
```

---

### 3. Layout Horizontal

```ptml
<row gap="1">
    <text>A</text>
    <text>B</text>
</row>
```

---

### 4. Containers

```ptml
<box>
    <text>Hello World</text>
</box>
```

---

### 5. Blocos Nomeados

```ptml
<block title="Informações">
    <text>CPU: 73%</text>
</block>
```

---

# Exemplo Completo

```ptml
<?ptml encoding="UTF-8" terminal-resize="reflow"?>

<terminal x-align="center">

    <block title="Sistema" border="rounded">

        <column>

            <text foreground="limegreen">
                Servidor Online
            </text>

            <hr />

            <row gap="2">
                <text>CPU: 73%</text>
                <text>RAM: 41%</text>
            </row>

        </column>

    </block>

</terminal>
```

---

# Estado Atual
```
✅ Funcionando: Spinners, Barra de Progresso                                                                                |
⚠️ Funcionando de forma errada: Depth Layer                                                                                 |
⛔ Não funcionando: ...                                                                                                     |
🚧 Em construção: Frames                                                                                                    |
📜 Planejados: Listas, Inputs avançados, Graphs, Trees, Output binding e Execução integrada de código                       |
```

---

# Filosofia

PTML não tenta substituir linguagens de programação.
Seu objetivo é ser uma **camada declarativa** para construção de interfaces de terminal, tornando o processo de criação de layouts complexos mais legíveis, reutilizáveis e fáceis de manter.
