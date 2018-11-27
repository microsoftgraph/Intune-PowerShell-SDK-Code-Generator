# IntuneGetEffectivePermissions
Write-Host "Testing IntuneGetEffectivePermissions..."
$permissions = Invoke-IntuneGetEffectivePermissions -Scope *
if ($permissions.resourceActions.allowedResourceActions[0] -ne '*')
{
    throw "Expected to have admin permissions, but found '$permissions'"
}