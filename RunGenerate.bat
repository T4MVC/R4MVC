@ECHO OFF
IF NOT DEFINED CONFIGURATION SET CONFIGURATION=Debug
IF NOT "%1"=="" SET CONFIGURATION=%1

@ECHO ON
.\src\R4Mvc.Tools\bin\%CONFIGURATION%\net462\R4Mvc.Tools.exe generate -p .\samples\AspNetSimple\AspNetSimple.csproj
.\src\R4Mvc.Tools\bin\%CONFIGURATION%\net462\R4Mvc.Tools.exe generate -p .\samples\AspNetSimple.NetCore3\AspNetSimple.NetCore3.csproj
.\src\R4Mvc.Tools\bin\%CONFIGURATION%\net462\R4Mvc.Tools.exe generate -p .\samples\AspNetFeatureFolders\AspNetFeatureFolders.csproj
