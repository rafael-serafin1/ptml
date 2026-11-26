export const navigation = [
  {
    label: "Primeiros passos",
    items: [
      { title: "Introdução", path: "/docs/introducao" },
      { title: "Sintaxe", path: "/docs/sintaxe" },
      { title: "Estilos", path: "/docs/estilos" },
    ],
  },
  {
    label: "Elementos",
    items: [
      { title: "<text>", path: "/docs/tags/text" },
      { title: "<frag>", path: "/docs/tags/frag" },
      { title: "<row>", path: "/docs/tags/row" },
      { title: "<column>", path: "/docs/tags/column" },
      { title: "<box>", path: "/docs/tags/box" },
      { title: "<block>", path: "/docs/tags/block" },
      { title: "<hr>", path: "/docs/tags/hr" },
      { title: "<escape>", path: "/docs/tags/escape" },
      { title: "<cursor>", path: "/docs/tags/cursor" },
      { title: "<char>", path: "/docs/tags/char" },
      { title: "<spinner>", path: "/docs/tags/spinner" },
      { title: "<list>", path: "/docs/tags/list" },
      { title: "<terminal>", path: "/docs/tags/terminal" },
    ],
  },
];

export const pages = {
  "/docs/introducao": {
    title: "Introdução",
    eyebrow: "Primeiros passos",
    description:
      "Conheça a Portable Terminal Markup Language e execute seu primeiro documento.",
    sections: [
      {
        title: "O que é PTML?",
        text:
          "PTML (Portable Terminal Markup Language) é uma linguagem declarativa para estruturar interfaces de terminal. Ela descreve conteúdo e organização; o executável PTML interpreta o documento e o apresenta no terminal.",
        examples: [
          {
            title: "Seu primeiro texto",
            code: `<text>Olá, terminal!</text>`,
          },
          {
            title: "Organize linhas em uma coluna",
            code: `<column gap="1">
  <text>Primeira linha</text>
  <text>Segunda linha</text>
</column>`,
          },
        ],
      },
      {
        title: "Executar um arquivo",
        text:
          "O comando documentado pelo executável é `ptml run <CAMINHO>`. Consulte as instruções de instalação do repositório para preparar o executável antes de usar.",
        examples: [
          {
            title: "No terminal",
            code: `ptml run exemplo.ptml`,
          },
        ],
      },
      {
        title: "Comece pelo essencial",
        text:
          "Aprenda primeiro a exibir texto, agrupar elementos em linhas e colunas e adicionar contêineres. Os exemplos desta documentação usam apenas elementos confirmados no parser e no renderizador.",
        links: [
          { title: "Entender a sintaxe", path: "/docs/sintaxe" },
          { title: "Ver estilos de texto", path: "/docs/estilos" },
          { title: "Conhecer os elementos", path: "/docs/tags/text" },
        ],
      },
    ],
  },
  "/docs/sintaxe": {
    title: "Sintaxe",
    eyebrow: "Primeiros passos",
    description:
      "Entenda a estrutura dos elementos, atributos e conteúdo de um arquivo PTML.",
    sections: [
      {
        title: "Elementos e conteúdo",
        text:
          "Os elementos usam marcação semelhante a XML: uma tag de abertura, conteúdo e uma tag de fechamento. O parser verifica as tags e os atributos reconhecidos.",
        examples: [
          {
            title: "Elemento com conteúdo",
            code: `<text>Conteúdo textual</text>`,
          },
          {
            title: "Elemento vazio",
            code: `<hr />`,
          },
        ],
      },
      {
        title: "Atributos",
        text:
          "Atributos são escritos na tag de abertura como pares `nome=\"valor\"`. Os nomes e valores aceitos dependem de cada elemento; um atributo não reconhecido pode causar erro de validação.",
        examples: [
          {
            title: "Atributo de layout",
            code: `<row gap="2">
  <text>Esquerda</text>
  <text>Direita</text>
</row>`,
          },
          {
            title: "Atributo obrigatório em block",
            code: `<block title="Status">
  <text>Disponível</text>
</block>`,
          },
        ],
      },
      {
        title: "Composição",
        text:
          "Contêineres podem agrupar outros elementos. Dentro de `<text>`, `<frag>` é permitido como trecho inline e não pode conter outro `<frag>`.",
        examples: [
          {
            title: "Composição de layout",
            code: `<column gap="1">
  <text>Resumo</text>
  <hr />
  <text>Detalhes</text>
</column>`,
          },
          {
            title: "Trecho dentro de texto",
            code: `<text>Estado: <frag foreground="green">ativo</frag></text>`,
          },
        ],
      },
    ],
  },
  "/docs/estilos": {
    title: "Estilos",
    eyebrow: "Primeiros passos",
    description:
      "Aplique cor, destaque e links ao texto usando os atributos implementados.",
    sections: [
      {
        title: "Cor do texto",
        text:
          "Use `foreground` para definir a cor do texto e `background` para o fundo. O projeto reconhece nomes de cores e valores hexadecimais; estes exemplos usam valores presentes na implementação.",
        examples: [
          {
            title: "Cores nomeadas",
            code: `<text foreground="limegreen">Tudo certo</text>
<text foreground="red">Atenção</text>`,
          },
          {
            title: "Cor hexadecimal",
            code: `<text foreground="#837878">Texto personalizado</text>`,
          },
          {
            title: "Cor de fundo",
            code: `<text foreground="white" background="blue"> Destaque </text>`,
          },
        ],
      },
      {
        title: "Fonte ANSI",
        text:
          "O atributo `font` aceita um estilo ANSI por elemento. Os exemplos abaixo correspondem a valores reconhecidos pelo parser.",
        examples: [
          {
            title: "Negrito e itálico",
            code: `<text font="bold">Importante</text>
<text font="italic">Observação</text>`,
          },
          {
            title: "Sublinhado e tachado",
            code: `<text font="underline">Link em destaque</text>
<text font="strike-through">Indisponível</text>`,
          },
        ],
      },
      {
        title: "Estilo em trechos",
        text:
          "Use `<frag>` para aplicar atributos a uma parte do conteúdo de `<text>`. A formatação do elemento pai é herdada quando o fragmento não define o mesmo atributo.",
        examples: [
          {
            title: "Destaque inline",
            code: `<text>Build: <frag foreground="limegreen" font="bold">concluído</frag></text>`,
          },
          {
            title: "Link em texto",
            code: `<text>Leia a <frag url="https://github.com/rafael-serafin1/ptml">documentação do projeto</frag>.</text>`,
          },
        ],
      },
    ],
  },
  "/docs/tags/text": {
    title: "<text>",
    eyebrow: "Elemento concreto",
    description:
      "Renderiza conteúdo textual e aceita atributos de estilo no texto.",
    sections: [
      {
        title: "Uso básico",
        text:
          "Use `<text>` para exibir uma mensagem. A implementação combina trechos adjacentes que compartilham os mesmos atributos de estilo.",
        examples: [
          { title: "Texto simples", code: `<text>Olá, mundo!</text>` },
          {
            title: "Conteúdo em várias linhas de marcação",
            code: `<text>
  Interface iniciada
</text>`,
          },
          {
            title: "Dentro de uma coluna",
            code: `<column gap="1">
  <text>CPU: 73%</text>
  <text>RAM: 41%</text>
</column>`,
          },
        ],
      },
      {
        title: "Atributos de texto",
        text:
          "A implementação declara `foreground`, `background`, `font` e `url` para `<text>`. Um fragmento pode definir atributos próprios dentro do texto.",
        examples: [
          {
            title: "Cor e fonte",
            code: `<text foreground="limegreen" font="bold">Servidor online</text>`,
          },
          {
            title: "Cor de fundo",
            code: `<text foreground="white" background="blue"> Aviso </text>`,
          },
          {
            title: "Link no texto",
            code: `<text url="https://github.com/rafael-serafin1/ptml">Abrir repositório</text>`,
          },
        ],
      },
    ],
  },
  "/docs/tags/frag": {
    title: "<frag>",
    eyebrow: "Elemento inline",
    description:
      "Aplica estilo a um trecho de texto sem criar um elemento de layout separado.",
    sections: [
      {
        title: "Contexto permitido",
        text:
          "`<frag>` só pode aparecer dentro de `<text>`. Ele não pode conter outro `<frag>`; seu conteúdo é texto inline.",
        examples: [
          {
            title: "Realce de uma palavra",
            code: `<text>Estado: <frag foreground="green">ativo</frag></text>`,
          },
          {
            title: "Mais de um trecho",
            code: `<text><frag font="bold">CPU</frag>: 73% — <frag font="bold">RAM</frag>: 41%</text>`,
          },
        ],
      },
      {
        title: "Atributos e herança",
        text:
          "`foreground`, `background`, `font` e `url` são atributos disponíveis. O valor definido no `<frag>` tem precedência sobre o mesmo atributo herdado de `<text>`.",
        examples: [
          {
            title: "Estilo herdado do elemento pai",
            code: `<text foreground="limegreen">
  Servidor <frag font="bold">online</frag>
</text>`,
          },
          {
            title: "Estilo local sobre o pai",
            code: `<text foreground="white">
  Estado: <frag foreground="green" font="bold">ativo</frag>
</text>`,
          },
          {
            title: "URL no trecho",
            code: `<text>Visite <frag url="https://github.com/rafael-serafin1/ptml">o projeto</frag>.</text>`,
          },
        ],
      },
    ],
  },
  "/docs/tags/row": {
    title: "<row>",
    eyebrow: "Elemento de layout",
    description:
      "Agrupa elementos em um contêiner de fluxo horizontal.",
    sections: [
      {
        title: "Organização horizontal",
        text:
          "Os filhos de `<row>` são organizados lado a lado. Use `gap` para definir o espaço entre eles.",
        examples: [
          {
            title: "Dois elementos com espaço",
            code: `<row gap="1">
  <text>Esquerda</text>
  <text>Direita</text>
</row>`,
          },
          {
            title: "Métricas em uma linha",
            code: `<row gap="2">
  <text>CPU: 73%</text>
  <text>RAM: 41%</text>
</row>`,
          },
          {
            title: "Layouts aninhados",
            code: `<row gap="2">
  <column>
    <text>Servidor</text>
    <text>Online</text>
  </column>
  <column>
    <text>Região</text>
    <text>Local</text>
  </column>
</row>`,
          },
        ],
      },
      {
        title: "Atributos implementados",
        text:
          "O parser reconhece `gap` (inteiro), `overflow` (`break`, `wrap`, `cut` ou `clip`) e `align` (`start`, `center` ou `end`).",
        examples: [
          {
            title: "Alinhamento",
            code: `<row gap="1" align="center">
  <text>Item A</text>
  <text>Item B</text>
</row>`,
          },
        ],
      },
    ],
  },
  "/docs/tags/column": {
    title: "<column>",
    eyebrow: "Elemento de layout",
    description:
      "Agrupa elementos em um contêiner de fluxo vertical.",
    sections: [
      {
        title: "Organização vertical",
        text:
          "Os filhos de `<column>` são organizados um abaixo do outro. Use `gap` para definir o espaço vertical entre os itens.",
        examples: [
          {
            title: "Lista de linhas",
            code: `<column gap="1">
  <text>Primeira linha</text>
  <text>Segunda linha</text>
  <text>Terceira linha</text>
</column>`,
          },
          {
            title: "Seção com separador",
            code: `<column gap="1">
  <text>Resumo do sistema</text>
  <hr />
  <text>CPU: 73%</text>
  <text>RAM: 41%</text>
</column>`,
          },
          {
            title: "Composição com contêiner",
            code: `<column gap="1">
  <block title="Status">
    <text foreground="limegreen">Online</text>
  </block>
  <text>Última atualização: agora</text>
</column>`,
          },
        ],
      },
      {
        title: "Atributos implementados",
        text:
          "O parser reconhece `gap` (inteiro), `overflow` (`break`, `wrap`, `cut` ou `clip`) e `y-align` (`start`, `center` ou `end`).",
        examples: [
          {
            title: "Alinhamento vertical",
            code: `<column gap="1" y-align="center">
  <text>Conteúdo centralizado</text>
</column>`,
          },
        ],
      },
    ],
  },
  "/docs/tags/box": {
    title: "<box>",
    eyebrow: "Elemento concreto",
    description:
      "Agrupa conteúdo em um contêiner com borda configurável.",
    sections: [
      {
        title: "Contêiner com borda",
        text:
          "Por padrão, `<box>` usa uma borda simples. Os filhos podem ser elementos de conteúdo e de layout.",
        examples: [
          {
            title: "Borda padrão",
            code: `<box>
  <text>Conteúdo dentro da caixa</text>
</box>`,
          },
          {
            title: "Com título usando block",
            code: `<box border="double">
  <column gap="1">
    <text>Nome: PTML</text>
    <text>Versão: 0.5.0</text>
  </column>
</box>`,
          },
          {
            title: "Espaçamento interno",
            code: `<box padding="1">
  <text>Conteúdo com padding</text>
</box>`,
          },
        ],
      },
      {
        title: "Atributos implementados",
        text:
          "O parser reconhece `border`, `border-color`, `width`, `height`, `overflow`, `align` e `padding`. O padding aceita um inteiro ou dois inteiros separados por `x` (vertical x horizontal).",
        examples: [
          {
            title: "Borda e cor",
            code: `<box border="rounded" border-color="cyan" padding="1x2">
  <text>Contêiner configurado</text>
</box>`,
          },
          {
            title: "Alinhamento do conteúdo",
            code: `<box width="30" align="center">
  <text>Centralizado</text>
</box>`,
          },
        ],
      },
    ],
  },
  "/docs/tags/block": {
    title: "<block>",
    eyebrow: "Elemento concreto",
    description:
      "Renderiza um contêiner com título e borda.",
    sections: [
      {
        title: "Título obrigatório",
        text:
          "Todo `<block>` precisa do atributo `title`. A borda é simples por padrão; conteúdo e layouts podem ser colocados dentro do elemento.",
        examples: [
          {
            title: "Bloco básico",
            code: `<block title="Status">
  <text>Servidor online</text>
</block>`,
          },
          {
            title: "Informações em coluna",
            code: `<block title="Sistema">
  <column gap="1">
    <text>CPU: 73%</text>
    <text>RAM: 41%</text>
  </column>
</block>`,
          },
          {
            title: "Borda arredondada",
            code: `<block title="Avisos" border="rounded" padding="1">
  <text foreground="gold">Nenhum alerta novo</text>
</block>`,
          },
        ],
      },
      {
        title: "Atributos implementados",
        text:
          "Além de `title`, o parser reconhece `border`, `border-color`, `width`, `height`, `overflow`, `align` e `padding`.",
        examples: [
          {
            title: "Cor e alinhamento",
            code: `<block title="Conexão" border="double" border-color="cyan" align="center">
  <text>Estável</text>
</block>`,
          },
        ],
      },
    ],
  },
  "/docs/tags/hr": {
    title: "<hr>",
    eyebrow: "Elemento concreto",
    description:
      "Desenha um separador horizontal ou vertical.",
    sections: [
      {
        title: "Orientação",
        text:
          "Sem atributos, `<hr />` usa orientação horizontal. Defina `orientation=\"vertical\"` para desenhar o separador na vertical.",
        examples: [
          {
            title: "Separador horizontal",
            code: `<column>
  <text>Resumo</text>
  <hr />
  <text>Detalhes</text>
</column>`,
          },
          {
            title: "Separador vertical",
            code: `<row gap="1">
  <text>Esquerda</text>
  <hr orientation="vertical" />
  <text>Direita</text>
</row>`,
          },
          {
            title: "Largura informada",
            code: `<hr width="24" />`,
          },
        ],
      },
      {
        title: "Atributos implementados",
        text:
          "O parser reconhece `orientation` (`horizontal` ou `vertical`), `width` e `height`. O efeito das dimensões depende da orientação e do espaço disponível no layout.",
        examples: [
          {
            title: "Separador horizontal com largura",
            code: `<hr orientation="horizontal" width="32" />`,
          },
        ],
      },
    ],
  },
  "/docs/tags/escape": {
    title: "<escape>",
    eyebrow: "Elemento de texto",
    description:
      "Insere caracteres de controle, como quebras de linha e tabulações, no conteúdo.",
    sections: [
      {
        title: "Sequências disponíveis",
        text:
          "O atributo `sequence` aceita `break` (padrão), `new-line`, `horizontal-tab`, `vertical-tab`, `audible-bell`, `backspace`, `form-feed` e `carriage-return`. `break` e `new-line` produzem uma quebra de linha.",
        examples: [
          {
            title: "Quebra de linha",
            code: `<text>Primeira linha<escape sequence="break" />Segunda linha</text>`,
          },
          {
            title: "Tabulação",
            code: `<text>Item:<escape sequence="horizontal-tab" />Terminal</text>`,
          },
        ],
      },
      {
        title: "Repetir uma sequência",
        text:
          "`multiplier` define quantas vezes a sequência será inserida. O padrão é `1`; o parser exige um inteiro positivo.",
        examples: [
          {
            title: "Duas quebras de linha",
            code: `<text>Antes<escape sequence="break" multiplier="2" />Depois</text>`,
          },
        ],
      },
    ],
  },
  "/docs/tags/cursor": {
    title: "<cursor>",
    eyebrow: "Elemento de terminal",
    description:
      "Define formato, cor, visibilidade e comportamento de piscar do cursor do terminal.",
    sections: [
      {
        title: "Formato e opções",
        text:
          "`shape` aceita `block`, `bar` ou `underline` (padrão). `blink` e `visible` aceitam `true` ou `false`; por padrão, o cursor não pisca e permanece visível.",
        examples: [
          {
            title: "Cursor em bloco piscante",
            code: `<cursor shape="block" blink="true" />`,
          },
          {
            title: "Ocultar o cursor",
            code: `<cursor visible="false" />`,
          },
        ],
      },
      {
        title: "Cor",
        text:
          "`color` define a cor do cursor. Os aliases reconhecidos são `red`, `yellow`, `white`, `black`, `teal`, `blue`, `pink`, `green` e `purple`; o padrão é branco. Também é possível informar uma cor hexadecimal.",
        examples: [
          {
            title: "Cor nomeada",
            code: `<cursor color="green" />`,
          },
          {
            title: "Cor hexadecimal",
            code: `<cursor color="#837878" />`,
          },
        ],
      },
    ],
  },
  "/docs/tags/char": {
    title: "<char>",
    eyebrow: "Elemento concreto",
    description:
      "Insere um caractere especial usando um nome (`codename`) reconhecido pelo parser.",
    sections: [
      {
        title: "Inserir um caractere",
        text:
          "`codename` é obrigatório. Os valores aceitos pelo parser são `copyright`, `rights-reserved`, `trade-mark`, `service-mark`, `phonogram`, `celsius`, `fahrenheit`, `number`, `section`, `nul`, `esc`, `del`, `infinite`, `integral` e `empty`.",
        examples: [
          {
            title: "Símbolo de copyright em texto",
            code: `<text>Copyright <char codename="copyright" /> — todos os direitos reservados.</text>`,
          },
          {
            title: "Vários caracteres especiais",
            code: `<row gap="1">
  <char codename="section" />
  <char codename="number" />
  <char codename="infinite" />
</row>`,
          },
        ],
      },
      {
        title: "Observação sobre o codename object",
        text:
          "A conversão interna contém o caractere `object`, mas esse valor não consta na lista permitida pelo parser. Portanto, `codename=\"object\"` é rejeitado atualmente.",
      },
    ],
  },
  "/docs/tags/spinner": {
    title: "<spinner>",
    eyebrow: "Elemento concreto",
    description:
      "Exibe uma animação de carregamento e um marcador quando ela termina.",
    sections: [
      {
        title: "Tipos de animação",
        text:
          "`type` escolhe a animação e, se omitido, usa `braille`. Os valores disponíveis são `braille`, `dots`, `waiting`, `burger`, `beam`, `ascii`, `circle`, `square`, `moon`, `arrow`, `bounce` e `fill`. O tipo `beam` tem uma falha de largura conhecida no primeiro quadro.",
        examples: [
          {
            title: "Spinner ASCII",
            code: `<spinner type="ascii" />`,
          },
          {
            title: "Spinner com duração e marcador de conclusão",
            code: `<spinner type="dots" interval="250ms" duration="3s" completed="check" />`,
          },
        ],
      },
      {
        title: "Intervalo, duração e conclusão",
        text:
          "`interval` define o tempo entre quadros e aceita `ms` ou `s`; o padrão é `250ms`. `duration` aceita `ms`, `s` ou `laps` (voltas completas) e usa `3000ms` por padrão. Ao terminar, o padrão é mostrar `✓`. Os aliases de `completed` são `check` (`✓`), `error` (`✖`), `star` (`✱`), `cog` (`⚙`) e `bright` (`✦`).",
        examples: [
          {
            title: "Duração por voltas",
            code: `<spinner type="circle" interval="200ms" duration="2laps" completed="bright" />`,
          },
        ],
      },
      {
        title: "Cores",
        text:
          "`foreground` e `background` definem as cores do spinner e usam os mesmos valores de cor documentados para `<text>`.",
        examples: [
          {
            title: "Spinner colorido",
            code: `<spinner type="dots" foreground="limegreen" background="black" />`,
          },
        ],
      },
    ],
  },
  "/docs/tags/list": {
    title: "<list>",
    eyebrow: "Elemento de layout",
    description:
      "Aplica marcadores aos itens e organiza-os no fluxo definido pelos elementos filhos.",
    sections: [
      {
        title: "Estilos de marcador",
        text:
          "`ofstyle` escolhe o marcador: `bullet` (padrão, ○), `unorder` (-), `enum` (1., 2., 3.), `lower-alphabet` (a., b., c.) ou `upper-alphabet` (A., B., C.). Os estilos alfabéticos continuam com letras adicionais após Z.",
        examples: [
          {
            title: "Lista numerada",
            code: `<list ofstyle="enum" after=" ">
  <column>
    <text>Preparar o arquivo</text>
    <text>Executar o programa</text>
  </column>
</list>`,
          },
        ],
      },
      {
        title: "Itens e texto ao redor do marcador",
        text:
          "Cada filho direto da lista representa um item; um `<row>` ou `<column>` pode agrupar esses itens e manter sua disposição. Sem esse contêiner, os itens são dispostos horizontalmente. `before` e `after` inserem texto antes e depois do marcador.",
        examples: [
          {
            title: "Prefixo e sufixo do marcador",
            code: `<list ofstyle="enum" before="Etapa " after=" ">
  <column>
    <text>Criar o documento</text>
    <text>Executar o documento</text>
  </column>
</list>`,
          },
          {
            title: "Itens em linha",
            code: `<list ofstyle="bullet" after=" ">
  <text>CPU</text>
  <text>Memória</text>
</list>`,
          },
        ],
      },
    ],
  },
  "/docs/tags/terminal": {
    title: "<terminal>",
    eyebrow: "Elemento de escopo",
    description:
      "Configura opções do terminal para o conteúdo da interface.",
    sections: [
      {
        title: "Conteúdo estruturado",
        text:
          "O elemento aplica opções ao terminal, como alinhamento, cores e títulos. Como `<terminal>` não organiza seus filhos em fluxo, use `<row>` ou `<column>` para estruturar conteúdo: filhos diretos partem da mesma origem.",
        examples: [
          {
            title: "Documento básico",
            code: `<terminal>
  <column gap="1">
    <text>Olá, terminal!</text>
    <text>Interface PTML</text>
  </column>
</terminal>`,
          },
          {
            title: "Alinhamento horizontal",
            code: `<terminal x-align="center">
  <column>
    <text>Conteúdo centralizado</text>
  </column>
</terminal>`,
          },
          {
            title: "Cores do terminal",
            code: `<terminal foreground="white" background="blue">
  <column>
    <text>Área de status</text>
  </column>
</terminal>`,
          },
        ],
      },
      {
        title: "Atributos implementados",
        text:
          "O parser reconhece `x-align` e `y-align` (`start`, `center` ou `end`), `foreground`, `background`, `window-title` e `icon-name`.",
        examples: [
          {
            title: "Títulos da janela e da aba",
            code: `<terminal window-title="Monitor PTML" icon-name="monitor">
  <column>
    <text>Aplicação iniciada</text>
  </column>
</terminal>`,
          },
        ],
      },
    ],
  },
};
