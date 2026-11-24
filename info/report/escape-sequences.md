# Catálogo de sequências de controle para terminais

Este documento amplia o levantamento iniciado em [`layouts.md`](./layouts.md). Ele reúne controles C0/C1, sequências ESC e CSI do repertório ECMA-48/VT, além de protocolos OSC, DCS e APC encontrados em terminais atuais.

> **Escopo:** não existe uma lista única de “todas” as sequências aceitas por todo terminal. Emuladores implementam subconjuntos diferentes e acrescentam extensões próprias. Este catálogo cobre famílias e comandos conhecidos, não promete exaustividade de todos os fabricantes. Uma sequência descrita aqui não implica suporte pelo PTML nem pelo terminal usado.

## Notação

| Notação | Bytes enviados |
| --- | --- |
| `ESC` | `\x1b` (hexadecimal `1B`) |
| `BEL` | `\x07` (hexadecimal `07`) |
| `ST` | `ESC \` (`\x1b\\`), terminador de strings de controle |
| `CSI` | `ESC [` (`\x1b[`) |
| `OSC` | `ESC ]` (`\x1b]`) |
| `DCS` | `ESC P` (`\x1bP`) |
| `SOS` | `ESC X` (`\x1bX`) |
| `PM` | `ESC ^` (`\x1b^`) |
| `APC` | `ESC _` (`\x1b_`) |

Nas formas abaixo, espaços só são usados para facilitar a leitura: não fazem parte da sequência, salvo quando indicados como `SP`. Parâmetros como `<n>`, `<texto>` e `<URL>` são substituídos por valores reais. A forma de 7 bits com `ESC` é preferível em aplicações UTF-8; os controles C1 de 8 bits podem conflitar com bytes de continuação UTF-8.

## Controles C0

C0 são controles de byte único, entre `0x00` e `0x1F`. Alguns são usados diretamente no fluxo de texto; os demais são controles históricos, de protocolo ou de transporte e podem ser ignorados pelo emulador.

| Byte | Nome | Efeito comum |
| --- | --- | --- |
| `0x00` | NUL | Nulo; geralmente ignorado |
| `0x01` | SOH | Início de cabeçalho |
| `0x02` | STX | Início de texto |
| `0x03` | ETX | Fim de texto; pode representar Ctrl+C em uma TTY |
| `0x04` | EOT | Fim de transmissão; pode representar Ctrl+D em uma TTY |
| `0x05` | ENQ | Solicitação de resposta/identificação; dependente do terminal |
| `0x06` | ACK | Reconhecimento |
| `0x07` | BEL | Campainha/alerta; também aceito como terminador de algumas OSC |
| `0x08` | BS | Backspace: move o cursor uma posição para trás |
| `0x09` | HT | Tabulação horizontal |
| `0x0A` | LF | Avança uma linha |
| `0x0B` | VT | Tabulação vertical; frequentemente tratado como LF |
| `0x0C` | FF | Avanço de página; frequentemente tratado como LF |
| `0x0D` | CR | Retorno do cursor ao início da linha |
| `0x0E` | SO | Shift Out; seleciona conjunto de caracteres G1 em terminais compatíveis |
| `0x0F` | SI | Shift In; seleciona conjunto de caracteres G0 em terminais compatíveis |
| `0x10` | DLE | Data Link Escape; controle de transporte |
| `0x11` | DC1 / XON | Retoma fluxo em protocolos com controle de fluxo |
| `0x12` | DC2 | Controle de dispositivo |
| `0x13` | DC3 / XOFF | Pausa fluxo em protocolos com controle de fluxo |
| `0x14` | DC4 | Controle de dispositivo |
| `0x15` | NAK | Reconhecimento negativo |
| `0x16` | SYN | Sincronismo |
| `0x17` | ETB | Fim de bloco de transmissão |
| `0x18` | CAN | Cancela uma sequência de controle em andamento |
| `0x19` | EM | Fim de meio de transmissão |
| `0x1A` | SUB | Substituição; pode cancelar/abortar uma sequência incompleta |
| `0x1B` | ESC | Inicia uma sequência ESC |
| `0x1C` | FS | Separador de arquivo |
| `0x1D` | GS | Separador de grupo |
| `0x1E` | RS | Separador de registro |
| `0x1F` | US | Separador de unidade |

Na prática, os controles que aplicações de terminal mais encontram diretamente são `BEL`, `BS`, `HT`, `LF`, `CR` e `ESC`. A interpretação de controles de transporte depende do sistema, do modo da TTY e do emulador.

## Controles C1 e introduzidos por ESC

C1 ocupa `0x80` a `0x9F`. A tabela mostra a forma de 8 bits e, quando aplicável, sua forma de 7 bits iniciada por `ESC`.

| C1 | Nome | Forma de 7 bits | Uso |
| --- | --- | --- | --- |
| `0x80` | PAD | — | Preenchimento; raramente usado |
| `0x81` | HOP | — | Preset de octeto alto; controle histórico |
| `0x82` | BPH | — | Permite quebra de linha neste ponto |
| `0x83` | NBH | — | Não permite quebra de linha neste ponto |
| `0x84` | IND | `ESC D` | Avança uma linha na posição atual do cursor |
| `0x85` | NEL | `ESC E` | Vai para o início da próxima linha |
| `0x86` | SSA | — | Início de área selecionada |
| `0x87` | ESA | — | Fim de área selecionada |
| `0x88` | HTS | `ESC H` | Define tabulação na coluna atual |
| `0x89` | HTJ | — | Tabulação horizontal com justificação |
| `0x8A` | VTS | — | Define um ponto de tabulação vertical |
| `0x8B` | PLD | — | Avança linha parcialmente |
| `0x8C` | PLU | — | Retrocede linha parcialmente |
| `0x8D` | RI | `ESC M` | Avança uma linha para cima; pode rolar a região |
| `0x8E` | SS2 | `ESC N` | Seleciona G2 para o próximo caractere |
| `0x8F` | SS3 | `ESC O` | Seleciona G3 para o próximo caractere |
| `0x90` | DCS | `ESC P` | Inicia uma Device Control String |
| `0x91` | PU1 | — | Uso privado |
| `0x92` | PU2 | — | Uso privado |
| `0x93` | STS | — | Define o estado de transmissão |
| `0x94` | CCH | — | Cancela caractere |
| `0x95` | MW | — | Mensagem em espera |
| `0x96` | SPA | `ESC V` | Início de área protegida |
| `0x97` | EPA | `ESC W` | Fim de área protegida |
| `0x98` | SOS | `ESC X` | Inicia uma string de controle |
| `0x99` | SGCI | — | Introdução de caractere gráfico único |
| `0x9A` | SCI | — | Introdução de caractere de controle único; `ESC Z` é interpretado como consulta antiga de identificação (DECID) por xterm |
| `0x9B` | CSI | `ESC [` | Inicia uma sequência CSI |
| `0x9C` | ST | `ESC \` | Termina uma string de controle |
| `0x9D` | OSC | `ESC ]` | Inicia um comando para o emulador |
| `0x9E` | PM | `ESC ^` | Inicia uma Privacy Message |
| `0x9F` | APC | `ESC _` | Inicia um Application Program Command |

Os controles C1 sem descrição operacional comum são reservados, raramente implementados ou voltados a terminais antigos. Não se deve presumir que um terminal moderno os execute.

`ESC Z` não é uma substituição portátil de `CSI c`: seu significado depende da emulação.

## Sequências ESC

Sequências ESC começam por `ESC` e não por `CSI`. Além dos introdutores C1 acima, estas são formas recorrentes:

| Sequência | Nome | Efeito |
| --- | --- | --- |
| `ESC D` | IND | Avança uma linha, mantendo a coluna |
| `ESC E` | NEL | Próxima linha e primeira coluna |
| `ESC H` | HTS | Define um ponto de tabulação na coluna atual |
| `ESC M` | RI | Índice reverso; move para cima e pode rolar a região |
| `ESC 7` | DECSC | Salva posição/estado do cursor (DEC) |
| `ESC 8` | DECRC | Restaura posição/estado do cursor salvo (DEC) |
| `ESC c` | RIS | Reinicialização completa do terminal; efeito amplo e deve ser usado com cuidado |
| `ESC =` | DECKPAM | Ativa modo de aplicação do teclado numérico |
| `ESC >` | DECKPNM | Retorna o teclado numérico ao modo numérico |
| `ESC N` | SS2 | Usa o conjunto G2 para o próximo caractere |
| `ESC O` | SS3 | Usa o conjunto G3 para o próximo caractere |
| `ESC \` | ST | Termina strings OSC, DCS, SOS, PM ou APC |
| `ESC # 3` / `ESC # 4` | DECDHL | Linha dupla-altura, metade superior/inferior; DEC |
| `ESC # 5` | DECSWL | Linha de largura normal; DEC |
| `ESC # 6` | DECDWL | Linha de largura dupla; DEC |
| `ESC # 8` | DECALN | Teste de alinhamento da tela; preenche a tela com `E` |
| `ESC ( B` | Designa G0 | Seleciona US-ASCII para G0 |
| `ESC ( 0` | Designa G0 | Seleciona o conjunto gráfico DEC (desenho de linhas) |
| `ESC ) B` / `ESC ) 0` | Designa G1 | Seleciona ASCII ou gráfico DEC para G1 |
| `ESC * <final>` | Designa G2 | Seleciona conjunto de caracteres G2 (ISO 2022/DEC) |
| `ESC + <final>` | Designa G3 | Seleciona conjunto de caracteres G3 (ISO 2022/DEC) |
| `ESC n` / `ESC o` | LS2 / LS3 | Invoca G2/G3 como conjunto de caracteres GL |
| <code>ESC ~</code> / <code>ESC }</code> / <code>ESC &#124;</code> | LS1R / LS2R / LS3R | Invoca G1/G2/G3 como conjunto de caracteres GR |
| `ESC % @` / `ESC % G` | ISO 2022 | Seleciona conjunto de caracteres padrão / UTF-8 em terminais compatíveis |
| `ESC SP F` | S7C1T | Solicita representação de controles C1 como sequências de 7 bits |
| `ESC SP G` | S8C1T | Solicita representação de controles C1 como bytes de 8 bits |

Designação de conjuntos G0–G3 pertence a modelos de terminais e codificações anteriores ao uso generalizado de Unicode. Em aplicações UTF-8, normalmente é preferível emitir Unicode diretamente.

## CSI — Control Sequence Introducer

Formato geral:

```text
CSI <parâmetros> <intermediários> <final>
```

`CSI` é `ESC [`; os parâmetros costumam ser números separados por `;`. `?`, `>`, `!` e `=` podem introduzir famílias privadas ou extensões. Os comandos abaixo são os mais encontrados em terminais VT/ECMA-48 e emuladores compatíveis.

### Cursor e posição

| Sequência | Nome | Efeito |
| --- | --- | --- |
| `CSI <n>A` | CUU | Move o cursor para cima |
| `CSI <n>B` | CUD | Move o cursor para baixo |
| `CSI <n>C` | CUF | Move o cursor para a direita |
| `CSI <n>D` | CUB | Move o cursor para a esquerda |
| `CSI <n>E` | CNL | Move para o início de uma linha abaixo |
| `CSI <n>F` | CPL | Move para o início de uma linha acima |
| `CSI <col>G` ou `CSI <col>\`` | CHA / HPA | Define a coluna |
| `CSI <row>d` | VPA | Define a linha |
| `CSI <row>;<col>H` ou `CSI <row>;<col>f` | CUP / HVP | Posiciona o cursor; linha e coluna são 1-based |
| `CSI <n>I` | CHT | Avança até a próxima tabulação |
| `CSI <n>Z` | CBT | Retrocede até a tabulação anterior |
| `CSI s` | SCOSC | Salva posição do cursor em alguns terminais |
| `CSI u` | SCORC | Restaura posição salva em alguns terminais |
| `CSI <n>b` | REP | Repete o último caractere imprimível |

`CSI s` e `CSI u` podem ter outros significados em extensões de terminal; para compatibilidade DEC, também existem `ESC 7` e `ESC 8`.

### Apagar, inserir, excluir e rolar

| Sequência | Nome | Efeito |
| --- | --- | --- |
| `CSI 0J` | ED 0 | Apaga do cursor até o fim da tela |
| `CSI 1J` | ED 1 | Apaga do início da tela até o cursor |
| `CSI 2J` | ED 2 | Apaga a tela |
| `CSI 3J` | ED 3 | Apaga o histórico/linhas salvas; extensão de emuladores como xterm |
| `CSI 0K` | EL 0 | Apaga do cursor até o fim da linha |
| `CSI 1K` | EL 1 | Apaga do início da linha até o cursor |
| `CSI 2K` | EL 2 | Apaga a linha inteira |
| `CSI <n>X` | ECH | Apaga caracteres sem deslocar o restante |
| `CSI <n>@` | ICH | Insere espaços na posição do cursor |
| `CSI <n>P` | DCH | Exclui caracteres a partir do cursor |
| `CSI <n>L` | IL | Insere linhas na região de rolagem |
| `CSI <n>M` | DL | Exclui linhas na região de rolagem |
| `CSI <n>S` | SU | Rola o conteúdo para cima |
| `CSI <n>T` | SD | Rola o conteúdo para baixo |

O parâmetro omitido costuma valer 1 nos comandos de movimento/edição. ED/EL e o comportamento de limpeza de histórico podem variar; por exemplo, `CSI 3J` não é universal.

### SGR — estilos, cores e aparência do texto

Formato:

```text
CSI <código>[;<código>...]m
```

Exemplo:

```text
ESC[1;38;2;255;80;20mtexto ESC[0m
```

`SGR 0` restaura o estilo padrão. Os estilos permanecem ativos até serem redefinidos ou substituídos.

| Código | Efeito | Código de cancelamento comum |
| --- | --- | --- |
| `0` | Redefine atributos para o padrão | — |
| `1` | Negrito/intensidade forte | `22` |
| `2` | Fraco/menos intenso | `22` |
| `3` | Itálico | `23` |
| `4` | Sublinhado | `24` |
| `5` | Piscar lento | `25` |
| `6` | Piscar rápido; suporte limitado | `25` |
| `7` | Inversão de foreground/background | `27` |
| `8` | Texto oculto | `28` |
| `9` | Tachado | `29` |
| `21` | Sublinhado duplo em implementações compatíveis | `24` |
| `22` | Cancela negrito e fraco | — |
| `23` | Cancela itálico | — |
| `24` | Cancela sublinhado | — |
| `25` | Cancela piscamento | — |
| `27` | Cancela inversão | — |
| `28` | Torna o texto visível | — |
| `29` | Cancela tachado | — |
| `30–37` | Foreground: 8 cores padrão | `39` |
| `38;5;<n>` | Foreground pela paleta indexada (normalmente 0–255) | `39` |
| `38;2;<r>;<g>;<b>` | Foreground RGB | `39` |
| `39` | Foreground padrão do terminal | — |
| `40–47` | Background: 8 cores padrão | `49` |
| `48;5;<n>` | Background pela paleta indexada | `49` |
| `48;2;<r>;<g>;<b>` | Background RGB | `49` |
| `49` | Background padrão do terminal | — |
| `51` / `52` | Emoldurado / circulado; suporte variável | `54` |
| `53` | Sobrelinha | `55` |
| `54` | Cancela moldura/círculo | — |
| `55` | Cancela sobrelinha | — |
| `58;5;<n>` | Cor de sublinhado pela paleta indexada | `59` |
| `58;2;<r>;<g>;<b>` | Cor RGB de sublinhado | `59` |
| `59` | Cor de sublinhado padrão | — |
| `60–65` | Marcas ideográficas; recurso raro | varia |
| `73` / `74` | Sobrescrito / subscrito; suporte variável | `75` |
| `75` | Cancela sobrescrito/subscrito | — |
| `90–97` | Foreground de alta intensidade (8 cores) | `39` |
| `100–107` | Background de alta intensidade (8 cores) | `49` |

As cores de `30–37` e `40–47` são preto, vermelho, verde, amarelo, azul, magenta, ciano e branco. Os valores RGB vão de `0` a `255`. A paleta indexada de 256 cores é comum, mas não garante que o terminal mostre a cor exata. Negrito não significa necessariamente uma cor mais clara em todos os emuladores.

Alguns terminais aceitam formas SGR com subparâmetros separados por `:` para variantes como estilos de sublinhado:

| Sequência | Variação de sublinhado |
| --- | --- |
| `CSI 4:0m` | Nenhum |
| `CSI 4:1m` | Simples |
| `CSI 4:2m` | Duplo |
| `CSI 4:3m` | Ondulado |
| `CSI 4:4m` | Pontilhado |
| `CSI 4:5m` | Tracejado |

O formato com `:` e alguns estilos mais novos são extensões; valide o suporte antes de depender deles.

### Modos, margens e formato do cursor

| Sequência | Efeito |
| --- | --- |
| `CSI 4h` / `CSI 4l` | Ativa/desativa inserção de caracteres |
| `CSI 20h` / `CSI 20l` | Ativa/desativa nova linha junto ao retorno de carro |
| `CSI ? 6h` / `CSI ? 6l` | Ativa/desativa modo de origem (DEC); muda a referência de posição do cursor |
| `CSI ? 7h` / `CSI ? 7l` | Ativa/desativa quebra automática de linha (DEC) |
| `CSI ? 25h` / `CSI ? 25l` | Mostra/oculta o cursor |
| `CSI ? 69h` / `CSI ? 69l` | Ativa/desativa margens esquerda/direita em terminais compatíveis |
| `CSI <top>;<bottom>r` | Define margens superior/inferior de rolagem |
| `CSI <left>;<right>s` | Define margens horizontais com modo de margem habilitado; DEC |
| `CSI <n> SP q` | Define formato do cursor (DECSCUSR) |
| `CSI ? 5h` / `CSI ? 5l` | Ativa/desativa vídeo reverso da tela; DEC |

Formato do cursor (`CSI <n> SP q`):

| `<n>` | Forma |
| --- | --- |
| `0` | Padrão do terminal |
| `1` / `2` | Bloco piscante / estável |
| `3` / `4` | Sublinhado piscante / estável |
| `5` / `6` | Barra piscante / estável |

### Tela alternativa, mouse e eventos de entrada

Estes modos privados são comuns em emuladores xterm e derivados, mas não fazem parte de um conjunto universal de recursos:

| Modo | Ativação/desativação | Uso |
| --- | --- | --- |
| `?47` | `CSI ? 47 h` / `CSI ? 47 l` | Alterna tela normal/alternativa em implementações antigas |
| `?1047` | `CSI ? 1047 h` / `CSI ? 1047 l` | Alterna tela alternativa |
| `?1048` | `CSI ? 1048 h` / `CSI ? 1048 l` | Salva/restaura cursor |
| `?1049` | `CSI ? 1049 h` / `CSI ? 1049 l` | Tela alternativa e salvamento/restauração do cursor |
| `?1000` | `CSI ? 1000 h` / `CSI ? 1000 l` | Eventos de botão do mouse |
| `?1002` | `CSI ? 1002 h` / `CSI ? 1002 l` | Movimento enquanto um botão está pressionado |
| `?1003` | `CSI ? 1003 h` / `CSI ? 1003 l` | Todo movimento do mouse |
| `?1004` | `CSI ? 1004 h` / `CSI ? 1004 l` | Eventos de foco da janela |
| `?1005` | `CSI ? 1005 h` / `CSI ? 1005 l` | Codificação UTF-8 antiga para mouse |
| `?1006` | `CSI ? 1006 h` / `CSI ? 1006 l` | Codificação SGR para mouse |
| `?1015` | `CSI ? 1015 h` / `CSI ? 1015 l` | Codificação de mouse estilo urxvt |
| `?2004` | `CSI ? 2004 h` / `CSI ? 2004 l` | Modo de colagem delimitada |
| `?2026` | `CSI ? 2026 h` / `CSI ? 2026 l` | Atualização sincronizada em emuladores compatíveis |

Com mouse SGR (`?1006`), eventos são enviados ao programa como:

```text
CSI < botão ; coluna ; linha M    pressionar/mover
CSI < botão ; coluna ; linha m    soltar
```

Coordenadas começam em 1. Os bits e valores de `botão` codificam botão, modificadores e, em alguns modos, movimento; a interpretação depende do modo de rastreamento habilitado. Esses comandos mudam o que o terminal envia à aplicação, portanto devem ser desligados ao sair.

O modo de colagem delimitada envolve o conteúdo colado com `CSI 200~` e `CSI 201~`, permitindo distingui-lo de teclas digitadas.

### Consultas e respostas

Uma consulta enviada ao terminal pode produzir uma resposta na entrada da aplicação. Não envie consultas sem um leitor preparado para separar essas respostas da digitação do usuário.

| Consulta | Resposta comum | Uso |
| --- | --- | --- |
| `CSI 5n` | `CSI 0n` | Estado do terminal |
| `CSI 6n` | `CSI <linha>;<coluna>R` | Posição do cursor (CPR/DSR) |
| `CSI c` ou `CSI 0c` | Varia, por exemplo `CSI ? ... c` | Atributos/identificação do dispositivo (DA) |
| `CSI > c` | Varia | Identificação secundária em alguns terminais |
| `CSI 14t` | Varia | Consulta de tamanho em pixels, extensão de emuladores |
| `CSI 16t` | Varia | Consulta de tamanho de célula em pixels, extensão |
| `CSI 18t` | Varia | Consulta de linhas/colunas, extensão |

Identificação e respostas variam por modelo, modo e emulador. Não use a resposta DA como inventário confiável de capacidades sem uma política de compatibilidade.

### Operações de janela e extensões de teclado

| Sequência | Efeito |
| --- | --- |
| `CSI 8;<linhas>;<colunas>t` | Solicita redimensionamento da janela; frequentemente ignorada ou bloqueada |
| `CSI 22;0t` / `CSI 23;0t` | Salva/restaura título da janela em emuladores compatíveis |
| `CSI > <flags> u` | Ativa recursos do protocolo de teclado Kitty em terminais compatíveis |
| `CSI <n> u` | Remove um nível/estado do protocolo de teclado Kitty |
| `CSI ? u` | Consulta o estado do protocolo de teclado Kitty |

O protocolo de teclado Kitty pode transmitir teclas com modificadores, códigos Unicode e tipos de evento (pressionar, soltar, repetir). Ele é uma extensão opt-in, não o formato de teclado universal.

## OSC — Operating System Command

Formato comum:

```text
OSC <número> ; <parâmetros> ST
```

`BEL` também encerra muitas OSC históricas, mas `ST` é a forma mais explícita. A aceitação de cada número é específica do terminal.

| OSC | Forma/exemplo | Uso e ressalvas |
| --- | --- | --- |
| `0` | `OSC 0 ; <título> ST` | Define título da janela e nome do ícone/aba |
| `1` | `OSC 1 ; <nome> ST` | Define nome do ícone/aba |
| `2` | `OSC 2 ; <título> ST` | Define título da janela |
| `4` | `OSC 4 ; <índice> ; <cor-ou-?> ST` | Define/consulta cor da paleta ANSI indexada; `?` solicita a cor atual |
| `7` | `OSC 7 ; file://<host>/<caminho> ST` | Informa diretório atual em terminais compatíveis |
| `8` | `OSC 8 ; <parâmetros> ; <URL> ST ... OSC 8 ; ; ST` | Abre/fecha hyperlink associado ao texto |
| `9` | `OSC 9 ; <mensagem> ST` | Notificação em alguns terminais; também usado por algumas extensões para progresso |
| `10` | `OSC 10 ; <cor> ST` | Define/consulta foreground padrão do terminal |
| `11` | `OSC 11 ; <cor> ST` | Define/consulta background padrão do terminal |
| `12` | `OSC 12 ; <cor> ST` | Define/consulta cor do cursor |
| `13–19` | `OSC <n> ; <cor> ST` | Cores dinâmicas adicionais em terminais xterm compatíveis |
| `52` | `OSC 52 ; <seletor> ; <base64> ST` | Acesso à área de transferência; pode ser bloqueado ou exigir confirmação |
| `99` | `OSC 99 ; <metadados> ; <conteúdo> ST` | Notificações em implementações específicas; o PTML também emite esta família |
| `104` | `OSC 104 ; <índice> ST` | Restaura cor(es) da paleta aos padrões |
| `110–119` | `OSC <n> ST` | Restaura cores dinâmicas 10–19 aos padrões em terminais compatíveis |
| `133` | `OSC 133 ; A/B/C/D ... ST` | Marcadores de prompt/comando/saída usados por integração de shell |
| `1337` | `OSC 1337 ; <comando iTerm2> ST` | Extensões iTerm2: imagens, arquivos, cursor, status, entre outras |
| `777` | `OSC 777 ; notify ; <título> ; <mensagem> ST` | Notificação em implementações rxvt-unicode compatíveis |

Para OSC 52, seletores frequentemente encontrados incluem `c` (clipboard), `p` (seleção primária) e `q` (seleção secundária). O protocolo pode permitir consulta da área de transferência, mas o suporte e as proteções variam. Não envie conteúdo não confiável como sequência de controle.

### OSC 8 — Hyperlinks

```text
ESC ] 8 ; id=<identificador> ; https://exemplo.com ESC \
Texto clicável
ESC ] 8 ; ; ESC \
```

Parâmetros podem estar vazios. Enviar URL vazia fecha o hyperlink. A URL não é aberta automaticamente por todos os terminais; geralmente o usuário precisa interagir com o texto.

### OSC 52 — Área de transferência

```text
ESC ] 52 ; c ; <texto-UTF-8-em-Base64> ESC \
```

É comum usar `c`, `p` e `q` como seletores de clipboard e seleções X11. Limites de tamanho, leitura versus escrita e confirmação do usuário variam significativamente entre terminais.

### OSC de notificações e recursos específicos

- **OSC 9:** implementações diferentes atribuem payloads diferentes. Por exemplo, iTerm2 documenta notificações simples e também uma forma própria de progresso `OSC 9;4;...`; não trate todos os payloads OSC 9 como intercambiáveis.
- **OSC 99:** há protocolos de notificação específicos de implementação. O módulo [Notification.fs](../src/components/elements/abstracts/Notification.fs) do PTML emite um formato com campos como `i`, `f`, `u`, `s`, `p`, `d` e `e`; isso não garante que um terminal arbitrário vá exibir a notificação.
- **OSC 1337:** família proprietária do iTerm2. O subcomando `File=...` pode incorporar conteúdo/imagens; o protocolo não é uma capacidade universal.
- **OSC 777:** convenção de notificação do rxvt-unicode, não um comando padronizado em todos os terminais.

## DCS — Device Control String

Formato:

```text
DCS <parâmetros e conteúdo> ST
```

DCS transporta comandos mais complexos em uma string terminada por `ST`.

| Forma | Uso |
| --- | --- |
| `DCS $ q <consulta> ST` | DECRQSS: consulta alguns estados/configurações em terminais VT/xterm compatíveis |
| `DCS + q <nome-em-hex> ST` | XTGETTCAP: consulta capacidades terminfo/termcap em xterm |
| `DCS <parâmetros> q <dados Sixel> ST` | Gráficos Sixel em terminais que implementam o protocolo |
| `ESC P tmux; <sequência encapsulada> ESC \` | Pass-through de controles por versões/configurações compatíveis do tmux; o conteúdo ESC precisa ser escapado conforme o protocolo do multiplexer |
| `DCS ... ST` | Outros comandos DEC, de impressão, estado ou extensões do emulador |

Sixel é um protocolo gráfico legado que codifica pixels em uma string DCS. Não confundir com imagens em protocolos proprietários de OSC/APC. O conteúdo de DCS é específico da família do protocolo e não deve ser interpretado como texto normal.

## APC — Application Program Command

APC é introduzido por `ESC _` e encerrado por `ST`. Em geral, emuladores que não reconhecem um protocolo APC o ignoram.

| Forma | Protocolo/uso |
| --- | --- |
| `ESC _ G <parâmetros> ; <payload> ESC \` | Protocolo de gráficos Kitty; payload de imagem normalmente codificado em Base64 |
| `ESC _ ... ESC \` | Outros comandos de aplicação, dependentes do emulador |

O protocolo de gráficos Kitty pode transmitir imagens em blocos, posicioná-las e atualizar/removê-las. Ele não é suportado universalmente e a sequência completa é mais complexa do que o exemplo resumido.

## SOS e PM

| Família | Forma | Observação |
| --- | --- | --- |
| SOS | `ESC X ... ST` | String de controle histórica; raramente usada por aplicações comuns |
| PM | `ESC ^ ... ST` | Privacy Message; historicamente reservado/privado |

Não há um conjunto amplamente interoperável de comandos de terminal para essas famílias. A presença de um introdutor não significa que o emulador execute ou exponha seu conteúdo.

## Caracteres de teclado e respostas do terminal

As sequências também circulam no sentido terminal → aplicação:

| Entrada | Forma comum |
| --- | --- |
| Setas em modo normal | `ESC [ A`, `ESC [ B`, `ESC [ C`, `ESC [ D` |
| Setas em modo de aplicação | `ESC O A`, `ESC O B`, `ESC O C`, `ESC O D` |
| F1–F4 em terminais VT comuns | frequentemente `ESC O P` a `ESC O S`; pode variar |
| Teclas especiais/combinações | Varia por terminal, modo, `TERM` e terminfo |
| Mouse | `CSI < ... M/m` quando um modo de mouse compatível está habilitado |
| Colagem delimitada | `CSI 200~` + conteúdo + `CSI 201~` |
| Foco | `CSI I` ao ganhar foco e `CSI O` ao perder foco, em terminais com modo 1004 |
| Resposta de consulta | Por exemplo, `CSI <linha>;<coluna>R` após DSR 6 |

Uma leitura ingênua de bytes pode confundir a tecla Esc isolada com o início de uma sequência, e também pode interpretar respostas do terminal como teclas. Um leitor de TUI precisa considerar timeout/estado de parsing, codificação UTF-8 e os modos ativos.

## Compatibilidade e cuidados

1. **Não existe suporte uniforme:** emulador, sistema, multiplexer, conexão SSH e configuração mudam o conjunto disponível. `TERM`/terminfo também afetam o que aplicações tradicionais emitem.
2. **Prefira formas de 7 bits:** por exemplo, `ESC [` em vez do byte C1 CSI `0x9B`, especialmente em UTF-8.
3. **Sempre restaure o estado:** ao sair, desligue modos de mouse, foco, colagem delimitada e tela alternativa que foram habilitados; restaure cursor, atributos e título quando aplicável.
4. **Não assuma efeitos inofensivos:** OSC pode mudar título/paleta, copiar para clipboard, abrir notificações ou incorporar conteúdo; `RIS` reinicializa o terminal.
5. **Não escreva escape sequences vindas de entrada não confiável:** texto que contenha sequências pode controlar o terminal do usuário. Faça escape/sanitização para logs e saída de dados externos.
6. **Teste a capacidade, não apenas o nome do terminal:** respostas a consultas também variam e algumas consultas são desabilitadas por segurança.

## Sequências identificadas na implementação do PTML

Esta tabela registra o que o código do repositório emite; não afirma suporte idêntico em todos os terminais.

| Família | Sequências observadas | Referência no código |
| --- | --- | --- |
| SGR | Estilos e cores de texto; encerramento com `CSI 0m` | [FontCode.fs](../src/attrs/FontCode.fs), [Output.fs](../src/Output.fs) |
| CSI | Forma do cursor (`CSI ... SP q`) e visibilidade (`CSI ? 25 h/l`) | [FontCode.fs](../src/attrs/FontCode.fs) |
| OSC 1 e 2 | Nome do ícone/aba e título da janela | [OSC.fs](../src/cmd/OSC.fs) |
| OSC 8 | Hiperlinks | [OSC.fs](../src/cmd/OSC.fs), [Output.fs](../src/Output.fs) |
| OSC 10 e 11 | Foreground/background padrão do terminal | [OSC.fs](../src/cmd/OSC.fs) |
| OSC 12 | Cor do cursor | [OSC.fs](../src/cmd/OSC.fs) |
| OSC 52 | Escrita em clipboard/seleções com conteúdo Base64 | [OSC.fs](../src/cmd/OSC.fs) |
| OSC 99 | Notificações com metadados e payload | [Notification.fs](../src/components/elements/abstracts/Notification.fs) |

As famílias de controle listadas neste documento que não aparecem nessa tabela são referências gerais de terminal, não recursos declarados como implementados pelo PTML.

## Referências

- [XTerm Control Sequences](https://invisible-island.net/xterm/ctlseqs/ctlseqs.html) — ECMA-48, VT e extensões xterm.
- [WezTerm Escape Sequences](https://wezterm.org/escape-sequences.html) — referência de implementação com famílias C0, CSI, OSC e DCS.
- [Console Virtual Terminal Sequences (Microsoft)](https://learn.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences) — CSI, SGR, modos e sequências aceitas pelo console Windows.
- [iTerm2 Proprietary Escape Codes](https://iterm2.com/documentation-escape-codes.html) — OSC 8, 9, 133, 1337 e extensões iTerm2.
- [urxvt Reference](https://manpages.debian.org/unstable/rxvt-unicode/urxvt.7.en.html) — referência e extensões rxvt-unicode.
- [Kitty Graphics Protocol](https://sw.kovidgoyal.net/kitty/graphics-protocol/) — protocolo gráfico sobre APC.
- [Kitty Keyboard Protocol](https://sw.kovidgoyal.net/kitty/keyboard-protocol/) — protocolo de teclado estendido.
