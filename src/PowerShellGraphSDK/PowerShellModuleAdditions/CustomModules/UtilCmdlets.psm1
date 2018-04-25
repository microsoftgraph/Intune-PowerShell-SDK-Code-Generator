function Get-MSGraphAllPages {
    [CmdletBinding(
        ConfirmImpact = 'Medium',
        DefaultParameterSetName = 'SearchResult'
    )]
    param (
        [Parameter(Mandatory = $true, ParameterSetName = 'NextLink', ValueFromPipelineByPropertyName = $true)]
        [ValidateNotNullOrEmpty]
        [Alias('@odata.nextLink')]
        [string]$NextLink,

        [Parameter(Mandatory = $true, ParameterSetName = 'SearchResult', ValueFromPipeline = $true)]
        [ValidateNotNull]
        [PSObject]$SearchResult
    )

    if ($PSCmdlet.ParameterSetName -eq 'SearchResult') {
        # Set the current page to the search result provided
        $page = $SearchResult

        # Extract the NextLink
        $currentNextLink = $page.'@odata.nextLink'

        # Output the items in the first page
        $values = $page.value
        if ($values) {
            $values | Write-Output
        }
    }

    while ($currentNextLink)
    {
        # Make the call to get the next page
        $page = Get-MSGraphNextPage $currentNextLink

        # Extract the NextLink
        $currentNextLink = $page.'@odata.nextLink'

        # Output the items in the page
        $values = $page.value
        if ($values) {
            $values | Write-Output
        }
    }
}