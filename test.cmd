@echo off
setlocal
pushd "%~dp0"
if not defined CI dotnet pack || goto :finally
set INSTALLED=
dotnet new --install src\ConsoleApp || goto :finally
set INSTALLED=1
dotnet new docopt-console -o tmp -n MyConsoleApp || goto :finally
dotnet run --project tmp -- --help || goto :finally
:finally
set EXIT_CODE=%ERRORLEVEL%
if exist tmp rmdir /s /q tmp
if defined INSTALLED dotnet new --uninstall src\ConsoleApp
popd
exit /b %EXIT_CODE%
