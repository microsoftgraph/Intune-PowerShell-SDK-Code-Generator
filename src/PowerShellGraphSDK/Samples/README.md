# Table of Contents
- [Table of Contents](#table-of-contents)
- [Intune-PowerShell-SDK](#intune-powershell-sdk)
- [Getting started](#getting-started)
    - [One-time setup](#one-time-setup)
    - [Before this module is used in your organization](#before-this-module-is-used-in-your-organization)
    - [Each time you use the module](#each-time-you-use-the-module)    
    - [Discovering available commands](#discovering-available-commands)
- [Basics](#basics)
    - [List Objects](#list-objects)
    - [Bulk create objects](#bulk-create-objects)
    - [Filter objects](#filter-objects)
    - [Paging](#paging)
- [Scenario Samples](#scenario-samples)
    - [Publish iOS LOB Application](#publish-ios-lob-application)
    - [Visualize summary of apps by type](#visualize-summary-of-apps-by-type)

# Intune-PowerShell-SDK
This repository contains the source code for the PowerShell module which provides support for the Intune API through Microsoft Graph.

This project welcomes contributions and suggestions.  Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# Getting started
## One-time setup
1. Download the module from the [Releases](https://github.com/Microsoft/Intune-PowerShell-SDK/releases) tab in the GitHub repository.
2. The "Release" folder in the zip file contains two folders: "net471" and "netstandard2.0".
    - If you are using Windows, extract the "net471" folder.  **You must have .NET 4.7.1 or higher installed**.
    - If you are using any other operating system or platform (including CloudShell), extract the "netstandard2.0" folder.  You may rename the extracted folder to whatever you like.
3. The module manifest is the "Microsoft.Graph.Intune.psd1" file inside this folder.  This is the file you would refer to when importing the module (see the next section below).

## Before this module is used in your organization
An admin user must provide consent for this app to be used in their organization.  This can be done with the following command:
```PowerShell
Connect-MSGraph -AdminConsent
```

## Each time you use the module
Import the module:
```PowerShell
Import-Module $sdkDir/Microsoft.Graph.Intune.psd1
```
To authenticate with Microsoft Graph (this is not required when using CloudShell):
```PowerShell
Connect-MSGraph
```
To authenticate with Microsoft Graph using [System.Management.Automation.PSCredential]
```PowerShell
$adminUPN=Read-Host -Prompt "Enter UPN"
$adminPwd=Read-Host -AsSecureString -Prompt "Enter password for $adminUPN"
$creds = New-Object System.Management.Automation.PSCredential ($AdminUPN, $adminPwd)
$connection = Connect-MSGraph -PSCredential $creds
```

## Discovering available commands
Get the full list of available cmdlets:
```PowerShell
Get-Command -Module Microsoft.Graph.Intune
```
Get documentation on a particular cmdlet:
```PowerShell
Get-Help <cmdlet name>
```
Use a UI to see the parameter sets more easily:
```PowerShell
Show-Command <cmdlet name>
```
# Basics
## List Objects
Get all Intune applications:
```PowerShell
Get-IntuneMobileApp
```
## Filter objects
Use -Select to restrict properties to display:
```PowerShell
Get-IntuneMobileApp -Select displayName, publisher
```
Use -Filter to filter results:
```PowerShell
Get-IntuneMobileApp -Select displayName, publisher -Filter "contains(publisher, 'Microsoft')"
```
## Bulk create objects
Bulk create multiple webApp objects (they should appear in the Azure Portal)
```PowerShell
$createdApps = 'https://www.bing.com', 'https://developer.microsoft.com/graph', 'https://portal.azure.com' `
| ForEach-Object { `
    New-IntuneMobileApp `
        -webApp `
        -displayName $_ `
        -publisher 'Rohit' `
        -appUrl $_ `
        -useManagedBrowser $false `
}
```
Display using GridView
```PowerShell
1..15 | ForEach-Object { `
    New-IntuneMobileApp `
        -webApp `
        -displayName "Bing #$_" `
        -publisher 'Microsoft' `
        -appUrl 'https://www.bing.com' `
        -useManagedBrowser ([bool]($_ % 2)) `
} | Out-GridView
```

Remove all webApps.
```PowerShell
# Remove all web apps
$appsToDelete = Get-IntuneMobileApp -Filter "isof('microsoft.graph.webApp')"
$appsToDelete | Remove-IntuneMobileApp
```

## Paging
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

# Scenario Samples
## Upload iOS LOB Application
Load the Apps scenario module
```PowerShell
Import-Module '.\Apps\Microsoft.Graph.Intune.Apps.psd1'
```

Upload the iOS LOB app.
```PowerShell
# Upload an iOS Line-Of-Business app
$appToUpload = New-MobileAppObject `
    -iosLobApp `
    -displayName "Intune test iOS Lob App" `
    -description 'This is a test iOS LOB app' `
    -publisher 'Test Publisher' `
    -bundleId '' `
    -applicableDeviceType (New-IosDeviceTypeObject -iPad $true -iPhoneAndIPod $true) `
    -minimumSupportedOperatingSystem (New-IosMinimumOperatingSystemObject -v9_0 $true) `
    -fileName 'test.ipa' `
    -buildNumber 'v1' -versionNumber 'v1' -expirationDateTime ((Get-Date).AddDays(90))

# Upload the app file with the app information
$uploadedAppFile = New-LobApp -filePath '.\Apps\test.ipa' -mobileApp $appToUpload
```

## Visualize summary of apps by type
Current state.
```PowerShell
# Get all apps
$apps = Get-IntuneMobileApp

# Group the apps by type
$appsGroupedByType = $apps | Group-Object -Property '@odata.type'

# Get the X axis and Y axis values for the graph (casting is required here)
[string[]]$XVals = $appsGroupedByType | ForEach-Object {$_.Name.Replace('#microsoft.graph.', '')}
[int[]]$YVals = $appsGroupedByType | ForEach-Object {$_.Count}

# Display the results
.\Apps\VisualizeData.ps1 `
    -Title 'Intune Apps by Type' `
    -ChartType 'Pie' `
    -XLabel 'App Type' -YLabel 'Number of Apps' `
    -XValues $XVals -YValues $YVals
```