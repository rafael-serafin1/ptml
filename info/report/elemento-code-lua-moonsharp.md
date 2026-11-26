# Uma ideia para o `<code>`: Lua, MoonSharp e PTML dinâmico

Este relatório é uma proposta para ajudar a escolher um caminho. Não transforma nada em especificação oficial do PTML; a sintaxe e o comportamento final ainda precisam ser decididos.

## Minha recomendação, sem rodeios

Se a regra é permitir **uma linguagem só**, eu começaria com **Lua executado dentro do PTML pelo MoonSharp**.

Pense no MoonSharp como um interpretador Lua que roda dentro do próprio processo .NET. O PTML continua sendo feito em F#, mas pode chamar o interpretador e entregar a ele uma API pequena. Não é preciso reescrever o projeto em C#, nem iniciar um executável Lua separado.

O próprio projeto MoonSharp descreve sua implementação como aproximadamente **99% compatível com Lua 5.2**; por isso, seria mais honesto anunciar “Lua 5.2 via MoonSharp” do que prometer compatibilidade perfeita com qualquer Lua.

Eu escolheria Lua aqui não porque JavaScript seja ruim, mas porque o PTML não é uma página web. Usar JavaScript não cria automaticamente um DOM, nem faz o terminal entender alterações de HTML. Em qualquer linguagem, ainda seria necessário decidir como as mudanças do script entram no modelo e chegam ao renderizador. Lua + MoonSharp parece uma fronteira menor e mais controlável para um runtime embutido em .NET.

## O que o repositório já diz — e o que ainda não faz

O [brainstorm](../utilities/intel/brainstorm.md) marca o `<code>` como **“IGNORE POR ENQUANTO”**. Ele registra a ideia de aceitar conteúdo dentro da tag ou um caminho em `src`, e os valores `async` e `defer` para `execute-method`. Isso é uma intenção em discussão, não uma funcionalidade pronta.

O código atual confirma essa diferença:

- [Global.fs](../../src/attrs/Global.fs) aceita `src` e restringe `execute-method` a `async` ou `defer`.
- [Code.fs](../../src/components/elements/abstracts/Code.fs) tem apenas um tipo para representar esses dois modos.
- Em [Tree.fs](../../src/Tree.fs), o ramo de `code` lê os atributos e os filhos, mas retorna uma lista vazia de widgets.
- [ContentExecution.fs](../../src/components/psi/ContentExecution.fs) processa o atributo `content`; não executa scripts do `<code>`.

Então, hoje a tag é reconhecida em parte, mas **não há um motor de scripts conectado ao PTML**. Também não encontrei MoonSharp nem outra biblioteca de scripting nas dependências atuais.

Um detalhe importante: o `<bind>` descrito no brainstorm trata de relações entre entidades. Não é, por si só, uma especificação de ligação entre valores de script e conteúdo renderizado. As instruções do projeto também classificam output binding e execução integrada de código como planejados.

## Como o script poderia mudar o que aparece

Uma página web tem um objeto chamado DOM que o JavaScript pode alterar. O PTML, pelo código atual, segue outro caminho: os dados passam por etapas de árvore, layout, operações de desenho, buffer e saída no terminal.

Uma forma simples de pensar nisso é:

```text
script Lua -> API pequena do PTML -> estado da aplicação -> renderizador F# -> buffer -> terminal
```

Em vez de deixar o script editar texto PTML, ANSI ou células do buffer diretamente, o host poderia guardar um **estado** da aplicação. O script pediria uma alteração nesse estado; o PTML/F# usaria os novos valores para preparar a próxima renderização.

Por exemplo, imagine um contador. O script não precisaria apagar uma linha do terminal e escrever outra. Ele pediria ao host para mudar o valor do contador; o renderizador redesenharia a parte necessária. O script descreve *o que mudou*; o renderizador continua responsável por *como desenhar*.

Um esboço de chamada Lua poderia ser:

```lua
-- Apenas um exemplo de API possível; ainda não é sintaxe oficial do PTML.
ptml.set_state("contador", (ptml.get_state("contador") or 0) + 1)
```

`ptml.set_state` e `ptml.get_state` são nomes ilustrativos, não uma proposta fechada de API. O ponto é manter a escrita no terminal dentro do pipeline que já existe, em vez de criar um segundo renderizador escondido dentro do script.

Para a primeira versão, eu limitaria a mudança a **valores** usados pelo conteúdo. Permitir que scripts alterem a estrutura inteira da árvore — criar e remover elementos, por exemplo — é uma etapa maior, com mais regras de layout, atualização e validação.

## Como MoonSharp conversa com F#

F# também compila para tipos .NET. Por isso, o MoonSharp pode chamar objetos .NET expostos pelo programa mesmo quando a lógica principal está em F#. Não é necessário tornar o domínio PTML inteiro visível ao Lua.

O caminho mais seguro e fácil de manter é criar uma **fachada**: um objeto pequeno, com operações pensadas para scripts, implementado em F# ou C#. O host registra e entrega esse objeto ao Lua. Conceitualmente:

```fsharp
UserData.RegisterType<ScriptApi>()
script.Globals.Set("ptml", UserData.Create(api))
```

`ScriptApi` também é apenas um nome de exemplo. A fachada poderia oferecer poucas operações, como ler e atualizar valores autorizados e solicitar uma nova renderização.

Isso é melhor do que entregar ao script objetos internos como o buffer, a árvore de widgets, o `Console` ou o renderizador. Na documentação do MoonSharp, tipos registrados podem expor métodos, propriedades e campos públicos. A própria documentação recomenda uma camada de interface dedicada para limitar o acesso e deixar o modelo interno livre para mudar. Para essa situação, eu evitaria o registro automático de assemblies ou de todos os tipos do projeto.

Os tipos F# mais ricos — por exemplo, unions, records, `Option` e estruturas internas do renderizador — podem ser úteis dentro do host, mas não precisam fazer parte da API de Lua. Começar com strings, números, booleanos e tabelas simples deixa a ponte mais fácil de explicar e testar.

## Segurança: o script é código, não configuração

Um `<code>` executável tem riscos diferentes de um atributo normal. Um script pode fazer loops sem fim, consumir memória ou tentar acessar recursos que o PTML não deveria controlar.

O guia de sandbox do MoonSharp alerta que o conjunto padrão inclui APIs de sistema e de arquivos e não deve ser tratado como seguro para scripts sem confiança. Como ponto de partida, a documentação descreve o `Preset_HardSandbox`, que deixa de fora grupos como `io`, `os` e `file`; em seguida, o PTML ainda precisaria expor apenas sua própria API limitada. Esse preset não substitui uma revisão das capacidades expostas nem uma política de limites para tempo e memória.

Se `src` for mantido, o próprio host deve abrir o arquivo. É importante definir de onde o caminho é resolvido, restringi-lo a uma pasta permitida e rejeitar caminhos que escapem dela. Não seria uma boa ideia dar ao Lua acesso geral ao sistema de arquivos para que ele mesmo carregue qualquer arquivo.

Também vale decidir se o autor do PTML é sempre confiável. Se documentos baixados de terceiros puderem conter `<code>`, a exigência de isolamento é muito maior do que para scripts escritos e executados apenas pelo próprio usuário.

## `async` e `defer` ainda precisam de significado preciso

O brainstorm descreve `async` como assíncrono e `defer` como executado em paralelo depois que o PTML tiver sido processado. Isso ainda deixa perguntas práticas: o primeiro desenho na tela espera o script? O resultado pode chegar depois? O que acontece se duas atualizações chegarem juntas? “Processado” significa analisado, transformado em árvore ou já desenhado?

Uma possível distinção — **para discussão, não comportamento atual** — seria:

- `async`: mostrar a interface inicial e deixar o script trabalhar em segundo plano; atualizações voltam ao loop do PTML por uma fila segura.
- `defer`: esperar a análise da árvore terminar, executar o script de inicialização e só então fazer a primeira renderização.

Para um protótipo, eu não implementaria esses dois modos antes de fechar essa diferença. Um modo inicial simples e previsível, executado antes da primeira renderização, também é uma opção; eventos e atualizações assíncronas podem vir depois.

## Um caminho de implementação em etapas

1. **Escolher Lua como única linguagem** e testar uma versão estável do MoonSharp no alvo .NET usado pelo projeto.
2. **Criar o host de scripts** separado do parser e do renderizador. Ele recebe o texto ou o arquivo e oferece somente a fachada aprovada.
3. **Começar com inicialização e estado**: o script altera valores simples; o F# atualiza a tela usando o pipeline de renderização existente.
4. **Conectar eventos depois**: quando os elementos interativos tiverem um fluxo de eventos definido, eles podem chamar funções registradas pelo Lua e pedir outra renderização.
5. **Adicionar erros e limites explícitos**: informar arquivo e linha do erro, estabelecer limites de execução e bloquear acesso a arquivos fora da pasta permitida.
6. **Só então fechar `async`, `defer` e a forma final do `<code>`** com exemplos e testes.

Isso também evita prometer interatividade completa logo na primeira etapa. Um script executado ao carregar a página pode calcular o estado inicial, mas uma aplicação interativa depende de eventos que acionem scripts e de um caminho seguro para redesenhar.

## Um cuidado sobre a versão do MoonSharp

Na consulta feita em 25 de novembro de 2026, o site do projeto lista **2.0.0 como estável** e **3.0.0-beta.1 como pré-lançamento**. A página do NuGet informa que o pacote está direcionado a .NET Standard 1.6 e calcula compatibilidade com frameworks .NET mais novos, mas isso não substitui um teste real no PTML. Eu começaria avaliando a versão estável e só usaria a beta se houver um motivo concreto e testes para ela.

## Resumo final

**Lua + MoonSharp é uma escolha inicial razoável** para ter uma linguagem de script embutida no PTML sem transformar a aplicação em JavaScript ou executar um processo externo. A parte que faz a mágica não é a linguagem sozinha: é a ponte entre o script e o estado F# que alimenta o renderizador.

O desenho que eu manteria em mente é: Lua pede mudanças por uma API pequena; F# valida e guarda essas mudanças; o renderizador atual desenha o resultado. Assim, o `<code>` pode ajudar a criar aplicações dinâmicas sem deixar cada script controlar diretamente o terminal.

### Fontes consultadas

- [Brainstorm do PTML](../utilities/intel/brainstorm.md) e instruções do repositório em [copilot-instructions.md](../../.github/copilot-instructions.md) e [SKILL.md](../../.github/SKILL.md).
- [MoonSharp: Getting Started](https://www.moonsharp.org/getting_started.html) — compatibilidade com linguagens CLR, incluindo F#.
- [MoonSharp no GitHub](https://github.com/moonsharp-devs/moonsharp) — compatibilidade declarada com Lua 5.2.
- [MoonSharp: Sharing objects](https://www.moonsharp.org/objects.html) — registro explícito e exposição de objetos .NET.
- [MoonSharp: Sandboxing](https://www.moonsharp.org/sandbox.html) — módulos de biblioteca e restrição de acesso dos scripts.
- [MoonSharp no NuGet](https://www.nuget.org/packages/MoonSharp) e [site do projeto](https://www.moonsharp.org/) — versões disponíveis na data da consulta.
