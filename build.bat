@echo off

path=%path%;%programfiles(x86)%\MSBuild\12.0\Bin

cd %~dp0

msbuild build.proj /p:RunTests="%1"

pause