#
# Set-IntuneContext: Sets up the context for running the tests
#
function Set-IntuneContext
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $false)]
        [ValidateNotNullOrEmpty()]
        [string]$ModuleName="$env:moduleName",

        [Parameter(Mandatory = $false)]
        [ValidateNotNullOrEmpty()]
        [string]$OutputDirectory="$env:sdkDir",

        [Parameter(Mandatory = $false)]
        [ValidateNotNullOrEmpty()]
        [string]$AdminUPN="$env:adminUPN",

        [Parameter(Mandatory = $false)]
        [ValidateNotNullOrEmpty()]
        [string]$IsInitialized="$env:initialized"

    )

    $OutputDirectory = $OutputDirectory | Resolve-Path
    $modulePath = "$OutputDirectory/$ModuleName.psd1"

    #
    # Import the Intune PowerShell SDK Module
    #
    Write-Output "Importing $modulePath..."
    Import-Module $modulePath

    #
    # Connect to MSGraph if necessary
    #
    if ($IsInitialized -eq $false)
    {    
        $adminPwd=Read-Host -AsSecureString -Prompt "Enter pwd for $env:adminUPN"
        $creds = New-Object System.Management.Automation.PSCredential ($AdminUPN, $adminPwd)
        $env:connection = Connect-MSGraph -PSCredential $creds        
    }
}
Export-ModuleMember -Function Set-IntuneContext