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

# Start the new PowerShell context
try {
    & powershell -NoExit -Command "& {$($commands -Join '; ')}"

    # Check that the special PowerShell context exited successfully
    if (-Not $?)
    {
        Write-Host "Child PowerShell context exited with error code '$LastExitCode'" -f Red
        Write-Host
    }
} catch {
    Write-Host "Failed to run child PowerShell context" -f Red
    Write-Host
} finally {
    # Restore the old settings
    (Get-Host).UI.RawUI.WindowTitle = $env:standardWindowTitle
    (Get-Host).UI.RawUI.ForegroundColor = $env:standardForegroundColor
    (Get-Host).UI.RawUI.BackgroundColor = $env:standardBackgroundColor

    exit
}
