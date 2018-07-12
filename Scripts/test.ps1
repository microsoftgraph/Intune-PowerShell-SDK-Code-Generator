# Check that a build of the SDK exists
if (-Not (Test-Path "$env:sdkDir"))
{
    throw "Cannot find a successful build of the SDK.  Run the 'build' command before running tests."
}

Write-Host
Write-Host 'Starting the test PowerShell context...' -f Cyan
Write-Host

# Run the tests in a new PowerShell context
try {
    powershell -NoExit -Command {
        (Get-Host).UI.RawUI.WindowTitle = "$env:moduleName"
        (Get-Host).UI.RawUI.ForegroundColor = 'Cyan'
        (Get-Host).UI.RawUI.BackgroundColor = 'Black'
        Import-Module "$env:sdkDir\$env:moduleName.$env:moduleExtension"
        $testScripts = Get-ChildItem -Path "$env:testDir" -Recurse -Filter '*.ps1'
        Connect-MSGraph
        $testScripts | ForEach-Object {
            Write-Host -f Yellow "RUNNING: $($_.BaseName)"
            try {
                Invoke-Expression "$($_.FullName)"
            } catch {
                Write-Error "Error: $_"
            }
            Write-Host -f Magenta "COMPLETED: $($_.BaseName)"
            Write-Host
        }
        exit
    }

    if (-Not $?)
    {
        throw "Tests failed with error code '$LastExitCode'"
    }
} catch {
    Write-Error "Error: $_"
} finally {
    # Restore the old settings
    (Get-Host).UI.RawUI.WindowTitle = $env:standardWindowTitle
    (Get-Host).UI.RawUI.ForegroundColor = $env:standardForegroundColor
    (Get-Host).UI.RawUI.BackgroundColor = $env:standardBackgroundColor

    exit
}