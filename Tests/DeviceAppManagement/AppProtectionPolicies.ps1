# Create a policy
Write-Host "Creating an iOS app protection policy..."
$policy = New-DeviceAppManagement_ManagedAppPolicies `
    -iosManagedAppProtection `
    -displayName "iOS MAM / APP Policy" `
    -periodOfflineBeforeAccessCheck (New-TimeSpan -Hours 12) `
    -periodOnlineBeforeAccessCheck (New-TimeSpan -Minutes 30) `
    -allowedInboundDataTransferSources managedApps `
    -allowedOutboundDataTransferDestinations managedApps `
    -allowedOutboundClipboardSharingLevel managedAppsWithPasteIn `
    -organizationalCredentialsRequired $false `
    -dataBackupBlocked $true `
    -managedBrowserToOpenLinksRequired $false `
    -deviceComplianceRequired $false `
    -saveAsBlocked $true `
    -periodOfflineBeforeWipeIsEnforced (New-TimeSpan -Days 30) `
    -pinRequired $true `
    -maximumPinRetries 5 `
    -simplePinBlocked $false `
    -minimumPinLength 4 `
    -pinCharacterSet numeric `
    -periodBeforePinReset (New-TimeSpan -Days 30) `
    -allowedDataStorageLocations @("oneDriveForBusiness","sharePoint") `
    -contactSyncBlocked $false `
    -printBlocked $true `
    -fingerprintBlocked $false `
    -disableAppPinIfDevicePinIsSet $false

# Get managed apps
Write-Host "Getting managed apps..."
$apps = Get-DeviceAppManagement_MobileApps -Expand assignments, categories | Where-Object { $_.'@odata.type' -like '#microsoft.graph.managed*' }

# Get app identifiers
Write-Host "Creating ManagedMobileApp objects from the retrieved iOS apps..."
$appIdentifiers = $apps | ForEach-Object {
    if (-not [string]::IsNullOrEmpty($_.bundleId)) {
        New-ManagedMobileAppObject -mobileAppIdentifier (New-MobileAppIdentifierObject -iosMobileAppIdentifier -bundleId $_.bundleId)
    }
}

# Target apps
Write-Host "Targeting the policy to the apps..."
Invoke-DeviceAppManagement_IosManagedAppProtections_TargetApps -iosManagedAppProtectionId $policy.id -apps $appIdentifiers

# Get an AAD group
Write-Host "Get a group that the current user is a member of..."
$group = (Get-Me_MemberOf)[0]

# Assign policy to group
Write-Host "Assign the policy to the group..."
Invoke-DeviceAppManagement_IosManagedAppProtections_Assign -iosManagedAppProtectionId $policy.id -assignments @(
    New-TargetedManagedAppPolicyAssignmentObject `
        -target (New-DeviceAndAppManagementAssignmentTargetObject -groupAssignmentTarget -groupId $group.id)
)

# Remove policy
Write-Host "Deleting the policy..."
$policy | Remove-DeviceAppManagement_ManagedAppPolicies