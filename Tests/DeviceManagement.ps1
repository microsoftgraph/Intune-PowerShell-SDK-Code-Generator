# IntuneGetEffectivePermission
Write-Host "Testing IntuneGetEffectivePermission..."
$permissions = Invoke-IntuneGetEffectivePermission -Scope *
if ($permissions.resourceActions.allowedResourceActions[0] -ne '*')
{
    throw "Expected to have admin permissions, but found '$permissions'"
}