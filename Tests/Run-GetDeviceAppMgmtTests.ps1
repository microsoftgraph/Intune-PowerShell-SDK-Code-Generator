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
    {(Get-ManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBookAssignments}
    {(Get-ManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBooksDeviceStates}
    {(Get-ManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBooksInstallSummary}
    {$env:managedEBooksUserState = (Get-ManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBooksUserStateSummary}
    {if ($env:managedEBooksUserState -ne $env:null) {(Get-ManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBooksUserStateSummaryDeviceStates}}
#
# mdmWindowsInformationProtectionPolicy
#
    {$env:mdmWindowsInfoProtPolicy = (Get-MdmWinInfoPP | Get-MSGraphAllPages)}
#
# managedAppPolicies
#
    {(Get-IntuneAppProtectionPolicies | Get-MSGraphAllPages)[0]}
#
# managedAppRegistrations
#    
    {(Get-IntuneManagedAppRegistrations | Get-MSGraphAllPages)}
#
# AmAppStatuses
#
    {(Get-AppStatuses | Get-MSGraphAllPages)[0]}
#
# AmMobileAppCats
#
    {Get-MobileAppCats}
#
# AmMobileAppConfigs
#    
    {$env:mobileAppConfig = (Get-MobileAppConfigs | Get-MSGraphAllPages)[0]}
    {(Get-MobileAppConfigs | Get-MSGraphAllPages)[0] | Get-MobileAppConfigAssignments}
    {(Get-MobileAppConfigs | Get-MSGraphAllPages)[0] | Get-MobileAppConfigDeviceStatuses}
    {(Get-MobileAppConfigs | Get-MSGraphAllPages)[0] | Get-MobileAppConfigDeviceStatusSummary}
    {(Get-MobileAppConfigs | Get-MSGraphAllPages)[0] | Get-MobileAppConfigUserStatuses}
    {(Get-MobileAppConfigs | Get-MSGraphAllPages)[0] | Get-MobileAppConfigUserStatusSummary}
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