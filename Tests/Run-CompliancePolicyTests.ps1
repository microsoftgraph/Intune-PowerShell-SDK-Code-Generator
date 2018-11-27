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
# Create iosCompliancePolicy
#
Write-Output "Creating iOS Compliance Policy ..."
$iosCompliancePolicy = New-IntuneDeviceCompliancePolicy -iosCompliancePolicy `
    -displayName "iOS Compliance Policy" -passcodeRequired $true -passcodeMinimumLength 6 `
    -passcodeMinutesOfInactivityBeforeLock 15 -securityBlockJailbrokenDevices $true `
    -scheduledActionsForRule (New-DeviceComplianceScheduledActionForRuleObject -ruleName PasswordRequired `
    -scheduledActionConfigurations (New-DeviceComplianceActionItemObject -gracePeriodHours 0 -actionType block -notificationTemplateId ""))

#
# Assign iosCompliancePolicy to 'Intune POC Users' group
#
$IPU_Id = (Get-Groups -Filter "displayName eq 'Intune POC Users'").id
Write-Output "Assigning $iosCompliancePolicy to 'Intune POC Users' group..."
Invoke-IntuneDeviceCompliancePolicyAssign -deviceCompliancePolicyId $iosCompliancePolicy.id `
    -assignments (New-DeviceCompliancePolicyAssignmentObject `
    -target (New-DeviceAndAppManagementAssignmentTargetObject `
    -groupAssignmentTarget -groupId "$IPU_Id"))

 Write-Output "Test complete."