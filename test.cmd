@echo off
setlocal
pushd "%~dp0"
dotnet pack
dotnet new --install src\ConsoleApp || goto :finally
dotnet new docopt-console -o tmp -n MyConsoleApp || goto :finally
dotnet run --project tmp -- --help || goto :finally
:finally
set EXIT_CODE=%ERRORLEVEL%
if exist tmp rmdir /s /q tmp
dotnet new --uninstall src\ConsoleApp
popd
exit /b %EXIT_CODE%
