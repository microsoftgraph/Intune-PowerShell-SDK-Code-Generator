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
    {$env:DmIntuneDeviceConfigurationPolicyStateSumm=Get-IntuneDeviceConfigurationDeviceStateSummary}
    {if ($env:DmIntuneDeviceConfigurationPolicyStateSumm -ne $null) {(Get-IntuneDeviceConfigurationDeviceStateSummary| Get-MSGraphAllPages) |Get-IntuneDeviceComplianceSettingState}}
    {Get-IntuneDeviceManagementPartner}
    {Get-IntuneExchangeConnector}
    {Get-IntuneIosUpdateStatus}
    {Get-IntuneManagedDeviceOverview}
    {Get-IntuneMobileThreatDefenseConnector}
    {$env:DmNotifMsgTemplates=(Get-IntuneNotificationMessageTemplate)}
    {if ($env:DmNotifMsgTemplates -ne $null) {(Get-IntuneNotificationMessageTemplate| Get-MSGraphAllPages)[0] |Get-IntuneLocalizedNotificationMessage}} 
    {Get-IntuneTroubleshootingEvent}
    {Get-IntuneWindowsInformationProtectionAppLearningSummary}
    {Get-IntuneWindowsInformationProtectionNetworkLearningSummary}
    {Get-IntuneRemoteAssistancePartner}
    {Get-IntuneResourceOperation}
    {Get-IntuneSoftwareUpdateStatusSummary}
    {Get-IntuneTelecomExpenseManagementPartner}
    {Get-IntuneDeviceCompliancePolicySettingSummary}
#
# DmDetectedApps
#
    {$env:DmDetectedApps=(Get-IntuneDetectedApp| Get-MSGraphAllPages)}
    {if ($env:DmDetectedApps -ne $null) {((Get-IntuneDetectedApp| Get-MSGraphAllPages)[0] | Get-IntuneDetectedAppDevice)}}
    {if ($env:DmDetectedApps -ne $null) {((Get-IntuneDetectedApp| Get-MSGraphAllPages)[0] | Get-IntuneManagedDeviceReference)}}
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
    {(Get-IntuneDeviceConfigurationPolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceConfigurationPolicyAssignment}
    {(Get-IntuneDeviceConfigurationPolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceConfigurationPolicyDeviceStatusOverview}
    {(Get-IntuneDeviceConfigurationPolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceConfigurationPolicyUserStatus}
    {(Get-IntuneDeviceConfigurationPolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceConfigurationPolicyUserStatusOverview}
    {(Get-IntuneDeviceConfigurationPolicy| Get-MSGraphAllPages)[0] | Get-IntuneDeviceConfigurationPolicyDeviceSettingStateSummary}

#
# DeviceManagement_DeviceEnrollmentConfigurations
# 
    {(Get-IntuneDeviceEnrollmentConfiguration| Get-MSGraphAllPages)[0] | Get-IntuneDeviceEnrollmentConfigurationAssignment}
#
# DeviceManagement_ManagedDevices
#  
    {$env:mgdDevices=(Get-IntuneManagedDevice)}
    {if ($env:mgdDevices -ne $null) {((Get-IntuneManagedDevice| Get-MSGraphAllPages)[0] | Get-IntuneManagedDeviceDeviceCategory)}}
    {if ($env:mgdDevices -ne $null) {((Get-IntuneManagedDevice| Get-MSGraphAllPages)[0] | Get-IntuneManagedDeviceDeviceCompliancePolicyState)}}
    {if ($env:mgdDevices -ne $null) {((Get-IntuneManagedDevice| Get-MSGraphAllPages)[0] | Get-IntuneManagedDeviceDeviceConfigurationState)}}
#
# DeviceManagement_RoleAssignments
#
    {$env:DmRoleAssignments = Get-IntuneRoleAssignment}    
#
# DeviceManagement_RoleDefinitions
#
    {$env:DmRoleDefinitions = Get-IntuneRoleDefinition}    
#
# DeviceManagement_TermsAndConditions
#
    {$env:DmIntuneTermsAndConditions=(Get-IntuneTermsAndConditions)}
    {if ($env:DmIntuneTermsAndConditions -ne $null) {((Get-IntuneTermsAndConditions| Get-MSGraphAllPages)[0] | Get-IntuneTermsAndConditionsAcceptanceStatus)}}
    {if ($env:DmIntuneTermsAndConditions -ne $null) {((Get-IntuneTermsAndConditions| Get-MSGraphAllPages)[0] | Get-IntuneTermsAndConditionsAssignment)}}
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