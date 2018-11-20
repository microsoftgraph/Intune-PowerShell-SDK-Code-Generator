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
        [string]$AdminUPN="$env:adminUPN"   
    )

    $OutputDirectory = $OutputDirectory | Resolve-Path
    $modulePath = "$OutputDirectory/$ModuleName.psd1"

    #
    # Import the Intune PowerShell SDK Module if necessary
    #
    if ((Get-Module $ModuleName) -eq $null)
    {        
        Import-Module $modulePath
    }

    #
    # Connect to MSGraph if necessary
    #
    try
    {
        $env:msGraphMeta = Get-MSGraphMetadata
        $connection = Connect-MSGraph
    }
    catch
    {    
        $adminPwd=Read-Host -AsSecureString -Prompt "Enter pwd for $env:adminUPN"
        $creds = New-Object System.Management.Automation.PSCredential ($AdminUPN, $adminPwd)
        $connection = Connect-MSGraph -PSCredential $creds
    }
}
Export-ModuleMember -Function Set-IntuneContext