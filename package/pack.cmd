@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

IF EXIST ".\package.log" DEL ".\package.log"

IF "%MAKENSIS%"=="" SET MAKENSIS=%PROGRAMFILES(X86)%\NSIS\makensis.exe

"%MAKENSIS%" ".\mygeneration.nsi" >> ".\package.log"
"%MAKENSIS%" ".\mymeta.nsi"       >> ".\package.log"
"%MAKENSIS%" ".\doodads.nsi"      >> ".\package.log"
"%MAKENSIS%" ".\cst2mygen.nsi"    >> ".\package.log"
SET MAKENSIS=

ECHO ON
