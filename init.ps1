$env:PowerShellSDKRepoRoot = Split-Path $script:MyInvocation.MyCommand.Path -Parent

$env:writerDir = "$($env:PowerShellSDKRepoRoot)\src\GraphODataPowerShellWriter"
$env:writerBuildDir = "$($env:writerDir)\bin\Release"
$env:generatedDir = "$($env:writerBuildDir)\output"
$env:sdkDir = "$($env:generatedDir)\bin\Release"

$env:buildScript = "$($env:PowerShellSDKRepoRoot)\Scripts\build.ps1"

function global:Build-Writer {
    powershell "$env:buildScript -WorkingDirectory '$env:writerDir' -OutputPath '$env:writerBuildDir' -BuildTargets 'Clean;Rebuild'"
}

function global:Run-Writer {
    powershell $env:buildScript "-WorkingDirectory '$env:writerDir' -OutputPath '$env:writerBuildDir' -BuildTargets 'Run'"
}

function global:Build-SDK {
    powershell $env:buildScript "-WorkingDirectory '$env:generatedDir' -OutputPath '$env:sdkDir'"
}

function global:Run-SDK {
    Import-Module "$env:sdkDir\PowerShellGraphSDK.dll"
    Connect-MSGraph
}

function global:BuildAndRun {
    Build-Writer
    Run-Writer
    Build-SDK
    Run-SDK
}

Write-Host 'Initialized repository' -f Green