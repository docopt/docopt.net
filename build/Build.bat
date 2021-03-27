@ECHO OFF
SETLOCAL
SET MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
SET VERBOSE=normal
SET BUILDIR=%~dp0

SET LOGDIR=%BUILDIR%\..\_build\log
IF EXIST "%LOGDIR%" DEL /S /Q %LOGDIR%
IF NOT EXIST "%LOGDIR%" MKDIR %LOGDIR%
SET LOGFILE=%LOGDIR%\build.log
ECHO Outputing log to %LOGFILE%

..\.paket\paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)

..\.paket\paket.exe restore
if errorlevel 1 (
  exit /b %errorlevel%
)

dotnet restore %BUILDIR%\..\src\DocoptNet
dotnet build %BUILDIR%\..\src\DocoptNet -c Release

%MSBUILD% "%BUILDIR%\Main.proj" /target:Build /clp:minimal /flp:PerformanceSummary;verbosity=%VERBOSE%;logFile=%LOGFILE%;Append /nologo

IF NOT EXIST %BUILDIR%\..\_build\dist MKDIR %BUILDIR%\..\_build\dist
%BUILDIR%\..\packages\NuGet.CommandLine\tools\NuGet.exe pack %BUILDIR%\..\src\NuGet\docopt.net.nuspec -OutputDirectory %BUILDIR%\..\_build\dist
