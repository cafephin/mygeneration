del ".\build.log"
"%PROGRAMFILES%\Microsoft Visual Studio 8\Common7\IDE\devenv.exe" "..\mygeneration\Zeus.sln" /out ".\build.log" /rebuild release
"%PROGRAMFILES%\Microsoft Visual Studio 8\Common7\IDE\devenv.exe" "..\plugins\MyMetaPlugins.sln" /out ".\build.log" /rebuild release
"%PROGRAMFILES%\Microsoft Visual Studio 8\Common7\IDE\devenv.exe" "..\codesmith2mygen\CodeSmith2MyGeneration\CodeSmith2MyGeneration.sln" /out ".\build.log" /rebuild release
"%PROGRAMFILES%\NSIS\makensis.exe" ".\mygeneration.nsi" > ".\installbuild_mygen.log"
"%PROGRAMFILES%\NSIS\makensis.exe" ".\mymeta.nsi" > ".\installbuild_mymeta.log"
"%PROGRAMFILES%\NSIS\makensis.exe" ".\doodads.nsi" > ".\installbuild_doodads.log"
