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

    #
    # Import the Intune PowerShell SDK Module if necessary
    #
    if (!(Get-Module $ModuleName))
    {        
        $modulePath = "$env:sdkDir\$env:moduleName"
        if (-Not (Test-Path "$modulePath" -PathType Leaf))
        {
            Write-Host "Install-Module $env:moduleName from PSGallery"
            Install-Module $env:moduleName -Force
        }
        else
        {
            Write-Host "Import-Module from $modulePath"
            Import-Module $modulePath
        }
    }

    #
    # Connect to MSGraph if necessary
    #
    try
    {
        $msGraphMetaData = Get-MSGraphMetadata
    }
    catch
    {    
        Write-Output "Connect with MSGraph first."
        $adminPwd=Read-Host -AsSecureString -Prompt "Enter pwd for $env:adminUPN"
        $creds = New-Object System.Management.Automation.PSCredential ($AdminUPN, $adminPwd)
        $connection = Connect-MSGraph -PSCredential $creds
    }
}
Export-ModuleMember -Function Set-IntuneContext