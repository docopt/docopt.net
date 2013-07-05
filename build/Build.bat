@ECHO OFF
SET MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
SET VERBOSE=normal
SET BUILDIR=%~dp0

SET LOGDIR=%BUILDIR%\..\_build\build-logs
IF EXIST "%LOGDIR%" DEL /S /Q %LOGDIR%
IF NOT EXIST "%LOGDIR%" MKDIR %LOGDIR%
SET LOGFILE=%LOGDIR%\build.log
ECHO Outputing log to %LOGFILE%

%MSBUILD% "%BUILDIR%\Bootstrap.proj" /target:RestorePackages /clp:minimal /flp:PerformanceSummary;verbosity=normal;logFile=%LOGFILE% /nologo
