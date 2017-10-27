@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

SET build_log=".\build_mygeneration.log"
IF EXIST %build_log% DEL %build_log%

SET config=%1
IF [%config%]==[] (
    ECHO A build configuration ^(debug or release^) must be specified
    EXIT /b 1
)

xcopy ..\src\lib\thirdparty\*.dll ..\src\mygeneration\MyGeneration\bin\%config% /Y /C
xcopy ..\src\lib\thirdparty\*.xml ..\src\mygeneration\MyGeneration\bin\%config% /Y /C
xcopy ..\src\lib\plugins\*.dll ..\src\mygeneration\MyGeneration\bin\%config% /Y /C
xcopy ..\src\lib\plugins\*.xml ..\src\mygeneration\MyGeneration\bin\%config% /Y /C

SET build_cmd=msbuild /t:Rebuild /p:Configuration=%config% /flp:logfile=%build_log%;Append
%build_cmd% "..\src\mygeneration\MyGeneration.sln"
SET build_cmd=

xcopy ..\src\mygeneration\ZeusCmd\bin\%config%\ZeusCmd.* ..\src\mygeneration\MyGeneration\bin\%config% /Y /C
xcopy ..\src\templates\*.* ..\src\mygeneration\MyGeneration\bin\%config%\Templates /E /C /Y

ECHO ON
