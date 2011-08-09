@echo off
if %1!==! goto help

if %1==-r goto release
if %1==-d goto debug
goto help

:release
set CONFIG=RELEASE
goto build

:debug
set CONFIG=DEBUG
goto build

:build
call cleanSolution.bat
c:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild CoreDeveloper.sln /p:Configuration=%CONFIG%
goto exit

:help
echo -r build a release
echo -d build with debug symbols

:exit