# Create a policy
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
$apps = Get-DeviceAppManagement_MobileApps | Where-Object { $_.'@odata.type' -like '#microsoft.graph.managed*' }

# Get app identifiers
$appIdentifiers = $apps | ForEach-Object {
    if (-not [string]::IsNullOrEmpty($_.bundleId)) {
        New-ManagedMobileAppObject -mobileAppIdentifier (New-MobileAppIdentifierObject -iosMobileAppIdentifier -bundleId $_.bundleId)
    }
}

# Target apps
Invoke-DeviceAppManagement_ManagedAppPolicies_TargetApps -managedAppPolicyId $policy.id -apps $appIdentifiers

# Remove policy
$policy | Remove-DeviceAppManagement_ManagedAppPolicies