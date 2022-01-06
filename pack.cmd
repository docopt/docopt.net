@echo off
pushd "%~dp0"
dotnet pwsh -NoProfile -File "%~n0.ps1" %*
popd && exit /b %ERRORLEVEL%
