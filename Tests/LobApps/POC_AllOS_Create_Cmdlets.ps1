
<#

.COPYRIGHT
Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.

#>

####################################################

#region PowerShell Module

if(!(Get-Module Microsoft.Graph.Intune)){

    Import-Module "$env:sdkDir\$env:moduleName.psd1"

}

#endregion

####################################################

#region Authentication

if(!(Connect-MSGraph)){

    Connect-MSGraph

}

Write-Host

#endregion

####################################################

#region Script Output

$Date = Get-Date
$SDate = Get-Date
$Project = "Intune"

write-host
write-host "------------------------------------------------------------------" -f yellow
write-host "[$Project] Intune POC Creation"                                     -f yellow
write-host "------------------------------------------------------------------" -f yellow
write-host "[$Project] Script Started on $SDate"                                  
write-host
write-host "This script will create a POC with Intune configuration for roles,"
write-host "configuration, policies and applications to test Intune."
write-host
write-host "------------------------------------------------------------------"
Write-Host

#endregion

#region AAD Groups

write-host "------------------------------------------------------------------"
Write-Host

Write-Host "Adding AAD Group to Azure AD..." -ForegroundColor Cyan
Write-Host

# AAD Group Helpdesk Team
Write-Host "Adding AAD Group Helpdesk Team" -ForegroundColor Yellow

if(Get-Groups -Filter "displayName eq 'Helpdesk Team'"){

    Write-Host "AAD Group already exists..." -f Red
    Write-Host

    $HDT_Id = (Get-Groups -Filter "displayName eq 'Helpdesk Team'").id

}

else {

    $CreateResult = New-Groups -displayName "Helpdesk Team" -description "This is a sample group created for RBAC Role assignment" `
    -mailEnabled $false -mailNickname "HelpdeskTeam" -securityEnabled $true

    write-host "AAD Group created with id" $CreateResult.id

    $HDT_Id = $CreateResult.id

}

# AAD Groups Intune POC Users
Write-Host "Adding AAD Group 'Intune POC Users'" -ForegroundColor Yellow

if(Get-Groups -Filter "displayName eq 'Intune POC Users'"){

    Write-Host "AAD Group already exists..." -f Red
    Write-Host

    $AADGroupId = (Get-Groups -Filter "displayName eq 'Intune POC Users'").id

}

else {

    $CreateResult = New-Groups -displayName "Intune POC Users" -description "This is a sample group created for Intune POC Users" `
    -mailEnabled $false -mailNickname "IntunePOCUsers" -securityEnabled $true

    write-host "AAD Group created with id" $CreateResult.id

    $AADGroupId = $CreateResult.id

}

#endregion

####################################################

#region Compliance Policies

write-host "------------------------------------------------------------------"
Write-Host

Write-Host "Adding Device Compliance Policies to Intune..." -ForegroundColor Cyan
Write-Host

# Create iOS Compliance Policy
Write-Host "Adding iOS Compliance Policy from PowerShell Module..." -ForegroundColor Yellow

$iOSCompliancePolicy = New-IntuneDeviceCompliancePolicy -iosCompliancePolicy `
    -displayName "Chicago - iOS Compliance Policy" `
    -passcodeRequired $true `
    -passcodeMinimumLength 6 `
    -passcodeMinutesOfInactivityBeforeLock 15 `
    -securityBlockJailbrokenDevices $true `
    -scheduledActionsForRule `
        (New-DeviceComplianceScheduledActionForRuleObject -ruleName PasswordRequired `
            -scheduledActionConfigurations `
                (New-DeviceComplianceActionItemObject -gracePeriodHours 0 `
                -actionType block `
                -notificationTemplateId "" `
                )`
        )

write-host "Policy created with id" $iOSCompliancePolicy.id
Write-Host

$AADGroup = (Get-Groups -groupId "$AADGroupId").displayName

write-host "Assigning Device Compliance Policy to AAD Group '$AADGroup'" -f Yellow

Invoke-IntuneDeviceCompliancePolicyAssign   -deviceCompliancePolicyId $iOSCompliancePolicy.id `
                                            -assignments `
                                                (New-DeviceCompliancePolicyAssignmentObject `
                                                    -target `
                                                        (New-DeviceAndAppManagementAssignmentTargetObject `
                                                            -groupAssignmentTarget `
                                                            -groupId "$AADGroupId" `
                                                        ) `
                                                )

Write-Host "Assigned '$AADGroup' to $($iOSCompliancePolicy.displayName)/$($iOSCompliancePolicy.id)"
Write-Host

# Create Android Compliance Policy
Write-Host "Adding Android Compliance Policy from PowerShell Module..." -ForegroundColor Yellow

$androidCompliancePolicy = New-IntuneDeviceCompliancePolicy -androidCompliancePolicy `
                                                            -displayName "Chicago - Android Compliance Policy"  `
                                                            -passwordRequired $true `
                                                            -passwordMinimumLength 6 `
                                                            -securityBlockJailbrokenDevices $true `
                                                            -passwordMinutesOfInactivityBeforeLock 15 `
                                                            -scheduledActionsForRule `
                                                                (New-DeviceComplianceScheduledActionForRuleObject `
                                                                    -ruleName PasswordRequired `
                                                                    -scheduledActionConfigurations `
                                                                    (New-DeviceComplianceActionItemObject `
                                                                        -gracePeriodHours 0 `
                                                                        -actionType block `
                                                                        -notificationTemplateId "" `
                                                                    )`
                                                                )

write-host "Policy created with id" $androidCompliancePolicy.id
Write-Host

write-host "Assigning Device Compliance Policy to AAD Group '$AADGroup'" -f Yellow

Invoke-IntuneDeviceCompliancePolicyAssign -deviceCompliancePolicyId $androidCompliancePolicy.id `
    -assignments `
        (New-DeviceCompliancePolicyAssignmentObject `
        -target `
            (New-DeviceAndAppManagementAssignmentTargetObject `
                -groupAssignmentTarget `
                -groupId "$AADGroupId" `
            ) `
        )

Write-Host "Assigned '$AADGroup' to $($androidCompliancePolicy.displayName)/$($androidCompliancePolicy.id)"
Write-Host

# Create Windows 10 Compliance Policy
Write-Host "Adding Windows 10 Compliance Policy from PowerShell Module..." -ForegroundColor Yellow

$windows10CompliancePolicy = New-IntuneDeviceCompliancePolicy -windows10CompliancePolicy `
    -displayName "Chicago - Windows 10 Compliance Policy" `
    -osMinimumVersion 10.0.16299 `
    -scheduledActionsForRule `
    (New-DeviceComplianceScheduledActionForRuleObject `
        -ruleName PasswordRequired `
        -scheduledActionConfigurations `
        (New-DeviceComplianceActionItemObject `
            -gracePeriodHours 0 `
            -actionType block `
            -notificationTemplateId "" `
        ) `
    )

write-host "Policy created with id" $windows10CompliancePolicy.id
Write-Host

write-host "Assigning Device Compliance Policy to AAD Group '$AADGroup'" -f Yellow

Invoke-IntuneDeviceCompliancePolicyAssign -deviceCompliancePolicyId $windows10CompliancePolicy.id `
    -assignments `
        (New-DeviceCompliancePolicyAssignmentObject `
            -target `
            (New-DeviceAndAppManagementAssignmentTargetObject `
                -groupAssignmentTarget `
                -groupId "$AADGroupId" `
            ) `
        )

Write-Host "Assigned '$AADGroup' to $($windows10CompliancePolicy.displayName)/$($windows10CompliancePolicy.id)"
Write-Host

# Create MacOS Compliance Policy
Write-Host "Adding MacOS Compliance Policy from PowerShell Module..." -ForegroundColor Yellow

$macOSCompliancePolicy = New-IntuneDeviceCompliancePolicy -macOSCompliancePolicy `
    -displayName "Chicago - MacOS Compliance Policy" `
    -passwordRequired $true `
    -passwordBlockSimple $false `
    -passwordRequiredType deviceDefault `
    -scheduledActionsForRule `
    (New-DeviceComplianceScheduledActionForRuleObject `
        -ruleName PasswordRequired `
        -scheduledActionConfigurations `
        (New-DeviceComplianceActionItemObject `
            -gracePeriodHours 0 `
            -actionType block `
            -notificationTemplateId "" `
        ) `
    )

write-host "Policy created with id" $macOSCompliancePolicy.id
Write-Host

write-host "Assigning Device Compliance Policy to AAD Group '$AADGroup'" -f Yellow

Invoke-IntuneDeviceCompliancePolicyAssign -deviceCompliancePolicyId $macOSCompliancePolicy.id `
    -assignments `
    (New-DeviceCompliancePolicyAssignmentObject `
    -target `
        (New-DeviceAndAppManagementAssignmentTargetObject `
            -groupAssignmentTarget `
            -groupId "$AADGroupId" `
        )`
    )

Write-Host "Assigned '$AADGroup' to $($macOSCompliancePolicy.displayName)/$($macOSCompliancePolicy.id)"
Write-Host

#endregion

####################################################

#region Configuration Policies

write-host "------------------------------------------------------------------"
Write-Host

Write-Host "Adding Device Configuration Policies to Intune..." -ForegroundColor Cyan
Write-Host

Write-Host "Adding iOS Restriction Policy from PowerShell Module..." -ForegroundColor Yellow

$CreateResult = New-IntuneDeviceConfigurationPolicy -iosGeneralDeviceConfiguration -displayName "Chicago - iOS Device Restriction Policy" -iCloudBlockBackup $true -iCloudBlockDocumentSync $true -iCloudBlockPhotoStreamSync $true

write-host "Policy created with id" $CreateResult.id
Write-Host

Invoke-IntuneDeviceConfigurationPolicyAssign -deviceConfigurationId $CreateResult.id `
-assignments (New-DeviceConfigurationAssignmentObject `
-target (New-DeviceAndAppManagementAssignmentTargetObject `
-groupAssignmentTarget -groupId "$AADGroupId"))

Write-Host "Assigned '$AADGroup' to $($CreateResult.displayName)/$($CreateResult.id)"
Write-Host

Write-Host "Adding Android Restriction Policy from PowerShell Module..." -ForegroundColor Yellow

$CreateResult = New-IntuneDeviceConfigurationPolicy -androidGeneralDeviceConfiguration -displayName "Chicago - Android Device Restriction Policy" -passwordRequired $true -passwordRequiredType deviceDefault -passwordMinimumLength 4

write-host "Policy created with id" $CreateResult.id
Write-Host

Invoke-IntuneDeviceConfigurationPolicyAssign -deviceConfigurationId $CreateResult.id `
-assignments (New-DeviceConfigurationAssignmentObject `
-target (New-DeviceAndAppManagementAssignmentTargetObject `
-groupAssignmentTarget -groupId "$AADGroupId"))

Write-Host "Assigned '$AADGroup' to $($CreateResult.displayName)/$($CreateResult.id)"
Write-Host

#endregion

####################################################

#region App Protection Policies

write-host "------------------------------------------------------------------"
Write-Host

Write-Host "Adding App Protection Policies to Intune..." -ForegroundColor Cyan
Write-Host

# iOS MAM / APP Policy Creation
Write-Host "Adding iOS Managed App Policy from PowerShell Module..." -ForegroundColor Yellow

$apps_iOS = @()

$iOSAPP_apps = Get-IntuneMobileApp | ? { $_.appAvailability -eq "global" -and ($_.'@odata.type').contains("managedIOS") }

foreach($app in $iOSAPP_apps){

    $BundleId = $app.bundleId
    $apps_iOS += (New-ManagedMobileAppObject -mobileAppIdentifier (New-MobileAppIdentifierObject -iosMobileAppIdentifier -bundleId "$BundleId"))

}

$CreateResult = New-IntuneAppProtectionPolicy -iosManagedAppProtection -displayName "iOS MAM / APP Policy" `
-periodOfflineBeforeAccessCheck (New-TimeSpan -Hours 12) `
-periodOnlineBeforeAccessCheck (New-TimeSpan -Minutes 30)`
-allowedInboundDataTransferSources managedApps -allowedOutboundDataTransferDestinations managedApps `
-allowedOutboundClipboardSharingLevel managedAppsWithPasteIn -organizationalCredentialsRequired $false `
-dataBackupBlocked $true -managedBrowserToOpenLinksRequired $false -deviceComplianceRequired $false `
-saveAsBlocked $true -periodOfflineBeforeWipeIsEnforced (New-TimeSpan -Days 30) `
-pinRequired $true -maximumPinRetries 5 -simplePinBlocked $false -minimumPinLength 4 `
-pinCharacterSet numeric -periodBeforePinReset (New-TimeSpan -Days 30) -allowedDataStorageLocations @("oneDriveForBusiness","sharePoint") `
-contactSyncBlocked $false -printBlocked $true -fingerprintBlocked $false `
-disableAppPinIfDevicePinIsSet $false `
-apps $apps_iOS

write-host "Policy created with id" $CreateResult.id 
Write-Host

<#
Invoke-IntuneDeviceConfigurationPolicyAssign -deviceConfigurationId $CreateResult.id `
-assignments (New-DeviceConfigurationAssignmentObject `
-target (New-DeviceAndAppManagementAssignmentTargetObject `
-groupAssignmentTarget -groupId "$AADGroupId"))
#>

# iOS MAM / APP Policy Creation
Write-Host "Adding iOS Managed App Policy from PowerShell Module..." -ForegroundColor Yellow

$apps_Android = @()

$AndroidAPP_apps = Get-IntuneMobileApp | ? { $_.appAvailability -eq "global" -and ($_.'@odata.type').contains("managedAndroid") }

foreach($app in $AndroidAPP_apps){

    $PackageId = $app.packageId
    $apps_Android += (New-ManagedMobileAppObject -mobileAppIdentifier (New-MobileAppIdentifierObject -androidMobileAppIdentifier -packageId "$PackageId"))

}

$CreateResult = New-IntuneAppProtectionPolicy -androidManagedAppProtection -displayName "Android MAM / APP Policy" `
-periodOfflineBeforeAccessCheck (New-TimeSpan -Hours 12) `
-periodOnlineBeforeAccessCheck (New-TimeSpan -Minutes 30)`
-allowedInboundDataTransferSources managedApps -allowedOutboundDataTransferDestinations managedApps `
-allowedOutboundClipboardSharingLevel managedAppsWithPasteIn -organizationalCredentialsRequired $false `
-dataBackupBlocked $true -managedBrowserToOpenLinksRequired $false -deviceComplianceRequired $false `
-saveAsBlocked $true -periodOfflineBeforeWipeIsEnforced (New-TimeSpan -Days 30) `
-pinRequired $true -maximumPinRetries 5 -simplePinBlocked $false -minimumPinLength 4 `
-pinCharacterSet numeric -periodBeforePinReset (New-TimeSpan -Days 30) -allowedDataStorageLocations @("oneDriveForBusiness","sharePoint") `
-contactSyncBlocked $false -printBlocked $true `
-disableAppPinIfDevicePinIsSet $false -screenCaptureBlocked $true `
-apps $apps_Android

write-host "Policy created with id" $CreateResult.id 
Write-Host

#endregion

####################################################

#region Script End

write-host "------------------------------------------------------------------"
Write-Host

$EDate = get-date

$ScriptTime = $Edate - $SDate

$ScriptMinutes = $ScriptTime.Minutes
$ScriptSeconds = $ScriptTime.Seconds

write-host "Script took" $ScriptTime.Minutes "minute and" $ScriptTime.seconds "seconds to complete..." -f Green
Write-Host
write-host "------------------------------------------------------------------"
Write-Host

#endregion