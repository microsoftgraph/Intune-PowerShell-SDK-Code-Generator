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
$tests = @(
#
# DeviceAppManagement Singleton
#
    {Get-DeviceAppManagement}
#
# AndroidManagedAppProtectionApps
#    
    {(Get-AndroidManagedAppProtections| Get-MSGraphAllPages)[0] | Get-AppsWithAndroidManagedAppProtections}
#
# DefaultManagedAppProtections
#
    {Get-DefaultManagedAppProtections}
#
# IodManagedAppProtections
#    
    {(Get-IosManagedAppProtections | Get-MSGraphAllPages)[0] | Get-AppsWithIosManagedAppProtections}  
#
# ManagedAppStatuses
#
    {Get-DeviceAppManagement_ManagedAppStatuses}
#
# ManagedEBooks
#
    {$env:managedEBookId=(Get-DeviceAppManagement_ManagedEBooks)[0].id}
    {Get-DeviceAppManagement_ManagedEBooks_Assignments -managedEBookId $env:managedEBookId}
    {Get-DeviceAppManagement_ManagedEBooks_DeviceStates -managedEBookId $env:managedEBookId}
    {Get-DeviceAppManagement_ManagedEBooks_InstallSummary -managedEBookId $env:managedEBookId}
    #{$env:userInstallStateSummaryId=Get-DeviceAppManagement_ManagedEBooks_UserStateSummary -managedEBookId $env:managedEBookId)[0].id}
    #{Get-DeviceAppManagement_ManagedEBooks_UserStateSummaryDeviceStates -managedEBookId $env:managedEBookId -userInstallStateSummaryId $env:userInstallStateSummaryId}
#
# mdmWindowsInformationProtectionPolicy
#
    {(Get-DeviceAppManagement_MdmWindowsInformationProtectionPolicies | Get-MSGraphAllPages)}
#
# managedAppPolicies
#
    {(Get-ManagedAppPolicies | Get-MSGraphAllPages)[0]}
#
# managedAppRegistrations
#    
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies}
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_Apps}
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_Assignments}
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_DeploymentSummary}
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_ExemptAppLockerFiles}
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_ProtectedAppLockerFiles}
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies}
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_Apps}
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_Assignments}
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_DeploymentSummary}
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_ExemptAppLockerFiles}
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_ProtectedAppLockerFiles}
    {Get-DeviceAppManagement_ManagedAppRegistrations | Get-DeviceAppManagement_ManagedAppRegistrations_Operations}
#
# MobileAppCategories
#
    {Get-DeviceAppManagement_MobileAppConfigurations}
#
# MobileAppConfigurations
#    
    {((Get-DeviceAppManagement_MobileAppConfigurations | Get-MSGraphAllPages)[0]) | Get-DeviceAppManagement_MobileAppConfigurations_Assignments}
    {((Get-DeviceAppManagement_MobileAppConfigurations | Get-MSGraphAllPages)[0]) | Get-DeviceAppManagement_MobileAppConfigurations_DeviceStatuses}
    {((Get-DeviceAppManagement_MobileAppConfigurations | Get-MSGraphAllPages)[0]) | Get-DeviceAppManagement_MobileAppConfigurations_DeviceStatusSummary}
    {((Get-DeviceAppManagement_MobileAppConfigurations | Get-MSGraphAllPages)[0]) | Get-DeviceAppManagement_MobileAppConfigurations_UserStatuses}
    {((Get-DeviceAppManagement_MobileAppConfigurations | Get-MSGraphAllPages)[0]) | Get-DeviceAppManagement_MobileAppConfigurations_UserStatusSummary}
#
# MobileApps
#
    {(Get-DeviceAppManagement_MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-DeviceAppManagement_MobileApps_Assignments}
    {(Get-DeviceAppManagement_MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-DeviceAppManagement_MobileApps_Categories}        
    #{(Get-DeviceAppManagement_MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-DeviceAppManagement_MobileApps_ContentVersionsFiles}
#
# TargetedManagedAppConfigurations
#

    {(Get-DeviceAppManagement_TargetedManagedAppConfigurations | Get-MSGraphAllPages) | Get-DeviceAppManagement_TargetedManagedAppConfigurations_Apps}
    {(Get-DeviceAppManagement_TargetedManagedAppConfigurations | Get-MSGraphAllPages) | Get-DeviceAppManagement_TargetedManagedAppConfigurations_Assignments}
    {(Get-DeviceAppManagement_TargetedManagedAppConfigurations | Get-MSGraphAllPages) | Get-DeviceAppManagement_TargetedManagedAppConfigurations_DeploymentSummary}
#
# VppTokens
#
    {(Get-DeviceAppManagement_VppTokens | Get-MSGraphAllPages)}
#
# windowsInformationProtectionPolicies
#
    {(Get-DeviceAppManagement_WindowsInformationProtectionPolicies | Get-MSGraphAllPages)}
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