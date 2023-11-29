# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/SuperCowAPI/*" -Force -Recurse
dotnet publish "./SuperCowAPI.csproj" -c Release -o "$env:RELOADEDIIMODS/SuperCowAPI" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location