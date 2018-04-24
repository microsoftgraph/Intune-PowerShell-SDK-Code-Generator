# POST some apps
$numApps = 10
Write-Host "Creating $numApps web apps..."
$newApps = (1..$numApps | ForEach-Object {
    New-DeviceAppManagement_MobileApp -webApp -displayName 'My new app' -publisher 'Test web app' -appUrl 'https://www.bing.com'
})

# SEARCH all web apps and make sure these all exist
Write-Host "Searching for all web apps and validating that the ones we created exist..."
$searchedApps = Get-DeviceAppManagement_MobileApp -Filter "isof('microsoft.graph.webApp')"
$ids = $newApps.id
$filteredApps = $searchedApps | Where-Object { $ids -Contains $_.id }
if ($filteredApps.Count -ne $newApps.Count)
{
    throw "Failed to create some web apps"
}

# PATCH all apps
$newAppName = 'Bing'
Write-Host "Updating the name of the created web apps to '$newAppName'..."
$newApps | Update-DeviceAppManagement_MobileApp -displayName $newAppName

# Batch GET apps (with PowerShell pipeline)
Write-Host "Retrieving the updated apps with the PowerShell pipeline..."
$updatedApps = $newApps | Get-DeviceAppManagement_MobileApp
$updatedApps | ForEach-Object {
    if ($_.displayName -ne $newAppName) {
        throw "Failed to update some web apps"
    }
}

# Batch DELETE apps (with PowerShell pipeline)
Write-Host "Deleting the updated apps with the PowerShell pipeline..."
$updatedApps | Remove-DeviceAppManagement_MobileApp

# Run some paging commands
Write-Host "Testing paging..."
$firstPage = Get-DeviceAppManagement_MobileApp -Top 10
$firstPage | Get-MSGraphNextPage | Out-Null
$allApps = $firstPage | Get-MSGraphAllPages
Write-Host "Found $($allApps.Count) apps"