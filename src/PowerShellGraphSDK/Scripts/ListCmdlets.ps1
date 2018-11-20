param(
    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]$ModuleName='Microsoft.Graph.Intune',

    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]$OutputDirectory='.\src\GraphODataPowerShellWriter\bin\Release\output\bin\Release\net471'
)

$OutputDirectory = $OutputDirectory | Resolve-Path
$modulePath = "$OutputDirectory/$ModuleName.psd1"
Write-Host "OutputDirectory: $OutputDirectory"
Write-Host "ModulePath: $modulePath"

Push-Location $OutputDirectory

Import-Module $modulePath
$sdkCmdlets = ((Get-Command -Module Microsoft.Graph.Intune) | Sort-Object)
$cmdletCount = $sdkCmdlets.Count
Write-Host "$cmdletCount cmdlets exported by $modulePath"

$sdkCmdletsList = @()
for($cmdletCount=0; $cmdletCount -lt $sdkCmdlets.Count; $cmdletCount++)
{
    $name = $sdkCmdlets[$cmdletCount]
    $helpTxt = (Get-Help $sdkCmdlets[$cmdletCount]).Description
    if ($helpTxt -ne $null)
    {   
        if ($helpTxt.Count -eq 1)
        {
            $description = $helpTxt[0].Text
            $sdkCmdletsList+= ("CMDLET: $name`n`t")
        }
        if ($helpTxt.Count -eq 2)
        {
            $route = $helpTxt[0].Text
            $description = $helpTxt[1].Text
            $sdkCmdletsList+= ("CMDLET: $name`n`tROUTE:$route`tDESCRIPTION:$description")
        }
        if ($helpTxt.Count -eq 3)
        {
            $route = $helpTxt[0].Text
            $description = $helpTxt[1].Text
            $returnValue = $helpTxt[2].Text
            $sdkCmdletsList+= ("CMDLET: $name`n`tROUTE:$route`tDESCRIPTION:$description`tRETURNS:$returnValue")
        }        
    }
    else
    {
        $route = $null
        $description = $null
        $returnValue = $null
    }
    $sdkCmdletsList+= ("CMDLET: " + $name)
}

$sdkCmdletsList | Out-File "$OutputDirectory\$ModuleName.cmdlets.txt"
Write-Debug "$sdkCmdletsList"
Write-Host "Module cmdlets list generated."