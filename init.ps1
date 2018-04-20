# Get init script directory
$env:PowerShellSDKRepoRoot = Split-Path $script:MyInvocation.MyCommand.Path -Parent

# Environment variables
$env:writerDir = "$($env:PowerShellSDKRepoRoot)\src\GraphODataPowerShellWriter"
$env:writerBuildDir = "$($env:writerDir)\bin\Release"
$env:generatedDir = "$($env:writerBuildDir)\output"
$env:sdkDir = "$($env:generatedDir)\bin\Release"
$env:moduleName = "PowerShellGraphSDK"

# Scripts
$env:buildScript = "$($env:PowerShellSDKRepoRoot)\Scripts\build.ps1"

function global:Build-Writer {
    param (
        [string]$GraphSchema
    )

    Invoke-Expression "$env:buildScript -WorkingDirectory '$env:writerDir' -OutputPath '$env:writerBuildDir' -BuildTargets 'Clean;Rebuild' -GraphSchema '$GraphSchema'"
}

function global:Run-Writer {
    Invoke-Expression "$env:buildScript -WorkingDirectory '$env:writerDir' -OutputPath '$env:writerBuildDir' -BuildTargets 'Run'"
}

function global:Build-SDK {
    Invoke-Expression "$env:buildScript -WorkingDirectory '$env:generatedDir' -OutputPath '$env:sdkDir'"
}

function global:Run-SDK {
    $commands = @(
        '(Get-Host).UI.RawUI.WindowTitle = "$env:moduleName"',
        '(Get-Host).UI.RawUI.ForegroundColor = ''Cyan''',
        '(Get-Host).UI.RawUI.BackgroundColor = ''Black''',
        '$ErrorActionPreference = ''Stop''',
        'Import-Module "$env:sdkDir\$env:moduleName.dll"',
        'Connect-MSGraph'
    )

    Write-Host
    Write-Host 'Starting a new PowerShell context with the following commands:' -f Cyan
    $commands | ForEach-Object { Write-Host "    $_" -f Cyan }
    Write-Host
    Write-Host 'WARNING: Type ''exit'' to return to this initialized PowerShell context.' -f Yellow
    Write-Host

    # Remember the settings that will change in the new PowerShell context
    $standardWindowTitle = (Get-Host).UI.RawUI.WindowTitle
    $standardForegroundColor = (Get-Host).UI.RawUI.ForegroundColor
    $standardBackgroundColor = (Get-Host).UI.RawUI.BackgroundColor

    # Start the new PowerShell context
    & powershell -NoExit -Command "& {$($commands -Join '; ')}"

    # Restore the old settings
    (Get-Host).UI.RawUI.WindowTitle = $standardWindowTitle
    (Get-Host).UI.RawUI.ForegroundColor = $standardForegroundColor
    (Get-Host).UI.RawUI.BackgroundColor = $standardBackgroundColor

    # Check that the special PowerShell context exited successfully
    if (-Not $?)
    {
        Write-Host "MSBuild exited with error code '$LastExitCode'" -f Red
        Write-Host
    }
}

function global:GenerateSDK {
    param (
        [string]$GraphSchema
    )

    global:Build-Writer -GraphSchema $GraphSchema
    global:Run-Writer
    global:Build-SDK
}

function global:GenerateAndRunSDK {
    param (
        [string]$GraphSchema
    )

    global:GenerateSDK -GraphSchema $GraphSchema
    global:Run-SDK
}

# Run "dotnet restore" just in case this is the first time the repo is being initialized (or if there are new dependencies)
dotnet restore --verbosity quiet

Write-Host 'Initialized repository.' -f Green
Write-Host
Write-Host 'Available commands:' -f Yellow
Write-Host '    GenerateAndRunSDK' -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Executes the commands 'GenerateSDK' and 'Run-SDK' (in that order)" -f DarkCyan
Write-Host '    GenerateSDK      ' -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Executes the commands 'Build-Writer', 'Run-Writer' and 'Build-SDK' (in that order)" -f DarkCyan
Write-Host '    Build-Writer     ' -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Builds the GraphODataPowerShellSDKWriter project" -f DarkCyan
Write-Host '    Run-Writer       ' -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Runs the GraphODataPowerShellSDKWriter project" -f DarkCyan
Write-Host '    Build-SDK        ' -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Builds the generated PowerShellSDK project" -f DarkCyan
Write-Host '    Run-SDK          ' -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Runs the generated PowerShellSDK project" -f DarkCyan
Write-Host
