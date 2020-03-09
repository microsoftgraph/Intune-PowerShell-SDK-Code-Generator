# Sumário
- [Sumário](#table-of-contents)
- [Intune-PowerShell-SDK-Code-Generator](#intune-powershell-sdk-code-generator)
- [Colaboração](#contributing)
- [Design](#design)
    - [Arquitetura de Alto Nível](#high-level-architecture)
    - [A Etapa de Geração Automática](#the-auto-generation-step)
- [Trabalho futuro](#future-work)
- [Perguntas Frequentes](#faq)
    - [Onde está o código gerado?](#where-is-the-generated-code)
    - [Por que precisamos deste módulo do PowerShell?](#why-do-we-need-this-powershell-module)
    - [Por que queremos gerar automaticamente o módulo do PowerShell?](#why-do-we-want-to-auto-generate-the-powershell-module)
    - [Por que o Intune está construindo (e por que a equipe do Graph SDK não está construindo)?](#why-is-intune-building-it-and-why-isnt-the-graph-sdk-team-building-it)
    - [O que é o Vipr?](#what-is-vipr)
    - [Por que um gravador personalizado?](#why-a-custom-writer)
    - [Por que não usar OpenAPI e Swagger?](#why-not-use-openapi-and-swagger)

# Intune-PowerShell-SDK-Code-Generator
Este repositório implementa o gravador Vipr que gera cmdlets do PowerShell para todas as operações, funções e ações de CRUD para suporte à API do Microsoft Intune e do Microsoft Graph.

# Colaboração
Este projeto recebe e agradece as contribuições e sugestões.
A maioria das contribuições exige que você concorde com um Contrato de Licença de Colaborador (CLA) declarando que você tem o direito a, nos conceder os direitos de uso de sua contribuição, e de fato o faz.
Para saber mais, acesse https://cla.microsoft.com.

Quando você envia uma solicitação de pull, um bot de CLA determina automaticamente se você precisa fornecer um CLA e decora o PR adequadamente (por exemplo, rótulo, comentário).
Basta seguir as instruções fornecidas pelo bot.
Você só precisa fazer isso uma vez em todos os repos que usam nosso CLA.

Este projeto adotou o [Código de Conduta de Código Aberto da Microsoft](https://opensource.microsoft.com/codeofconduct/).
Para saber mais, confira as [Perguntas frequentes sobre o Código de Conduta](https://opensource.microsoft.com/codeofconduct/faq/)
ou contate [opencode@microsoft.com](mailto:opencode@microsoft.com) se tiver outras dúvidas ou comentários.

# Design
## Arquitetura de Alto Nível
![Arquitetura de Alto Nível](Design.jpg)
Esta é a visão geral de como o módulo do PowerShell é elaborado.
1. O Vipr.exe é executado com os seguintes argumentos para gerar o código que define o comportamento dos cmdlets.
- A saída de compilação do projeto GraphODataPowerShellWriter (ou seja, GraphODataPowerShellWriter.dll).
- Um esquema de gráfico (por exemplo, o resultado da chamada https://graph.microsoft.com/v1.0/$metadata)- esse arquivo tem a extensão ".csdl" e está localizado em "~/TestGraphSchemas".
2. A documentação gerada na saída do Vipr é extraída em um arquivo XML. Esse arquivo XML é usado pelo PowerShell para exibir a saída do cmdlet 'Get-Help'.
3. Algumas funcionalidades fazem mais sentido serem escritas à mão no PowerShell; portanto, esses cmdlets são gravados em seus próprios módulos. Esses módulos estão localizados em "~/src/PowerShellGraphSDK/PowerShellModuleAdditions/CustomModules".
4. As saídas das 3 etapas anteriores (ou seja, os cmdlets gerados, a documentação do cmdlet e os cmdlets escritos à mão) são combinados usando um arquivo de manifesto do PowerShell. Esse arquivo tem a extensão ".psd1".
5. O arquivo de manifesto do PowerShell criado na etapa anterior é usado para importar e usar o módulo. Por exemplo, se o arquivo de manifesto foi chamado "Intune.psd1", em uma janela do PowerShell, você pode executar "Import-Module ./Intune.psd1" para importar o módulo.

## A Etapa de Geração Automática
![Gerando o Módulo Binário do PowerShell](Generating_the_PowerShell_Binary_Module.jpg)
Essa é uma visão detalhada na etapa 1 da seção [Arquitetura de Alto Nível](#high-level-architecture).
- O arquivo de definição de esquema (CSDL) é alimentado em Vipr.
– O leitor do OData v4, que está integrado ao Vipr, é usado para ler o arquivo CSDL. Este leitor converte a definição de esquema em uma representação intermediária, chamada Modelo ODCM.
- Nosso gravador personalizado é passado para o Vipr para processar esse modelo ODCM. O gravador consiste em 4 etapas para converter o modelo ODCM para os arquivos C# gerados finais:
1. Percorra o modelo ODCM (por exemplo, o esquema) para descobrir cada roteiro.
2. Para cada rota, crie uma representação abstrata dos cmdlets que representem cada operação no roteiro.
3. Converta cada representação de cmdlet abstrata em uma representação abstrata em C#.
4. Para cada representação abstrata em C#, converta-a em uma sequência que será gravada como o conteúdo de um arquivo.

# Trabalho futuro
- [ ] Melhore a nomeação de cmdlets gerados.
    - [ ] Veja se podemos adicionar anotações ao Esquema de gráfico que podem ser usadas para criar nomes amigáveis ​​para os roteiros.
- [ ] Encontre uma maneira de gerar e criar cmdlets para todo o Esquema de gráfico em um período de tempo razoável.
    - O número de roteiros (devido às propriedades de navegação) aumenta drasticamente quando as entidades do AAD são incluídas.
- [ ] Obtenha a documentação de ajuda do cmdlet gerada para incluir um link para a página apropriada na documentação oficial do Graph.
- [x] Implemente o pipe de saídas de cmdlets diretamente em outros cmdlets.
- [ ] Implementar autenticação não interativa.
    - [ ] Autenticação com certificados.
    - [x] Autenticação com um objeto PSCredential.
- [x] Verifique se obtemos o tempo de expiração correto do token de autenticação - isso é tratado ao atualizar automaticamente o token quando necessário.
- [x] Crie um repositório separado para os cmdlets de cenário (a serem usados ​​como um submódulo).
- [ ] Integração Travis CI GitHub + casos de teste.
- [x] Crie um arquivo de licença para o software de terceiros que usamos e inclua-o no módulo.
- [x] Atualize o arquivo \*.psd1 para incluir os links corretos para o GitHub público, arquivo de licença e ícone.
- [x] Crie cmdlets auxiliares que criam objetos que podem ser serializados para os tipos definidos no esquema.
- [ ] Obtenha as Restrições de Capacidade do OData adicionadas ao Esquema de gráfico para que os cmdlets que executam operações não permitidas não sejam gerados em primeiro lugar.

# Perguntas Frequentes
## Onde está o código gerado?
GitHub: https://github.com/Microsoft/Intune-PowerShell-SDK

## Por que precisamos deste módulo do PowerShell?
Os clientes expressam muitos interesse no uso do PowerShell para automatizar as tarefas que estão sendo executadas pela extensão do Intune no portal do Azure.

## Por que queremos gerar automaticamente o módulo do PowerShell?
Se escrevermos cada cmdlet manualmente, precisaremos atualizar manualmente o módulo sempre que o Esquema de gráfico for atualizado.

## Por que o Intune está construindo (e por que a equipe de SDK do Graph não está construindo)?
A maior parte da base de usuários do Intune (Profissionais de TI) prefere trabalhar com o PowerShell, enquanto o SDK do Graph é direcionado para desenvolvedores que preferem trabalhar com o C#, Java e Python, por exemplo. Esse é o motivo pelo qual precisamos dele urgente. A estrutura da sintaxe do PowerShell não se encaixa na estrutura do gerador SDK .Net/Java/Python existente (confira a explicação de por que temos um [Gravador Vipr Personalizado](#why-a-custom-writer)).

## O que é o Vipr?
- A arquitetura é construída em torno de um projeto de pesquisa da Microsoft chamado Vipr.
- Ele generaliza o conceito de transformar informações de uma sintaxe para outra.
- Ele é essencialmente um analisador de texto modular especialmente adequado para definições de serviço no estilo OData.
- O executável aceita um assembly Leitor e um assembly Gravador.
    - O Leitor é capaz de analisar o arquivo de texto de entrada na representação intermediária (genérica) do Vipr.
    - O Gravador aceita essa representação intermediária e retorna uma lista de objetos de arquivo, ou seja, pares (cadeia de caracteres, caminho).
        - O Vipr grava esses arquivos na estrutura de pastas especificada para a pasta de saída.

## Por que um gravador personalizado?
Nenhum gravador do PowerShell existente está disponível para Vipr. Além disso, o gravador SDK do Graph .Net/Java/Python não se prestaria facilmente à sintaxe, comportamento ou práticas recomendadas do PowerShell.

## Por que não usar OpenAPI e Swagger?
Isso teria sido possível, mas exigiria uma transformação extra do Esquema de gráfico do OData para o formato OpenAPI. A cada transformação, corre-se o risco de perder informações que podem ser valiosas no módulo do PowerShell gerado.