[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string]$SdkDirectory,

    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]$AdminUPN="$env:adminUPN"
)

$moduleLocation = "$SdkDirectory\$($env:moduleName).$($env:moduleExtension)"
$installFromPSGallery = $false

# Check that a build of the SDK exists
if (-Not (Test-Path "$moduleLocation" -PathType Leaf))
{
    Write-Host "Cannot find '$moduleLocation'. Installing Module from PowerShell Gallery"
    $installFromPSGallery = $true
}

# Run the tests
try {
    (Get-Host).UI.RawUI.WindowTitle = "$module"
    (Get-Host).UI.RawUI.ForegroundColor = 'Cyan'
    (Get-Host).UI.RawUI.BackgroundColor = 'Black'
    $testScripts = Get-ChildItem -Path "$env:testDir" -Recurse -Filter '*.ps1'

    #
    # Import the Intune PowerShell SDK Module
    #
    Write-Host "Import-Module from $moduleLocation"
    Import-Module $moduleLocation

    #
    # Setup the test context
    #
    Import-Module $env:testDir\Set-IntuneContext.psm1
    Set-IntuneContext

    #
    # Run the Tests
    #
    $testScripts | ForEach-Object {
        Write-Host -f Yellow "RUNNING: $($_.BaseName)"
        try {
            Invoke-Expression "$($_.FullName)"
        } catch {
            throw "Error: $_"
        }
        Write-Host -f Magenta "COMPLETED: $($_.BaseName)"
        Write-Host
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