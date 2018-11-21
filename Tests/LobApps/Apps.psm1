function Get-MobileApp {
    [CmdletBinding(DefaultParameterSetName='__AllParameterSets')]
    param(
        [Parameter(Mandatory, ValueFromPipelineByPropertyName, ParameterSetName='GetSingleApp')]
        [ValidateNotNullOrEmpty()]
        [string]$mobileAppId
    )

    if ($PSCmdlet.ParameterSetName -eq 'GetSingleApp') {
        Get-MobileApps -mobileAppId $mobileAppId
    } else {
        Get-MobileApps | Get-MSGraphAllPages
    }
}