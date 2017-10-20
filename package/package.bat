if exist ".\package.log" del ".\package.log"

if "%PROGRAMFILES(X86)%"=="" goto :x86
goto :x64
:x86
	if "%MAKENSIS%"=="" set MAKENSIS=%PROGRAMFILES%\NSIS\makensis.exe
	goto done
:x64
	if "%MAKENSIS%"=="" set MAKENSIS=%PROGRAMFILES(X86)%\NSIS\makensis.exe
    goto done
:done

"%MAKENSIS%" ".\mygeneration.nsi" >> ".\package.log"
"%MAKENSIS%" ".\mymeta.nsi"       >> ".\package.log"
"%MAKENSIS%" ".\doodads.nsi"      >> ".\package.log"
"%MAKENSIS%" ".\cst2mygen.nsi"    >> ".\package.log"
set MAKENSIS=
