@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

SET build_log=".\build.log"
IF EXIST %build_log% DEL %build_log%

SET config=%1
IF [%config%]==[] (
    ECHO A build configuration ^(debug or release^) must be specified
    EXIT /b 1
)

ECHO Building plugin assemblies... >> %build_log%
CALL .\build_plugins.cmd %config%
ECHO OFF
ECHO Done >> %build_log%

ECHO Building ZeusCmd... >> %build_log%
CALL .\build_zeuscmd.cmd %config%
ECHO OFF
ECHO Done >> %build_log%

ECHO Building MyGeneration... >> %build_log%
CALL .\build_mygeneration.cmd %config%
ECHO OFF
ECHO Done >> %build_log%

ECHO ON
