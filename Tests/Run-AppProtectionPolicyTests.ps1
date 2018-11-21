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
# Create iosManagedAppProtection
#
Write-Output "Creating iOS MAM / APP Policy ..."
$iosManagedAppProtection = New-ManagedAppPolicies -iosManagedAppProtection -displayName "iOS MAM / APP Policy" `
    -periodOfflineBeforeAccessCheck (New-TimeSpan -Hours 12) `
    -periodOnlineBeforeAccessCheck (New-TimeSpan -Minutes 30)`
    -allowedInboundDataTransferSources managedApps -allowedOutboundDataTransferDestinations managedApps `
    -allowedOutboundClipboardSharingLevel managedAppsWithPasteIn -organizationalCredentialsRequired $false `
    -dataBackupBlocked $true -managedBrowserToOpenLinksRequired $false -deviceComplianceRequired $false `
    -saveAsBlocked $true -periodOfflineBeforeWipeIsEnforced (New-TimeSpan -Days 30) `
    -pinRequired $true -maximumPinRetries 5 -simplePinBlocked $false -minimumPinLength 4 `
    -pinCharacterSet numeric -periodBeforePinReset (New-TimeSpan -Days 30) -allowedDataStorageLocations @("oneDriveForBusiness","sharePoint") `
    -contactSyncBlocked $false -printBlocked $true -fingerprintBlocked $false `
    -disableAppPinIfDevicePinIsSet $false

#
# Assign iosManagedAppProtection to 'Intune POC Users' group
#
$IPU_Id = (Get-Groups -Filter "displayName eq 'Intune POC Users'").id
Write-Output "Assigning $iosGeneralDeviceConfiguration to 'Intune POC Users' group..."
Invoke-AssignIosAPPsApps -iosManagedAppProtectionId $iosManagedAppProtection.id `
    -assignments (New-TargetedManagedAppPolicyAssignmentObject `
    -target (New-DeviceAndAppManagementAssignmentTargetObject `
    -groupAssignmentTarget -groupId "$IPU_Id"))

 Write-Output "Test complete."