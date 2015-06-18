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
REM CALL packages\KoreBuild\build\dnvm upgrade -runtime CLR -x86 || set errorlevel=1
REM CALL packages\KoreBuild\build\dnvm install 1.0.0-beta2 -runtime CoreCLR -x86 || set errorlevel=1

@powershell -NoProfile -ExecutionPolicy unrestricted -Command "&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}"
CALL %USERPROFILE%\.dnx\bin\dnvm install latest -runtime CLR -x86 -alias default || set errorlevel=1
CALL %USERPROFILE%\.dnx\bin\dnvm install latest -runtime CoreCLR -x86 || set errorlevel=1

:run
REM CALL packages\KoreBuild\build\dnvm use default -runtime CLR -x86 || set errorlevel=1
REM packages\Sake\tools\Sake.exe -I packages\KoreBuild\build -f makefile.shade %*

CALL %USERPROFILE%\.dnx\bin\dnvm use default -runtime CLR -x86 || set errorlevel=1

CALL dnu restore || set errorlevel=1
CALL dnu build src\R4Mvc || set errorlevel=1
CALL dnu build src\R4MvcHostApp || set errorlevel=1
CALL dnu build test\R4Mvc.Test || set errorlevel=1


IF NOT EXIST test-results MD test-results

cd test\R4Mvc.Test
CALL dnx test -xml ..\..\test-results\R4Mvc.Test.clr.xml || set errorlevel=1

REM CALL %USERPROFILE%\.dnx\bin\dnvm use default -r coreclr || set errorlevel=1
REM CALL dnx test -xml ..\..\test-results\R4Mvc.Test.coreclr.xml || set errorlevel=1

exit /b %errorlevel%