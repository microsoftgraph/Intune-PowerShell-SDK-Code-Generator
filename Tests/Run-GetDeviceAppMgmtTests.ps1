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
    {$managedEBook = (Get-ManagedEBooks| Get-MSGraphAllPages)[0]}
    {$managedEBook | Get-ManagedEBookAssignments}
    {$managedEBook | Get-ManagedEBooksDeviceStates}
    {$managedEBook | Get-ManagedEBooksInstallSummary}
    {$managedEBooksUserState = (Get-ManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBooksUserStateSummary}
    {if ($managedEBooksUserState -ne $null) {(Get-ManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBooksUserStateSummaryDeviceStates}}
#
# mdmWindowsInformationProtectionPolicy
#
    {$mdmWindowsInfoProtPolicy = (Get-MdmWindowsInformationProtectionPolicies | Get-MSGraphAllPages)[0]}
    {$mdmWindowsInfoProtPolicy | Get-MdmWindowsInformationProtectionPoliciesAssignments}
    {$mdmWindowsInfoProtPolicy | Get-MdmWindowsInformationProtectionPoliciesExemptAppLockerFiles}
    {$mdmWindowsInfoProtPolicy | Get-MdmWindowsInformationProtectionPoliciesProtectedAppLockerFiles}
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
    $msGraphMeta = Get-MSGraphMetadata
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
        Write-Output "$test, $output"
    }
    catch
    {
        Write-Error "$test,$_"
    }
}