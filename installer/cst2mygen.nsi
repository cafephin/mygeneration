; CodeSmith2MyGeneration 0.1 Beta Installation Script
;-----------------------------------------

; Set the compressions to lzma, which is always the best compression!
SetCompressor lzma 

; The name of the installer
Name "CodeSmith2MyGeneration Convertion Tool Plugin 0.1 Beta"

; The file to write
OutFile "mygen_plugin_cst2mygen010b.exe"

; Icon doesn't work for some reason
Icon ".\modern-install.ico"

XPStyle on

ShowInstDetails show

LicenseText "Liscence Agreement"
LicenseData "BSDLicense.rtf"


; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM SOFTWARE\CodeSmith2MyGeneration "Install_Dir"

; The text to prompt the user to enter a directory
ComponentText "This will install the CodeSmith2MyGeneration on your computer. Select which optional things you want installed."

; The text to prompt the user to enter a directory
;DirText "Choose an install directory for CodeSmith2MyGeneration."

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
Section "Install Files, Reg Entries, Uninstaller, & Shortcuts"

  ReadRegStr $0 HKLM Software\MyGeneration13 "Install_Dir"
  DetailPrint "MyGeneration is installed at: $0"
  StrCpy $INSTDIR $0

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  CreateDirectory "$INSTDIR"

  File /oname=MyGeneration.UI.Plugins.CodeSmith2MyGen.dll ..\plugins\MyGeneration.UI.Plugins.CodeSmith2MyGen\bin\Release\MyGeneration.UI.Plugins.CodeSmith2MyGen.dll

  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\CodeSmith2MyGeneration "Install_Dir" "$INSTDIR"

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CodeSmith2MyGeneration" "DisplayName" "CodeSmith2MyGeneration Plugin (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CodeSmith2MyGeneration" "UninstallString" '"$INSTDIR\CodeSmith2MyGenUninstall.exe"'

  WriteUninstaller "CodeSmith2MyGenUninstall.exe"
  
  CreateDirectory "$SMPROGRAMS\MyGeneration 1.3"
  CreateShortCut "$SMPROGRAMS\MyGeneration 1.3\Uninstall CodeSmith2MyGen.lnk" "$INSTDIR\CodeSmith2MyGenUninstall.exe" "" "$INSTDIR\CodeSmith2MyGenUninstall.exe" 0

SectionEnd ; end the section

; uninstall stuff
UninstallText "This will uninstall the CodeSmith2MyGeneration conversion tool plugin. Hit next to continue."
UninstallIcon ".\modern-uninstall.ico"

; special uninstall section.
Section "Uninstall"
    
  ; remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CodeSmith2MyGeneration"
  DeleteRegKey HKLM SOFTWARE\CodeSmith2MyGeneration

  ; MUST REMOVE UNINSTALLER, too
  Delete $INSTDIR\CodeSmith2MyGenUninstall.exe
  Delete $INSTDIR\MyGeneration.UI.Plugins.CodeSmith2MyGen.dll
   
  Delete "$SMPROGRAMS\MyGeneration 1.3\Uninstall CodeSmith2MyGen.lnk"
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