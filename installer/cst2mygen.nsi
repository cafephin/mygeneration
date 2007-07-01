; CodeSmith2MyGeneration 0.1 Beta Installation Script
;-----------------------------------------

; Set the compressions to lzma, which is always the best compression!
SetCompressor lzma 

; The name of the installer
Name "CodeSmith2MyGeneration Convertion Tool 0.1 Beta"

; The file to write
OutFile "cst2mygen010b.exe"

; Icon doesn't work for some reason
Icon ".\modern-install.ico"

XPStyle on

ShowInstDetails show

LicenseText "Liscence Agreement"
LicenseData "BSDLicense.rtf"

; The default installation directory
InstallDir $PROGRAMFILES\CodeSmith2MyGeneration

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM SOFTWARE\CodeSmith2MyGeneration "Install_Dir"

; The text to prompt the user to enter a directory
ComponentText "This will install the CodeSmith2MyGeneration on your computer. Select which optional things you want installed."

; The text to prompt the user to enter a directory
DirText "Choose an install directory for CodeSmith2MyGeneration."

; Install .Net Framework 2.0
Section "Detect .Net Framework 2.0"
  Call DotNet20Exists
  Pop $1
  IntCmp $1 0 SkipFramework
    MessageBox MB_OK|MB_ICONINFORMATION "You cannot run CodeSmith2MyGeneration without having the .Net Framework 2.0 installed. It is not included $\r$\nin the installer because the file is huge and most people already have it installed." IDOK
    ExecShell open http://www.microsoft.com/downloads/details.aspx?familyid=0856EACB-4362-4B0D-8EDD-AAB15C5E04F5&displaylang=en
    DetailPrint ".Net Framework 2.0 not installed... Aborting Installation."
    Abort
    Goto FrameworkDone
	SkipFramework:
		DetailPrint ".Net Framework 2.0 found... Continuing."
	FrameworkDone:
SectionEnd

; The stuff to install
Section "Install Files and Reg Entries"

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  CreateDirectory "$INSTDIR\Plugins"

  File /oname=CodeSmith2MyGeneration.exe ..\codesmith2mygen\CodeSmith2MyGeneration\bin\Release\CodeSmith2MyGeneration.exe
  File /oname=MyGeneration.CodeSmithConversion.dll ..\codesmith2mygen\CodeSmith2MyGeneration\bin\Release\MyGeneration.CodeSmithConversion.dll
  File /oname=earth.ico .\earth.ico
  File /oname=Plugins\SamplePlugin.plugin.cs ..\codesmith2mygen\CodeSmith2MyGeneration\Plugins\SamplePlugin.plugin.cs
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\CodeSmith2MyGeneration "Install_Dir" "$INSTDIR"

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CodeSmith2MyGeneration" "DisplayName" "CodeSmith2MyGeneration (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CodeSmith2MyGeneration" "UninstallString" '"$INSTDIR\uninstall.exe"'

  WriteUninstaller "uninstall.exe"

SectionEnd ; end the section

; optional section
Section "Start Menu Shortcuts"
  CreateDirectory "$SMPROGRAMS\CodeSmith2MyGeneration"
  CreateShortCut "$SMPROGRAMS\CodeSmith2MyGeneration\CodeSmith2MyGeneration.lnk" "$INSTDIR\CodeSmith2MyGeneration.exe" "" "$INSTDIR\CodeSmith2MyGeneration.exe" 0
  CreateShortCut "$SMPROGRAMS\CodeSmith2MyGeneration\MyGeneration Website.lnk" "http://www.mygenerationsoftware.com/" "" "$INSTDIR\earth.ico" 0
  CreateShortCut "$SMPROGRAMS\CodeSmith2MyGeneration\CodeSmith Website.lnk" "http://www.codesmithtools.com/" "" "$INSTDIR\earth.ico" 0
  CreateShortCut "$SMPROGRAMS\CodeSmith2MyGeneration\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
SectionEnd

; Launch Program After Install with messagebox
Section
  MessageBox MB_YESNO|MB_ICONQUESTION "Launch CodeSmith2MyGeneration?" IDNO DontLaunchThingy
  	ExecShell open "$INSTDIR\CodeSmith2MyGeneration.exe" SW_SHOWNORMAL
  	Quit
  DontLaunchThingy:
SectionEnd


; uninstall stuff
UninstallText "This will uninstall the CodeSmith2MyGeneration conversion tool. Hit next to continue."
UninstallIcon ".\modern-uninstall.ico"

; special uninstall section.
Section "Uninstall"
    
  ; remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CodeSmith2MyGeneration"
  DeleteRegKey HKLM SOFTWARE\CodeSmith2MyGeneration

  ; MUST REMOVE UNINSTALLER, too
  Delete $INSTDIR\uninstall.exe
 
  ; remove shortcuts, if any.
  Delete "$SMPROGRAMS\CodeSmith2MyGeneration\*.*"
  
  ; remove directories used.
  RMDir "$SMPROGRAMS\CodeSmith2MyGeneration"
  
  ;RMDir /r "$INSTDIR"
  Delete $INSTDIR\*.exe
  Delete $INSTDIR\*.dll
  
SectionEnd

; detects Microsoft .Net Framework 2.0
Function DotNet20Exists

	ClearErrors
	ReadRegStr $1 HKLM "SOFTWARE\Microsoft\.NETFramework\policy\v2.0" "50727"
	IfErrors MDNFNotFound MDNFFound

	MDNFFound:
		Push 0
		Goto ExitFunction
		
	MDNFNotFound:
		Push 1
		Goto ExitFunction

	ExitFunction:

FunctionEnd

; eof