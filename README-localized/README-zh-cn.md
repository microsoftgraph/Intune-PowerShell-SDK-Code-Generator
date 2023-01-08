# 目录
- [目录](#table-of-contents)
- [Intune-PowerShell-SDK-Code-Generator](#intune-powershell-sdk-code-generator)
- [参与](#contributing)
- [设计](#design)
    - [高级体系结构](#high-level-architecture)
    - [自动生成步骤](#the-auto-generation-step)
- [未来工作](#future-work)
- [常见问题解答](#faq)
    - [生成的代码在哪里？](#where-is-the-generated-code)
    - [为什么需要此 PowerShell 模块？](#why-do-we-need-this-powershell-module)
    - [为什么要自动生成 PowerShell 模块？](#why-do-we-want-to-auto-generate-the-powershell-module)
    - [为什么是 Intune 构建（为什么不是 Graph SDK 团队构建）？](#why-is-intune-building-it-and-why-isnt-the-graph-sdk-team-building-it)
    - [什么是 Vipr？](#what-is-vipr)
    - [为什么是自定义编写器？](#why-a-custom-writer)
    - [为什么不使用 OpenAPI 和 Swagger？](#why-not-use-openapi-and-swagger)

# Intune-PowerShell-SDK-Code-Generator
此存储库实现的 Vipr 编写器将为 Microsoft Intune Graph Api 支持的所有 CRUD 运算、函数和操作生成 PowerShell cmdlet。

# 参与
本项目欢迎供稿和建议。
大多数的供稿都要求你同意“参与者许可协议 (CLA)”，声明你有权并确定授予我们使用你所供内容的权利。
有关详细信息，请访问 https://cla.microsoft.com。

在提交拉取请求时，CLA 机器人会自动确定你是否需要提供 CLA 并适当地修饰 PR（例如标记、批注）。
只需按照机器人提供的说明操作即可。
只需在所有存储库上使用我们的 CLA 执行此操作一次。

此项目已采用 [Microsoft 开放源代码行为准则](https://opensource.microsoft.com/codeofconduct/)。
有关详细信息，请参阅[行为准则常见问题解答](https://opensource.microsoft.com/codeofconduct/faq/)。
如有其他任何问题或意见，也可联系 [opencode@microsoft.com](mailto:opencode@microsoft.com)。

# 设计
## 高级体系结构
![高级体系结构](Design.jpg)
如何组合 PowerShell 模块的概览。
1.Vipr.exe 运行下列参数，以生成定义 cmdlet 行为的代码。
- GraphODataPowerShellWriter 项目的生成输出（即 GraphODataPowerShellWriter.dll）。
- Graph 架构（例如调用 https://graph.microsoft.com/v1.0/$metadata 的结果） - 该文件的扩展名为 ".csdl"，位于"~/TestGraphSchemas"中。
2.Vipr 输出中生成的文档被提取至 XML 文件中。该 XML 文件由 PowerShell 使用，以显示 'Get-Help' cmdlet 的输出。
3.某些功能对于在PowerShell中手动编写更有意义，因此在各自的模块中编写这些 cmdlet。这些模块位于 "~/src/PowerShellGraphSDK/PowerShellModuleAdditions/CustomModules"。
4.前 3 步的输出（即生成的 cmdlet、cmdlet 文档和手动编写的 cmdlet）使用 PowerShell 清单文件进行合并。此文件的扩展名为 ".psd1"。
5.上一步中创建的 PowerShell 清单文件随后用于导入和使用模块。例如，如果清单文件名称为 "Intune. psd1"，则可以在 PowerShell 窗口中运行 "Import-Module/Intune.psd1" 导入该模块。

## 自动生成步骤
![生成 PowerShell 二进制模块](Generating_the_PowerShell_Binary_Module.jpg)
这是[高级体系结构](#high-level-architecture)部分步骤
1 的详细信息。- 架构定义（CSDL）文件被输入 Vipr。
- 内置到 Vipr 的 OData v4 读取器用于读取 CSDL 文件。此读取器将架构定义转换为中间表现形式，称为 ODCM 模型。
- 传递自定义编写器至 Vipr，以处理此 ODCM 模型。编写器由 4 步组成，以将 ODCM 模型转换为最终生成的 C# 文件：
1.遍历 ODCM 模型（即架构）以发现各路由。
2.针对各路由，创建 cmdlet 的抽象表现形式来呈现路由上的各操作。
3.将各 cmdlet 的抽象表示形式，转换为抽象的 C# 表示形式。
4.对于各抽象的 C# 表示形式，将之转换为写为文件内容的字符串。

# 未来工作
- [ ] 改进所生成 cmdlet 的命名。
    - [ ] 查看是否可以添加批注至用于为路由创建友好名称的 Graph 架构中。
- [ ] 在合理的时间内找到针对整个 Graph 架构生成并构建 cmdlet 的方法。
    - 包含 AAD 实体后，路由数量（由于导航属性）将显著增加。
- [ ] 获取生成的 cmdlet 帮助文档，以包含链接至 Graph 官方文档的相应页面。
- [x] 将 cmdlet 输出管道直接实现到其他 cmdlet。
- [ ] 实现非交互身份验证。
    - [ ] 使用证书验证身份。
    - [x] 使用 PSCredential 对象验证身份。
- [x] 确保获得正确的身份验证令牌过期时间 - 需要时通过自动刷新令牌进行处理。
- [x] 针对方案 cmdlet 创建单独的存储库（常用作子模块）。
- [ ] Travis CI GitHub 集成 + 测试用例。
- [x] 针对第三方软件创建许可证文件并包含在模块中。
- [x] 更新 \*.psd1 文件并包含正确的链接至公共 GitHub、许可证文件和图标。
- [x] 创建帮助程序 cmdlet，使用它可创建序列化为架构中定义的类型。
- [] 将 OData 功能限制添加到 Graph 架构中，以便无法在第一位置生成执行不允许的操作的 cmdlet。

# 常见问题解答
## 生成的代码在哪里？
GitHub: https://github.com/Microsoft/Intune-PowerShell-SDK

## 为什么需要此 PowerShell 模块？
客户非常有兴趣使用 PowerShell 自动执行任务，目前这些任务通过扩展 Intune 至 Azure 门户执行。

## 为什么要自动生成 PowerShell 模块？
如果手动编写各 cmdlet，我们需要在每次更新 Graph 架构时，手动更新模块。

## 为什么是 Intune 构建（为什么不是 Graph SDK 团队构建）？
大多数 Intune 用户群体（IT 专业人员）更喜欢使用 PowerShell，而Graph SDK 适合喜欢使用 C#、Java 和 Python 等工作的开发人员。这就是我们迫切需要的原因。PowerShell 语法的结构不适合现有 .Net/Java/Python SDK 生成器的结构（请参阅有关为何我们拥有[自定义 Vipr 编写器](#why-a-custom-writer)的说明。）

## 什么是 Vipr？
- 此体系结构围绕 Microsoft Research 项目 Vipr 创建。
- 它生成将信息从一个语法转换为另一个语法的概念。
- 本质上是一个模块化文本分析程序，特别适用于 OData 样式服务定义。
- 可执行文件接受读取器程序集和编写器程序集。
    - 读取器能够将输入文本文件解析为 Vip 的中间（通用）表示形式。
    - 编写器接受此中间表示形式，并返回文件对象列表，即（字符串、路径）对。
        - Vipr 随后写入这些文件至指定的文件夹结构中，以输出文件夹。

## 为什么是自定义编写器？
没有 PowerShell 编写器可供 Vipr 使用。另外 .Net/Java/Python Graph SDK 编写器很难适用于 PowerShell 语法、行为或最佳做法。

## 为什么不使用 OpenAPI 和 Swagger？
可以使用，但需要另外从 Graph 的 OData 架构转换为 OpenAPI 格式。每次转换时，所生成 PowerShell 模块中的有价值的信息，可能出现丢失风险。