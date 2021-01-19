$ErrorActionPreference = "Stop";

New-Item -ItemType Directory -Force -Path ./artifacts

dotnet tool restore

Push-Location ./src/FileHandler
Invoke-Expression "./build.ps1 $args"
Pop-Location