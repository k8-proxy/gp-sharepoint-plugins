$solutionName = "Glasswall.FileHandler.wsp"

Write-Host "Uninstalling solution from farm ..." -ForegroundColor Yellow
Uninstall-SPSolution -identity $solutionName
Write-Host "Removing from the farm ..." -ForegroundColor Yellow
Start-Sleep -Seconds 20
Remove-SPSolution –Identity $solutionName
Start-Sleep -Seconds 10
Write-Host "Completed" -ForegroundColor Green

Write-Host "Restarting iis ..." -ForegroundColor Yellow
iisreset
Write-Host "Completed" -ForegroundColor Green