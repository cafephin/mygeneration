del ".\build.log"
del ".\installbuild_mygen.log"
del ".\installbuild_mymeta.log"
del ".\installbuild_doodads.log"
del ".\installbuild_cst2mygen.log"
del ".\installbuild_sqltool.log"

if "%DEVENV%"=="" set DEVENV="%PROGRAMFILES%\Microsoft Visual Studio 8\Common7\IDE\devenv.exe"  /out ".\build.log" /rebuild release

rem "..\mygeneration\MyGeneration\PluginResources\UpdateDLLs.bat"

%DEVENV% "..\plugins\MyMetaPlugins.sln"
%DEVENV% "..\plugins\ZeusPlugins.sln"
%DEVENV% "..\mygeneration\Zeus.sln"
%DEVENV% "..\ideplugins\visualstudio2005\MyGenVS2005.sln"
%DEVENV% "..\codesmith2mygen\CodeSmith2MyGeneration\CodeSmith2MyGeneration.sln"
set DEVENV=

if "%MAKENSIS%"=="" set MAKENSIS=%PROGRAMFILES%\NSIS\makensis.exe
"%MAKENSIS%" ".\mygeneration.nsi" > ".\installbuild_mygen.log"
"%MAKENSIS%"  ".\mymeta.nsi" > ".\installbuild_mymeta.log"
"%MAKENSIS%"  ".\doodads.nsi" > ".\installbuild_doodads.log"
"%MAKENSIS%"  ".\cst2mygen.nsi" > ".\installbuild_cst2mygen.log"
"%MAKENSIS%"  ".\mygen_plugin_sqltool.nsi" > ".\installbuild_sqltool.log"
set MAKENSIS=

