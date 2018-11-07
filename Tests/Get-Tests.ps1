pushd ..\src\GraphODataPowerShellWriter\bin\Release\output\bin\Release\net471
Import-Module .\Microsoft.Graph.Intune.psd1

$tests = @(
    "Get-DeviceAppMgt"
    "Get-DeviceAppMgtAndroidMgdAppProts -androidManagedAppProtectionId 'T_4a4cdc44-3a87-4d66-81a4-24f31ff2b5af'"
    "Get-DeviceAppMgtAndroidMgdAppProtsApps -androidManagedAppProtectionId 'T_4a4cdc44-3a87-4d66-81a4-24f31ff2b5af'"
    "Get-DeviceAppMgtAndroidMgdAppProtsAssignments"
    "Get-DeviceAppMgtAndroidMgdAppProtsDeploymentSummary"
    "Get-DeviceAppMgtDefaultMgdAppProts"
    "Get-DeviceAppMgtDefaultMgdAppProtsApps"
    "Get-DeviceAppMgtDefaultMgdAppProtsDeploymentSummary"
    "Get-DeviceAppMgtIosMgdAppProts"
    "Get-DeviceAppMgtIosMgdAppProtsApps"
    "Get-DeviceAppMgtIosMgdAppProtsAssignments"
    "Get-DeviceAppMgtIosMgdAppProtsDeploymentSummary"
    "Get-DeviceAppMgtMgdAppPolicies"
    "Get-DeviceAppMgtMgdAppPoliciesApps"
    "Get-DeviceAppMgtMgdAppPoliciesAssignments"
    "Get-DeviceAppMgtMgdAppPoliciesDeploymentSummary"
    "Get-DeviceAppMgtMgdAppPoliciesExemptAppLockerFiles"
    "Get-DeviceAppMgtMgdAppPoliciesProtectedAppLockerFiles"
    "Get-DeviceAppMgtMgdAppRegs"
    "Get-DeviceAppMgtMgdAppRegsAppliedPolicies"
    "Get-DeviceAppMgtMgdAppRegsAppliedPoliciesApps"
    "Get-DeviceAppMgtMgdAppRegsAppliedPoliciesAssignments"
    "Get-DeviceAppMgtMgdAppRegsAppliedPoliciesDeploymentSummary"
    "Get-DeviceAppMgtMgdAppRegsAppliedPoliciesExemptAppLockerFiles"
    "Get-DeviceAppMgtMgdAppRegsAppliedPoliciesProtectedAppLockerFiles"
    "Get-DeviceAppMgtMgdAppRegsIntendedPolicies"
    "Get-DeviceAppMgtMgdAppRegsIntendedPoliciesApps"
    "Get-DeviceAppMgtMgdAppRegsIntendedPoliciesAssignments"
    "Get-DeviceAppMgtMgdAppRegsIntendedPoliciesDeploymentSummary"
    "Get-DeviceAppMgtMgdAppRegsIntendedPoliciesExemptAppLockerFiles"
    "Get-DeviceAppMgtMgdAppRegsIntendedPoliciesProtectedAppLockerFiles"
    "Get-DeviceAppMgtMgdAppRegsOperations"
    "Get-DeviceAppMgtManagedAppStatuses"
    "Get-DeviceAppMgtManagedEBooks"
    "Get-DeviceAppMgtManagedEBooksAssignments"
    "Get-DeviceAppMgtManagedEBooksDeviceStates"
    "Get-DeviceAppMgtManagedEBooksInstallSummary"
    "Get-DeviceAppMgtManagedEBooksUserStateSummary"
    "Get-DeviceAppMgtManagedEBooksUserStateSummaryDeviceStates"
    "Get-DeviceAppMgtMdmWindowsInformationProtectionPolicies"
    "Get-DeviceAppMgtMdmWindowsInformationProtectionPoliciesAssignments"
    "Get-DeviceAppMgtMdmWindowsInformationProtectionPoliciesExemptAppLockerFiles"
    "Get-DeviceAppMgtMdmWindowsInformationProtectionPoliciesProtectedAppLockerFiles"
    "Get-DeviceAppMgtMobileAppCategories"
    "Get-DeviceAppMgtMobileAppConfigurations"
    "Get-DeviceAppMgtMobileAppConfigurationsAssignments"
    "Get-DeviceAppMgtMobileAppConfigurationsDeviceStatuses"
    "Get-DeviceAppMgtMobileAppConfigurationsDeviceStatusSummary"
    "Get-DeviceAppMgtMobileAppConfigurationsUserStatuses"
    "Get-DeviceAppMgtMobileAppConfigurationsUserStatusSummary"
    "Get-DeviceAppMgtMobileApps"
    "Get-DeviceAppMgtMobileAppsAssignments"
    "Get-DeviceAppMgtMobileAppsCategories"
    "Get-DeviceAppMgtMobileAppsCategoriesReferences"
    "Get-DeviceAppMgtMobileAppsContentVersions"
    "Get-DeviceAppMgtMobileAppsContentVersionsFiles"
    "Get-DeviceAppMgtTgtdMgdAppConfigs"
    "Get-DeviceAppMgtTgtdMgdAppConfigsApps"
    "Get-DeviceAppMgtTgtdMgdAppConfigsAssignments"
    "Get-DeviceAppMgtTgtdMgdAppConfigsDeploymentSummary"
    "Get-DeviceAppMgtVppTokens"
    "Get-DeviceAppMgtWinInfoProtPolicies"
    "Get-DeviceAppMgtWinInfoProtPoliciesAssignments"
    "Get-DeviceAppMgtWinInfoProtPoliciesExemptAppLockerFiles"
    "Get-DeviceAppMgtWinInfoProtPoliciesProtectedAppLockerFiles"
)

Connect-MSGraph
foreach ($test in $tests)
{
    try
    {
        Write-Output $test
        & $test
    }
    catch
    {
        Write-Output $_.Exception
    }
}