# Start the new PowerShell context
try {
    Write-Host 'Importing module in a new PowerShell context...'
    Write-Host 'NOTE: Type ''exit'' to return to the current context.' -f Yellow

    powershell -NoExit -Command {
        try {
            (Get-Host).UI.RawUI.WindowTitle = "$env:moduleName"
            (Get-Host).UI.RawUI.ForegroundColor = 'Cyan'
            (Get-Host).UI.RawUI.BackgroundColor = 'Black'
            Import-Module "$env:sdkDir\$env:moduleName.$env:moduleExtension"
            Connect-MSGraph
        } catch {
            Write-Error "Failed to initialize new PowerShell context: '$_'"
            exit
        }
    }

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
}
