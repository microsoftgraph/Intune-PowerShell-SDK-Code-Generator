# 目次
- [目次](#table-of-contents)
- [Intune-PowerShell-SDK-Code-Generator](#intune-powershell-sdk-code-generator)
- [投稿](#contributing)
- [デザイン](#design)
    - [アーキテクチャの概要](#high-level-architecture)
    - [自動生成ステップ](#the-auto-generation-step)
- [今後の作業](#future-work)
- [FAQ](#faq)
    - [生成されたコードはどこにありますか?](#where-is-the-generated-code)
    - [この PowerShell モジュールはなぜ必要なのですか?](#why-do-we-need-this-powershell-module)
    - [PowerShell モジュールを自動生成した方がよいのはなぜですか?](#why-do-we-want-to-auto-generate-the-powershell-module)
    - [Intune によってビルドされるのはなぜですか? (Graph SDK チームによってビルドされないのはなぜですか?)](#why-is-intune-building-it-and-why-isnt-the-graph-sdk-team-building-it)
    - [Vipr とは](#what-is-vipr)
    - [カスタム ライターを使うのはなぜですか?](#why-a-custom-writer)
    - [OpenAPI と Swagger はなぜ使わないのですか?](#why-not-use-openapi-and-swagger)

# Intune-PowerShell-SDK-Code-Generator
このリポジトリでは、Microsoft Intune Graph API のサポート用のすべての CRUD 操作、関数、アクションの PowerShell コマンドレットを生成する、Vipr ライターが実装されます。

# 投稿
このプロジェクトは投稿や提案を歓迎します。たいていの投稿には、投稿者のライセンス契約 (CLA) に同意することにより、
投稿内容を使用する権利を Microsoft に付与する権利が自分にあり、
実際に付与する旨を宣言していただく必要があります。詳細については、https://cla.microsoft.com をご覧ください。

プル要求を送信すると、CLA を提供する必要があるかどうかを CLA ボットが自動的に判断してプル要求を適切に修飾 (ラベル、コメントなど) します。
ボットの指示に従ってください。この操作は、
CLA を使用してすべてのリポジトリ全体に対して 1 度のみ行う必要があります。

このプロジェクトでは、[Microsoft Open Source Code of Conduct (Microsoft オープン ソース倫理規定)](https://opensource.microsoft.com/codeofconduct/) が採用されています。
詳細については、「[Code of Conduct の FAQ (倫理規定の FAQ)](https://opensource.microsoft.com/codeofconduct/faq/)」
を参照してください。また、その他の質問やコメントがあれば、[opencode@microsoft.com](mailto:opencode@microsoft.com) までお問い合わせください。

# デザイン
## アーキテクチャの概要
![アーキテクチャの概要](Design.jpg)
PowerShell モジュールがどのように作られているかの概要です。
1.Vipr.exe は、コマンドレットの動作を定義するコードを生成するために次の引数を使用して実行されます。
- GraphODataPowerShellWriter プロジェクトのビルド出力 (つまり、GraphODataPowerShellWriter.dll).
- Graph スキーマ (例: https://graph.microsoft.com/v1.0/$metadata の呼び出しの結果) - このファイルは拡張子 ".csdl" を持ち、"~/TestGraphSchemas" にあります。
2.Vipr の出力で生成されたドキュメントは、XML ファイルに抽出されます。この XML ファイルは、"Get-help" コマンドレットの出力を表示するために PowerShell によって使用されます。
3.一部の機能は PowerShell で手動で記述する方がより適しているため、これらのコマンドレットはそれぞれ独自のモジュールに記述されます。これらのモジュールは、 "~/src/PowerShellGraphSDK/PowerShellModuleAdditions/CustomModules" にあります。
4.前の 3 つの手順の出力 (つまり、生成されたコマンドレット、コマンドレットのドキュメント、および手書きのコマンドレット) は、PowerShell マニフェスト ファイルを使用して結合されます。このファイルの拡張子は ".psd1" です。
5.前の手順で作成した PowerShell マニフェスト ファイルは、モジュールをインポートして使用するために使われます。たとえば、マニフェスト ファイルの名前が "Intune.psd1"である場合は、PowerShell ウィンドウで "Import-Module ./Intune.psd1" を実行してモジュールをインポートできます。

## 自動生成ステップ
![Generating the PowerShell Binary Module (PowerShell バイナリ モジュールの生成)](Generating_the_PowerShell_Binary_Module.jpg)
「[アーキテクチャの概要](#high-level-architecture)」セクションの手順 1 を詳細に示したものです。
- スキーマ定義 (CSDL) ファイルが Vipr に送られます。
- Vipr に組み込まれている OData v4 リーダーを使用して CSDL ファイルが読み込まれます。リーダーはスキーマ定義を ODCM モデルと呼ばれる中間表現に変換します。
- この ODCM モデルを処理するために、カスタム ライターが Vipr に渡されます。ライターは、4 つの手順で ODCM モデルを最終的に生成された C# ファイルに変換します。
1.ODCM モデル (つまり、スキーマ) をスキャンして各ルートを検出します。
2.ルートごとに、ルート上の各操作を表すコマンドレットの抽象表現を作成します。
3.それぞれのコマンドレットの抽象表現を C# の抽象表現に変換します。
4.それぞれの C# の抽象表現を、ファイルのコンテンツとして記述される文字列に変換します。

# 今後の作業
- [ ] 生成されたコマンドレットの名前付けを改善する。
    - [ ] ルートのフレンドリ名を作成するのに使用できる注釈を Graph スキーマに追加できるかどうかを検討する。
- [ ] Graph スキーマ全体のコマンドレットを合理的な時間内に生成してビルドする方法を見つける。
    - AAD エンティティを含めた場合、(ナビゲーション プロパティによる) ルート数は大幅に増加します。
- [ ] 生成されたコマンドレットのヘルプドキュメントに、公式の Graph ドキュメント内の正しいページへのリンクを含める。
- [x] コマンドレット出力のパイプ処理を、他のコマンドレットに直接実装する。
- [ ] 非対話型認証を実装する。
    - [ ] 証明書を使用して認証を行う。
    - [x] PSCredential オブジェクトを使用して認証を行う。
- [x] 認証トークンの正しい有効期限を確実に取得できるようにする。これは、必要に応じてトークンを自動的に更新することにより処理します。
- [x] (サブモジュールとして使用する) シナリオ コマンドレット用に別のレポジトリを作成する。
- [ ] Travis CI GitHub の統合とテスト ケース。
- [x] モジュールに含めて使用するサード パーティー製ソフトウェア用のライセンス ファイルを作成する。
- [x] \*.psd1 ファイルを更新してパブリック GitHub、ライセンス ファイル、およびアイコンへの正しいリンクを含める。
- [x] スキーマで定義される種類にシリアル化することが可能なオブジェクトを作成する、ヘルパー コマンドレットを作成する。
- [ ] OData Capability Restrictions を Graph スキーマに追加して、禁止操作を実行するコマンドレットがそもそも生成されないようにする。

# FAQ
## 生成されたコードはどこにありますか?
GitHub: https://github.com/Microsoft/Intune-PowerShell-SDK

## この PowerShell モジュールはなぜ必要なのですか?
現在は Intune 拡張機能を使用して Azure ポータルで実行されている作業を PowerShell を使用して自動化することに対して、お客様から強い関心が寄せられました。

## PowerShell モジュールを自動生成した方がよいのはなぜですか?
各コマンドレットを手動で作成した場合、Graph スキーマを更新するたびに、手動でモジュールを更新する必要があります。

## Intune によってビルドされるのはなぜですか? (Graph SDK チームによってビルドされないのはなぜですか?)
Intune のユーザー ベース (IT プロフェッショナル) のほとんどが PowerShell を使用して作業することを希望することが予想されます。一方、Graph SDK は C#、Java、および Python を使用することを希望する開発者向けに作られています。PowerShell モジュールを緊急に必要とする理由はここにあります。PowerShell の構文構造は、既存の Net/Java/Python SDK ジェネレーターの構造には適していません ([カスタム Vipr ライター](#why-a-custom-writer)がある理由の説明を参照してください)。

## Vipr とは
- このアーキテクチャは、Vipr と呼ばれる Microsoft Research プロジェクトに基づいて構築されています。
- Vipr は、情報を 1 つの構文から別の構文に変換する概念を一般化します。
- 本質的には、OData スタイルのサービス定義に最適なモジュール式のテキスト パーサーです。
- この実行可能ファイルは、リーダー アセンブリとライター アセンブリを受け取ります。
    - リーダーは、入力テキスト ファイルを Vipr の中間 (汎用) 表現に解析できます。
    - ライターは、この中間表現を受け入れ、ファイル オブジェクト (つまり、文字列とパスのペア) のリストを返します。
        - 次に、Vipr は、指定されたフォルダー構造でこれらのファイルを出力フォルダーに書き込みます。

## カスタム ライターを使うのはなぜですか?
いずれの既存の PowerShell ライターも、Vipr には使用できません。また、.Net/Java/Python 用 Graph SDK のライターは、PowerShell の構文、動作、または作業には適していません。

## OpenAPI と Swagger はなぜ使わないのですか?
使用することは可能ですが、使用するには Graph の OData スキーマから OpenAPI 形式への追加の変換が必要になります。変換を行うたびに、生成された PowerShell モジュールで役立つ可能性のある情報を失う危険が生じます。