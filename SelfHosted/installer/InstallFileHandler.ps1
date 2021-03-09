$solutionName = "Glasswall.FileHandler.wsp"
$folder = $args[0] #absolute path to folder where wsp file is located for e.g. c:\folder
if($null -eq $folder) {
    throw("Absolute folder path to WSP not provided.")
}
if (-not (Test-Path -Path "$folder\$solutionName")) {
    throw("Package $solutionName not found at $folder.")
}

# Check if solution exists, remove if exists
Write-Host "Checking if solution exists..."
$s = Get-SPSolution -ErrorVariable err -ErrorAction SilentlyContinue -identity $solutionName
if($err) {
    Write-Host "Solution does not exist..."
}
else {
    Write-Host "Solution exists in farm..."
    Write-Host "Uninstalling solution from farm ..."
    Uninstall-SPSolution -identity $solutionName -Confirm:$false
    $wspSolutionForUninstall = Get-SPSolution -identity $solutionName
    while ($wspSolutionForUninstall.JobExists) {
        Write-Host '.' -NoNewline
        Start-Sleep -Seconds:1
        $wspSolutionForUninstall = Get-SPSolution -identity $solutionName
    }
    Write-Host "`nRemoving from the farm ..."
    Remove-SPSolution -identity $solutionName -Confirm:$false -Force
    Start-Sleep -Seconds 20
    Write-Host Completed
}

# Add solution
Write-Host Uploading solution to farm ...
$s = Add-SPSolution $folder\$solutionName
Write-Host Deploying solution to the webapplications ... 
Start-Sleep -Seconds 20
Install-SPSolution -identity $solutionName -GACDeployment -Force
Start-Sleep -Seconds 20

Write-Host Restarting iis ...
iisreset
Write-Host Completed
