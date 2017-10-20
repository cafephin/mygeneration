;-----------------------------------------
; CodeSmith2MyGeneration 0.1 Beta Installation Script
;-----------------------------------------

!include ".\common_functions.nsh"
!define DNF4_URL "http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe"

!define OUT_PATH ".\installers"
!system 'mkdir "${OUT_PATH}"'

;--------------------------------------------------------
; Configurations
;--------------------------------------------------------
SetCompressor lzma ; Set the compressions to lzma
Name "CodeSmith2MyGeneration Convertion Tool Plugin 0.1 Beta" ; The name of the installer
OutFile "${OUT_PATH}\cst2mygen-installer.exe" ; The file to write
XPStyle on
Icon ".\icos\modern-install.ico"
ShowInstDetails show
LicenseText "Liscence Agreement"
LicenseData "..\LICENSE"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM SOFTWARE\CodeSmith2MyGeneration "Install_Dir"

; The text to prompt the user to enter a directory
ComponentText "This will install CodeSmith2MyGeneration on your computer."

;--------------------------------------------------------
; Download and install the .NET Framework 4
;--------------------------------------------------------
Section "-.Net Framework 4" net4_section_id

    Call DotNet4Exists
    Pop $1
    IntCmp $1 1 SkipDotNet4

    StrCpy $1 "dotNetFx40_Full_x86_x64.exe"
    StrCpy $2 "$EXEDIR\$1"
    IfFileExists $2 FileExistsAlready FileMissing

    FileMissing:
        DetailPrint ".NET Framework 4 not installed; downloading setup file"
        StrCpy $2 "$TEMP\$1"
        NSISdl::download "${DNF4_URL}" $2

    FileExistsAlready:
        DetailPrint "Installing the .NET Framework 4..."
        ExecWait '"$2" /quiet'

        Call DotNet4Exists
        Pop $1
        IntCmp $1 1 DotNet4Done DotNet4Failed

    DotNet4Failed:
        DetailPrint ".NET Framework 4 install failed; aborting installing"
        MessageBox MB_OK ".NET Framework 4 install failed; aborting installing"
        Abort

    SkipDotNet4:
        DetailPrint ".NET Framework 4 found; skipping installation"

    DotNet4Done:

SectionEnd

;--------------------------------------------------------
; Install
;--------------------------------------------------------
Section "Install Files, Registry Entries, Uninstaller, & Shortcuts"

  ReadRegStr $0 HKLM Software\MyGeneration13 "Install_Dir"
  DetailPrint "MyGeneration is installed at: $0"
  StrCpy $INSTDIR $0

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  CreateDirectory "$INSTDIR"

  File /oname=MyGeneration.UI.Plugins.CodeSmith2MyGen.dll ..\src\plugins\MyGeneration.UI.Plugins.CodeSmith2MyGen\bin\Release\MyGeneration.UI.Plugins.CodeSmith2MyGen.dll

  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\CodeSmith2MyGeneration "Install_Dir" "$INSTDIR"

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CodeSmith2MyGeneration" "DisplayName" "CodeSmith2MyGeneration Plugin (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CodeSmith2MyGeneration" "UninstallString" '"$INSTDIR\CodeSmith2MyGenUninstall.exe"'

  WriteUninstaller "CodeSmith2MyGenUninstall.exe"
  
  CreateDirectory "$SMPROGRAMS\MyGeneration 1.3"
  CreateShortCut "$SMPROGRAMS\MyGeneration 1.3\Uninstall CodeSmith2MyGen.lnk" "$INSTDIR\CodeSmith2MyGenUninstall.exe" "" "$INSTDIR\CodeSmith2MyGenUninstall.exe" 0

SectionEnd

;--------------------------------------------------------
; Uninstall
;--------------------------------------------------------
UninstallText "This will uninstall the CodeSmith2MyGeneration conversion tool plugin. Hit next to continue."
UninstallIcon ".\icos\modern-uninstall.ico"

Section "Uninstall"

  ; remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CodeSmith2MyGeneration"
  DeleteRegKey HKLM SOFTWARE\CodeSmith2MyGeneration

  ; MUST REMOVE UNINSTALLER, too
  Delete $INSTDIR\CodeSmith2MyGenUninstall.exe
  Delete $INSTDIR\MyGeneration.UI.Plugins.CodeSmith2MyGen.dll
   
  Delete "$SMPROGRAMS\MyGeneration 1.3\Uninstall CodeSmith2MyGen.lnk"

SectionEnd
