del ".\build.log"
del ".\installbuild_mygen.log"
del ".\installbuild_mymeta.log"
del ".\installbuild_doodads.log"

if "%DEVENV%"=="" set DEVENV="%PROGRAMFILES%\Microsoft Visual Studio 9.0\Common7\IDE\VCSExpress.exe"  /out ".\build.log" /rebuild release


%DEVENV% "..\plugins\MyMetaPlugins.sln"
%DEVENV% "..\plugins\ZeusPlugins.sln"
%DEVENV% "..\mygeneration\Zeus.sln"
%DEVENV% "..\ideplugins\visualstudio2005\MyGenVS2005.sln"
set DEVENV=

if "%MAKENSIS%"=="" set MAKENSIS=%PROGRAMFILES%\NSIS\makensis.exe
"%MAKENSIS%" ".\mygeneration.nsi" > ".\installbuild_mygen.log"
"%MAKENSIS%"  ".\mymeta.nsi" > ".\installbuild_mymeta.log"
"%MAKENSIS%"  ".\doodads.nsi" > ".\installbuild_doodads.log"
"%MAKENSIS%"  ".\cst2mygen.nsi" > ".\installbuild_cst2mygen.log"
set MAKENSIS=

