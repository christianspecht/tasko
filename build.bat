@echo off

path=%path%;%programfiles(x86)%\MSBuild\12.0\Bin

cd %~dp0

call version-number.bat

msbuild build.proj /p:RunTests="%1"

pause