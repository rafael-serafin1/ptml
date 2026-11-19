─ │ ┌ ┐ └ ┘
├ ┤ ┬ ┴ ┼

═ ║ ╔ ╗ ╚ ╝
╠ ╣ ╦ ╩ ╬

╭ ╮ ╰ ╯
╓ ╖ ╙ ╜
╒ ╕ ╘ ╛

╞ ╡ ╪
╟ ╢ ╫
╤ ╧

┏ ┓ ┗ ┛
┣ ┫ ┳ ┻ ╋

┍ ┑ ┕ ┙
┝ ┥ ┯ ┷

█ ▉ ▊ ▋ ▌ ▍ ▎ ▏
▓ ▒ ░

■ □ ▪ ▫
▢ ▣

░ ░ ░
▒ ▒ ▒
▓ ▓ ▓
█ █ █

▁ ▂ ▃ ▄ ▅ ▆ ▇ █
█ ▇ ▆ ▅ ▄ ▃ ▂ ▁

▌ ▐
▖ ▗ ▘ ▙ ▚ ▛ ▜ ▝ ▞ ▟

← ↑ → ↓
↖ ↗ ↘ ↙

⇒ ⇐ ⇑ ⇓
⇢ ⇠

➜ ➝ ➞ ➟
⟶ ⟵

┄ ┅ ┈ ┉
╌ ╍

╱ ╲ ╳
╴ ╵ ╶ ╷

⠁ ⠂ ⠄ ⡀
⣿ ⣶ ⣤ ⣀
⢀ ⢠ ⢰ ⢸

◜ ◝ ◞ ◟
◢ ◣ ◤ ◥

⌜ ⌝
⌞ ⌟

⎛ ⎜ ⎝
⎞ ⎟ ⎠

▞▚
╱╲
╲╱
╳

⟦ ⟧
⟨ ⟩
⟪ ⟫

⫷ ⫸
《 》
「 」
『 』

══════════════
──────────────
━━━━━━━━━━━━━━
▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▁▁▁▁▁▁▁▁▁▁▁▁

● ○ ◌ ◍
■ □ ▣ ▤
◆ ◇

✔ ✖
✦ ✧
✱ ✲

⚙ ⚡
☰ ☱ ☲ ☳

estes são caracteres unicodes para uso em CMD.

ANSI CODE:

\x1b[NUMEROm --> adicionar cor

Troque NUMERO por qualquer um destes:
-- estilo da fonte
0 = reset
1 = bold
2 = dim
3 = italic
4 = underline
5 = slow blink
6 = rapid blink
7 = reverse                 (marked)
8 = conceal                 (hidden)
9 = strikethrough
-- cor da fonte
31 = vermelho
32 = verde
33 = amarelo
34 = azul
35 = magenta
36 = ciano 


OSC (Operating System Commands)

\x1b]Nº; parametro \x07

Troque Nº por qualquer um destes:

### OSC TERMINAL
0   =>  parametro esperado: texto   =   Altera o título da janela, quanto o nome do ícone/aba
1   =>  p' esperado: texto  =   Altera apenas o nome do ícone/aba do terminal
2   =>  p' esperado: texto  =   Altera apenas o título da janela do terminal

### HYPERLINK
8   => p' esperado: params; url     =   Híperlink clicável

### POP-UP MERDA
9 (não sei)  =>  p' esperado: msg    =   Envia uma mensagem em um pop-up de terminal

### POP-UP BOM
- Código: 99
- Sintaxe esperada:
    `ESC ]99; <<campo=<valor> : <campo2>=<valor2>> ; <mensagem> BEL`

Campos com suas funções e valores
| Campo | Função | Valor |
| - | - | - |
| `i` | identificador da notificação | <qualquer_numero> |
| `p` | Tipo de conteúdo | title/body/close/icon/?/alive/buttons |
| `e` | Indica se o conteúdo está em Base64 | 0/1 |
| `u` | Nível de urgência | 0 (baixa), 1 (normal), 2 (crítica) | 
| `a` | Ações disponíveis | focus/report/-focus/-report | 
| `f` | Nome da aplicação responsável pela notificação | Texto em UTF-8 Base64 |
| `s` | Som produzido pela notificação | system/silent/error/warn/warning/info/question | 
 
| Valor | Explicação |
| - | - |
| focus | Foca a janela de origem |
| report | Envia uma resposta ao programa |
| -focus | Desativa o foco |
| -report | Desativa o envio de resposta |
| ? | Consultar capacidades |
| alive | Consultar notificações ainda ativas |

> Tem como combinar valores, como por exemplo: `a=-focus,-report` 

### NOTIFICAÇÃO
- Código: 777 
- Sintaxe esperada:
    `ESC ]777;notify; <título>; <mensagem> BEL`

### CURSOR COLOR
12  => p' esperado: cor     =   Altera a cor do cursor do texto

### OSC 52
- Sintaxe esperada:
    `ESC ]52; <seletor>; <texto-embase64> BEL` 

Possíveis seletores:
```
c           => Clipboard    ... Área de transferência convencional
p           => Primary      ... Seleção primária do X11 (texto selecionado pelo mouse)
q           => Secondary    ... Seleção secundária do X11
s           => Select       ... Seleção configurável do X11
0 - 7       => Cut buffers  ... Buffers legado do X11
```

-- imagens no terminal
1337    =>  p' esperado: params     =   Exibe imagens diretamente no terminal


TIPOS DE ANSI (para ver melhor depois):
C0 Control Characters   =>  '\n' '\a'
ESC Sequences   =>  Comandos simples ESC X
CSI (Control Sequence Introducer)   =>      ESC[ o famoso \x1b[
OSC (Operating System Command)
DCS (Device Control String)
PM (Privacy Message)
