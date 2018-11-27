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
# Setup the test context
#
Import-Module $env:testDir\Set-IntuneContext.psm1
Write-Output "Setting IntuneContext..."
Set-IntuneContext -AdminUPN $AdminUPN

#
# Create iosGeneralDeviceConfiguration
#
Write-Output "Creating iOS Compliance Policy ..."
$iosGeneralDeviceConfiguration = New-DeviceConfigurations -iosGeneralDeviceConfiguration `
    -displayName "Chicago - iOS Device Restriction Policy" `
    -iCloudBlockBackup $true -iCloudBlockDocumentSync $true -iCloudBlockPhotoStreamSync $true

#
# Assign iosGeneralDeviceConfiguration to 'Intune POC Users' group
#
$IPU_Id = (Get-Groups -Filter "displayName eq 'Intune POC Users'").id
Write-Output "Assigning $iosGeneralDeviceConfiguration to 'Intune POC Users' group..."
Invoke-IntuneDeviceConfigurationsAssign -deviceConfigurationId $iosGeneralDeviceConfiguration.id `
    -assignments (New-DeviceConfigurationAssignmentObject `
    -target (New-DeviceAndAppManagementAssignmentTargetObject `
    -groupAssignmentTarget -groupId "$IPU_Id"))


 Write-Output "Test complete."