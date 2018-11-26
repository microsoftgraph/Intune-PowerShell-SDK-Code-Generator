function Get-MobileApp {
    [CmdletBinding(DefaultParameterSetName='__AllParameterSets')]
    param(
        [Parameter(Mandatory, ValueFromPipelineByPropertyName, ParameterSetName='GetSingleApp')]
        [ValidateNotNullOrEmpty()]
        [string]$mobileAppId
    )

    if ($PSCmdlet.ParameterSetName -eq 'GetSingleApp') {
        Get-IntuneMobileApps -mobileAppId $mobileAppId
    } else {
        Get-IntuneMobileApps | Get-MSGraphAllPages
    }
}