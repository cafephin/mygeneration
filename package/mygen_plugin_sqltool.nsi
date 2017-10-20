;-----------------------------------------
; MyGenerationSqlTool 0.1 Beta Installation Script
;-----------------------------------------

!include ".\common_functions.nsh"

!define OUT_PATH ".\installers"
!system 'mkdir "${OUT_PATH}"'

;--------------------------------------------------------
; Configurations
;--------------------------------------------------------
SetCompressor lzma ; Set the compressions to lzma
Name "MyGeneration Sql Tool Plugin 0.1 Beta" ; The name of the installer
OutFile "${OUT_PATH}\mygen-plugin-sqltool-installer.exe" ; The file to write
Icon ".\icos\modern-install.ico"
XPStyle on
ShowInstDetails show
LicenseText "Liscence Agreement"
LicenseData "..\LICENSE"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM SOFTWARE\MyGenerationSqlTool "Install_Dir"

; The text to prompt the user to enter a directory
ComponentText "Install MyGeneration SqlTool"

;--------------------------------------------------------
; Download and install the .NET Framework 4
;--------------------------------------------------------
Section "-.Net Framework 4" net4_section_id
	Call DotNet4Exists
	Pop $1
	IntCmp $1 1 SkipDotNet4

	StrCpy $1 "dotNetFx40_Full_setup.exe"
	StrCpy $2 "$EXEDIR\$1"
	IfFileExists $2 FileExistsAlready FileMissing

	FileMissing:
		DetailPrint ".Net Framework 4 not installed... Downloading file."
		StrCpy $2 "$TEMP\$1"
		NSISdl::download "${DNF4_URL}" $2

	FileExistsAlready:
		DetailPrint "Installing the .Net Framework 4."
		;ExecWait '"$SYSDIR\msiexec.exe" "$2" /quiet'
		ExecWait '"$2" /quiet'

		Call DotNet4Exists
		Pop $1
		IntCmp $1 1 DotNet4Done DotNet4Failed

	DotNet4Failed:
		DetailPrint ".Net Framework 4 install failed... Aborting Install"
		MessageBox MB_OK ".Net Framework 4 install failed... Aborting Install"
		Abort

	SkipDotNet4:
		DetailPrint ".Net Framework 4 found... Continuing."

	DotNet4Done:
SectionEnd

;--------------------------------------------------------
; Install
;--------------------------------------------------------
Section "Install Files, Reg Entries, Uninstaller, & Shortcuts"

  ReadRegStr $0 HKLM Software\MyGeneration13 "Install_Dir"
  DetailPrint "MyGeneration is installed at: $0"
  StrCpy $INSTDIR $0

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  CreateDirectory "$INSTDIR"

  File /oname=MyGeneration.UI.Plugins.SqlTool.dll ..\plugins\MyGeneration.UI.Plugins.SqlTool\bin\Release\MyGeneration.UI.Plugins.SqlTool.dll

  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\MyGenerationSqlTool "Install_Dir" "$INSTDIR"

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyGenerationSqlTool" "DisplayName" "MyGenerationSqlTool Plugin (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyGenerationSqlTool" "UninstallString" '"$INSTDIR\MyGenerationSqlToolUninstall.exe"'

  WriteUninstaller "MyGenerationSqlToolUninstall.exe"
  
  CreateDirectory "$SMPROGRAMS\MyGeneration 1.3"
  CreateShortCut "$SMPROGRAMS\MyGeneration 1.3\Uninstall MyGeneration SqlTool.lnk" "$INSTDIR\MyGenerationSqlToolUninstall.exe" "" "$INSTDIR\MyGenerationSqlToolUninstall.exe" 0

SectionEnd

;--------------------------------------------------------
; Uninstall
;--------------------------------------------------------
UninstallText "Uninstall MyGeneration SqlTool"
UninstallIcon ".\icos\modern-uninstall.ico"

Section "Uninstall"

  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyGenerationSqlTool"
  DeleteRegKey HKLM SOFTWARE\MyGenerationSqlTool

  ; must remove uninstaller too
  Delete $INSTDIR\MyGenerationSqlToolUninstall.exe
  Delete $INSTDIR\MyGeneration.UI.Plugins.SqlTool.dll
   
  Delete "$SMPROGRAMS\MyGeneration 1.3\Uninstall MyGeneration SqlTool.lnk"

SectionEnd
