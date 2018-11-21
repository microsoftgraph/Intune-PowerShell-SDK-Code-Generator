# Create a compliance policy
Write-Host 'Creating a compliance policy'
$compliancePolicy = New-DeviceCompliancePolicies `
    -iosCompliancePolicy `
    -displayName "Chicago" `
    -scheduledActionsForRule (New-DeviceComplianceScheduledActionForRuleObject `
        -ruleName test `
        -scheduledActionConfigurations (New-DeviceComplianceActionItemObject `
            -gracePeriodHours 0 `
            -actionType block `
            -notificationTemplateId ""`
        )`
    )

# Remove the compliance policy
Write-Host 'Deleting compliance policy'
$compliancePolicy | Remove-DeviceCompliancePolicies