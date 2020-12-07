$solutionName = “FileHandlerv1.wsp”
$featureName = "Custom File Handler Feature"
$folder = Get-Location
$webAppName=$args[0]

<#
if($webAppName -eq $null -or $webAppName -eq ''){
    Write-Host "$(Get-Date)": "  Enter valid URL to deploy" -ForegroundColor Red
    exit
}#>

Write-Host "Uploading solution to farm ..." -ForegroundColor Yellow
Add-SPSolution $folder\$solutionName
Write-Host "Deploying solution to the webapplications ..." -ForegroundColor Yellow
Start-Sleep -Seconds 5
Install-SPSolution –Identity $solutionName -GACDeployment -Force
Start-Sleep -Seconds 20

Write-Host "Restarting iis ..." -ForegroundColor Yellow
iisreset

Write-Host "Completed" -ForegroundColor Green