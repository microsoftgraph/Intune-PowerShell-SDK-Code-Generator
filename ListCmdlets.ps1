pushd .\src\GraphODataPowerShellWriter\bin\Release\output\bin\Release\net471
Import-Module .\Microsoft.Graph.Intune.psd1
$sdkCmdlets = Get-Command -Module Microsoft.Graph.Intune
$sdkCmdlets > .\cmdlets.short.txt
$sdkCmdlets