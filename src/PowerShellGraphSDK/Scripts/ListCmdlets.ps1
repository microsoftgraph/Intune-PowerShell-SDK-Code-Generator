param(
    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]$ModuleName="$env:moduleName",

    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]$OutputDirectory="$env:sdkDir",

    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [switch]$IsFullOutput=$false
)

$OutputDirectory = $OutputDirectory | Resolve-Path
$modulePath = "$OutputDirectory\$ModuleName.psd1"

Push-Location $OutputDirectory

Import-Module $modulePath
$sdkCmdlets = ((Get-Command -Module Microsoft.Graph.Intune) | Sort-Object)
$cmdletCount = $sdkCmdlets.Count
Write-Host "$cmdletCount cmdlets exported by $modulePath"

$sdkCmdletsList = @()
for($cmdletCount=0; $cmdletCount -lt $sdkCmdlets.Count; $cmdletCount++)
{
    $name = $sdkCmdlets[$cmdletCount]
    if ($IsFullOutput)
    {
        $helpTxt = (Get-Help $sdkCmdlets[$cmdletCount]).Description
        if ($helpTxt -ne $null)
        {   
            if ($helpTxt[0] -ne $null) {$route = ($helpTxt[0].Text).Replace('`r`nE','')}
            if ($helpTxt[1] -ne $null) {$description = ($helpTxt[1].Text).Replace('`r`nE','')}
            if ($helpTxt[2] -ne $null) {$returnValue = ($helpTxt[2].Text).Replace('`r`nE','')}
            $sdkCmdletsList+= ("$name`t$description`t$route`t$returnValue")        
        }
        else
        {
            $route = $null
            $description = $null
            $returnValue = $null
            $sdkCmdletsList+= ("$name")
        }
    }
    else
    {
        $sdkCmdletsList+=("$name")
    }
}

$sdkCmdletsList | Out-File "$OutputDirectory\$ModuleName.cmdlets.txt"
Write-Debug "$sdkCmdletsList"
popd