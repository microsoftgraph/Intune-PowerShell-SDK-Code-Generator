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
    {$env:mdmWindowsInfoProtPolicy = (Get-MdmWinInfoPP | Get-MSGraphAllPages)[0]}
    #BUGBUG: Missing Route {(Get-MdmWinInfoPP | Get-MSGraphAllPages)[0] | Get-MdmWinInfoPPAssignments}
    #BUGBUG: Missing Route {(Get-MdmWinInfoPP | Get-MSGraphAllPages)[0] | Get-MdmWinInfoPPExemptAppLockerFiles}
    #BUGBUG: Missing Route {(Get-MdmWinInfoPP | Get-MSGraphAllPages)[0] | Get-MdmWinInfoPPProtectedAppLockerFiles}
#
# managedAppPolicies
#
    {(Get-ManagedAP | Get-MSGraphAllPages)[0]}
#
# managedAppRegistrations
#    
    {Get-AppRegns | Get-AppRegnPolicies}
    {Get-AppRegns | Get-AppRegnApps}
    {Get-AppRegns | Get-AppRegnAssignments}
    {Get-AppRegns | Get-AppRegnDeploymentSummary}
    {Get-AppRegns | Get-AppRegnExemptAppLockerFiles}
    {Get-AppRegns | Get-AppRegnProtectedAppLockerFiles}
    {Get-AppRegns | Get-AppRegnIntendedPolicies}
    {Get-AppRegns | Get-AppRegnIntendedPoliciesApps}
    {Get-AppRegns | Get-AppRegnIntendedPoliciesAsignments}
    {Get-AppRegns | Get-AppRegnIntendedPoliciesDeploymentSummary}
    {Get-AppRegns | Get-AppRegnIntendedPoliciesExemptAppLockerFiles}
    {Get-AppRegns | Get-AppRegnIntendedPoliciesProtectedAppLockerFiles}
    {Get-AppRegns | Get-AppRegnOps}
#
# AppStatuses
#
    {(Get-AppStatuses | Get-MSGraphAllPages)[0]}
#
# MobileAppCats
#
    {Get-MobileAppCats}
#
# MobileAppConfigs
#    
    {$env:mobileAppConfig = (Get-MobileAppConfigs | Get-MSGraphAllPages)[0]}
    {(Get-MobileAppConfigs | Get-MSGraphAllPages)[0] | Get-MobileAppConfigAssignments}
    {(Get-MobileAppConfigs | Get-MSGraphAllPages)[0] | Get-MobileAppConfigDeviceStatuses}
    {(Get-MobileAppConfigs | Get-MSGraphAllPages)[0] | Get-MobileAppConfigDeviceStatusSummary}
    {(Get-MobileAppConfigs | Get-MSGraphAllPages)[0] | Get-MobileAppConfigUserStatuses}
    {(Get-MobileAppConfigs | Get-MSGraphAllPages)[0] | Get-MobileAppConfigUserStatusSummary}
#
# MobileApps
#
    {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsAssignments}
    {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsCategories}
    #BUGBUG: Missing Route {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsCategoriesReferences}
    #BUGBUG: Missing Route {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsContentVersions}
    #BUGBUG: Missing Route {(Get-MobileApps | Get-MSGraphAllPages).Where({$_.displayName -Match 'Intune Managed Browser'}) | Get-MobileAppsContentVersionsFiles}
#
# TargetedAppConfigs
#

    {(Get-TargetedAppConfigs | Get-MSGraphAllPages) | Get-TargetedAppConfigApps}
    {(Get-TargetedAppConfigs | Get-MSGraphAllPages) | Get-TargetedAppConfigAssignments}
    {(Get-TargetedAppConfigs | Get-MSGraphAllPages) | Get-TargetedAppConfigDeploymentSummary}
#
# VppTokens
#
    {(Get-VppTokens | Get-MSGraphAllPages)}
#
# WinInfoPP
#
    {$env:WinPP = (Get-WinInfoPP | Get-MSGraphAllPages)}
    #BUGBUG: Missing Route {if ($env:WinPP -ne $null) {(Get-WinInfoPP | Get-MSGraphAllPages) | Get-WinInfoPPAssignments}}
    #BUGBUG: Missing Route {if ($env:WinPP -ne $null) {(Get-WinInfoPP | Get-MSGraphAllPages) | Get-WinInfoPPExemptAppLockerFiles}}
    #BUGBUG: Missing Route {if ($env:WinPP -ne $null) {(Get-WinInfoPP | Get-MSGraphAllPages) | Get-WinInfoPPProtectedAppLockerFiles}}   
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