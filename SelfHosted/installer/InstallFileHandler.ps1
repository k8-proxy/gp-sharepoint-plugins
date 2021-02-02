$solutionName = "Glasswall.FileHandler.wsp"
$folder = $args[0]

Write-Host "Uploading solution to farm ..." -ForegroundColor Yellow
try
{
    Add-SPSolution $folder\$solutionName
}
catch
{
    Write-Host "Solution exists in farm ..." -ForegroundColor Yellow
}
Write-Host "Deploying solution to the webapplications ..." -ForegroundColor Yellow
Start-Sleep -Seconds 5
Install-SPSolution -Identity $solutionName -GACDeployment -Force
Start-Sleep -Seconds 20

Write-Host "Restarting iis ..." -ForegroundColor Yellow
iisreset

Write-Host "Completed" -ForegroundColor Green