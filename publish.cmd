cls

dotnet build RobotsTxtParser.sln --configuration Release

@echo off

set ApiKey=%NUGET_API_KEY%

if not defined ApiKey (
    cls
    echo API key not set. Please set the NUGET_API_KEY environment variable and restart the terminal.
    exit /b 1
)

for /R ".\.published\" %%A in ("RobotsTxtParser.*.nupkg") do (set LatestPackage=%%A)
"./nuget.exe" push %LatestPackage% -Source https://api.nuget.org/v3/index.json -ApiKey %ApiKey%

pause