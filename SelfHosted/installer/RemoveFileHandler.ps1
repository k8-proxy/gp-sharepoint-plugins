$solutionName = “FileHandlerv1.wsp”
$featureName = "Custom File Handler Feature"
$folder = Get-Location
$webAppName=$args[0]

<#
if($webAppName -eq $null -or $webAppName -eq ''){
    Write-Host "$(Get-Date)": "  Enter valid URL to deploy" -ForegroundColor Red
    exit
}#>

Write-Host "Uninstalling solution from farm ..." -ForegroundColor Yellow
Uninstall-SPSolution -identity $solutionName
Write-Host "Removing from the farm ..." -ForegroundColor Yellow
Start-Sleep -Seconds 20
Remove-SPSolution –Identity $solutionName
Start-Sleep -Seconds 10
Write-Host "Completed" -ForegroundColor Green

Write-Host "Restarting iis ..." -ForegroundColor Yellow
iisreset
