$ErrorActionPreference = "Stop";

New-Item -ItemType Directory -Force -Path ./samples-artifacts

dotnet tool restore

Push-Location ./samples/Glasswall.O365.FileHandler.App.Test
Invoke-Expression "./build.ps1 $args"
Pop-Location