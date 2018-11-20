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

$tests = @(
    {$env:scheduledActionConfigurations = (New-DeviceComplianceActionItemObject -gracePeriodHours 0 -actionType block -notificationTemplateId "")}
    {$env:scheduledActionsForRule = (New-DeviceComplianceScheduledActionForRuleObject -ruleName PasswordRequired `
        -scheduledActionConfigurations $env:scheduledActionConfigurations)}
    {$env:iosCompliancePolicy = (New-DmDeviceCompliancePolicies -iosCompliancePolicy `
        -displayName "iOS Compliance Policy" -passcodeRequired $true -passcodeMinimumLength 6 `
        -passcodeMinutesOfInactivityBeforeLock 15 -securityBlockJailbrokenDevices $true `
        -scheduledActionsForRule $env:scheduledActionsForRule)}
    {$env:IPU_Id = (Get-Groups -Filter "displayName eq 'Intune POC Users'").id}
    {$env:assignmentTarget = (New-DeviceAndAppManagementAssignmentTargetObject `
        -groupAssignmentTarget -groupId "$env:IPU_Id")}
    {$env:assignment = New-DeviceCompliancePolicyAssignmentObject `
        -target $env:assignmentTarget}
    #BUGBUG: A valid URL could not be constructed {Invoke-DmAssignDeviceCompliancePolicies -deviceCompliancePolicyId $env:iosCompliancePolicy `
    #    -assignments $env:assignment}
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