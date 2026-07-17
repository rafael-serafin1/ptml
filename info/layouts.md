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
-- janela/ícone/aba do terminal
0   =>  parametro esperado: texto   =   Altera o título da janela, quanto o nome do ícone/aba
1   =>  p' esperado: texto  =   Altera apenas o nome do ícone/aba do terminal
2   =>  p' esperado: texto  =   Altera apenas o título da janela do terminal
-- hyperlink
8   => p' esperado: params; url     =   Híperlink clicável
-- pop-up
9 ou 99 (não sei)  =>  p' esperado: msg    =   Envia uma mensagem em um pop-up de terminal
-- cores
12  => p' esperado: cor     =   Altera a cor do cursor do texto
-- Clipboard
52  =>  p' esperado: texto  =   Copia o texto para a área de transferência
-- imagens no terminal
1337    =>  p' esperado: params     =   Exibe imagens diretamente no terminal


TIPOS DE ANSI (para ver melhor depois):
C0 Control Characters   =>  '\n' '\a'
ESC Sequences   =>  Comandos simples ESC X
CSI (Control Sequence Introducer)   =>      ESC[ o famoso \x1b[
OSC (Operating System Command)
DCS (Device Control String)
PM (Privacy Message)
