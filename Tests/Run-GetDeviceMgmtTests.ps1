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
    {$env:DmIntuneDeviceConfigurationsStateSumm=Get-IntuneDeviceConfigurationDeviceStateSummaries}
    {if ($env:DmIntuneDeviceConfigurationsStateSumm -ne $null) {(Get-IntuneDeviceConfigurationDeviceStateSummaries| Get-MSGraphAllPages) |Get-IntuneDeviceComplianceSettingStates}}
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
    {$env:DmDetectedApps=(Get-IntuneDetectedApps| Get-MSGraphAllPages)}
    {if ($env:DmDetectedApps -ne $null) {((Get-IntuneDetectedApps| Get-MSGraphAllPages)[0] | Get-IntuneDetectedAppDevices)}}
    {if ($env:DmDetectedApps -ne $null) {((Get-IntuneDetectedApps| Get-MSGraphAllPages)[0] | Get-IntuneManagedDevicesReferences)}}
#
# DmDeviceCompliancePolicies
#
    {Get-IntuneDeviceCompliancePolicies}
    {(Get-IntuneDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyAssignments}
    {(Get-IntuneDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyDeviceSettingStateSummaries}
    {(Get-IntuneDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyDeviceStatuses}
    {(Get-IntuneDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyDeviceStatusOverview}
    #BUGBUG: Missing Route {(Get-IntuneDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyScheduledActionsForRule}
    #BUGBUG: Unable to figure -deviceComplianceScheduledActionForRuleId {(Get-IntuneDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyScheduledActionsForRuleConfigs}
    {(Get-IntuneDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyUserStatuses}
    {(Get-IntuneDeviceCompliancePolicies| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyUserStatusOverview}
#
# DeviceManagement_DeviceConfigurations
#
    {(Get-IntuneDeviceConfigurations| Get-MSGraphAllPages)[0] | Get-IntuneDeviceConfigurationsAssignments}
    {(Get-IntuneDeviceConfigurations| Get-MSGraphAllPages)[0] | Get-IntuneDeviceConfigurationsDeviceStatusOverview}
    {(Get-IntuneDeviceConfigurations| Get-MSGraphAllPages)[0] | Get-IntuneDeviceConfigurationsUserStatuses}
    {(Get-IntuneDeviceConfigurations| Get-MSGraphAllPages)[0] | Get-IntuneDeviceConfigurationsUserStatusOverview}
    {(Get-IntuneDeviceConfigurations| Get-MSGraphAllPages)[0] | Get-IntuneDeviceConfigurationsDeviceSettingStateSummaries}

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