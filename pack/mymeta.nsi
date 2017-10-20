;-----------------------------------------
; MyMeta Installation Script
;-----------------------------------------

!include ".\common_functions.nsh"
!define DNF4_URL "http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe"

!define OUT_PATH ".\installers"
!system 'mkdir "${OUT_PATH}"'

;--------------------------------------------------------
; Configuration
;--------------------------------------------------------

SetCompressor lzma ; set the compressions to lzma
Name "MyMeta" ; The name of the installer
OutFile "${OUT_PATH}\mymeta-installer.exe" ; The file to write
Icon ".\icos\modern-install.ico"
XPStyle on
ShowInstDetails show
LicenseText "Liscence Agreement"
LicenseData "..\LICENSE"

; The default installation directory
InstallDir $PROGRAMFILES\MyGenerations

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM SOFTWARE\MyGeneration "Install_Dir"

; The text to prompt the user to enter a directory
ComponentText "This will install the MyMeta Metadata API."

; The text to prompt belithe user to enter a directory
DirText "Choose an install directory for MyMeta."

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
; Install MDAC 2.7
;--------------------------------------------------------
Section "Detect MDAC 2.7+"

  Call GetWindowsVersion
  Pop $R0
  StrCmp $R0 "Vista" SkipVistaMDAC
  
  Call MDAC27Exists
  Pop $1
  IntCmp $1 0 SkipMDAC
    MessageBox MB_OK|MB_ICONINFORMATION "You cannot run MyGeneration without having MDAC 2.7+ installed. It is not included $\r$\nin the installer because the file is large and most people already have it installed." IDOK
    ExecShell open http://www.microsoft.com/downloads/details.aspx?FamilyID=6c050fe3-c795-4b7d-b037-185d0506396c&DisplayLang=en
    DetailPrint "MDAC 2.7+ not installed; aborting installation"
    Abort
    Goto MDACDone
  SkipVistaMDAC:
    DetailPrint "MDAC is not required on Windows Vista; skipping installation"
    Goto MDACDone
  SkipMDAC:
    DetailPrint "MDAC 2.7+ found; skipping installation"
  MDACDone:

SectionEnd

;--------------------------------------------------------
; Install
;--------------------------------------------------------
Section "Install Files and Reg Entries"

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ;Create Settings Directory 
  ;ExecShell mkdir $INSTDIR\Settings
  
  File /oname=adodb.dll ..\src\lib\thirdparty\adodb.dll
  File /oname=System.Data.SQLite.DLL ..\src\lib\thirdparty\System.Data.SQLite.DLL
  File /oname=Npgsql.dll ..\src\lib\thirdparty\Npgsql.dll
  File /oname=Mono.Security.dll ..\src\lib\thirdparty\Mono.Security.dll
  File /oname=FirebirdSql.Data.FirebirdClient.dll ..\src\lib\thirdparty\FirebirdSql.Data.FirebirdClient.dll
  File /oname=MySql.Data.dll ..\src\lib\thirdparty\MySql.Data.dll
  File /oname=EffiProz.dll ..\src\lib\thirdparty\EffiProz.dll
    
  File /nonfatal /oname=MyMeta.Plugins.DelimitedText.dll ..\src\plugins\MyMetaTextFilePlugin\bin\Release\MyMeta.Plugins.DelimitedText.dll
  File /nonfatal /oname=MyMeta.Plugins.SqlCe.dll ..\src\plugins\MyMetaSqlCePlugin\bin\Release\MyMeta.Plugins.SqlCe.dll
  File /nonfatal /oname=MyMeta.Plugins.SybaseASE.dll ..\src\plugins\MyMetaSybaseASEPlugin\bin\Release\MyMeta.Plugins.SybaseASE.dll
  File /nonfatal /oname=MyMeta.Plugins.SybaseASA.dll ..\src\plugins\MyMetaSybaseASAPlugin\bin\Release\MyMeta.Plugins.SybaseASA.dll
  File /nonfatal /oname=MyMeta.Plugins.Ingres2006.dll ..\src\plugins\MyMetaIngres2006Plugin\bin\Release\MyMeta.Plugins.Ingres2006.dll
  File /nonfatal /oname=MyMeta.Plugins.EffiProz.dll ..\src\plugins\MyMetaEffiProzPlugin\bin\Release\MyMeta.Plugins.EffiProz.dll

  File /oname=Interop.ADOX.dll ..\src\mymeta\bin\Release\Interop.ADOX.dll
  File /oname=MyMeta.dll ..\src\mymeta\bin\Release\MyMeta.dll
  File /oname=MyMeta.tlb ..\src\mymeta\bin\Release\MyMeta.tlb
  File /oname=MyMeta.chm ..\src\mymeta\MyMeta.chm

  CreateDirectory "$INSTDIR\Settings"
  
  ;Rename file if it already exists
  Delete Settings\Languages.xml.4.old
  Rename Settings\Languages.xml.3.old Settings\Languages.xml.4.old
  Rename Settings\Languages.xml.2.old Settings\Languages.xml.3.old
  Rename Settings\Languages.xml.1.old Settings\Languages.xml.2.old
  Rename Settings\Languages.xml Settings\Languages.xml.1.old

  ;Rename file if it already exists
  Delete Settings\DbTargets.xml.4.old
  Rename Settings\DbTargets.xml.3.old Settings\DbTargets.xml.xml.4.old
  Rename Settings\DbTargets.xml.2.old Settings\DbTargets.xml.3.old
  Rename Settings\DbTargets.xml.1.old Settings\DbTargets.xml.2.old
  Rename Settings\DbTargets.xml Settings\DbTargets.xml.1.old

  ; Copy the config files into the Settings folder
  File /oname=Settings\DbTargets.xml ..\src\mygeneration\MyGeneration\Settings\DbTargets.xml
  File /oname=Settings\Languages.xml ..\src\mygeneration\MyGeneration\Settings\Languages.xml
 
SectionEnd

;--------------------------------------------------------
; Install Xsd3b Provider
;--------------------------------------------------------
Section "Install Xsd3b Provider for xml (xsd, uml, entityrelationship)"
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  File /nonfatal /oname=MyMeta.Plugins.Xsd3b.dll ..\src\plugins\MyMetaXsd3bPlugin\bin\Release\MyMeta.Plugins.Xsd3b.dll
  File /nonfatal ..\src\lib\thirdparty\Dl3bak.*.dll
  File /nonfatal ..\docs\help\*xsd3b*.chm

  SetOutPath "$INSTDIR\Templates\Xsd3b"
  File ..\src\plugins\MyMetaXsd3bPlugin\templates\xsd3b\*.*
  
  SetOutPath $INSTDIR
  
SectionEnd

;--------------------------------------------------------
; Reset MSDTC Log
;--------------------------------------------------------
Section "Reset MSDTC Log (sometimes needed)"

    DetailPrint "Resetting the MSDTC Log"
    ExecWait `"$WINDIR\system32\msdtc.exe" -resetlog`

SectionEnd

;--------------------------------------------------------
; Register MyMeta assembly
;--------------------------------------------------------
Section "Register MyMeta Assembly"

    DetailPrint "Register the MyMeta DLL into the Global Assembly Cache"
    ExecWait `"$WINDIR\Microsoft.NET\Framework\v2.0.50727\regasm.exe" "$INSTDIR\MyMeta.dll" /tlb:MyMeta.tlb`

SectionEnd

;--------------------------------------------------------
; Optional section
;--------------------------------------------------------
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\MyGeneration"
  CreateShortCut "$SMPROGRAMS\MyGeneration\MyGeneration Website.lnk" "https://github.com/khaister/mygeneration" "" "$INSTDIR\MyGeneration.exe" 0
  CreateShortCut "$SMPROGRAMS\MyGeneration\MyGeneration SourceForge Page.lnk" "https://github.com/khaister/mygeneration" "" "$INSTDIR\MyGeneration.exe" 0
  CreateShortCut "$SMPROGRAMS\MyGeneration\MyMeta Reference.lnk" "$INSTDIR\MyMeta.chm"

SectionEnd

;--------------------------------------------------------
; Show splash screen
;--------------------------------------------------------
Function .onInit

    SetOutPath $TEMP
    File /oname=splash_screen.bmp ".\imgs\logo.bmp"

    advsplash::show 1600 100 100 -1 $TEMP\splash_screen

    Pop $0 ; $0 has '1' if the user closed the splash screen early,
           ; '0' if everything closed normal, and '-1' if some error occured.

    Delete $TEMP\splash_screen.bmp

    Return

FunctionEnd
