@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

SET build_log=".\build_zeuscmd.log"
IF EXIST %build_log% DEL %build_log%

SET config=%1
IF [%config%]==[] (
    ECHO A build configuration ^(debug or release^) must be specified
    EXIT /b 1
)

xcopy ..\src\lib\thirdparty\*.dll ..\src\mygeneration\ZeusCmd\bin\%config% /Y /C
xcopy ..\src\lib\thirdparty\*.xml ..\src\mygeneration\ZeusCmd\bin\%config% /Y /C
xcopy ..\src\lib\plugins\*.dll ..\src\mygeneration\ZeusCmd\bin\%config% /Y /C
xcopy ..\src\lib\plugins\*.xml ..\src\mygeneration\ZeusCmd\bin\%config% /Y /C

SET build_cmd=msbuild /t:Rebuild /p:Configuration=%config% /flp:logfile=%build_log%;Append
%build_cmd% ..\src\mygeneration\ZeusCmd.sln
rem SET build_cmd=

ECHO ON
