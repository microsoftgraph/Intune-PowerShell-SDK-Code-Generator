function Get-MobileApp {
    [CmdletBinding(DefaultParameterSetName='__AllParameterSets')]
    param(
        [Parameter(Mandatory, ValueFromPipelineByPropertyName, ParameterSetName='GetSingleApp')]
        [ValidateNotNullOrEmpty()]
        [string]$mobileAppId
    )

    if ($PSCmdlet.ParameterSetName -eq 'GetSingleApp') {
        Get-DeviceAppManagement_MobileApps -mobileAppId $mobileAppId
    } else {
        Get-DeviceAppManagement_MobileApps | Get-MSGraphAllPages
    }
}