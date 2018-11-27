# Presentation Notes
## Intro to the module
- Explain how the Intune PowerShell SDK fits into how we can use Intune
    - Intune -> Graph -> UI vs. Graph -> PowerShell SDK -> Scenario modules
- Show GitHub repo + releases + docs (aka.ms/intuneposh)

Load the SDK.
```PowerShell
# Load SDK module
Import-Module "$env:sdkDir\Microsoft.Graph.Intune.psd1"
```

Log in.
```PowerShell
# Log in
Connect-MSGraph

# Show that we support credentials
Get-Help Connect-MSGraph
```

How many cmdlets are there?
```PowerShell
# Show sample of names
Get-Command -Module Microsoft.Graph.Intune

Get-Command -Module Microsoft.Graph.Intune | measure
```

Graph docs will help with discovering cmdlets: https://developer.microsoft.com/en-us/graph/docs/api-reference/v1.0/api/intune_apps_mobileapp_get
Generated PowerShell documentation has information about types as well.
```PowerShell
Get-Help Get-IntuneMobileApps
```

Basics.
```PowerShell
Get-IntuneMobileApps

# $select
Get-IntuneMobileApps -Select displayName, publisher

# $filter
Get-IntuneMobileApps -Select displayName, publisher -Filter "contains(publisher, 'Microsoft')"
```

Bulk create multiple webApp objects (they should appear in the Azure Portal).
```PowerShell
$createdApps = 'https://www.bing.com', 'https://developer.microsoft.com/graph', 'https://portal.azure.com' `
| ForEach-Object { `
    New-IntuneMobileApps `
        -webApp `
        -displayName $_ `
        -publisher 'Rohit' `
        -appUrl $_ `
        -useManagedBrowser $false `
}

# Out-GridView
1..15 | ForEach-Object { `
    New-IntuneMobileApps `
        -webApp `
        -displayName "Bing #$_" `
        -publisher 'Microsoft' `
        -appUrl 'https://www.bing.com' `
        -useManagedBrowser ([bool]($_ % 2)) `
} | Out-GridView
```

Show paging of audit events (run this in a different window).
```PowerShell
# Audit events are accessible from the beta schema
Update-MSGraphEnvironment -SchemaVersion 'beta'
Connect-MSGraph

# Make the call to get audit events
$auditEvents = Invoke-MSGraphRequest -HttpMethod GET -Url 'deviceManagement/auditEvents'
$auditEvents # more than 1000 results, so they are wrapped in an object with the nextLink
$auditEvents.value | measure

# We can get the next page
$auditEvents2 = $auditEvents | Get-MSGraphNextPage
$auditEvents.value | measure # have to unwrap the results again

# Get all pages of audit events
$auditEvents = Invoke-MSGraphRequest -HttpMethod GET -Url 'deviceManagement/auditEvents' | Get-MSGraphAllPages

# Switch back to v1.0
Update-MSGraphEnvironment -SchemaVersion 'v1.0'
```

## Upload an iOS LOB app and assign it to a few groups
Load the Apps scenario module **(link to scenario module repo is available in SDK repo)**.
```PowerShell
Import-Module '.\Microsoft.Graph.Intune.Apps.psd1'
```

Upload the iOS LOB app.
```PowerShell
# Upload an iOS Line-Of-Business app
$appToUpload = New-MobileAppObject `
    -iosLobApp `
    -displayName "Rohit's App" `
    -description 'This is a test iOS LOB app' `
    -publisher 'Rohit' `
    -bundleId '' `
    -applicableDeviceType (New-IosDeviceTypeObject -iPad $true -iPhoneAndIPod $true) `
    -minimumSupportedOperatingSystem (New-IosMinimumOperatingSystemObject -v9_0 $true) `
    -fileName 'test.ipa' `
    -buildNumber 'v1' -versionNumber 'v1' -expirationDateTime ((Get-Date).AddDays(90))

# Upload the app file with the app information
$uploadedAppFile = New-LobApp -filePath 'test.ipa' -mobileApp $appToUpload
```

## Visualize a summary of apps by type
Current state.
```PowerShell
# Get all apps
$apps = Get-IntuneMobileApps

# Group the apps by type
$appsGroupedByType = $apps | Group-Object -Property '@odata.type'

# Get the X axis and Y axis values for the graph (casting is required here)
[string[]]$XVals = $appsGroupedByType | ForEach-Object {$_.Name.Replace('#microsoft.graph.', '')}
[int[]]$YVals = $appsGroupedByType | ForEach-Object {$_.Count}

# Display the results
.\VisualizeData.ps1 `
    -Title 'Intune Apps by Type' `
    -ChartType 'Pie' `
    -XLabel 'App Type' -YLabel 'Number of Apps' `
    -XValues $XVals -YValues $YVals
```

Remove all webApps.
```PowerShell
# Remove all web apps
$appsToDelete = Get-IntuneMobileApps -Filter "isof('microsoft.graph.webApp')"
$appsToDelete | Remove-IntuneMobileApps
```

## POC script
3000 -> 300 lines.
```PowerShell
.\POC_AllOS_Create_Cmdlets.ps1
```