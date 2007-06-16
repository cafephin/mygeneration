;-----------------------------------------
; DNPUtils MyGeneration Plugin Installer Script
;-----------------------------------------

; Set the compressions to lzma, which is always the best compression!
SetCompressor lzma 

; The name of the installer
Name "DnpUtils MyGeneration Plugin"

; The file to write
OutFile "dnputils_plugin.exe"

Icon ".\modern-install.ico"

XPStyle on

ShowInstDetails show

;LicenseText "Liscence Agreement"
;LicenseData "BSDLicense.rtf"

; The default installation directory
InstallDir $PROGRAMFILES\MyGeneration

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM SOFTWARE\MyGeneration\Plugins\DnpUtils "Install_Dir"

; The text to prompt the user to enter a directory
ComponentText "This will install the DnpUtils MyGeneration Plugin on your computer. Select which optional things you want installed."

; The text to prompt the user to enter a directory
DirText "Please select your MyGeneration Directory."


; The stuff to install
Section "Install Files and Reg Entries"

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR

  ; Get latest DLLs and EXE
  File /oname=Dnp.Utils.dll .\Dnp.Utils.dll
  File /oname=Dnp.Utils.chm .\Dnp.Utils.chm
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\MyGeneration\Plugins\DnpUtils "Install_Dir" "$INSTDIR"

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DnpUtilsPlugin" "DisplayName" "MyGeneration DnpUtils Plugin (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DnpUtilsPlugin" "UninstallString" '"$INSTDIR\uninstallDnpUtils.exe"'

  WriteUninstaller "uninstallDnpUtils.exe"

SectionEnd ; end the section

; optional section
Section "Start Menu Shortcuts"
  CreateDirectory "$SMPROGRAMS\MyGeneration\DnpUtils"
  CreateShortCut "$SMPROGRAMS\MyGeneration\DnpUtils\DnpUtils Plugin Reference.lnk" "$INSTDIR\Dnp.Utils.chm"
  CreateShortCut "$SMPROGRAMS\MyGeneration\DnpUtils\Uninstall.lnk" "$INSTDIR\uninstallDnpUtils.exe" "" "$INSTDIR\uninstallDnpUtils.exe" 0
SectionEnd

Section "Update ZeusConfig to Register Plugin in MyGeneration"
	;<IntrinsicObject assembly="%ZEUSHOME%\Dnp.Utils.dll" classpath="Dnp.Utils.Utils" varname="DnpUtils" /> 
	; -aio <dllpath> <classpath> <varname> | add an intrinsic object
	ExecWait `"$INSTDIR\ZeusCmd.exe" -aio "%ZEUSHOME%\Dnp.Utils.dll" "Dnp.Utils.Utils" "DnpUtils"`

SectionEnd

; uninstall stuff
UninstallText "This will uninstall the DnpUtils MyGeneration Plugin. Hit next to continue."
UninstallIcon ".\modern-uninstall.ico"

; special uninstall section.
Section "Uninstall"
  ; remove the entry in ZeusConfig.xml
	; -rio <varname>                       | remove an intrinsic object
 	ExecWait `"$INSTDIR\ZeusCmd.exe" -rio DnpUtils`

  ; remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DnpUtilsPlugin"
  DeleteRegKey HKLM SOFTWARE\MyGeneration\Plugins\DnpUtils

  ; MUST REMOVE UNINSTALLER, too
  Delete $INSTDIR\uninstallDnpUtils.exe
  Delete $INSTDIR\Dnp.Utils.*
 
  ; remove shortcuts, if any.
  Delete "$SMPROGRAMS\MyGeneration\DnpUtils\*.*"
  
  ; remove directories used.
  RMDir "$SMPROGRAMS\MyGeneration\DnpUtils"
  
SectionEnd

; eof