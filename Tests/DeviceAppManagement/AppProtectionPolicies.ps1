# Create a policy
Write-Host "Creating an iOS app protection policy..."
$policy = New-ManagedAppPolicies `
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
$apps = Get-MobileApps -Expand assignments, categories | Where-Object { $_.'@odata.type' -like '#microsoft.graph.managed*' }

# Get app identifiers
Write-Host "Creating ManagedMobileApp objects from the retrieved iOS apps..."
$appIdentifiers = $apps | ForEach-Object {
    if (-not [string]::IsNullOrEmpty($_.bundleId)) {
        New-ManagedMobileAppObject -mobileAppIdentifier (New-MobileAppIdentifierObject -iosMobileAppIdentifier -bundleId $_.bundleId)
    }
}

# Target apps
Write-Host "Targeting the policy to the apps..."
Invoke-IntuneIosAppProtectionPoliciesTargetApps -iosManagedAppProtectionId $policy.id -apps $appIdentifiers

# Get an AAD group
Write-Host "Get security groups..."
$groups = Get-Groups | Where-Object { $_.securityEnabled -eq $true }

# Assign policy to groups
Write-Host "Assign the policy to the groups..."
$groups | ForEach-Object {
    Invoke-IntuneIosAppProtectionPoliciesAssign -iosManagedAppProtectionId $policy.id -assignments @(
        New-TargetedManagedAppPolicyAssignmentObject `
            -target (New-DeviceAndAppManagementAssignmentTargetObject -groupAssignmentTarget -groupId $_.id)
    )
}

# Remove policy
Write-Host "Deleting the policy..."
$policy | Remove-ManagedAppPolicies