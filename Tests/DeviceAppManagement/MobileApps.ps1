# POST some apps
$numApps = 5
Write-Host "Creating $numApps web apps..."
$newApps = (1..$numApps | ForEach-Object {
    New-IntuneMobileApps -webApp -displayName 'My new app' -publisher 'Test web app' -appUrl 'https://www.bing.com'
})

# SEARCH all web apps and make sure these all exist
Write-Host "Searching for all web apps and validating that the ones we created exist..."
$searchedApps = Get-IntuneMobileApps -Filter "isof('microsoft.graph.webApp')"
$ids = $newApps.id
$filteredApps = $searchedApps | Where-Object { $ids -Contains $_.id }
if ($filteredApps.Count -ne $newApps.Count)
{
    throw "Failed to create some web apps"
}

# PATCH all apps
$newAppName = 'Bing'
Write-Host "Updating the name of the created web apps to '$newAppName'..."
$newApps | Update-IntuneMobileApps -displayName $newAppName

# Batch GET apps (with PowerShell pipeline)
Write-Host "Retrieving the updated apps with the PowerShell pipeline..."
$updatedApps = $newApps | Get-IntuneMobileApps -Select id, displayName
$updatedApps | ForEach-Object {
    if ($_.displayName -ne $newAppName) {
        throw "Failed to update some web apps"
    }
}

# Batch DELETE apps (with PowerShell pipeline)
Write-Host "Deleting the updated apps with the PowerShell pipeline..."
$updatedApps | Remove-IntuneMobileApps

# SEARCH all app categories
Write-Host "Getting all app categories..."
Get-IntuneMobileAppCategories | Out-Null

# Create a custom category
Write-Host "Creating a new app category..."
$appCategory = New-IntuneMobileAppCategories -displayName 'Test Category'

# SEARCH all apps
Write-Host "Getting all apps..."
$allApps = Get-IntuneMobileApps

# Create a reference between an app and the custom category
Write-Host "Creating a reference between an app and the new category..."
$app = $allApps[0]
$appCategory | New-IntuneMobileAppsCategoriesReferences -mobileAppId $app.id

# Get the referenced categories on this app
Write-Host "Getting the app with categories and assignments expanded..."
$app = $app | Get-IntuneMobileApps -Expand assignments, categories

# DELETE the reference
Write-Host "Removing the reference between the app and the category"
$appCategory | Remove-IntuneMobileAppsCategoriesReferences -mobileAppId $app.id

# DELETE the category
Write-Host "Deleting the category"
$appCategory | Remove-IntuneMobileAppCategories

# Run some paging commands
Write-Host "Testing paging..."
$firstPage = Get-IntuneMobileApps -Top ($allApps.Count / 3)
$firstPage | Get-MSGraphNextPage | Out-Null
$allPagedApps = $firstPage | Get-MSGraphAllPages
if ($allPagedApps.Count -eq 0) {
    throw "Paging returned no apps"
}
Write-Host "Found $($allPagedApps.Count) apps"

Write-Host "Testing the pipeline..." -NoNewline
$success = $false
try {
    Get-IntuneMobileApps | Select-Object -First 1 | Out-Null

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
