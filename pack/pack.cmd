@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

IF EXIST ".\pack.log" DEL ".\pack.log"

IF "%MAKENSIS%"=="" SET MAKENSIS=%PROGRAMFILES(X86)%\NSIS\makensis.exe

"%MAKENSIS%" ".\mygeneration.nsi" >> ".\pack.log"
"%MAKENSIS%" ".\mymeta.nsi"       >> ".\pack.log"
"%MAKENSIS%" ".\doodads.nsi"      >> ".\pack.log"
"%MAKENSIS%" ".\cst2mygen.nsi"    >> ".\pack.log"
SET MAKENSIS=

ECHO ON
