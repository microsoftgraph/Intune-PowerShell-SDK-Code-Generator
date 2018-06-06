# Get init script directory
$env:PowerShellSDKRepoRoot = Split-Path (Split-Path $script:MyInvocation.MyCommand.Path -Parent) -Parent

# Environment variables
$env:writerDir = "$($env:PowerShellSDKRepoRoot)\src\GraphODataPowerShellWriter"
$env:writerBuildDir = "$($env:writerDir)\bin\Release"
$env:generatedDir = "$($env:writerBuildDir)\output"
$env:sdkDir = "$($env:generatedDir)\bin\Release"
$env:testDir = "$($env:PowerShellSDKRepoRoot)\Tests"
$env:moduleName = 'IntunePreview'
$env:moduleExtension = 'psd1'
$env:defaultGraphSchema = "$($env:PowerShellSDKRepoRoot)\Test Graph Schemas\v1.0-20180406 - Intune.csdl"
# Remember the settings that will change when launching a child PowerShell context
$env:standardWindowTitle = (Get-Host).UI.RawUI.WindowTitle
$env:standardForegroundColor = (Get-Host).UI.RawUI.ForegroundColor
$env:standardBackgroundColor = (Get-Host).UI.RawUI.BackgroundColor

# Scripts
$env:buildScript = "$($env:PowerShellSDKRepoRoot)\Scripts\build.ps1"
$env:runScript = "$($env:PowerShellSDKRepoRoot)\Scripts\run.ps1"
$env:testScript = "$($env:PowerShellSDKRepoRoot)\Scripts\test.ps1"

function global:WriterBuild {
    Write-Host "Building the writer..." -f Cyan
    Invoke-Expression "$env:buildScript -WorkingDirectory '$env:writerDir' -OutputPath '$env:writerBuildDir' -BuildTargets 'Clean;Rebuild' -Verbosity 'quiet'"
    Write-Host "Finished building the writer" -f Cyan
    Write-Host
}

function global:WriterRun {
    param (
        [string]$GraphSchema
    )

    Write-Host "Running the writer (i.e. generating the cmdlets)..." -f Cyan
    Invoke-Expression "$env:buildScript -WorkingDirectory '$env:writerDir' -OutputPath '$env:writerBuildDir' -BuildTargets 'Run' -GraphSchema '$GraphSchema'"
    Write-Host "Finished running the writer" -f Cyan
    Write-Host
}

function global:SDKBuild {
    Write-Host "Building the SDK (i.e. building the generated cmdlets)..." -f Cyan
    Invoke-Expression "$env:buildScript -WorkingDirectory '$env:generatedDir' -OutputPath '$env:sdkDir' -Verbosity 'quiet'"
    Write-Host "Finished building the SDK" -f Cyan
    Write-Host
}

function global:SDKRun {
[alias("run")]
    param()

    Write-Host "Running the SDK (importing '$env:moduleName' and running 'Connect-MSGraph')..." -f Cyan
    Invoke-Expression "$env:runScript"
}

function global:SDKTest {
[alias("test")]
    param()

    Invoke-Expression "$env:testScript"
}

function global:GenerateSDK {
[alias("build")]
    param (
        [string]$GraphSchema
    )

    global:WriterBuild
    global:WriterRun -GraphSchema $GraphSchema
    global:SDKBuild
}

function global:GenerateAndRunSDK {
    param (
        [string]$GraphSchema
    )

    global:GenerateSDK -GraphSchema $GraphSchema
    global:SDKRun
}

# Restore NuGet packages
nuget restore -Verbosity quiet
# Restore packages in the PowerShellGraphSDK separately so it is available when the project folder is copied elsewhere
nuget restore "src\PowerShellGraphSDK" -Verbosity quiet

Write-Host "Initialized repository." -f Green
Write-Host "Available commands:" -f Yellow
Write-Host "    GenerateAndRunSDK       " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Executes the commands 'GenerateSDK' and 'SDKRun' (in that order)" -f DarkCyan
Write-Host "    GenerateSDK (or 'build')" -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Executes the commands 'WriterBuild', 'WriterRun' and 'SDKBuild' (in that order)" -f DarkCyan
Write-Host "    WriterBuild             " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Builds the GraphODataPowerShellSDKWriter project" -f DarkCyan
Write-Host "    WriterRun               " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Runs the GraphODataPowerShellSDKWriter project" -f DarkCyan
Write-Host "    SDKBuild                " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Builds the generated PowerShellSDK project" -f DarkCyan
Write-Host "    SDKRun (or 'run')       " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Runs the generated PowerShellSDK project" -f DarkCyan
Write-Host "    SDKTest (or 'test')     " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Runs tests against the generated PowerShellSDK project" -f DarkCyan
Write-Host
