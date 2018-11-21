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
# Create iosCompliancePolicy
#
Write-Output "Creating iOS Compliance Policy ..."
$iosCompliancePolicy = New-DeviceCompliancePolicies -iosCompliancePolicy `
    -displayName "iOS Compliance Policy" -passcodeRequired $true -passcodeMinimumLength 6 `
    -passcodeMinutesOfInactivityBeforeLock 15 -securityBlockJailbrokenDevices $true `
    -scheduledActionsForRule (New-DeviceComplianceScheduledActionForRuleObject -ruleName PasswordRequired `
    -scheduledActionConfigurations (New-DeviceComplianceActionItemObject -gracePeriodHours 0 -actionType block -notificationTemplateId ""))

#
# Assign iosCompliancePolicy to 'Intune POC Users' group
#
$IPU_Id = (Get-Groups -Filter "displayName eq 'Intune POC Users'").id
Write-Output "Assigning $iosCompliancePolicy to 'Intune POC Users' group..."
Invoke-AssignDeviceCompliancePolicies -deviceCompliancePolicyId $iosCompliancePolicy.id `
    -assignments (New-DeviceCompliancePolicyAssignmentObject `
    -target (New-DeviceAndAppManagementAssignmentTargetObject `
    -groupAssignmentTarget -groupId "$IPU_Id"))

 Write-Output "Test complete."