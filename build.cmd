@echo off
cd %~dp0

SETLOCAL ENABLEEXTENSIONS
SET CACHED_NUGET=%LocalAppData%\NuGet\NuGet.exe

IF EXIST %CACHED_NUGET% goto copynuget
echo Downloading latest version of NuGet.exe...
IF NOT EXIST %LocalAppData%\NuGet md %LocalAppData%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://www.nuget.org/nuget.exe' -OutFile '%CACHED_NUGET%'"

:copynuget
IF EXIST .nuget\nuget.exe goto restore
md .nuget
copy %CACHED_NUGET% .nuget\nuget.exe > nul

:restore
IF EXIST packages\KoreBuild goto run
.nuget\NuGet.exe install KoreBuild -ExcludeVersion -o packages -nocache -pre
.nuget\NuGet.exe install Sake -version 0.2 -o packages -ExcludeVersion 

IF "%SKIP_KRE_INSTALL%"=="1" goto run
REM CALL packages\KoreBuild\build\kvm upgrade -runtime CLR -x86
REM CALL packages\KoreBuild\build\kvm install 1.0.0-beta2 -runtime CoreCLR -x86

@powershell -NoProfile -ExecutionPolicy unrestricted -Command "iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/master/kvminstall.ps1'))"
kvm install 1.0.0-beta2

:run
REM CALL packages\KoreBuild\build\kvm use default -runtime CLR -x86
REM packages\Sake\tools\Sake.exe -I packages\KoreBuild\build -f makefile.shade %*

CALL kpm restore 
CALL kpm build src\R4Mvc
CALL kpm build src\R4MvcHostApp
CALL kpm build test\R4Mvc.Test

cd test\R4Mvc.Test
CALL k test
