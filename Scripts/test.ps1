# Check that a build of the SDK exists
if (-Not (Test-Path "$env:sdkDir"))
{
    throw "Cannot find a successful build of the SDK.  Run the 'build' command before running tests."
}

# Build the command to get and run test scripts
$getTestScriptsCommand = "`$testScripts = Get-ChildItem -Path '$env:testDir' -Recurse -Filter '*.ps1'"
$runTestScriptsCommand = "`$testScripts | ForEach-Object { Write-Host -f Magenta `"RUNNING: `"`$_.BaseName; & `$_.FullName; Write-Host -f Green `"SUCCESS: `"`$_.BaseName; Write-Host; }"

# Get the commands
$commands = @(
    '(Get-Host).UI.RawUI.WindowTitle = "$env:moduleName"',
    '(Get-Host).UI.RawUI.ForegroundColor = ''Cyan''',
    '(Get-Host).UI.RawUI.BackgroundColor = ''Black''',
    # '$ErrorActionPreference = ''Stop''',
    'Import-Module "$env:sdkDir\$env:moduleName.dll"',
    $getTestScriptsCommand,
    'Connect-MSGraph',
    $runTestScriptsCommand,
    'exit'
)

Write-Host
Write-Host 'Starting the test PowerShell context with the following commands:' -f Cyan
$commands | ForEach-Object { Write-Host "    $_" -f Cyan }
Write-Host

# Run the tests in a new PowerShell context
try {
    & powershell -NoExit -Command "& {$($commands -Join '; ')}"
} catch {
    Write-Error 'Tests failed'
} finally {
    # Restore the old settings
    (Get-Host).UI.RawUI.WindowTitle = $env:standardWindowTitle
    (Get-Host).UI.RawUI.ForegroundColor = $env:standardForegroundColor
    (Get-Host).UI.RawUI.BackgroundColor = $env:standardBackgroundColor

    exit
}