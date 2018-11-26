
<#

.COPYRIGHT
Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.

#>

####################################################

#region PowerShell Module

if(!(Get-Module Intune)){

    Import-Module "$env:sdkDir\$env:moduleName.psd1"

}

#endregion

####################################################

#region Authentication

if(!(Connect-MSGraph)){

    Connect-MSGraph

}

Write-Host

#endregion

####################################################

#region Script Output

$Date = Get-Date
$Project = "Intune"

write-host
write-host "------------------------------------------------------------------" -f yellow
write-host "[$Project] Intune POC Windows 10 Modern Management Creation"        -f yellow
write-host "------------------------------------------------------------------" -f yellow
write-host "[$Project] Script Started on $Date"                                  
write-host
write-host "This script will reset the Intune Tenant to default configurations,"
write-host "with no policies, Intune Roles or Applications."
write-host
write-host "------------------------------------------------------------------"
Write-Host

#endregion

#region Compliance Policies

write-host "------------------------------------------------------------------"
Write-Host

Write-Host "Removing all Compliance Policies..." -f Cyan
Write-Host

$CPs = Get-DeviceCompliancePolicies

foreach($CP in $CPs){

    write-host "Removing Compliance Policy..." -f Yellow
    $CP.displayname

    Remove-DeviceCompliancePolicies -deviceCompliancePolicyId $CP.id
    Write-Host

}

#endregion

####################################################

#region Configuration Policies

write-host "------------------------------------------------------------------"
Write-Host

Write-Host "Removing all Configuration Policies..." -f Cyan

Write-Host

$DCPs = Get-DeviceConfigurations

foreach($DCP in $DCPs){

    write-host "Removing Configuration Policy..." -f Yellow
    $DCP.displayname

    Remove-DeviceConfigurations -deviceConfigurationId $DCP.id
    Write-Host

}

#endregion

####################################################

#region App Protection Policies

write-host "------------------------------------------------------------------"
Write-Host

Write-Host "Removing all App Protection Policies..." -f Cyan

Write-Host

$MAMs = Get-IntuneAppProtectionPolicies

foreach($MAM in $MAMs){

    write-host "Removing Managed App Policy..." -f Yellow
    $MAM.displayname + ": " + $MAM.'@odata.type'
    $MAM.id
    
    Remove-IntuneAppProtectionPolicies -managedAppPolicyId $MAM.id
    Write-Host

}

#endregion

####################################################