#
# Import the Intune PowerShell SDK Module if necessary
#
if ((Get-Module 'Microsoft.Graph.Intune') -eq $env:null)
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
    {(Get-AndroidAPP | Get-MSGraphAllPages)[0] | Get-AndroidAPPApps}
#
# DefaultManagedAppProtections
#
    {Get-DefaultAPP}
#
# IodManagedAppProtections
#    
    {(Get-IosAPP | Get-MSGraphAllPages)[0] | Get-IosAPPApps}  
#
# ManagedEBooks
#    
    {(Get-ManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBookAssignments}
    {(Get-ManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBooksDeviceStates}
    {(Get-ManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBooksInstallSummary}
    {$env:managedEBooksUserState = (Get-ManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBooksUserStateSummary}
    {if ($env:managedEBooksUserState -ne $env:null) {(Get-ManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBooksUserStateSummaryDeviceStates}}
#
# mdmWindowsInformationProtectionPolicy
#
    {$env:mdmWindowsInfoProtPolicy = (Get-MdmWindowsInformationProtectionPolicies | Get-MSGraphAllPages)[0]}
    #BUGBUG: Missing Route {(Get-MdmWindowsInformationProtectionPolicies | Get-MSGraphAllPages)[0] | Get-MdmWindowsInformationProtectionPoliciesAssignments}
    #BUGBUG: Missing Route {(Get-MdmWindowsInformationProtectionPolicies | Get-MSGraphAllPages)[0] | Get-MdmWindowsInformationProtectionPoliciesExemptAppLockerFiles}
    #BUGBUG: Missing Route {(Get-MdmWindowsInformationProtectionPolicies | Get-MSGraphAllPages)[0] | Get-MdmWindowsInformationProtectionPoliciesProtectedAppLockerFiles}
#
# managedAppPolicies
#
    {(Get-ManagedAppPolicies | Get-MSGraphAllPages)[0]}
#
# managedAppRegistrations
#    
    {Get-AppRegistrations | Get-AppRegistrationPolicies}
    {Get-AppRegistrations | Get-AppRegistrationApps}
    {Get-AppRegistrations | Get-AppRegistrationAssignments}
    {Get-AppRegistrations | Get-AppRegistrationDeploymentSummary}
    {Get-AppRegistrations | Get-AppRegistrationExemptAppLockerFiles}
    {Get-AppRegistrations | Get-AppRegistrationProtectedAppLockerFiles}
    {Get-AppRegistrations | Get-IntendedPoliciesForAppRegistrations}
    {Get-AppRegistrations | Get-AppsWithIntendedPoliciesForAppRegistrations}
    {Get-AppRegistrations | Get-AssignmentsOfIntendedPoliciesForAppRegistrations}
    {Get-AppRegistrations | Get-DeploymentSummaryOfIntendedPoliciesForAppRegistrations}
    {Get-AppRegistrations | Get-ExemptAppLockerFilesOfIntendedPoliciesForAppRegistrations}
    {Get-AppRegistrations | Get-ProtectedAppLockerFilesOfIntendedPoliciesForAppRegistrations}
    {Get-AppRegistrations | Get-ManagedAppRegistrationsOperations}
#
# ManagedAppStatuses
#
    {(Get-ManagedAppStatuses | Get-MSGraphAllPages)[0]}
#
# MobileAppCategories
#
    {Get-MobileAppCategories}
#
# MobileAppConfigurations
#    
    {$env:mobileAppConfig = (Get-MobileAppConfigurations | Get-MSGraphAllPages)[0]}
    {(Get-MobileAppConfigurations | Get-MSGraphAllPages)[0] | Get-MobileAppConfigurationAssignments}
    {(Get-MobileAppConfigurations | Get-MSGraphAllPages)[0] | Get-MobileAppConfigurationDeviceStatuses}
    {(Get-MobileAppConfigurations | Get-MSGraphAllPages)[0] | Get-MobileAppConfigurationDeviceStatusSummary}
    {(Get-MobileAppConfigurations | Get-MSGraphAllPages)[0] | Get-MobileAppConfigurationsUserStatuses}
    {(Get-MobileAppConfigurations | Get-MSGraphAllPages)[0] | Get-MobileAppConfigurationsUserStatusSummary}
#
# MobileApps
#
    {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsAssignments}
    {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsCategories}
    {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsCategoriesReferences}
    {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsContentVersions}
    {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsContentVersionsFiles}
#
# TargetedManagedAppConfigurations
#

    {(Get-DeviceAppManagement_TargetedManagedAppConfigurations | Get-MSGraphAllPages) | Get-DeviceAppManagement_TargetedManagedAppConfigurations_Apps}
    {(Get-DeviceAppManagement_TargetedManagedAppConfigurations | Get-MSGraphAllPages) | Get-DeviceAppManagement_TargetedManagedAppConfigurations_Assignments}
    {(Get-DeviceAppManagement_TargetedManagedAppConfigurations | Get-MSGraphAllPages) | Get-DeviceAppManagement_TargetedManagedAppConfigurations_DeploymentSummary}
#
# VppTokens
#
    {(Get-VppTokens | Get-MSGraphAllPages)}
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
    $env:msGraphMeta = Get-MSGraphMetadata
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
        $output = Invoke-Command -scriptblock $test
        Write-Output "$test, $output"
    }
    catch
    {
        Write-Error "$test,$_"
    }
}