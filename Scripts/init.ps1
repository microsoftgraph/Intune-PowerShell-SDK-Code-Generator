# Get init script directory
$env:PowerShellSDKRepoRoot = Split-Path (Split-Path $script:MyInvocation.MyCommand.Path -Parent) -Parent

# Environment variables
$env:buildConfiguration = "Release"
$env:writerDir = "$($env:PowerShellSDKRepoRoot)\src\GraphODataPowerShellWriter"
$env:writerBuildDir = "$($env:writerDir)\bin\$($env:buildConfiguration)"
$env:generatedDir = "$($env:writerBuildDir)\output"
$env:sdkDir = "$($env:generatedDir)\bin\$($env:buildConfiguration)"
$env:testDir = "$($env:PowerShellSDKRepoRoot)\Tests"
$env:moduleName = 'Intune'
$env:moduleExtension = 'psd1'
$env:sdkSubmoduleSrc = "$($env:PowerShellSDKRepoRoot)\submodules\Intune-PowerShell-SDK\src"
$env:sdkSubmoduleBuild = "$($env:sdkSubmoduleSrc)\bin\$($env:buildConfiguration)"
$env:sdkAssemblyName = 'Microsoft.Intune.PowerShellGraphSDK'

# Remember the settings that will change when launching a child PowerShell context
$env:standardWindowTitle = (Get-Host).UI.RawUI.WindowTitle
$env:standardForegroundColor = (Get-Host).UI.RawUI.ForegroundColor
$env:standardBackgroundColor = (Get-Host).UI.RawUI.BackgroundColor

# Scripts
$env:buildScript = "$($env:PowerShellSDKRepoRoot)\Scripts\build.ps1"
$env:generateModuleManifestScript = "$($env:PowerShellSDKRepoRoot)\Scripts\generateModuleManifest.ps1"
$env:runScript = "$($env:PowerShellSDKRepoRoot)\Scripts\run.ps1"
$env:testScript = "$($env:PowerShellSDKRepoRoot)\Scripts\test.ps1"

function global:BuildWriter {
    Write-Host "Building the writer..." -f Cyan
    Invoke-Expression "$env:buildScript -WorkingDirectory '$env:writerDir' -OutputPath '$env:writerBuildDir' -BuildTargets 'Rebuild' -Verbosity 'minimal'"
    Write-Host "Finished building the writer" -f Cyan
    Write-Host
}

function global:RunWriter {
    param (
        [string]$GraphSchema
    )

    Write-Host "Running the writer (i.e. generating the cmdlets)..." -f Cyan
    Invoke-Expression "$env:buildScript -WorkingDirectory '$env:writerDir' -OutputPath '$env:writerBuildDir' -BuildTargets 'Run' -GraphSchema '$GraphSchema'"
    Write-Host "Finished running the writer" -f Cyan
    Write-Host
}

function global:GenerateModuleManifest {
    param(
        [string]$ModuleName = $null,
        [string]$OutputDirectory = $null,
        [string]$MainModuleRelativePath = $null,
        [string[]]$NestedModulesRelativePaths = $null
    )

    Write-Host "Generating module manifest..." -f Cyan

    # Validate values
    if (-Not $ModuleName) {
        $ModuleName = $env:moduleName
    }
    if (-Not $OutputDirectory) {
        $OutputDirectory = $env:sdkDir
    }
    if (-Not (Test-Path $OutputDirectory -PathType Container)) {
        throw "Directory '$OutputDirectory' does not exist"
    }
    if (-Not $MainModuleRelativePath) {
        $MainModuleRelativePath = ".\$($env:sdkAssemblyName).dll"
    }
    if (-Not $NestedModulesRelativePaths) {
        Push-Location $OutputDirectory
        try {
            $NestedModulesRelativePaths = Get-ChildItem -Include '*.psm1', '*.ps1' -Recurse -File | Resolve-Path -Relative
        } finally {
            Pop-Location
        }
    }

    # Call the script to generate the manifest
    Invoke-Expression "$env:generateModuleManifestScript -ModuleName $ModuleName -OutputDirectory $OutputDirectory -MainModuleRelativePath $MainModuleRelativePath -NestedModulesRelativePaths $NestedModulesRelativePaths"

    Write-Host "Finished generating module manifest" -f Cyan
    Write-Host
}

function global:BuildSDK {
    param(
        $WorkingDirectory
    )

    # Build the SDK
    Write-Host "Building the SDK (i.e. building the generated cmdlets)..." -f Cyan
    Invoke-Expression "$env:buildScript -WorkingDirectory '$WorkingDirectory' -OutputPath '$env:sdkDir' -Verbosity 'quiet' -AssemblyName '$env:sdkAssemblyName'"
    Write-Host "Finished building the SDK" -f Cyan
    Write-Host

    # Generate the module manifest as part of the build
    GenerateModuleManifest
}

function global:RunSDK {
[alias("run")]
    param()

    Write-Host "Running the SDK (importing '$env:moduleName' and running 'Connect-MSGraph')..." -f Cyan
    Invoke-Expression "$env:runScript"
}

function global:TestSDK {
[alias("test")]
    param()

    Invoke-Expression "$env:testScript"
}

function global:GenerateSDK {
[alias("build")]
    param (
        [string]$GraphSchema
    )

    global:BuildWriter
    global:RunWriter -GraphSchema $GraphSchema
    global:BuildSDK -WorkingDirectory "$env:generatedDir"
}

function global:GenerateAndRunSDK {
    param (
        [string]$GraphSchema
    )

    global:GenerateSDK -GraphSchema $GraphSchema
    global:RunSDK
}

function global:ReleaseSDK {
[alias("release")]
    param()

    if (-Not (Test-Path $env:generatedDir)) {
        throw "An SDK build was not found at '$env:generatedDir' - run 'build' before running 'release'"
    }

    Write-Host "Syncing '$env:sdkSubmoduleSrc'..."

    Write-Host "Copying generated SDK" -f Cyan
    Remove-Item "$env:sdkSubmoduleSrc" -Recurse
    New-Item "$env:sdkSubmoduleSrc" -ItemType directory | Out-Null
    Copy-Item "$env:generatedDir\*" -Destination "$env:sdkSubmoduleSrc\" -Recurse -Force -Container

    Write-Host "REMINDER: Make sure to correctly commit this change to the 'Intune-PowerShell-SDK' git submodule" -f Yellow
}

# Restore NuGet packages
nuget restore -Verbosity quiet
# Restore packages in the PowerShellGraphSDK separately so it is available when the project folder is copied elsewhere
nuget restore "src\PowerShellGraphSDK" -Verbosity quiet

Write-Host "Initialized repository." -f Green
Write-Host "Available commands:" -f Yellow
Write-Host "    GenerateAndRunSDK             " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Executes the commands 'GenerateSDK' and 'RunSDK' (in that order)" -f DarkCyan
Write-Host "    GenerateSDK (or 'build')      " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Executes the commands 'BuildWriter', 'RunWriter' and 'BuildSDK' (in that order)" -f DarkCyan
Write-Host "    BuildWriter                   " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Builds the GraphODataPowerShellSDKWriter project" -f DarkCyan
Write-Host "    RunWriter                     " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Runs the GraphODataPowerShellSDKWriter project" -f DarkCyan
Write-Host "    BuildSDK                      " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Builds the generated PowerShellSDK project" -f DarkCyan
Write-Host "    RunSDK (or 'run')             " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Runs the generated PowerShellSDK project" -f DarkCyan
Write-Host "    TestSDK (or 'test')           " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Runs tests against the generated PowerShellSDK project" -f DarkCyan
Write-Host "    ReleaseSDK (or 'release')     " -NoNewline -f Cyan; Write-Host ' | ' -NoNewline -f Gray; Write-Host "Releases the generated SDK to https://github.com/Microsoft/Intune-PowerShell-SDK." -f DarkCyan
Write-Host
