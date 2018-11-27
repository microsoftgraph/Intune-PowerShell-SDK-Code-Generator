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
# DeviceManagement Singleton
#
    {Get-IntuneDeviceAppManagement}
    {Get-IntuneApplePushNotificationCertificate}
    {Get-IntuneDeviceCategories}
    {Get-IntuneDeviceCompliancePolicyDeviceStateSummary}
    {Get-IntuneDeviceCompliancePolicySettingStateSummaries}
    {$env:DmDCStateSumm=Get-IntuneDeviceConfigurationDeviceStateSummaries}
    {if ($env:DmDCStateSumm -ne $null) {(Get-IntuneDeviceConfigurationDeviceStateSummaries| Get-MSGraphAllPages) |Get-IntuneDeviceComplianceSettingStates}}
    {Get-IntuneDeviceManagementPartners}
    {Get-IntuneExchangeConnectors}
    {Get-IntuneIosUpdateStatuses}
    {Get-IntuneManagedDeviceOverview}
    {Get-IntuneMobileThreatDefenseConnectors}
    {$env:DmNotifMsgTemplates=(Get-NotifMsgTemplates)}
    {if ($env:DmNotifMsgTemplates -ne $null) {(Get-NotifMsgTemplates| Get-MSGraphAllPages)[0] |Get-NotifMsgTemplateLocMsgs}} 
    {Get-IntuneTroubleshootingEvents}
    {Get-IntuneWindowsInformationProtectionAppLearningSummaries}
    {Get-IntuneWindowsInformationProtectionNetworkLearningSummaries}
    {Get-IntuneRemoteAssistancePartners}
    {Get-IntuneResourceOperations}
    {Get-IntuneSoftwareUpdateStatusSummary}
    {Get-IntuneTelecomExpenseManagementPartners}
#
# DmDetectedApps
#
    {$env:DmDetectedApps=(Get-DetectedApps| Get-MSGraphAllPages)}
    {if ($env:DmDetectedApps -ne $null) {((Get-DetectedApps| Get-MSGraphAllPages)[0] | Get-DetectedAppDevices)}}
    {if ($env:DmDetectedApps -ne $null) {((Get-DetectedApps| Get-MSGraphAllPages)[0] | Get-DetectedAppDeviceRefs)}}
#
# DmDeviceCompliancePolicies
#
    {Get-DeviceCompliancePolicies}
    {(Get-DeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DeviceCompliancePolicyAssignments}
    {(Get-DeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DeviceCompliancePolicyDeviceSettingStateSummaries}
    {(Get-DeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DeviceCompliancePolicyDeviceStatuses}
    {(Get-DeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DeviceCompliancePolicyDeviceStatusOverview}
    #BUGBUG: Missing Route {(Get-DeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DeviceCompliancePolicyScheduledActionsForRule}
    #BUGBUG: Unable to figure -deviceComplianceScheduledActionForRuleId {(Get-DeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DeviceCompliancePolicyScheduledActionsForRuleConfigs}
    {(Get-DeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DeviceCompliancePolicyUserStatuses}
    {(Get-DeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-DeviceCompliancePolicyUserStatusOverview}
#
# DeviceManagement_DeviceConfigurations
#
    {(Get-DeviceConfigurations| Get-MSGraphAllPages)[0] | Get-DCAssignments}
    {(Get-DeviceConfigurations| Get-MSGraphAllPages)[0] | Get-DCDeviceStatusOverview}
    {(Get-DeviceConfigurations| Get-MSGraphAllPages)[0] | Get-DCUserStatuses}
    {(Get-DeviceConfigurations| Get-MSGraphAllPages)[0] | Get-DCUserStatusOverview}
    {(Get-DeviceConfigurations| Get-MSGraphAllPages)[0] | Get-DCDeviceSettingStateSummaries}

#
# DeviceManagement_DeviceEnrollmentConfigurations
# 
    {(Get-DeviceEnrollmentConfigs| Get-MSGraphAllPages)[0] | Get-DeviceEnrollmentConfigAssignments}
#
# DeviceManagement_ManagedDevices
#  
    {$env:mgdDevices=(Get-ManagedDevices)}
    {if ($env:mgdDevices -ne $null) {((Get-ManagedDevices| Get-MSGraphAllPages)[0] | Get-DeviceCategory)}}
    {if ($env:mgdDevices -ne $null) {((Get-ManagedDevices| Get-MSGraphAllPages)[0] | Get-DeviceCompliancePolicyStates)}}
    {if ($env:mgdDevices -ne $null) {((Get-ManagedDevices| Get-MSGraphAllPages)[0] | Get-DeviceConfigurationStates)}}
#
# DeviceManagement_RoleAssignments
#
    {$env:DmRoleAssignments = Get-RoleAssignments}    
#
# DeviceManagement_RoleDefinitions
#
    {$env:DmRoleDefinitions = Get-RoleDefinitions}    
#
# DeviceManagement_TermsAndConditions
#
    {$env:DmTnC=(Get-TnCs)}
    {if ($env:DmTnC -ne $null) {((Get-TnCs| Get-MSGraphAllPages)[0] | Get-TnCAcceptanceStatuses)}}
    {if ($env:DmTnC -ne $null) {((Get-TnCs| Get-MSGraphAllPages)[0] | Get-TnCAssignments)}}
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
        $debugInfo="$_"
        Write-Error "$test,$debugInfo"
    }
}