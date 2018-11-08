#
# Import the Intune PowerShell SDK Module
#
pushd ..\src\GraphODataPowerShellWriter\bin\Release\output\bin\Release\net471
Import-Module .\Microsoft.Graph.Intune.psd1

#
# Declare the cmdlets the test along with their inputs
# TODO: Data drive the inputs
# TODO: Auto-generate the list of cmdlets to test
#

$tests = @(
    {Get-DeviceAppMgt}
    {$env:androidManagedAppProtectionId=(Get-DeviceAppMgtAndroidMgdAppProts)[0].id}
    {Get-DeviceAppMgtAndroidMgdAppProtsApps -androidManagedAppProtectionId $env:androidManagedAppProtectionId}
    {Get-DeviceAppMgtAndroidMgdAppProtsAssignments -androidManagedAppProtectionId $env:androidManagedAppProtectionId -androidManagedAppProtectionODataType 'microsoft.graph.androidManagedAppProtection'}
    {Get-DeviceAppMgtAndroidMgdAppProtsDeploymentSummary -androidManagedAppProtectionId $env:androidManagedAppProtectionId}
    #{Get-DeviceAppMgtDefaultMgdAppProts}
    #{Get-DeviceAppMgtDefaultMgdAppProtsApps}
    #{Get-DeviceAppMgtDefaultMgdAppProtsDeploymentSummary}
    {$env:iosManagedAppProtectionId=(Get-DeviceAppMgtIosMgdAppProts)[0].id}
    {Get-DeviceAppMgtIosMgdAppProtsApps -iosManagedAppProtectionId $env:iosManagedAppProtectionId}
    {Get-DeviceAppMgtIosMgdAppProtsAssignments -iosManagedAppProtectionId $env:iosManagedAppProtectionId -iosManagedAppProtectionODataType 'microsoft.graph.iosManagedAppProtection'}
    {Get-DeviceAppMgtIosMgdAppProtsDeploymentSummary -iosManagedAppProtectionId $env:iosManagedAppProtectionId}
    {Get-DeviceAppMgtManagedAppStatuses}
    {$env:managedEBookId=(Get-DeviceAppMgtManagedEBooks)[0].id}
    {Get-DeviceAppMgtManagedEBooksAssignments -managedEBookId $env:managedEBookId}
    {Get-DeviceAppMgtManagedEBooksDeviceStates -managedEBookId $env:managedEBookId}
    {Get-DeviceAppMgtManagedEBooksInstallSummary -managedEBookId $env:managedEBookId}
    #{$env:userInstallStateSummaryId=Get-DeviceAppMgtManagedEBooksUserStateSummary -managedEBookId $env:managedEBookId)[0].id}
    #{Get-DeviceAppMgtManagedEBooksUserStateSummaryDeviceStates -managedEBookId $env:managedEBookId -userInstallStateSummaryId $env:userInstallStateSummaryId}
    {$env:mdmWindowsInformationProtectionPolicyId=(Get-DeviceAppMgtMdmWindowsInformationProtectionPolicies)[0].id}
    {Get-DeviceAppMgtMdmWindowsInformationProtectionPoliciesAssignments -mdmWindowsInformationProtectionPolicyId $env:mdmWindowsInformationProtectionPolicyId -mdmWindowsInformationProtectionPolicyODataType 'microsoft.graph.mdmWindowsInformationProtectionPolicy'}
    {Get-DeviceAppMgtMdmWindowsInformationProtectionPoliciesExemptAppLockerFiles -mdmWindowsInformationProtectionPolicyId $env:mdmWindowsInformationProtectionPolicyId -mdmWindowsInformationProtectionPolicyODataType 'microsoft.graph.mdmWindowsInformationProtectionPolicy'}
    {Get-DeviceAppMgtMdmWindowsInformationProtectionPoliciesProtectedAppLockerFiles -mdmWindowsInformationProtectionPolicyId $env:mdmWindowsInformationProtectionPolicyId -mdmWindowsInformationProtectionPolicyODataType 'microsoft.graph.mdmWindowsInformationProtectionPolicy'}
    {$env:managedAppPolicyId=(Get-DeviceAppMgtMgdAppPolicies)[0].id}
    {Get-DeviceAppMgtMgdAppPoliciesApps -managedAppPolicyId $env:managedAppPolicyId -managedAppPolicyODataType 'microsoft.graph.managedAppPolicy'}
    {Get-DeviceAppMgtMgdAppPoliciesAssignments -managedAppPolicyId $env:managedAppPolicyId -managedAppPolicyODataType 'microsoft.graph.managedAppPolicy'}
    {Get-DeviceAppMgtMgdAppPoliciesDeploymentSummary -managedAppPolicyId $env:managedAppPolicyId -managedAppPolicyODataType 'microsoft.graph.managedAppPolicy'}
    {Get-DeviceAppMgtMgdAppPoliciesExemptAppLockerFiles -managedAppPolicyId $env:managedAppPolicyId -managedAppPolicyODataType 'microsoft.graph.managedAppPolicy'}
    {Get-DeviceAppMgtMgdAppPoliciesProtectedAppLockerFiles -managedAppPolicyId $env:managedAppPolicyId -managedAppPolicyODataType 'microsoft.graph.managedAppPolicy'}
    {Get-DeviceAppMgtMgdAppRegs}
    #{Get-DeviceAppMgtMgdAppRegsAppliedPolicies}
    #{Get-DeviceAppMgtMgdAppRegsAppliedPoliciesApps}
    #{Get-DeviceAppMgtMgdAppRegsAppliedPoliciesAssignments}
    #{Get-DeviceAppMgtMgdAppRegsAppliedPoliciesDeploymentSummary}
    #{Get-DeviceAppMgtMgdAppRegsAppliedPoliciesExemptAppLockerFiles}
    #{Get-DeviceAppMgtMgdAppRegsAppliedPoliciesProtectedAppLockerFiles}
    #{Get-DeviceAppMgtMgdAppRegsIntendedPolicies}
    #{Get-DeviceAppMgtMgdAppRegsIntendedPoliciesApps}
    #{Get-DeviceAppMgtMgdAppRegsIntendedPoliciesAssignments}
    #{Get-DeviceAppMgtMgdAppRegsIntendedPoliciesDeploymentSummary}
    #{Get-DeviceAppMgtMgdAppRegsIntendedPoliciesExemptAppLockerFiles}
    #{Get-DeviceAppMgtMgdAppRegsIntendedPoliciesProtectedAppLockerFiles}
    #{Get-DeviceAppMgtMgdAppRegsOperations}
    {Get-DeviceAppMgtMobileAppCategories}
    {Get-DeviceAppMgtMobileAppConfigurations}
    #{Get-DeviceAppMgtMobileAppConfigurationsAssignments}
    #{Get-DeviceAppMgtMobileAppConfigurationsDeviceStatuses}
    #{Get-DeviceAppMgtMobileAppConfigurationsDeviceStatusSummary}
    #{Get-DeviceAppMgtMobileAppConfigurationsUserStatuses}
    #{Get-DeviceAppMgtMobileAppConfigurationsUserStatusSummary}
    {Get-DeviceAppMgtMobileApps}
    #{Get-DeviceAppMgtMobileAppsAssignments}
    #{Get-DeviceAppMgtMobileAppsCategories}
    #{Get-DeviceAppMgtMobileAppsCategoriesReferences}
    #{Get-DeviceAppMgtMobileAppsContentVersions}
    #{Get-DeviceAppMgtMobileAppsContentVersionsFiles}
    #{Get-DeviceAppMgtTgtdMgdAppConfigs}
    #{Get-DeviceAppMgtTgtdMgdAppConfigsApps}
    #{Get-DeviceAppMgtTgtdMgdAppConfigsAssignments}
    #{Get-DeviceAppMgtTgtdMgdAppConfigsDeploymentSummary}
    {Get-DeviceAppMgtVppTokens}
    {$env:windowsInformationProtectionPolicyId=(Get-DeviceAppMgtWinInfoProtPolicies)[0].id}
    {Get-DeviceAppMgtWinInfoProtPoliciesAssignments -windowsInformationProtectionPolicyId $env:windowsInformationProtectionPolicyId -windowsInformationProtectionPolicyODataType 'microsoft.graph.windowsInformationProtectionPolicy'}
    {Get-DeviceAppMgtWinInfoProtPoliciesExemptAppLockerFiles -windowsInformationProtectionPolicyId $env:windowsInformationProtectionPolicyId -windowsInformationProtectionPolicyODataType 'microsoft.graph.windowsInformationProtectionPolicy'}
    {Get-DeviceAppMgtWinInfoProtPoliciesProtectedAppLockerFiles -windowsInformationProtectionPolicyId $env:windowsInformationProtectionPolicyId -windowsInformationProtectionPolicyODataType 'microsoft.graph.windowsInformationProtectionPolicy'}
)

#
# Connect to MSGraph
#
$adminUPN = 'admin@roramutesta063.onmicrosoft.com'
$adminPwd = Read-Host -AsSecureString -Prompt "Enter pwd for $adminUPN"
$creds = New-Object System.Management.Automation.PSCredential ($adminUPN, $adminPwd)
$connection = Connect-MSGraph -PSCredential $creds

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