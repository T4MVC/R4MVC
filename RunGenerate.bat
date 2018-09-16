@ECHO OFF
IF NOT DEFINED CONFIGURATION SET CONFIGURATION=Debug
IF NOT "%1"=="" SET CONFIGURATION=%1

@ECHO ON
.\src\R4Mvc.Tools\bin\%CONFIGURATION%\net462\r4mvc.exe generate .\samples\AspNetSimple\AspNetSimple.csproj
.\src\R4Mvc.Tools\bin\%CONFIGURATION%\net462\r4mvc.exe generate .\samples\AspNetFeatureFolders\AspNetFeatureFolders.csproj
