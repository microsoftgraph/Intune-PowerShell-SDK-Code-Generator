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
# Import the Intune PowerShell SDK Module if necessary
#
if ((Get-Module $ModuleName) -eq $null)
{        
    Import-Module $modulePath
}

#
# Connect to MSGraph if necessary
#
try
{
    $env:msGraphMeta = Get-MSGraphMetadata
    $connection = Connect-MSGraph
}
catch
{    
    $adminPwd=Read-Host -AsSecureString -Prompt "Enter pwd for $env:adminUPN"
    $creds = New-Object System.Management.Automation.PSCredential ($AdminUPN, $adminPwd)
    $connection = Connect-MSGraph -PSCredential $creds
}

#
# Create iosCompliancePolicy
#
$iosCompliancePolicy = New-DmDeviceCompliancePolicies -iosCompliancePolicy `
    -displayName "iOS Compliance Policy" -passcodeRequired $true -passcodeMinimumLength 6 `
    -passcodeMinutesOfInactivityBeforeLock 15 -securityBlockJailbrokenDevices $true `
    -scheduledActionsForRule (New-DeviceComplianceScheduledActionForRuleObject -ruleName PasswordRequired `
    -scheduledActionConfigurations (New-DeviceComplianceActionItemObject -gracePeriodHours 0 -actionType block -notificationTemplateId ""))

#
# Assign iosCompliancePolicy to 'Intune POC Users' group
#
$IPU_Id = (Get-Groups -Filter "displayName eq 'Intune POC Users'").id
Invoke-DmAssignDeviceCompliancePolicies -deviceCompliancePolicyId $iosCompliancePolicy.id `
    -assignments (New-DeviceCompliancePolicyAssignmentObject `
    -target (New-DeviceAndAppManagementAssignmentTargetObject `
    -groupAssignmentTarget -groupId "$IPU_Id"))
