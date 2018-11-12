#
# Import the Intune PowerShell SDK Module if necessary
#
if ((Get-Module 'Microsoft.Graph.Intune') -eq $null)
{
    #
    # BUGBUG: Pass this as parameter on cmdline
    #
    $moduleInstallFolderPath="..\src\GraphODataPowerShellWriter\bin\Release\output\bin\Release\net471"
    Import-Module $moduleInstallFolderPath\Microsoft.Graph.Intune.psd1
}

#
# Declare the cmdlets the test along with their inputs
# TODO: Data drive the inputs
# TODO: Auto-generate the list of cmdlets to test
#

$notImplTests = @(
    {((Get-DeviceAppMgtAndroidMgdAppProts | Get-MSGraphAllPages)[0]) | Get-DeviceAppMgtAndroidMgdAppProtsAssignments}
    {((Get-DeviceAppMgtAndroidMgdAppProts | Get-MSGraphAllPages)[0]) | Get-DeviceAppMgtAndroidMgdAppProtsDeploymentSummary}
    {Get-DeviceAppMgtDefaultMgdAppProtsApps}
    {Get-DeviceAppMgtDefaultMgdAppProtsDeploymentSummary}
    {(Get-DeviceAppMgtIosMgdAppProts | Get-MSGraphAllPages)[0] | Get-DeviceAppMgtIosMgdAppProtsAssignments}
    {(Get-DeviceAppMgtIosMgdAppProts | Get-MSGraphAllPages)[0] | Get-DeviceAppMgtIosMgdAppProtsDeploymentSummary}
    {Get-DeviceAppMgtMgdAppPoliciesApps -managedAppPolicyId $env:managedAppPolicyId -managedAppPolicyODataType 'microsoft.graph.managedAppPolicy'}
    {Get-DeviceAppMgtMgdAppPoliciesAssignments -managedAppPolicyId $env:managedAppPolicyId -managedAppPolicyODataType 'microsoft.graph.managedAppPolicy'}
    {Get-DeviceAppMgtMgdAppPoliciesDeploymentSummary -managedAppPolicyId $env:managedAppPolicyId -managedAppPolicyODataType 'microsoft.graph.managedAppPolicy'}
    {Get-DeviceAppMgtMgdAppPoliciesExemptAppLockerFiles -managedAppPolicyId $env:managedAppPolicyId -managedAppPolicyODataType 'microsoft.graph.managedAppPolicy'}
    {Get-DeviceAppMgtMgdAppPoliciesProtectedAppLockerFiles -managedAppPolicyId $env:managedAppPolicyId -managedAppPolicyODataType 'microsoft.graph.managedAppPolicy'}
    {Get-DeviceAppMgtMdmWindowsInformationProtectionPoliciesAssignments -mdmWindowsInformationProtectionPolicyId $env:mdmWindowsInformationProtectionPolicyId -mdmWindowsInformationProtectionPolicyODataType 'microsoft.graph.mdmWindowsInformationProtectionPolicy'}    
    {Get-DeviceAppMgtMdmWindowsInformationProtectionPoliciesExemptAppLockerFiles -mdmWindowsInformationProtectionPolicyId $env:mdmWindowsInformationProtectionPolicyId -mdmWindowsInformationProtectionPolicyODataType 'microsoft.graph.mdmWindowsInformationProtectionPolicy'}
    {Get-DeviceAppMgtMdmWindowsInformationProtectionPoliciesProtectedAppLockerFiles -mdmWindowsInformationProtectionPolicyId $env:mdmWindowsInformationProtectionPolicyId -mdmWindowsInformationProtectionPolicyODataType 'microsoft.graph.mdmWindowsInformationProtectionPolicy'}
   
    {((Get-DeviceAppMgtWinInfoProtPolicies | Get-MSGraphAllPages)[0]) | Get-DeviceAppMgtWinInfoProtPoliciesAssignments}
    {((Get-DeviceAppMgtWinInfoProtPolicies | Get-MSGraphAllPages)[0]) | Get-DeviceAppMgtWinInfoProtPoliciesExemptAppLockerFiles}
    {((Get-DeviceAppMgtWinInfoProtPolicies | Get-MSGraphAllPages)[0]) | Get-DeviceAppMgtWinInfoProtPoliciesProtectedAppLockerFiles}
    {(Get-DeviceAppMgtMobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-DeviceAppMgtMobileAppsCategoriesReferences}
    {(Get-DeviceAppMgtMobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-DeviceAppMgtMobileAppsContentVersions}
)

$tests = @(
#
# DeviceAppManagement Singleton
#
    {Get-DeviceAppMgt}
#
# AndroidManagedAppProtectionApps
#    
    {(Get-DeviceAppMgtAndroidMgdAppProts| Get-MSGraphAllPages)[0] | Get-DeviceAppMgtAndroidMgdAppProtsApps}
#
# DefaultManagedAppProtections
#
    {Get-DeviceAppMgtDefaultMgdAppProts}
#
# IodManagedAppProtections
#    
    {(Get-DeviceAppMgtIosMgdAppProts | Get-MSGraphAllPages)[0] | Get-DeviceAppMgtIosMgdAppProtsApps}  
#
# ManagedAppStatuses
#
    {Get-DeviceAppMgtManagedAppStatuses}
#
# ManagedEBooks
#
    {$env:managedEBookId=(Get-DeviceAppMgtManagedEBooks)[0].id}
    {Get-DeviceAppMgtManagedEBooksAssignments -managedEBookId $env:managedEBookId}
    {Get-DeviceAppMgtManagedEBooksDeviceStates -managedEBookId $env:managedEBookId}
    {Get-DeviceAppMgtManagedEBooksInstallSummary -managedEBookId $env:managedEBookId}
    #{$env:userInstallStateSummaryId=Get-DeviceAppMgtManagedEBooksUserStateSummary -managedEBookId $env:managedEBookId)[0].id}
    #{Get-DeviceAppMgtManagedEBooksUserStateSummaryDeviceStates -managedEBookId $env:managedEBookId -userInstallStateSummaryId $env:userInstallStateSummaryId}
#
# mdmWindowsInformationProtectionPolicy
#
    {(Get-DeviceAppMgtMdmWindowsInformationProtectionPolicies | Get-MSGraphAllPages)}
#
# managedAppPolicies
#
    {(Get-DeviceAppMgtMgdAppPolicies | Get-MSGraphAllPages)[0]}
#
# managedAppRegistrations
#    
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsAppliedPolicies}
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsAppliedPoliciesApps}
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsAppliedPoliciesAssignments}
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsAppliedPoliciesDeploymentSummary}
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsAppliedPoliciesExemptAppLockerFiles}
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsAppliedPoliciesProtectedAppLockerFiles}
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsIntendedPolicies}
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsIntendedPoliciesApps}
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsIntendedPoliciesAssignments}
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsIntendedPoliciesDeploymentSummary}
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsIntendedPoliciesExemptAppLockerFiles}
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsIntendedPoliciesProtectedAppLockerFiles}
    {Get-DeviceAppMgtMgdAppRegs | Get-DeviceAppMgtMgdAppRegsOperations}
#
# MobileAppCategories
#
    {Get-DeviceAppMgtMobileAppCategories}
#
# MobileAppConfigurations
#    
    {((Get-DeviceAppMgtMobileAppConfigurations | Get-MSGraphAllPages)[0]) | Get-DeviceAppMgtMobileAppConfigurationsAssignments}
    {((Get-DeviceAppMgtMobileAppConfigurations | Get-MSGraphAllPages)[0]) | Get-DeviceAppMgtMobileAppConfigurationsDeviceStatuses}
    {((Get-DeviceAppMgtMobileAppConfigurations | Get-MSGraphAllPages)[0]) | Get-DeviceAppMgtMobileAppConfigurationsDeviceStatusSummary}
    {((Get-DeviceAppMgtMobileAppConfigurations | Get-MSGraphAllPages)[0]) | Get-DeviceAppMgtMobileAppConfigurationsUserStatuses}
    {((Get-DeviceAppMgtMobileAppConfigurations | Get-MSGraphAllPages)[0]) | Get-DeviceAppMgtMobileAppConfigurationsUserStatusSummary}
#
# MobileApps
#
    {(Get-DeviceAppMgtMobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-DeviceAppMgtMobileAppsAssignments}
    {(Get-DeviceAppMgtMobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-DeviceAppMgtMobileAppsCategories}        
    #{(Get-DeviceAppMgtMobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-DeviceAppMgtMobileAppsContentVersionsFiles}
#
# TargetedManagedAppConfigurations
#

    {(Get-DeviceAppMgtTgtdMgdAppConfigs | Get-MSGraphAllPages) | Get-DeviceAppMgtTgtdMgdAppConfigsApps}
    {(Get-DeviceAppMgtTgtdMgdAppConfigs | Get-MSGraphAllPages) | Get-DeviceAppMgtTgtdMgdAppConfigsAssignments}
    {(Get-DeviceAppMgtTgtdMgdAppConfigs | Get-MSGraphAllPages) | Get-DeviceAppMgtTgtdMgdAppConfigsDeploymentSummary}
#
# VppTokens
#
    {(Get-DeviceAppMgtVppTokens | Get-MSGraphAllPages)}
#
# windowsInformationProtectionPolicies
#
    {(Get-DeviceAppMgtWinInfoProtPolicies | Get-MSGraphAllPages)}
)

#
# Connect to MSGraph if necessary
#
try
{
    Get-MSGraphMetadata
}
catch
{    
    $adminUPN = 'admin@roramutesta063.onmicrosoft.com'
    $adminPwd = Read-Host -AsSecureString -Prompt "Enter pwd for $adminUPN"
    $creds = New-Object System.Management.Automation.PSCredential ($adminUPN, $adminPwd)
    $connection = Connect-MSGraph -PSCredential $creds
}

#
# Run the tests
#
foreach ($test in $tests)
{
    try
    {        
        $output = Invoke-Command -scriptblock $($test)
        Write-Output "$test,$output"
    }
    catch
    {
        Write-Warning "$test,$_"
    }
}