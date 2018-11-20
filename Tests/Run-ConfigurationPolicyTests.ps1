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
# Import the Intune PowerShell SDK Module
#
Write-Output "Importing $ModuleName..."
Import-Module $modulePath

#
# Setup the test context
#
Import-Module $env:testDir\Set-IntuneContext.psm1
Write-Output "Setting IntuneContext..."
Set-IntuneContext -AdminUPN $AdminUPN

#
# Create iosGeneralDeviceConfiguration
#
Write-Output "Creating iOS Compliance Policy ..."
$iosGeneralDeviceConfiguration = New-DmDeviceConfigurations -iosGeneralDeviceConfiguration `
    -displayName "Chicago - iOS Device Restriction Policy" `
    -iCloudBlockBackup $true -iCloudBlockDocumentSync $true -iCloudBlockPhotoStreamSync $true

#
# Assign iosGeneralDeviceConfiguration to 'Intune POC Users' group
#
$IPU_Id = (Get-Groups -Filter "displayName eq 'Intune POC Users'").id
Write-Output "Assigning $iosGeneralDeviceConfiguration to 'Intune POC Users' group..."
Invoke-DmAssignDCs -deviceConfigurationId $iosGeneralDeviceConfiguration.id `
    -assignments (New-DeviceConfigurationAssignmentObject `
    -target (New-DeviceAndAppManagementAssignmentTargetObject `
    -groupAssignmentTarget -groupId "$IPU_Id"))


 Write-Output "Test complete."