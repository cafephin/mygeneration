;-----------------------------------------
; MyMeta Installation Script
;-----------------------------------------

; Set the compressions to lzma, which is always the best compression!
SetCompressor lzma 

; The name of the installer
Name "MyMeta"

; The file to write
OutFile "mymeta_installer.exe"

; Icon doesn't work for some reason
Icon ".\modern-install.ico"

XPStyle on

ShowInstDetails show

LicenseText "Liscence Agreement"
LicenseData "BSDLicense.rtf"

; The default installation directory
InstallDir $PROGRAMFILES\MyGenerations

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM SOFTWARE\MyGeneration "Install_Dir"

; The text to prompt the user to enter a directory
ComponentText "This will install the MyMeta Meta Data API."

; The text to prompt belithe user to enter a directory
DirText "Choose an install directory for MyMeta."

; Install .Net Framework 2.0
Section "Detect .Net Framework 2.0"
  Call DotNet20Exists
  Pop $1
  IntCmp $1 0 SkipFramework
    MessageBox MB_OK|MB_ICONINFORMATION "You cannot run MyGeneration without having the .Net Framework 2.0 installed. It is not included $\r$\nin the installer because the file is huge and most people already have it installed." IDOK
    ExecShell open http://www.microsoft.com/downloads/details.aspx?familyid=0856EACB-4362-4B0D-8EDD-AAB15C5E04F5&displaylang=en
    DetailPrint ".Net Framework 2.0 not installed... Aborting Installation."
    Abort
    Goto FrameworkDone
	SkipFramework:
		DetailPrint ".Net Framework 2.0 found... Continuing."
	FrameworkDone:
SectionEnd

; Install MDAC 2.7
Section "Detect MDAC 2.7+"
	Call MDAC27Exists
	Pop $1
	IntCmp $1 0 SkipMDAC
		MessageBox MB_OK|MB_ICONINFORMATION "You cannot run MyGeneration without having MDAC 2.7+ installed. It is not included $\r$\nin the installer because the file is large and most people already have it installed." IDOK
		ExecShell open http://www.microsoft.com/downloads/details.aspx?FamilyID=6c050fe3-c795-4b7d-b037-185d0506396c&DisplayLang=en
		DetailPrint "MDAC 2.7+ not installed... Aborting Installation."
		Abort
		Goto MDACDone
	SkipMDAC:
		DetailPrint "MDAC 2.7+ found... Continuing."
	MDACDone:
SectionEnd

; The stuff to install
Section "Install Files and Reg Entries"

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ;Create Settings Directory 
  ;ExecShell mkdir $INSTDIR\Settings

  File /oname=Interop.ADOX.dll ..\mygeneration\MyGeneration\bin\Release\Interop.ADOX.dll
  File /oname=Interop.MSDASC.dll ..\mygeneration\MyGeneration\bin\Release\Interop.MSDASC.dll

  File /oname=adodb.dll .\adodb.dll
  File /oname=System.Data.SQLite.DLL ..\mymeta\ThirdParty\System.Data.SQLite.DLL
  File /oname=Npgsql.dll ..\mymeta\ThirdParty\Npgsql.dll
  File /oname=Mono.Security.dll ..\mymeta\ThirdParty\Mono.Security.dll
  File /oname=FirebirdSql.Data.Firebird.dll ..\mymeta\ThirdParty\FirebirdSql.Data.Firebird.dll
  File /oname=MyMeta.Plugins.DelimitedText.dll ..\plugins\MyMetaTextFilePlugin\bin\Release\MyMeta.Plugins.DelimitedText.dll
  File /oname=MyMeta.Plugins.VistaDB3x.dll ..\plugins\MyMetaVistaDB3xPlugin\bin\Release\MyMeta.Plugins.VistaDB3x.dll
  File /oname=MyMeta.Plugins.SqlCe.dll ..\plugins\MyMetaSqlCePlugin\bin\Release\MyMeta.Plugins.SqlCe.dll
  File /oname=MyMeta.Plugins.Xsd3b.dll ..\plugins\MyMetaXsd3bPlugin\bin\Release\MyMeta.Plugins.Xsd3b.dll
  File /oname=MyMeta.dll ..\mymeta\bin\Release\MyMeta.dll
  File /oname=MyMeta.tlb ..\mymeta\bin\Release\MyMeta.tlb
  File /oname=MyMeta.chm ..\mymeta\MyMeta.chm

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
  File /oname=Settings\DbTargets.xml ..\mygeneration\MyGeneration\Settings\DbTargets.xml
  File /oname=Settings\Languages.xml ..\mygeneration\MyGeneration\Settings\Languages.xml
 
SectionEnd ; end the section

Section "MSDTC Reset Log (sometimes needed)"

    DetailPrint "Resetting the MSDTC Log"
    ExecWait `"$WINDIR\system32\msdtc.exe" -resetlog`
SectionEnd

; Register  MyMeta DLL
Section "Register MyMeta Assembly"

    DetailPrint "Register the MyMeta DLL into the Global Assembly Cache"
    ;ExecWait `"$WINDIR\Microsoft.Net\Framework\v1.1.4322\regasm.exe" "$INSTDIR\MyMeta.dll" /codebase`
    ExecWait `"$WINDIR\Microsoft.NET\Framework\v2.0.50727\regasm.exe" "$INSTDIR\MyMeta.dll" /tlb:MyMeta.tlb`
SectionEnd

; optional section
Section "Start Menu Shortcuts"
  CreateDirectory "$SMPROGRAMS\MyGeneration"
  CreateShortCut "$SMPROGRAMS\MyGeneration\MyGeneration Website.lnk" "http://www.mygenerationsoftware.com/" "" "$INSTDIR\MyGeneration.exe" 0
  CreateShortCut "$SMPROGRAMS\MyGeneration\MyGeneration SourceForge Page.lnk" "http://sourceforge.net/projects/mygeneration/" "" "$INSTDIR\MyGeneration.exe" 0
  CreateShortCut "$SMPROGRAMS\MyGeneration\MyMeta Reference.lnk" "$INSTDIR\MyMeta.chm"
SectionEnd


; functions defined here:

Function .onInit

    SetOutPath $TEMP
    File /oname=spltmp.bmp "logo.bmp"
    File /oname=spltmp.wav "start.wav"

    advsplash::show 1600 600 600 -1 $TEMP\spltmp

    Pop $0 ; $0 has '1' if the user closed the splash screen early,
           ; '0' if everything closed normal, and '-1' if some error occured.

    Delete $TEMP\spltmp.bmp
    Delete $TEMP\spltmp.wav

    Return
FunctionEnd

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

; detects MDAC 2.7
Function MDAC27Exists

	ClearErrors
	ReadRegStr $1 HKLM "SOFTWARE\Microsoft\DataAccess" "FullInstallVer"
	IfErrors MDACNotFound MDACFound

	MDACFound:
		StrCpy $2 $1 3

		StrCmp $2 "2.7" MDAC27Found
		StrCmp $2 "2.8" MDAC27Found
		Goto MDACNotFound
		
	MDAC27Found:
		Push 0
		Goto ExitFunction

	MDACNotFound:
		Push 1
		Goto ExitFunction
	ExitFunction:

FunctionEnd

; eof