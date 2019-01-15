param(
    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]$ModuleName="$env:moduleName",

    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]$OutputDirectory="$env:sdkDir",

    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]$AdminUPN="$env:adminUPN"   
)

#
# Setup the test context
#
Import-Module $env:testDir\Set-IntuneContext.psm1
Write-Output "Setting IntuneContext..."
Set-IntuneContext -AdminUPN $AdminUPN

#
# Declare the cmdlets the test along with their inputs
# TODO: Data drive the inputs
# TODO: Auto-generate the list of cmdlets to test
#
$tests = @(
#
# DeviceAppManagement Singleton
#
    {Get-IntuneDeviceAppManagement}
#
# AndroidManagedAppProtectionApps
#    
    {(Get-IntuneAppProtectionPolicyAndroid | Get-MSGraphAllPages) | Get-IntuneAppProtectionPolicyAndroidApp}
#
# DefaultManagedAppProtections
#
    {Get-IntuneAppProtectionPolicyDefault}
#
# IodManagedAppProtections
#    
    {(Get-IntuneAppProtectionPolicyIos | Get-MSGraphAllPages)[0] | Get-IntuneAppProtectionPolicyIosApp}  
#
# AmManagedEBooks
#    
    {(Get-IntuneManagedEBook| Get-MSGraphAllPages)[0] | Get-IntuneManagedEBookAssignment}
    {(Get-IntuneManagedEBook| Get-MSGraphAllPages)[0] | Get-IntuneManagedEBookDeviceState}
    {(Get-IntuneManagedEBook| Get-MSGraphAllPages)[0] | Get-IntuneManagedEBookInstallSummary}
    {$env:managedEBooksUserState = (Get-IntuneManagedEBook| Get-MSGraphAllPages)[0] | Get-IntuneManagedEBookUserStateSummary}
    {if ($env:managedEBooksUserState -ne $env:null) {(Get-IntuneManagedEBook| Get-MSGraphAllPages)[0] | Get-IntuneManagedEBookUserStateSummaryDeviceState}}
#
# mdmWindowsInformationProtectionPolicy
#
    {$env:mdmWindowsInfoProtPolicy = (Get-IntuneMdmWindowsInformationProtectionPolicy | Get-MSGraphAllPages)}
#
# managedAppPolicies
#
    {(Get-IntuneAppProtectionPolicy | Get-MSGraphAllPages)[0]}
#
# managedAppRegistrations
#    
    {(Get-IntuneManagedAppRegistration | Get-MSGraphAllPages)}
#
# AmIntuneManagedAppStatus
#
    {(Get-IntuneManagedAppStatus | Get-MSGraphAllPages)[0]}
#
# AmIntuneMobileAppCategory
#
    {Get-IntuneMobileAppCategory}
#
# AmIntuneMobileAppConfigurationPolicy
#    
    {$env:mobileAppConfig = (Get-IntuneMobileAppConfigurationPolicy | Get-MSGraphAllPages)[0]}
    {(Get-IntuneMobileAppConfigurationPolicy | Get-MSGraphAllPages)[0] | Get-IntuneMobileAppConfigurationPolicyAssignment}
    {(Get-IntuneMobileAppConfigurationPolicy | Get-MSGraphAllPages)[0] | Get-IntuneMobileAppConfigurationPolicyDeviceStatus}
    {(Get-IntuneMobileAppConfigurationPolicy | Get-MSGraphAllPages)[0] | Get-IntuneMobileAppConfigurationPolicyDeviceStatusSummary}
    {(Get-IntuneMobileAppConfigurationPolicy | Get-MSGraphAllPages)[0] | Get-IntuneMobileAppConfigurationPolicyUserStatus}
    {(Get-IntuneMobileAppConfigurationPolicy | Get-MSGraphAllPages)[0] | Get-IntuneMobileAppConfigurationPolicyUserStatusSummary}
#
# AmMobileApps
#
    {(Get-IntuneMobileApp | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-IntuneMobileAppAssignment}
    {(Get-IntuneMobileApp | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-IntuneMobileAppCategorySet}   
#
# AmTargetedAppConfigs
#

    {(Get-IntuneAppConfigurationPolicyTargeted | Get-MSGraphAllPages) | Get-IntuneAppConfigurationPolicyTargetedApp}
    {(Get-IntuneAppConfigurationPolicyTargeted | Get-MSGraphAllPages) | Get-IntuneAppConfigurationPolicyTargetedAssignment}
    {(Get-IntuneAppConfigurationPolicyTargeted | Get-MSGraphAllPages) | Get-IntuneAppConfigurationPolicyTargetedDeploymentSummary}
#
# AmVppTokens
#
    {(Get-IntuneVppToken | Get-MSGraphAllPages)}
#
# AmIntuneWindowsInformationProtectionPolicy
#
    {$env:WinPP = (Get-IntuneWindowsInformationProtectionPolicy | Get-MSGraphAllPages)}
    #BUGBUG: Missing Route {if ($env:WinPP -ne $null) {(Get-IntuneWindowsInformationProtectionPolicy | Get-MSGraphAllPages) | Get-IntuneWindowsInformationProtectionPolicyAssignment}}
    #BUGBUG: Missing Route {if ($env:WinPP -ne $null) {(Get-IntuneWindowsInformationProtectionPolicy | Get-MSGraphAllPages) | Get-IntuneWindowsInformationProtectionPolicyExemptAppLockerFile}}
    #BUGBUG: Missing Route {if ($env:WinPP -ne $null) {(Get-IntuneWindowsInformationProtectionPolicy | Get-MSGraphAllPages) | Get-IntuneWindowsInformationProtectionPolicyProtectedAppLockerFile}}   
)

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
        Throw "$test,$_"
    }
}