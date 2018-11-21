# GetEffectivePermissions
Write-Host "Testing GetEffectivePermissions..."
$permissions = Invoke-GetEffectivePermissions -Scope *
if ($permissions.resourceActions.allowedResourceActions[0] -ne '*')
{
    throw "Expected to have admin permissions, but found '$permissions'"
}