@echo off
pushd "%~dp0"
dotnet pwsh -NoProfile -File pack.ps1 %*
popd && exit /b %ERRORLEVEL%
