# Get init script directory
$env:PowerShellSDKRepoRoot = Split-Path (Split-Path $script:MyInvocation.MyCommand.Path -Parent) -Parent

# Environment variables
$env:writerDir = "$($env:PowerShellSDKRepoRoot)\src\GraphODataPowerShellWriter"
$env:writerBuildDir = "$($env:writerDir)\bin\Release"
$env:generatedDir = "$($env:writerBuildDir)\output"
$env:sdkDir = "$($env:generatedDir)\bin\Release"
$env:testDir = "$($env:PowerShellSDKRepoRoot)\Tests"
$env:moduleName = 'PowerShellGraphSDK'
$env:moduleExtension = 'psd1'
# Remember the settings that will change when launching a child PowerShell context
$env:standardWindowTitle = (Get-Host).UI.RawUI.WindowTitle
$env:standardForegroundColor = (Get-Host).UI.RawUI.ForegroundColor
$env:standardBackgroundColor = (Get-Host).UI.RawUI.BackgroundColor

# Scripts
$env:buildScript = "$($env:PowerShellSDKRepoRoot)\Scripts\build.ps1"
$env:runScript = "$($env:PowerShellSDKRepoRoot)\Scripts\run.ps1"
$env:testScript = "$($env:PowerShellSDKRepoRoot)\Scripts\test.ps1"

function global:WriterBuild {
    param (
        [string]$GraphSchema
    )

    Invoke-Expression "$env:buildScript -WorkingDirectory '$env:writerDir' -OutputPath '$env:writerBuildDir' -BuildTargets 'Clean;Rebuild' -GraphSchema '$GraphSchema'"
}

function global:WriterRun {
    Invoke-Expression "$env:buildScript -WorkingDirectory '$env:writerDir' -OutputPath '$env:writerBuildDir' -BuildTargets 'Run'"
}

function global:SDKBuild {
    Invoke-Expression "$env:buildScript -WorkingDirectory '$env:generatedDir' -OutputPath '$env:sdkDir'"
}

function global:SDKRun {
[alias("run")]
    param()

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

    global:WriterBuild -GraphSchema $GraphSchema
    global:WriterRun
    global:SDKBuild
}

function global:GenerateAndRunSDK {
    param (
        [string]$GraphSchema
    )

    global:GenerateSDK -GraphSchema $GraphSchema
    global:SDKRun
}

# Run "dotnet restore" just in case this is the first time the repo is being initialized (or if there are new dependencies)
dotnet restore --verbosity quiet

Write-Host "Initialized repository." -f Green
Write-Host
Write-Host "Available commands:" -f Yellow
Write-Host "    GenerateAndRunSDK       " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Executes the commands 'GenerateSDK' and 'SDKRun' (in that order)" -f DarkCyan
Write-Host "    GenerateSDK (or 'build')" -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Executes the commands 'WriterBuild', 'WriterRun' and 'SDKBuild' (in that order)" -f DarkCyan
Write-Host "    WriterBuild             " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Builds the GraphODataPowerShellSDKWriter project" -f DarkCyan
Write-Host "    WriterRun               " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Runs the GraphODataPowerShellSDKWriter project" -f DarkCyan
Write-Host "    SDKBuild                " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Builds the generated PowerShellSDK project" -f DarkCyan
Write-Host "    SDKRun (or 'run')       " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Runs the generated PowerShellSDK project" -f DarkCyan
Write-Host "    SDKTest (or 'test')     " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Runs tests against the generated PowerShellSDK project" -f DarkCyan
Write-Host
