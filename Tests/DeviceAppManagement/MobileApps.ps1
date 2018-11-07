# POST some apps
$numApps = 5
Write-Host "Creating $numApps web apps..."
$newApps = (1..$numApps | ForEach-Object {
    New-DamMobileApps -webApp -displayName 'My new app' -publisher 'Test web app' -appUrl 'https://www.bing.com'
})

# SEARCH all web apps and make sure these all exist
Write-Host "Searching for all web apps and validating that the ones we created exist..."
$searchedApps = Get-DamMobileApps -Filter "isof('microsoft.graph.webApp')"
$ids = $newApps.id
$filteredApps = $searchedApps | Where-Object { $ids -Contains $_.id }
if ($filteredApps.Count -ne $newApps.Count)
{
    throw "Failed to create some web apps"
}

# PATCH all apps
$newAppName = 'Bing'
Write-Host "Updating the name of the created web apps to '$newAppName'..."
$newApps | Update-DamMobileApps -displayName $newAppName

# Batch GET apps (with PowerShell pipeline)
Write-Host "Retrieving the updated apps with the PowerShell pipeline..."
$updatedApps = $newApps | Get-DamMobileApps -Select id, displayName
$updatedApps | ForEach-Object {
    if ($_.displayName -ne $newAppName) {
        throw "Failed to update some web apps"
    }
}

# Batch DELETE apps (with PowerShell pipeline)
Write-Host "Deleting the updated apps with the PowerShell pipeline..."
$updatedApps | Remove-DamMobileApps

# SEARCH all app categories
Write-Host "Getting all app categories..."
Get-DamMobileAppCategories | Out-Null

# Create a custom category
Write-Host "Creating a new app category..."
$appCategory = New-DamMobileAppCategories -displayName 'Test Category'

# SEARCH all apps
Write-Host "Getting all apps..."
$allApps = Get-DamMobileApps

# Create a reference between an app and the custom category
Write-Host "Creating a reference between an app and the new category..."
$app = $allApps[0]
$appCategory | New-DamMobileAppsCategoriesReferences -mobileAppId $app.id

# Get the referenced categories on this app
Write-Host "Getting the app with categories and assignments expanded..."
$app = $app | Get-DamMobileApps -Expand assignments, categories

# DELETE the reference
Write-Host "Removing the reference between the app and the category"
$appCategory | Remove-DamMobileAppsCategoriesReferences -mobileAppId $app.id

# DELETE the category
Write-Host "Deleting the category"
$appCategory | Remove-DamMobileAppCategories

# Run some paging commands
Write-Host "Testing paging..."
$firstPage = Get-DamMobileApps -Top ($allApps.Count / 3)
$firstPage | Get-MSGraphNextPage | Out-Null
$allPagedApps = $firstPage | Get-MSGraphAllPages
if ($allPagedApps.Count -eq 0) {
    throw "Paging returned no apps"
}
Write-Host "Found $($allPagedApps.Count) apps"

Write-Host "Testing the pipeline..." -NoNewline
$success = $false
try {
    Get-DamMobileApps | Select-Object -First 1 | Out-Null

    # The script won't reach this line if the pipeline is not ended gracefully
    $success = $true
} finally {
    if ($success) {
        Write-Host "Done"
    } else {
        Write-Host "Failed" -ForegroundColor Red
        throw "Pipeline did not end gracefully"
    }
}
