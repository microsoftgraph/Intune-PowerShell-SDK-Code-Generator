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
    {Get-DeviceAppManagement}
#
# AndroidManagedAppProtectionApps
#    
    {(Get-IntuneAndroidAppProtectionPolicies | Get-MSGraphAllPages) | Get-IntuneAndroidAppProtectionPoliciesApps}
#
# DefaultManagedAppProtections
#
    {Get-IntuneDefaultAppProtectionPolicies}
#
# IodManagedAppProtections
#    
    {(Get-IntuneIosAppProtectionPolicies | Get-MSGraphAllPages)[0] | Get-IntuneIosAppProtectionPoliciesApps}  
#
# AmManagedEBooks
#    
    {(Get-IntuneManagedEBooks| Get-MSGraphAllPages)[0] | Get-IntuneManagedEBooksAssignments}
    {(Get-IntuneManagedEBooks| Get-MSGraphAllPages)[0] | Get-IntuneManagedEBooksDeviceStates}
    {(Get-IntuneManagedEBooks| Get-MSGraphAllPages)[0] | Get-IntuneManagedEBooksInstallSummary}
    {$env:managedEBooksUserState = (Get-IntuneManagedEBooks| Get-MSGraphAllPages)[0] | Get-IntuneManagedEBooksUserStateSummary}
    {if ($env:managedEBooksUserState -ne $env:null) {(Get-IntuneManagedEBooks| Get-MSGraphAllPages)[0] | Get-IntuneManagedEBooksUserStateSummaryDeviceStates}}
#
# mdmWindowsInformationProtectionPolicy
#
    {$env:mdmWindowsInfoProtPolicy = (Get-IntuneMdmWindowsInformationProtectionPolicies | Get-MSGraphAllPages)}
#
# managedAppPolicies
#
    {(Get-IntuneAppProtectionPolicies | Get-MSGraphAllPages)[0]}
#
# managedAppRegistrations
#    
    {(Get-IntuneManagedAppRegistrations | Get-MSGraphAllPages)}
#
# AmIntuneManagedAppStatus
#
    {(Get-IntuneManagedAppStatus | Get-MSGraphAllPages)[0]}
#
# AmIntuneMobileAppCategories
#
    {Get-IntuneMobileAppCategories}
#
# AmIntuneMobileAppConfigurations
#    
    {$env:mobileAppConfig = (Get-IntuneMobileAppConfigurations | Get-MSGraphAllPages)[0]}
    {(Get-IntuneMobileAppConfigurations | Get-MSGraphAllPages)[0] | Get-IntuneMobileAppConfigurationsAssignments}
    {(Get-IntuneMobileAppConfigurations | Get-MSGraphAllPages)[0] | Get-IntuneMobileAppConfigurationsDeviceStatuses}
    {(Get-IntuneMobileAppConfigurations | Get-MSGraphAllPages)[0] | Get-IntuneMobileAppConfigurationsDeviceStatusSummary}
    {(Get-IntuneMobileAppConfigurations | Get-MSGraphAllPages)[0] | Get-IntuneMobileAppConfigurationsUserStatuses}
    {(Get-IntuneMobileAppConfigurations | Get-MSGraphAllPages)[0] | Get-IntuneMobileAppConfigurationsUserStatusSummary}
#
# AmMobileApps
#
    {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsAssignments}
    {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsCategories}
    #BUGBUG: Missing Route {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsCategoriesReferences}
    #BUGBUG: Missing Route {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsContentVersions}
    #BUGBUG: Missing Route {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsContentVersionsFiles}
#
# AmTargetedAppConfigs
#

    {(Get-TargetedAppConfigs | Get-MSGraphAllPages) | Get-TargetedAppConfigApps}
    {(Get-TargetedAppConfigs | Get-MSGraphAllPages) | Get-TargetedAppConfigAssignments}
    {(Get-TargetedAppConfigs | Get-MSGraphAllPages) | Get-TargetedAppConfigDeploymentSummary}
#
# AmVppTokens
#
    {(Get-VppTokens | Get-MSGraphAllPages)}
#
# AmWinInfoPP
#
    {$env:WinPP = (Get-WinInfoPP | Get-MSGraphAllPages)}
    #BUGBUG: Missing Route {if ($env:WinPP -ne $null) {(Get-WinInfoPP | Get-MSGraphAllPages) | Get-WinInfoPPAssignments}}
    #BUGBUG: Missing Route {if ($env:WinPP -ne $null) {(Get-WinInfoPP | Get-MSGraphAllPages) | Get-WinInfoPPExemptAppLockerFiles}}
    #BUGBUG: Missing Route {if ($env:WinPP -ne $null) {(Get-WinInfoPP | Get-MSGraphAllPages) | Get-WinInfoPPProtectedAppLockerFiles}}   
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