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
    {Get-IntuneDeviceCategory}
    {Get-IntuneDeviceCompliancePolicyDeviceStateSummary}    
    {$env:DmIntuneDeviceConfigurationsStateSumm=Get-IntuneDeviceConfigurationDeviceStateSummary}
    {if ($env:DmIntuneDeviceConfigurationsStateSumm -ne $null) {(Get-IntuneDeviceConfigurationDeviceStateSummary| Get-MSGraphAllPages) |Get-IntuneDeviceComplianceSettingState}}
    {Get-IntuneDeviceManagementPartner}
    {Get-IntuneExchangeConnector}
    {Get-IntuneIosUpdateStatus}
    {Get-IntuneManagedDeviceOverview}
    {Get-IntuneMobileThreatDefenseConnector}
    {$env:DmNotifMsgTemplates=(Get-IntuneNotificationMessageTemplates)}
    {if ($env:DmNotifMsgTemplates -ne $null) {(Get-IntuneNotificationMessageTemplates| Get-MSGraphAllPages)[0] |Get-IntuneLocalizedNotificationMessages}} 
    {Get-IntuneTroubleshootingEvent}
    {Get-IntuneWindowsInformationProtectionAppLearningSummary}
    {Get-IntuneWindowsInformationProtectionNetworkLearningSummary}
    {Get-IntuneRemoteAssistancePartner}
    {Get-IntuneResourceOperation}
    {Get-IntuneSoftwareUpdateStatusSummary}
    {Get-IntuneTelecomExpenseManagementPartner}
#
# DmDetectedApps
#
    {$env:DmDetectedApps=(Get-IntuneDetectedApp| Get-MSGraphAllPages)}
    {if ($env:DmDetectedApps -ne $null) {((Get-IntuneDetectedApp| Get-MSGraphAllPages)[0] | Get-IntuneDetectedAppDevice)}}
    {if ($env:DmDetectedApps -ne $null) {((Get-IntuneDetectedApp| Get-MSGraphAllPages)[0] | Get-IntuneManagedDevicesReference)}}
#
# DmDeviceCompliancePolicies
#
    {Get-IntuneDeviceCompliancePolicy}
    {(Get-IntuneDeviceCompliancePolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyAssignment}
    {(Get-IntuneDeviceCompliancePolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyDeviceSettingStateSummary}
    {(Get-IntuneDeviceCompliancePolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyDeviceStatus}
    {(Get-IntuneDeviceCompliancePolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyDeviceStatusOverview}
    #BUGBUG: Missing Route {(Get-IntuneDeviceCompliancePolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyScheduledActionsForRule}
    #BUGBUG: Unable to figure -deviceComplianceScheduledActionForRuleId {(Get-IntuneDeviceCompliancePolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyScheduledActionsForRuleConfiguration}
    {(Get-IntuneDeviceCompliancePolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyUserStatus}
    {(Get-IntuneDeviceCompliancePolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceCompliancePolicyUserStatusOverview}
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
    {(Get-IntuneDeviceEnrollmentConfigurations| Get-MSGraphAllPages)[0] | Get-IntuneDeviceEnrollmentConfigurationsAssignments}
#
# DeviceManagement_ManagedDevices
#  
    {$env:mgdDevices=(Get-IntuneManagedDevices)}
    {if ($env:mgdDevices -ne $null) {((Get-IntuneManagedDevices| Get-MSGraphAllPages)[0] | Get-IntuneManagedDevicesDeviceCategory)}}
    {if ($env:mgdDevices -ne $null) {((Get-IntuneManagedDevices| Get-MSGraphAllPages)[0] | Get-IntuneManagedDevicesDeviceCompliancePolicyStates)}}
    {if ($env:mgdDevices -ne $null) {((Get-IntuneManagedDevices| Get-MSGraphAllPages)[0] | Get-IntuneManagedDevicesDeviceConfigurationStates)}}
#
# DeviceManagement_RoleAssignments
#
    {$env:DmRoleAssignments = Get-IntuneRoleAssignments}    
#
# DeviceManagement_RoleDefinitions
#
    {$env:DmRoleDefinitions = Get-IntuneRoleDefinitions}    
#
# DeviceManagement_TermsAndConditions
#
    {$env:DmIntuneTermsAndConditions=(Get-IntuneTermsAndConditions)}
    {if ($env:DmIntuneTermsAndConditions -ne $null) {((Get-IntuneTermsAndConditions| Get-MSGraphAllPages)[0] | Get-IntuneTermsAndConditionsAcceptanceStatuses)}}
    {if ($env:DmIntuneTermsAndConditions -ne $null) {((Get-IntuneTermsAndConditions| Get-MSGraphAllPages)[0] | Get-IntuneTermsAndConditionsAssignments)}}
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
       Throw "$test,$debugInfo"
    }
}