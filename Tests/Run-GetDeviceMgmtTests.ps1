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
# DeviceManagement Singleton
#
    {Get-DeviceAppManagement}
    {Get-DmAPNSCert}
    #BUGBUG: Investigate 400 Bad Request {Get-DmCASettings}
    {Get-DmDeviceCategories}
    {Get-DmDeviceCompliancePolicyDeviceStateSummary}
    {Get-DmDeviceCompliancePolicySettingStateSummaries}
    {$env:DmDCStateSumm=Get-DmDeviceConfigurationDeviceStateSummaries}
    {if ($env:DmDCStateSumm -ne $null) {((Get-DmDeviceConfigurationDeviceStateSummaries| Get-MSGraphAllPages)[0] |Get-DmDeviceComplianceSettingStates}}
    {Get-DmDeviceManagementPartners}
    {Get-DmExchangeConnectors}
    {Get-DmIosUpdateStatuses}
    {Get-DmManagedDeviceOverview}
    {Get-DmMobileThreatDefenseConnectors}
    {$env:DmNotifMsgTemplates=(Get-DmNotifMsgTemplates)}
    {if ($env:DmNotifMsgTemplates -ne $null) {((Get-DmNotifMsgTemplates| Get-MSGraphAllPages)[0] |Get-DmNotifMsgTemplateLocMsgs}    
    {Get-DmTroubleshootingEvents}
    {Get-DmWIPAppLearningSummaries}
    {Get-DmWIPNetworkLearningSummaries}
    {Get-DmRemoteAssistancePartners}
    {Get-DmResourceOperations}
    {Get-DmSoftwareUpdateStatusSummary}
    {Get-DmTelecomExpenseManagementPartners}
#
# DmDetectedApps
#
    {$env:DmDetectedApps=(Get-DmDetectedApps| Get-MSGraphAllPages)}
    {if ($env:DmDetectedApps -ne $null) {((Get-DmDetectedApps| Get-MSGraphAllPages)[0] | Get-DmDetectedAppDevices)}}
    {if ($env:DmDetectedApps -ne $null) {((Get-DmDetectedApps| Get-MSGraphAllPages)[0] | Get-DmDetectedAppDeviceRefs)}}
#
# DmDeviceCompliancePolicies
#
    {Get-DmDeviceCompliancePolicies}
    {(Get-DmDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DmDeviceCompliancePolicyAssignments}
    {(Get-DmDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DmDeviceCompliancePolicyDeviceSettingStateSummaries}
    {(Get-DmDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DmDeviceCompliancePolicyDeviceStatuses}
    {(Get-DmDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DmDeviceCompliancePolicyDeviceStatusOverview}
    #BUGBUG: Missing Route {(Get-DmDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DmDeviceCompliancePolicyScheduledActionsForRule}
    #BUGBUG: Unable to figure -deviceComplianceScheduledActionForRuleId {(Get-DmDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DmDeviceCompliancePolicyScheduledActionsForRuleConfigs}
    {(Get-DmDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DmDeviceCompliancePolicyUserStatuses}
    {(Get-DmDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DmDeviceCompliancePolicyUserStatusOverview}
#
# DeviceManagement_DeviceConfigurations
#
    {(Get-DmDeviceConfigurations| Get-MSGraphAllPages)[0] | Get-DmDCAssignments}
    {(Get-DmDeviceConfigurations| Get-MSGraphAllPages)[0] | Get-DmDCDeviceStatusOverview}
    {(Get-DmDeviceConfigurations| Get-MSGraphAllPages)[0] | Get-DmDCUserStatuses}
    {(Get-DmDeviceConfigurations| Get-MSGraphAllPages)[0] | Get-DmDCUserStatusOverview}
    {(Get-DmDeviceConfigurations| Get-MSGraphAllPages)[0] | Get-DmDCDeviceSettingStateSummaries}

#
# DeviceManagement_DeviceEnrollmentConfigurations
# 
    {(Get-DmDeviceEnrollmentConfigs| Get-MSGraphAllPages)[0] | Get-DmDeviceEnrollmentConfigAssignments}
#
# DeviceManagement_ManagedDevices
#  
    {$env:mgdDevices=(Get-DmManagedDevices)}
    {if ($env:mgdDevices -ne $null) {((Get-DmManagedDevices| Get-MSGraphAllPages)[0] | Get-DmDeviceCategory)}}
    {if ($env:mgdDevices -ne $null) {((Get-DmManagedDevices| Get-MSGraphAllPages)[0] | Get-DmDeviceCompliancePolicyStates)}}
    {if ($env:mgdDevices -ne $null) {((Get-DmManagedDevices| Get-MSGraphAllPages)[0] | Get-DmDeviceConfigurationStates)}}
#
# DeviceManagement_RoleAssignments
#
    {$env:DmRoleAssignments = Get-DmRoleAssignments}    
#
# DeviceManagement_RoleDefinitions
#
    {$env:DmRoleDefinitions = Get-DmRoleDefinitions}    
#
# DeviceManagement_TermsAndConditions
#
    {$env:DmTnC=(Get-DmTermsAndConditions)}
    {if ($env:DmTnC -ne $null) {((Get-DmTermsAndConditions| Get-MSGraphAllPages)[0] | Get-DmTermsAndConditionsAcceptanceStatuses)}}
    {if ($env:DmTnC -ne $null) {((Get-DmTermsAndConditions| Get-MSGraphAllPages)[0] | Get-DmTermsAndConditionsAssignments)}}
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
        $debugInfo="$_"
        Write-Error "$test,$debugInfo"
    }
}