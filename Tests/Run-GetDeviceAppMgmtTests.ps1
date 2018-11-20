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

$OutputDirectory = $OutputDirectory | Resolve-Path
$modulePath = "$OutputDirectory/$ModuleName.psd1"

#
# Import the Intune PowerShell SDK Module
#
Write-Output "Importing $ModuleName..."
Import-Module $modulePath

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
    {(Get-AmAndroidAPP | Get-MSGraphAllPages)[0] | Get-AmAndroidAPPApps}
#
# DefaultManagedAppProtections
#
    {Get-AmDefaultAPP}
#
# IodManagedAppProtections
#    
    {(Get-AmIosApp | Get-MSGraphAllPages)[0] | Get-AmIosAppApps}  
#
# AmManagedEBooks
#    
    {(Get-AmManagedEBooks| Get-MSGraphAllPages)[0] | Get-AmManagedEBookAssignments}
    {(Get-AmManagedEBooks| Get-MSGraphAllPages)[0] | Get-ManagedEBooksDeviceStates}
    {(Get-AmManagedEBooks| Get-MSGraphAllPages)[0] | Get-AmManagedEBooksInstallSummary}
    {$env:managedEBooksUserState = (Get-AmManagedEBooks| Get-MSGraphAllPages)[0] | Get-AmManagedEBooksUserStateSummary}
    {if ($env:managedEBooksUserState -ne $env:null) {(Get-AmManagedEBooks| Get-MSGraphAllPages)[0] | Get-AmManagedEBooksUserStateSummaryDeviceStates}}
#
# mdmWindowsInformationProtectionPolicy
#
    {$env:mdmWindowsInfoProtPolicy = (Get-AmMdmWinInfoPP | Get-MSGraphAllPages)[0]}
    #BUGBUG: Missing Route {(Get-AmMdmWinInfoPP | Get-MSGraphAllPages)[0] | Get-AmMdmWinInfoPPAssignments}
    #BUGBUG: Missing Route {(Get-AmMdmWinInfoPP | Get-MSGraphAllPages)[0] | Get-AmMdmWinInfoPPExemptAppLockerFiles}
    #BUGBUG: Missing Route {(Get-AmMdmWinInfoPP | Get-MSGraphAllPages)[0] | Get-AmMdmWinInfoPPProtectedAppLockerFiles}
#
# managedAppPolicies
#
    {(Get-AmManagedAP | Get-MSGraphAllPages)[0]}
#
# managedAppRegistrations
#    
    {Get-AmAppRegns | Get-AmAppRegnPolicies}
    {Get-AmAppRegns | Get-AmAppRegnApps}
    {Get-AmAppRegns | Get-AmAppRegnAssignments}
    {Get-AmAppRegns | Get-AmAppRegnDeploymentSummary}
    {Get-AmAppRegns | Get-AmAppRegnExemptAppLockerFiles}
    {Get-AmAppRegns | Get-AmAppRegnProtectedAppLockerFiles}
    {Get-AmAppRegns | Get-AmAppRegnIntendedPolicies}
    {Get-AmAppRegns | Get-AmAppRegnIntendedPoliciesApps}
    {Get-AmAppRegns | Get-AmAppRegnIntendedPoliciesAsignments}
    {Get-AmAppRegns | Get-AmAppRegnIntendedPoliciesDeploymentSummary}
    {Get-AmAppRegns | Get-AmAppRegnIntendedPoliciesExemptAppLockerFiles}
    {Get-AmAppRegns | Get-AmAppRegnIntendedPoliciesProtectedAppLockerFiles}
    {Get-AmAppRegns | Get-AmAmAppRegnOps}
#
# AmAppStatuses
#
    {(Get-AmAppStatuses | Get-MSGraphAllPages)[0]}
#
# AmMobileAppCats
#
    {Get-AmMobileAppCats}
#
# AmMobileAppConfigs
#    
    {$env:mobileAppConfig = (Get-AmMobileAppConfigs | Get-MSGraphAllPages)[0]}
    {(Get-AmMobileAppConfigs | Get-MSGraphAllPages)[0] | Get-AmMobileAppConfigAssignments}
    {(Get-AmMobileAppConfigs | Get-MSGraphAllPages)[0] | Get-AmMobileAppConfigDeviceStatuses}
    {(Get-AmMobileAppConfigs | Get-MSGraphAllPages)[0] | Get-AmMobileAppConfigDeviceStatusSummary}
    {(Get-AmMobileAppConfigs | Get-MSGraphAllPages)[0] | Get-AmMobileAppConfigUserStatuses}
    {(Get-AmMobileAppConfigs | Get-MSGraphAllPages)[0] | Get-AmMobileAppConfigUserStatusSummary}
#
# AmMobileApps
#
    {(Get-AmMobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-AmMobileAppsAssignments}
    {(Get-AmMobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-AmMobileAppsCategories}
    #BUGBUG: Missing Route {(Get-AmMobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-AmMobileAppsCategoriesReferences}
    #BUGBUG: Missing Route {(Get-AmMobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-AmMobileAppsContentVersions}
    #BUGBUG: Missing Route {(Get-AmMobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-AmMobileAppsContentVersionsFiles}
#
# AmTargetedAppConfigs
#

    {(Get-AmTargetedAppConfigs | Get-MSGraphAllPages) | Get-AmTargetedAppConfigApps}
    {(Get-AmTargetedAppConfigs | Get-MSGraphAllPages) | Get-AmTargetedAppConfigAssignments}
    {(Get-AmTargetedAppConfigs | Get-MSGraphAllPages) | Get-AmTargetedAppConfigDeploymentSummary}
#
# AmVppTokens
#
    {(Get-AmVppTokens | Get-MSGraphAllPages)}
#
# AmWinInfoPP
#
    {$env:WinPP = (Get-AmWinInfoPP | Get-MSGraphAllPages)}
    #BUGBUG: Missing Route {if ($env:WinPP -ne $null) {(Get-AmWinInfoPP | Get-MSGraphAllPages) | Get-AmWinInfoPPAssignments}}
    #BUGBUG: Missing Route {if ($env:WinPP -ne $null) {(Get-AmWinInfoPP | Get-MSGraphAllPages) | Get-AmWinInfoPPExemptAppLockerFiles}}
    #BUGBUG: Missing Route {if ($env:WinPP -ne $null) {(Get-AmWinInfoPP | Get-MSGraphAllPages) | Get-AmWinInfoPPProtectedAppLockerFiles}}   
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
        Write-Error "$test,$_"
    }
}