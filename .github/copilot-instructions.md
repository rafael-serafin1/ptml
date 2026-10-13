# Copilot Instructions — PTML

Estas instruções definem como o GitHub Copilot deve trabalhar neste repositório. O objetivo é contribuir para o PTML de forma consistente com a arquitetura, a sintaxe e a filosofia do projeto, sem inventar comportamentos que não estejam documentados ou confirmados.

## 1. Objetivo do projeto

PTML significa **Portable Terminal Markup Language**. É uma linguagem de marcação declarativa, baseada em sintaxe semelhante a XML, para construir interfaces de terminal estruturadas, legíveis e expressivas.

O PTML procura oferecer uma experiência conceitualmente semelhante à combinação de HTML + CSS para interfaces gráficas, mas voltada a interfaces de terminal. A linguagem abstrai tarefas como:

- Manipulação manual de ANSI Escape Codes.
- Posicionamento de conteúdo caractere por caractere.
- Gerenciamento procedural de layouts.
- Reutilização e manutenção de estruturas visuais.

O PTML é uma **camada declarativa para interfaces de terminal**. Não tem como objetivo substituir linguagens de programação. O projeto utiliza F# para integração e implementação, conforme o código existente no repositório.

## 2. Regra fundamental: não inventar especificações

**Quando houver dúvida sobre a sintaxe, a semântica, o comportamento esperado, a arquitetura ou qualquer conceito específico do PTML, pergunte ao mantenedor antes de implementar ou documentar uma suposição.**

Não deduza que um recurso funciona de determinada maneira apenas porque o nome parece familiar ou porque HTML, CSS, XML, React, curses ou outra tecnologia funciona assim.

Em particular:

1. Não invente atributos, tags, valores aceitos, regras de validação ou comportamentos.
2. Não presuma que uma tag do PTML se comporta como uma tag HTML de nome semelhante.
3. Não decida silenciosamente a semântica de recursos incompletos, planejados ou que funcionam incorretamente.
4. Não transforme uma ideia planejada em funcionalidade implementada sem confirmação explícita.
5. Não documente como funcional um recurso apenas porque há código relacionado a ele.
6. Se os arquivos do repositório e este documento forem insuficientes ou contraditórios, pare e faça uma pergunta objetiva.
7. Ao fazer uma pergunta, explique brevemente qual decisão técnica depende da resposta e apresente opções somente quando forem genuinamente plausíveis. Não apresente uma opção como fato estabelecido.
8. Se for possível continuar com uma parte independente e bem definida da tarefa, faça essa parte e deixe explícita a decisão pendente. Não implemente a parte ambígua.

Antes de perguntar, consulte o código, os testes, os exemplos e a documentação existentes para verificar se a resposta já está estabelecida. Não pergunte ao mantenedor algo que o próprio repositório responde claramente.

## 3. Fontes de verdade e resolução de conflitos

Use esta ordem prática para entender o comportamento atual:

1. **Código-fonte e testes existentes**, para identificar o que está realmente implementado.
2. **Documentação do repositório**, incluindo este arquivo e o README, para identificar a intenção e as regras declaradas.
3. **Decisões explícitas do mantenedor** na conversa ou em documentos de projeto.
4. Conhecimento geral de engenharia de software, apenas para decisões genéricas que não definam a linguagem.

Essa ordem não autoriza alterar a especificação com base apenas no comportamento atual do código. O código pode conter bugs. Quando implementação e documentação divergirem, não escolha silenciosamente qual delas deve prevalecer: identifique a divergência e pergunte se a correção envolver mudança de comportamento ou de especificação.

Não trate padrões de outras linguagens como especificação do PTML. Use-os apenas como referência técnica quando isso não alterar a semântica do projeto.

## 4. Categorias de elementos PTML

A documentação do projeto separa os elementos em duas categorias.

### 4.1 Elementos concretos

Elementos que renderizam conteúdo visual no terminal. Exemplos documentados:

- `<text>`
- `<frag>`
- `<box>`
- `<block>`
- `<hr>`
- `<spinner>`

O projeto também menciona barra de progresso e frames como recursos que estão funcionando.

Não deduza detalhes de renderização, atributos opcionais, valores padrão, tratamento de filhos ou regras de dimensionamento além do que o código, os testes ou o mantenedor confirmarem.

### 4.2 Elementos abstratos

Elementos que controlam o layout ou o fluxo sem necessariamente renderizar conteúdo por conta própria. Exemplos documentados:

- `<row>`
- `<column>`
- `<layer>`
- `<cell>`
- `<terminal>`

A classificação acima é conceitual. Para cada elemento, confirme no código quais responsabilidades ele realmente tem e quais limitações existem.

## 5. Sintaxe e recursos conhecidos

Os exemplos a seguir aparecem na documentação do PTML e podem ser usados como referências iniciais. Eles não autorizam inventar atributos ou comportamentos adicionais.

### 5.1 Texto

```ptml
<text>Hello World</text>
```

Atributo de cor exemplificado:

```ptml
<text foreground="red">
    Hello World
</text>
```

Estilo de fonte exemplificado:

```ptml
<text foreground="limegreen" font="bold">
    Sistema Operacional
</text>
```

### 5.2 Layout horizontal

```ptml
<row gap="1">
    <text>A</text>
    <text>B</text>
</row>
```

O exemplo documentado produz `A B`.

### 5.3 Layout vertical

```ptml
<column>
    <text>Item 1</text>
    <text>Item 2</text>
</column>
```

### 5.4 Containers e blocos

```ptml
<box>
    <text>Hello World</text>
</box>
```

```ptml
<block title="Status">
    <text>Servidor Online</text>
</block>
```

O README ilustra um bloco com borda e título, mas os valores aceitos para `border` e o comportamento completo do componente devem ser confirmados no código ou com o mantenedor.

### 5.5 Snippets reutilizáveis

```ptml
<snippet id="warning">
    foreground="red"
    font="bold"
</snippet>

<text snippet="warning">
    Erro crítico
</text>
```

Não presuma como snippets são resolvidos, se podem ser aninhados, como conflitos são tratados ou se atributos locais sobrescrevem os do snippet. Confirme a implementação antes de alterar esse sistema.

### 5.6 Diretiva de documento e terminal

Exemplo documentado:

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

Não deduza que todos os atributos desse exemplo já estão implementados ou que os únicos valores válidos são os mostrados. Consulte o parser, o renderizador e os testes.

### 5.7 Interatividade e integração

O projeto prevê os elementos:

```ptml
<input />
<output />
<code />
```

A documentação também menciona integração com scripts F#. Esses recursos não devem ser tratados como totalmente implementados só porque estão planejados ou aparecem em documentação. Verifique o estado atual antes de utilizá-los, ampliá-los ou anunciá-los como funcionais.

## 6. Estilização

Os recursos de estilo descritos no README incluem:

- Cores de foreground.
- Backgrounds.
- Fontes ANSI.
- Negrito.
- Itálico.
- Sublinhado.
- Blink.
- Tachado (strike-through).

O README exemplifica `foreground="red"`, `foreground="limegreen"` e `font="bold"`.

Não invente a lista de nomes de cores, a gramática de valores de estilo, a forma de combinar fontes ou a precedência entre estilos. Procure definições e testes no repositório. Se não houver uma definição suficiente, pergunte ao mantenedor.

## 7. Estado conhecido do projeto

O README informa o seguinte estado, que pode ficar desatualizado:

- **Funcionando:** spinners, barra de progresso e frames.
- **Funcionando de forma incorreta:** Depth Layer.
- **Planejados:** listas, inputs avançados, graphs, trees, output binding e execução integrada de código.
- **Em construção, não funcionando ou ainda sem classificação detalhada:** outros recursos.

Essas informações são um retrato documentado, não uma garantia do estado atual. Inspecione o código e os testes antes de declarar o status de um recurso.

Não afirme que `layer` funciona corretamente sem verificar. Como o README registra problemas em Depth Layer, não faça uma refatoração que altere seu comportamento sem entender a implementação e os testes existentes.

Ao atualizar o README ou outros documentos de status, diferencie claramente:

- Implementado e testado.
- Implementado, mas com falhas conhecidas.
- Em desenvolvimento.
- Planejado.
- Não implementado.

Não mova um recurso de uma categoria para outra sem evidências no código/testes ou confirmação do mantenedor.

## 8. Arquitetura e exploração do repositório

Antes de implementar uma funcionalidade:

1. Inspecione a estrutura do repositório.
2. Localize o parser, a representação interna da árvore/documento, o sistema de layout, o renderizador de terminal, a integração com F# e os scripts de build, conforme existirem.
3. Descubra como os componentes são registrados e como atributos e filhos são processados.
4. Procure testes e exemplos relacionados.
5. Identifique a menor mudança que resolve o problema sem duplicar lógica existente.
6. Preserve a arquitetura atual, salvo se a tarefa pedir explicitamente uma mudança arquitetural.

Não presuma nomes de diretórios, nomes de classes, padrões de projeto ou APIs que não tenham sido encontrados no repositório. Não crie uma segunda implementação de uma funcionalidade sem verificar se já existe uma equivalente.

Se não for possível determinar onde uma responsabilidade pertence, pergunte antes de introduzir uma nova camada ou dependência.

## 9. Regras de implementação

- Priorize mudanças pequenas, coesas e fáceis de revisar.
- Siga os padrões de estilo e nomenclatura já utilizados no projeto.
- Reutilize abstrações existentes quando elas forem adequadas.
- Evite duplicação de lógica de parsing, layout, estilo e renderização.
- Não adicione dependências externas sem necessidade concreta. Explique o custo e peça confirmação se a escolha afetar significativamente o projeto.
- Não introduza APIs públicas, tags, atributos ou convenções novas sem confirmar que fazem parte da especificação desejada.
- Não faça refatorações amplas e não relacionadas ao pedido.
- Não silencie exceções nem esconda erros de renderização para fazer uma funcionalidade parecer estável.
- Não use valores mágicos quando o projeto já tiver uma abstração apropriada para eles.
- Trate entradas inválidas de acordo com o padrão de erro já adotado no projeto; se esse padrão não existir ou a decisão for específica da linguagem, pergunte.
- Preserve compatibilidade com os exemplos existentes, exceto quando a tarefa exigir explicitamente uma mudança incompatível.
- Não prometa portabilidade entre sistemas operacionais sem verificar as diferenças relevantes de terminal, shell e sistema de build.

## 10. Layout e renderização de terminal

O PTML é destinado a terminais, portanto a renderização deve levar em conta limitações de texto e controle de cursor. Ao trabalhar nessa área:

- Inspecione o algoritmo de layout e o fluxo de renderização existente antes de alterá-los.
- Evite presumir que a largura visual de uma string é igual ao número de caracteres Unicode ou bytes.
- Considere sequências ANSI separadamente do texto visível, se isso for compatível com a arquitetura atual.
- Não introduza regras de largura Unicode, quebra de linha, alinhamento, clipping, redimensionamento ou bordas sem verificar o comportamento existente e esclarecer as lacunas da especificação.
- Não presuma que todos os terminais suportam os mesmos recursos ANSI.
- Não reescreva o sistema de renderização inteiro para resolver um defeito localizado.
- Ao corrigir um bug visual, inclua um caso reproduzível e verifique o resultado em diferentes dimensões de terminal quando isso for viável.

As recomendações acima são cuidados de implementação, não uma especificação definitiva sobre como o PTML deve tratar cada caso.

## 11. Animações, spinners, frames e barra de progresso

O README informa que spinners, barra de progresso e frames funcionam, enquanto alguns outros recursos ainda estão incompletos.

Antes de alterar esses mecanismos:

- Verifique como atualização, temporização, limpeza e restauração do terminal são feitas atualmente.
- Preserve o comportamento existente quando não fizer parte do problema relatado.
- Não introduza threads, tarefas assíncronas, temporizadores ou loops de atualização sem entender o ciclo de vida atual.
- Não afirme que animações estão corretas apenas porque a saída é atualizada.
- Se a semântica desejada de uma animação ou frame não estiver clara, pergunte ao mantenedor.

## 12. Scripts e comandos de build

O README descreve estes fluxos de uso.

### Executável

```sh
git clone "https://github.com/rafael-serafin1/ptml.git"
python build.py
ptml --version
```

Também são mencionados scripts sob `./shell`:

```powershell
.\build.ps1
```

```sh
.\build.sh
```

### Extensão do Visual Studio Code

```sh
python package.py "pack"
```

Também são mencionados scripts sob `./shell`:

```powershell
.\package.ps1 "pack"
```

```sh
.\package.sh "pack"
```

O pacote `.vsix` é descrito como sendo gerado dentro de `./syntax-highlighter`, para instalação manual no VS Code.

Antes de alterar esses fluxos, confirme os caminhos e o comportamento real dos scripts no repositório. Não presuma que comandos de Windows e Unix são equivalentes ou que o executável já está no `PATH` após o build sem verificar como o script funciona.

Não execute comandos destrutivos, publique releases, faça deploy ou altere configurações globais do usuário sem autorização explícita.

## 13. Extensão de sintaxe do VS Code

O repositório inclui uma extensão em desenvolvimento chamada syntax-highlighter, de acordo com o README.

Ao trabalhar nela:

- Inspecione a implementação e a configuração existentes antes de alterar a gramática.
- Mantenha a sintaxe destacada coerente com a sintaxe efetivamente suportada pelo PTML.
- Não destaque como recurso oficial uma tag ou atributo apenas porque ele parece plausível.
- Diferencie recursos experimentais, planejados e implementados quando a extensão puder induzir o usuário a acreditar que são suportados.
- Confirme como a extensão reconhece arquivos PTML e como seu pacote é montado antes de mudar esses mecanismos.

## 14. Testes e validação

Para cada alteração:

1. Identifique os testes relevantes e execute-os, quando disponíveis.
2. Adicione ou atualize testes para o comportamento alterado, seguindo o framework já usado pelo projeto.
3. Para bugs, procure criar primeiro um caso que reproduza a falha.
4. Verifique os casos normais, os limites e as entradas inválidas pertinentes à alteração.
5. Execute o build ou a validação mais próxima do fluxo real afetado.
6. Informe quais comandos foram executados e quais não puderam ser executados.

Não diga que os testes passaram se eles não foram executados. Não invente resultados. Se não houver testes para uma área, informe essa lacuna e proponha uma forma de validação adequada, sem criar um framework novo desnecessariamente.

## 15. Documentação e exemplos

A documentação deve refletir o comportamento confirmado do projeto.

- Use exemplos pequenos, válidos e consistentes com a implementação.
- Não documente tags, atributos, valores ou recursos hipotéticos como se fossem reais.
- Preserve a filosofia de linguagem declarativa para interfaces de terminal.
- Explique o que um recurso faz sem prometer que o PTML substitui linguagens de programação.
- Mantenha explícita a diferença entre o que já funciona, o que tem problemas, o que está em construção e o que é apenas planejado.
- Se a implementação divergir do README, reporte a divergência antes de escolher qual lado alterar.
- Evite afirmações vagas como “suporte completo” ou “totalmente portátil” sem evidência.

## 16. Fluxo de trabalho recomendado para o Copilot

Ao receber uma tarefa, siga esta sequência:

### Etapa A — Entender

- Reescreva mentalmente o objetivo técnico em termos concretos.
- Identifique os arquivos, componentes e comportamentos afetados.
- Consulte as implementações e testes relacionados.
- Separe fatos confirmados de hipóteses.

### Etapa B — Verificar ambiguidades

- Liste as decisões necessárias para implementar a tarefa.
- Resolva pelo repositório as decisões que já estiverem documentadas.
- Pergunte ao mantenedor sobre qualquer decisão específica do PTML que permaneça indefinida.
- Não preencha lacunas da linguagem com suposições.

### Etapa C — Planejar

- Proponha a menor solução coerente com a arquitetura existente.
- Identifique riscos de compatibilidade e áreas que podem ser afetadas.
- Não inclua refatorações sem relação direta com a tarefa.

### Etapa D — Implementar

- Faça alterações localizadas.
- Preserve APIs e comportamentos existentes, salvo decisão explícita em contrário.
- Adicione testes ou atualize os testes pertinentes.
- Atualize a documentação somente quando o comportamento estiver confirmado.

### Etapa E — Validar e relatar

- Execute testes e builds relevantes.
- Revise o diff para detectar alterações acidentais.
- Informe os arquivos modificados, o comportamento alterado, os testes executados e os problemas restantes.
- Declare explicitamente qualquer ponto que ainda dependa de uma decisão do mantenedor.

## 17. Formato esperado para respostas do Copilot

Ao propor ou concluir uma mudança não trivial, responda de forma objetiva com:

1. **Entendimento:** o que a tarefa exige.
2. **Evidências:** quais arquivos, implementações ou testes sustentam a solução.
3. **Decisões pendentes:** perguntas sobre especificações que não podem ser inferidas.
4. **Alterações:** o que foi mudado e por quê.
5. **Validação:** comandos executados e resultados reais.
6. **Limitações:** riscos, comportamentos não verificados e trabalho restante.

Para tarefas simples, não produza relatórios longos sem necessidade. Porém, nunca omita uma ambiguidade que possa levar a uma especificação inventada ou a uma mudança incompatível.

## 18. Critério de sucesso

Uma contribuição é adequada quando:

- Resolve o problema solicitado.
- Respeita a arquitetura e os padrões existentes.
- Não inventa a especificação do PTML.
- Não transforma recursos planejados em recursos supostamente implementados.
- Tem validação proporcional ao risco da mudança.
- Mantém a documentação honesta e alinhada ao comportamento confirmado.
- Pergunta ao mantenedor quando uma decisão específica do PTML não puder ser estabelecida com segurança.

**Regra final: é preferível interromper uma implementação ambígua e fazer uma pergunta precisa a produzir uma solução aparentemente completa baseada em uma suposição.**
