;-----------------------------------------
; MyGeneration Installation Script
; This version gets its dll-/exe-files from the 
;	local build paths below the src directories
;-----------------------------------------

!include ".\common_functions.nsh"
!define DNF4_URL "http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe"

!define OUT_PATH ".\installers"
!system 'mkdir "${OUT_PATH}"'

;--------------------------------------------------------
; Configuration
;--------------------------------------------------------
SetCompressor lzma ; Set the compressions to lzma
Name "MyGeneration 1.3" ; The name of the installer
OutFile "${OUT_PATH}\mygeneration-installer.exe" ; The file to write
Icon ".\icos\modern-install.ico"
XPStyle on
ShowInstDetails show
LicenseText "Liscence Agreement"
LicenseData "..\LICENSE"

; The default installation directory
InstallDir $PROGRAMFILES\MyGeneration13

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM SOFTWARE\MyGeneration13 "Install_Dir"

; The text to prompt the user to enter a directory
ComponentText "This will install MyGeneration Code Generation Tool 1.3 on your computer."

; The text to prompt belithe user to enter a directory
DirText "Choose an install directory for MyGeneration 1.3."

;--------------------------------------------------------
; Download and install the .Net Framework 4
;--------------------------------------------------------
Section /o "Install .NET Framework 4" net4_section_id

	Call DotNet4Exists
	Pop $1
	IntCmp $1 1 SkipDotNet4

	StrCpy $1 "dotNetFx40_Full_setup.exe"
	StrCpy $2 "$EXEDIR\$1"
	IfFileExists $2 FileExistsAlready FileMissing

	FileMissing:
		DetailPrint ".NET Framework 4 not installed... Downloading file."
		StrCpy $2 "$TEMP\$1"
		NSISdl::download "${DNF4_URL}" $2

	FileExistsAlready:
		DetailPrint "Installing the .NET Framework 4."
		ExecWait '"$2" /quiet'

		Call DotNet4Exists
		Pop $1
		IntCmp $1 1 DotNet4Done DotNet4Failed

	DotNet4Failed:
		DetailPrint ".NET Framework 4 install failed; aborting installation"
		MessageBox MB_OK ".NET Framework 4 install failed; aborting installation"
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
		MessageBox MB_OK|MB_ICONINFORMATION "Please install MDAC; MyGeneration requires MDAC 2.7+ for it to work." IDOK
		ExecShell open "https://www.microsoft.com/en-us/download/details.aspx?id=5793"
		DetailPrint "MDAC not installed; aborting installation"
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
; Install Microsoft Script Control
;--------------------------------------------------------
Section "Detect Microsoft Script Control"

	Call ScriptControlExists
	Pop $1
	IntCmp $1 0 SkipMSC
		MessageBox MB_OK|MB_ICONINFORMATION "Please install Microsoft Script Control." IDOK
		Goto MSCDone
	SkipMSC:
		DetailPrint "Microsoft Script Control found; skipping installation"
	MSCDone:

SectionEnd

;--------------------------------------------------------
; Install
;--------------------------------------------------------
Section "Install MyGeneration"

  ; Set output path to the installation directory
  SetOutPath $INSTDIR
  
  ;Create Settings Directory 
  ;ExecShell mkdir $INSTDIR\Settings

  ; Unregister current MyMeta.dll if it exists
  IfFileExists "$INSTDIR\MyMeta.dll" 0 +2
  ExecWait `"$WINDIR\Microsoft.NET\Framework\v2.0.50727\regasm.exe" /u "$INSTDIR\MyMeta.dll" /tlb:MyMeta.tlb`
  
  ; Delete some old assemblies
  IfFileExists "$INSTDIR\Settings\ZeusConfig.xml" 0 +3
    Rename $INSTDIR\Settings\ZeusConfig.xml $INSTDIR\Settings\ZeusConfig.xml.upgrade.backup
    Delete "$INSTDIR\Settings\ZeusConfig.xml"
  
  IfFileExists "$INSTDIR\DotNetScriptingEngine.dll" 0 +2
    Delete "$INSTDIR\DotNetScriptingEngine.dll"

  IfFileExists "$INSTDIR\MicrosoftScriptingEngine.dll" 0 +2
    Delete "$INSTDIR\MicrosoftScriptingEngine.dll"

  IfFileExists "$INSTDIR\ContextProcessor.dll" 0 +2
    Delete "$INSTDIR\ContextProcessor.dll"

  IfFileExists "$INSTDIR\MyWinformUI.dll" 0 +2
    Delete "$INSTDIR\MyWinformUI.dll"

  IfFileExists "$INSTDIR\TypeSerializer.dll" 0 +2
    Delete "$INSTDIR\TypeSerializer.dll"

  IfFileExists "$INSTDIR\Templates\Other\WinformDemo.vbgen" 0 +2
    Delete "$INSTDIR\Templates\Other\WinformDemo.vbgen"
    
  IfFileExists "$INSTDIR\FirebirdSql.Data.Firebird.dll" 0 +2
    Delete "$INSTDIR\FirebirdSql.Data.Firebird.dll"
  
  ; Get the latest DLLs and EXE
  File /oname=ZeusCmd.exe ..\src\mygeneration\ZeusCmd\bin\Release\ZeusCmd.exe
  File /oname=MyGeneration.exe ..\src\mygeneration\MyGeneration\bin\Release\MyGeneration.exe
  File /oname=ZeusCmd.exe.config ..\src\mygeneration\ZeusCmd\bin\Release\ZeusCmd.exe.config
  File /oname=MyGeneration.exe.config ..\src\mygeneration\MyGeneration\bin\Release\MyGeneration.exe.config

  File /oname=Interop.ADOX.dll ..\src\mygeneration\MyGeneration\bin\Release\Interop.ADOX.dll
  File /oname=Interop.MSDASC.dll ..\src\mygeneration\MyGeneration\bin\Release\Interop.MSDASC.dll
  File /oname=Interop.MSScriptControl.dll ..\src\mygeneration\MyGeneration\bin\Release\Interop.MSScriptControl.dll
  File /oname=Interop.Scripting.dll ..\src\mygeneration\MyGeneration\bin\Release\Interop.Scripting.dll

  File /oname=adodb.dll ..\src\lib\thirdparty\adodb.dll
  File /oname=System.Data.SQLite.DLL ..\src\lib\thirdparty\System.Data.SQLite.DLL
  File /oname=Npgsql.dll ..\src\lib\thirdparty\Npgsql.dll
  File /oname=Mono.Security.dll ..\src\lib\thirdparty\Mono.Security.dll
  File /oname=FirebirdSql.Data.FirebirdClient.dll ..\src\lib\thirdparty\FirebirdSql.Data.FirebirdClient.dll
  File /oname=MySql.Data.dll ..\src\lib\thirdparty\MySql.Data.dll
  File /oname=EffiProz.dll ..\src\lib\thirdparty\EffiProz.dll
  File /oname=CollapsibleSplitter.dll ..\src\mygeneration\MyGeneration\bin\Release\CollapsibleSplitter.dll
  File /oname=ScintillaNET.dll ..\src\lib\thirdparty\ScintillaNET.dll
  File /oname=SciLexer.dll ..\src\lib\thirdparty\SciLexer.dll
  File /oname=WeifenLuo.WinFormsUI.Docking.dll ..\src\lib\thirdparty\WeifenLuo.WinFormsUI.Docking.dll

  ; Plugins nonfatal means create installer even if the files do not exist
  File /nonfatal /oname=MyMeta.Plugins.DelimitedText.dll ..\src\plugins\MyMetaTextFilePlugin\bin\Release\MyMeta.Plugins.DelimitedText.dll
  File /nonfatal /oname=MyMeta.Plugins.SqlCe.dll ..\src\plugins\MyMetaSqlCePlugin\bin\Release\MyMeta.Plugins.SqlCe.dll
  File /nonfatal /oname=MyMeta.Plugins.SybaseASE.dll ..\src\plugins\MyMetaSybaseASEPlugin\bin\Release\MyMeta.Plugins.SybaseASE.dll
  File /nonfatal /oname=MyMeta.Plugins.SybaseASA.dll ..\src\plugins\MyMetaSybaseASAPlugin\bin\Release\MyMeta.Plugins.SybaseASA.dll
  File /nonfatal /oname=MyMeta.Plugins.Ingres2006.dll ..\src\plugins\MyMetaIngres2006Plugin\bin\Release\MyMeta.Plugins.Ingres2006.dll
  File /nonfatal /oname=MyMeta.Plugins.EffiProz.dll ..\src\plugins\MyMetaEffiProzPlugin\bin\Release\MyMeta.Plugins.EffiProz.dll
  File /nonfatal /oname=MyGeneration.UI.Plugins.SqlTool.dll ..\src\plugins\MyGeneration.UI.Plugins.SqlTool\bin\Release\MyGeneration.UI.Plugins.SqlTool.dll
  
  Delete $INSTDIR\WeifenLuo.WinFormsUI.dll
  Delete $INSTDIR\VistaDBHelper.dll
 
  File /oname=Zeus.dll ..\src\mygeneration\Zeus\bin\Release\Zeus.dll
  File /oname=PluginInterfaces.dll ..\src\mygeneration\EngineInterface\bin\Release\PluginInterfaces.dll
  File /oname=MyMeta.dll ..\src\mymeta\bin\Release\MyMeta.dll
  File /oname=MyMeta.tlb ..\src\mymeta\bin\Release\MyMeta.tlb
  File /oname=MyGenUtility.dll ..\src\mygeneration\MyGenUtility\bin\Release\MyGenUtility.dll
  
  File /oname=MyMeta.chm ..\src\mymeta\MyMeta.chm
  File /oname=dOOdads.chm ..\src\doodads\dOOdads.chm
  File /oname=Zeus.chm ..\src\mygeneration\Zeus\Zeus.chm
  File /oname=MyGeneration.chm ..\docs\help\MyGeneration.chm
  
  File /oname=changelog.txt .\changelog.txt
  
  File /oname=UnregisterMyMeta12.reg .\registry\UnregisterMyMeta12.reg
  File /oname=UnregisterMyMeta13.reg .\registry\UnregisterMyMeta13.reg
  File /oname=RegisterMyMeta.bat .\registry\RegisterMyMeta.cmd

  File /oname=MyGeneration.ico ..\src\mygeneration\MyGeneration\Icons\MainWindow.ico
  File /oname=ZeusProject.ico ..\src\mygeneration\MyGeneration\Icons\NewZeus.ico
  
  ; Install DnpUtils
  File /oname=Dnp.Utils.chm ..\src\plugins\Dnp.Utils\Dnp.Utils.chm
  File /oname=Dnp.Utils.dll ..\src\plugins\Dnp.Utils\bin\Release\Dnp.Utils.dll
  ExecWait `"$INSTDIR\ZeusCmd.exe" -aio "%ZEUSHOME%\Dnp.Utils.dll" "Dnp.Utils.Utils" "DnpUtils"`
  
  ; Create Folders
  CreateDirectory "$INSTDIR\Templates"
  CreateDirectory "$INSTDIR\Settings"
  CreateDirectory "$INSTDIR\GeneratedCode"
  CreateDirectory "$INSTDIR\Architectures"
  
  ; Create Architecture Sub-Folders
  CreateDirectory "$INSTDIR\Architectures\dOOdads"
  CreateDirectory "$INSTDIR\Architectures\dOOdads\VB.NET"
  CreateDirectory "$INSTDIR\Architectures\dOOdads\VB.NET\dOOdad_Demo"
  CreateDirectory "$INSTDIR\Architectures\dOOdads\VB.NET\dOOdad_Demo\Generated"
  CreateDirectory "$INSTDIR\Architectures\dOOdads\VB.NET\dOOdad_Demo\My Project"
  CreateDirectory "$INSTDIR\Architectures\dOOdads\VB.NET\MyGeneration.dOOdads"
  CreateDirectory "$INSTDIR\Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\"
  CreateDirectory "$INSTDIR\Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\My Project\"  
  CreateDirectory "$INSTDIR\Architectures\dOOdads\CSharp"
  CreateDirectory "$INSTDIR\Architectures\dOOdads\CSharp\dOOdad_Demo"
  CreateDirectory "$INSTDIR\Architectures\dOOdads\CSharp\dOOdad_Demo\Generated"
  CreateDirectory "$INSTDIR\Architectures\dOOdads\CSharp\MyGeneration.dOOdads"
  CreateDirectory "$INSTDIR\Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\"
  
  ; Create Template Sub-Folders
  CreateDirectory "$INSTDIR\Templates\Microsoft_Access"
  CreateDirectory "$INSTDIR\Templates\Microsoft_SQL_Server"
  CreateDirectory "$INSTDIR\Templates\VB.Net"
  CreateDirectory "$INSTDIR\Templates\C#"
  CreateDirectory "$INSTDIR\Templates\PHP"
  CreateDirectory "$INSTDIR\Templates\Other"
  CreateDirectory "$INSTDIR\Templates\IBM_DB2"
  CreateDirectory "$INSTDIR\Templates\MySQL"
  CreateDirectory "$INSTDIR\Templates\Oracle"
  CreateDirectory "$INSTDIR\Templates\Java"
  CreateDirectory "$INSTDIR\Templates\HTML"
  CreateDirectory "$INSTDIR\Templates\Firebird"
  CreateDirectory "$INSTDIR\Templates\GentleNET"
  CreateDirectory "$INSTDIR\Templates\VistaDB"
  CreateDirectory "$INSTDIR\Templates\Tutorials"
  CreateDirectory "$INSTDIR\Templates\Samples"
  CreateDirectory "$INSTDIR\Templates\PostgreSQL"
  CreateDirectory "$INSTDIR\Templates\MySQL"
  CreateDirectory "$INSTDIR\Templates\SQLite"
  CreateDirectory "$INSTDIR\Templates\IBM_ISeries"

  ; dOOdads
  ; VB.NET Demo
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\README.TXT ..\src\doodads\VB.NET\dOOdad_Demo\README.TXT
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\About.resx ..\src\doodads\VB.NET\dOOdad_Demo\About.resx
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\About.vb ..\src\doodads\VB.NET\dOOdad_Demo\About.vb
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\App.config ..\src\doodads\VB.NET\dOOdad_Demo\App.config
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\AssemblyInfo.vb ..\src\doodads\VB.NET\dOOdad_Demo\AssemblyInfo.vb
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\dOOdad_Demo.sln ..\src\doodads\VB.NET\dOOdad_Demo\dOOdad_Demo.sln
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\dOOdad_Demo_2005.sln ..\src\doodads\VB.NET\dOOdad_Demo\dOOdad_Demo_2005.sln  
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\dOOdad_Demo.vbproj ..\src\doodads\VB.NET\dOOdad_Demo\dOOdad_Demo.vbproj
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\dOOdad_Demo_2005.vbproj ..\src\doodads\VB.NET\dOOdad_Demo\dOOdad_Demo_2005.vbproj  
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\Employees.vb ..\src\doodads\VB.NET\dOOdad_Demo\Employees.vb
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\EmployeesEdit.resx ..\src\doodads\VB.NET\dOOdad_Demo\EmployeesEdit.resx
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\EmployeesEdit.vb ..\src\doodads\VB.NET\dOOdad_Demo\EmployeesEdit.vb
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\FillComboBox.resx ..\src\doodads\VB.NET\dOOdad_Demo\FillComboBox.resx
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\FillComboBox.vb ..\src\doodads\VB.NET\dOOdad_Demo\FillComboBox.vb
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\Form1.resx ..\src\doodads\VB.NET\dOOdad_Demo\Form1.resx
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\Form1.vb ..\src\doodads\VB.NET\dOOdad_Demo\Form1.vb
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\Invoices.vb ..\src\doodads\VB.NET\dOOdad_Demo\Invoices.vb
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\Products.vb ..\src\doodads\VB.NET\dOOdad_Demo\Products.vb
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\TheMasterSample.vb ..\src\doodads\VB.NET\dOOdad_Demo\TheMasterSample.vb
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\Generated\_Employees.vb ..\src\doodads\VB.NET\dOOdad_Demo\Generated\_Employees.vb
  File /oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\Generated\_Products.vb ..\src\doodads\VB.NET\dOOdad_Demo\Generated\_Products.vb
  File "/oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\My Project\Application.Designer.vb" "..\src\doodads\VB.NET\dOOdad_Demo\My Project\Application.Designer.vb"
  File "/oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\My Project\Application.myapp" "..\src\doodads\VB.NET\dOOdad_Demo\My Project\Application.myapp"
  File "/oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\My Project\Application1.Designer.vb" "..\src\doodads\VB.NET\dOOdad_Demo\My Project\Application1.Designer.vb"
  File "/oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\My Project\Resources.Designer.vb" "..\src\doodads\VB.NET\dOOdad_Demo\My Project\Resources.Designer.vb"
  File "/oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\My Project\Resources.resx" "..\src\doodads\VB.NET\dOOdad_Demo\My Project\Resources.resx"
  File "/oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\My Project\Settings.Designer.vb" "..\src\doodads\VB.NET\dOOdad_Demo\My Project\Settings.Designer.vb"
  File "/oname=Architectures\dOOdads\VB.NET\dOOdad_Demo\My Project\Settings.settings" "..\src\doodads\VB.NET\dOOdad_Demo\My Project\Settings.settings"

  ; VB.NET dOOdads
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\README.TXT ..\src\doodads\VB.NET\MyGeneration.dOOdads\README.TXT
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\AssemblyInfo.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\AssemblyInfo.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\BusinessEntity.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\BusinessEntity.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DynamicQuery.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DynamicQuery.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\MyGeneration.dOOdads.vbproj ..\src\doodads\VB.NET\MyGeneration.dOOdads\MyGeneration.dOOdads.vbproj
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\MyGeneration.dOOdads_2005.vbproj ..\src\doodads\VB.NET\MyGeneration.dOOdads\MyGeneration.dOOdads_2005.vbproj  
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\MyGeneration.dOOdads.prjx ..\src\doodads\VB.NET\MyGeneration.dOOdads\MyGeneration.dOOdads.prjx  
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\TransactionMgr.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\TransactionMgr.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\WhereParameter.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\WhereParameter.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\AggregateParameter.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\AggregateParameter.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\MyGeneration.dOOdads.sln ..\src\doodads\VB.NET\MyGeneration.dOOdads\MyGeneration.dOOdads.sln
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\MyGeneration.dOOdads_2005.sln ..\src\doodads\VB.NET\MyGeneration.dOOdads\MyGeneration.dOOdads_2005.sln  
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\MyGeneration.dOOdads.cmbx ..\src\doodads\VB.NET\MyGeneration.dOOdads\MyGeneration.dOOdads.cmbx  
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\SqlClientEntity.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\SqlClientEntity.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\SqlClientDynamicQuery.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\SqlClientDynamicQuery.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\OleDbEntity.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\OleDbEntity.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\OleDbDynamicQuery.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\OleDbDynamicQuery.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\OracleClientEntity.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\OracleClientEntity.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\OracleClientDynamicQuery.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\OracleClientDynamicQuery.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\FirebirdSqlEntity.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\FirebirdSqlEntity.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\FirebirdSqlDynamicQuery.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\FirebirdSqlDynamicQuery.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\VistaDBDynamicQuery.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\VistaDBDynamicQuery.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\VistaDBEntity.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\VistaDBEntity.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\PostgreSqlDynamicQuery.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\PostgreSqlDynamicQuery.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\PostgreSqlEntity.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\PostgreSqlEntity.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\MySql4DynamicQuery.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\MySql4DynamicQuery.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\MySql4Entity.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\MySql4Entity.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\SQLiteDynamicQuery.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\SQLiteDynamicQuery.vb
  File /oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\DbAdapters\SQLiteEntity.vb ..\src\doodads\VB.NET\MyGeneration.dOOdads\DbAdapters\SQLiteEntity.vb
  File "/oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\My Project\Application.Designer.vb" "..\src\doodads\VB.NET\MyGeneration.dOOdads\My Project\Application.Designer.vb"
  File "/oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\My Project\Application.myapp" "..\src\doodads\VB.NET\MyGeneration.dOOdads\My Project\Application.myapp"
  File "/oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\My Project\Application1.Designer.vb" "..\src\doodads\VB.NET\MyGeneration.dOOdads\My Project\Application1.Designer.vb"
  File "/oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\My Project\Resources.Designer.vb" "..\src\doodads\VB.NET\MyGeneration.dOOdads\My Project\Resources.Designer.vb"
  File "/oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\My Project\Resources.resx" "..\src\doodads\VB.NET\MyGeneration.dOOdads\My Project\Resources.resx"
  File "/oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\My Project\Settings.Designer.vb" "..\src\doodads\VB.NET\MyGeneration.dOOdads\My Project\Settings.Designer.vb"
  File "/oname=Architectures\dOOdads\VB.NET\MyGeneration.dOOdads\My Project\Settings.settings" "..\src\doodads\VB.NET\MyGeneration.dOOdads\My Project\Settings.settings"
									
  ; CSharp Demo
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\README.TXT ..\src\doodads\CSharp\dOOdad_Demo\README.TXT
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\App.ico ..\src\doodads\CSharp\dOOdad_Demo\App.ico
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\About.resx ..\src\doodads\CSharp\dOOdad_Demo\About.resx
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\About.cs ..\src\doodads\CSharp\dOOdad_Demo\About.cs
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\App.config ..\src\doodads\CSharp\dOOdad_Demo\App.config
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\AssemblyInfo.cs ..\src\doodads\CSharp\dOOdad_Demo\AssemblyInfo.cs
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\dOOdad_Demo.sln ..\src\doodads\CSharp\dOOdad_Demo\dOOdad_Demo.sln
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\dOOdad_Demo_2005.sln ..\src\doodads\CSharp\dOOdad_Demo\dOOdad_Demo_2005.sln  
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\dOOdad_Demo.csproj ..\src\doodads\CSharp\dOOdad_Demo\dOOdad_Demo.csproj
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\dOOdad_Demo_2005.csproj ..\src\doodads\CSharp\dOOdad_Demo\dOOdad_Demo_2005.csproj  
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\Employees.cs ..\src\doodads\CSharp\dOOdad_Demo\Employees.cs
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\EmployeesEdit.resx ..\src\doodads\CSharp\dOOdad_Demo\EmployeesEdit.resx
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\EmployeesEdit.cs ..\src\doodads\CSharp\dOOdad_Demo\EmployeesEdit.cs
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\FillComboBox.resx ..\src\doodads\CSharp\dOOdad_Demo\FillComboBox.resx
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\FillComboBox.cs ..\src\doodads\CSharp\dOOdad_Demo\FillComboBox.cs
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\Form1.resx ..\src\doodads\CSharp\dOOdad_Demo\Form1.resx
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\Form1.cs ..\src\doodads\CSharp\dOOdad_Demo\Form1.cs
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\Invoices.cs ..\src\doodads\CSharp\dOOdad_Demo\Invoices.cs
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\Products.cs ..\src\doodads\CSharp\dOOdad_Demo\Products.cs
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\TheMasterSample.cs ..\src\doodads\CSharp\dOOdad_Demo\TheMasterSample.cs
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\Generated\_Employees.cs ..\src\doodads\CSharp\dOOdad_Demo\Generated\_Employees.cs
  File /oname=Architectures\dOOdads\CSharp\dOOdad_Demo\Generated\_Products.cs ..\src\doodads\CSharp\dOOdad_Demo\Generated\_Products.cs

  ; CSharp dOOdads
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\README.TXT ..\src\doodads\CSharp\MyGeneration.dOOdads\README.TXT
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\AssemblyInfo.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\AssemblyInfo.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\BusinessEntity.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\BusinessEntity.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DynamicQuery.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DynamicQuery.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\MyGeneration.dOOdads.csproj ..\src\doodads\CSharp\MyGeneration.dOOdads\MyGeneration.dOOdads.csproj
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\MyGeneration.dOOdads_2005.csproj ..\src\doodads\CSharp\MyGeneration.dOOdads\MyGeneration.dOOdads_2005.csproj  
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\MyGeneration.dOOdads.prjx ..\src\doodads\CSharp\MyGeneration.dOOdads\MyGeneration.dOOdads.prjx   
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\TransactionMgr.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\TransactionMgr.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\WhereParameter.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\WhereParameter.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\AggregateParameter.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\AggregateParameter.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\MyGeneration.dOOdads.sln ..\src\doodads\CSharp\MyGeneration.dOOdads\MyGeneration.dOOdads.sln
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\MyGeneration.dOOdads_2005.sln ..\src\doodads\CSharp\MyGeneration.dOOdads\MyGeneration.dOOdads_2005.sln  
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\MyGeneration.dOOdads.cmbx ..\src\doodads\CSharp\MyGeneration.dOOdads\MyGeneration.dOOdads.cmbx   
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\SqlClientEntity.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\SqlClientEntity.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\SqlClientDynamicQuery.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\SqlClientDynamicQuery.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\OleDbEntity.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\OleDbEntity.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\OleDbDynamicQuery.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\OleDbDynamicQuery.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\OracleClientEntity.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\OracleClientEntity.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\OracleClientDynamicQuery.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\OracleClientDynamicQuery.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\FirebirdSqlEntity.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\FirebirdSqlEntity.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\FirebirdSqlDynamicQuery.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\FirebirdSqlDynamicQuery.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\VistaDBDynamicQuery.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\VistaDBDynamicQuery.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\VistaDBEntity.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\VistaDBEntity.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\PostgreSqlDynamicQuery.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\PostgreSqlDynamicQuery.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\PostgreSqlEntity.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\PostgreSqlEntity.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\MySql4DynamicQuery.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\MySql4DynamicQuery.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\MySql4Entity.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\MySql4Entity.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\SQLiteDynamicQuery.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\SQLiteDynamicQuery.cs
  File /oname=Architectures\dOOdads\CSharp\MyGeneration.dOOdads\DbAdapters\SQLiteEntity.cs ..\src\doodads\CSharp\MyGeneration.dOOdads\DbAdapters\SQLiteEntity.cs

  ; Copy the template files into the Templates folder
  File /oname=Templates\Microsoft_Access\Access_StoredProcs.vbgen ..\src\templates\Microsoft_Access\Access_StoredProcs.vbgen
  File /oname=Templates\Microsoft_SQL_Server\SQL_DataInserts.jgen ..\src\templates\Microsoft_SQL_Server\SQL_DataInserts.jgen
  File /oname=Templates\Microsoft_SQL_Server\SQL_DataReplication.jgen ..\src\templates\Microsoft_SQL_Server\SQL_DataReplication.jgen
  File /oname=Templates\Microsoft_SQL_Server\SQL_StoredProcs.jgen ..\src\templates\Microsoft_SQL_Server\SQL_StoredProcs.jgen
  File /oname=Templates\Microsoft_SQL_Server\SQL_StoredProcs.vbgen ..\src\templates\Microsoft_SQL_Server\SQL_StoredProcs.vbgen
  File /oname=Templates\Microsoft_SQL_Server\SQL_DeleteAllData.jgen ..\src\templates\Microsoft_SQL_Server\SQL_DeleteAllData.jgen
  File /oname=Templates\Microsoft_SQL_Server\sql_library.js ..\src\templates\Microsoft_SQL_Server\sql_library.js
  File /oname=Templates\IBM_ISeries\iSeries_StoredProcs.vbgen ..\src\templates\IBM_ISeries\iSeries_StoredProcs.vbgen
  File /oname=Templates\Oracle\Oracle_StoredProcs.vbgen ..\src\templates\Oracle\Oracle_StoredProcs.vbgen
  File /oname=Templates\VB.Net\VbNet_SQL_BusinessObject.vbgen ..\src\templates\VB.Net\VbNet_SQL_BusinessObject.vbgen
  File /oname=Templates\VB.Net\VbNet_Access_BusinessObject.vbgen ..\src\templates\VB.Net\VbNet_Access_BusinessObject.vbgen
  File /oname=Templates\VB.Net\VbNet_SQL_dOOdads_BusinessEntity.vbgen ..\src\templates\VB.Net\VbNet_SQL_dOOdads_BusinessEntity.vbgen
  File /oname=Templates\VB.Net\VbNet_SQL_dOOdads_View.vbgen ..\src\templates\VB.Net\VbNet_SQL_dOOdads_View.vbgen
  File /oname=Templates\VB.Net\VBNet_SQL_dOOdads_ConcreteClass.vbgen ..\src\templates\VB.Net\VBNet_SQL_dOOdads_ConcreteClass.vbgen
  File "/oname=Templates\C#\CSharp_SQL_BusinessObject.vbgen" "..\src\templates\C#\CSharp_SQL_BusinessObject.vbgen"
  File "/oname=Templates\C#\CSharp_Access_BusinessObject.vbgen" "..\src\templates\C#\CSharp_Access_BusinessObject.vbgen"
  File "/oname=Templates\C#\CSharp_SQL_dOOdads_BusinessEntity.vbgen" "..\src\templates\C#\CSharp_SQL_dOOdads_BusinessEntity.vbgen"
  File "/oname=Templates\C#\CSharp_SQL_dOOdads_View.vbgen" "..\src\templates\C#\CSharp_SQL_dOOdads_View.vbgen"
  File "/oname=Templates\C#\CSharp_dOOdads_StoredProc.vbgen" "..\src\templates\C#\CSharp_dOOdads_StoredProc.vbgen"
  File "/oname=Templates\C#\CSharp_SQL_dOOdads_ConcreteClass.vbgen" "..\src\templates\C#\CSharp_SQL_dOOdads_ConcreteClass.vbgen"
  File /oname=Templates\PHP\PHP_BusinessObject.jgen ..\src\templates\PHP\PHP_BusinessObject.jgen
  File /oname=Templates\Other\TemplateGroupExample.jgen ..\src\templates\Other\TemplateGroupExample.jgen
  File /oname=Templates\Other\UserMetaData.vbgen ..\src\templates\Other\UserMetaData.vbgen
  File /oname=Templates\Other\UserMetaData.jgen ..\src\templates\Other\UserMetaData.jgen
  File /oname=Templates\HTML\HTML_DatabaseReport.csgen ..\src\templates\HTML\HTML_DatabaseReport.csgen
  File /oname=Templates\HTML\HTML_TableDefinition.vbgen ..\src\templates\HTML\HTML_TableDefinition.vbgen
  File /oname=Templates\Firebird\FirebirdStoredProcs.vbgen ..\src\templates\Firebird\FirebirdStoredProcs.vbgen
  File /oname=Templates\GentleNET\BusinessEntity.csgen ..\src\templates\Gentle.NET\BusinessEntity.csgen
  File /oname=Templates\VistaDB\VistaDB_CSharp_BusinessEntity.vbgen ..\src\templates\VistaDB\VistaDB_CSharp_BusinessEntity.vbgen
  File /oname=Templates\VistaDB\VistaDB_VBNet_BusinessEntity.vbgen ..\src\templates\VistaDB\VistaDB_VBNet_BusinessEntity.vbgen
  File /oname=Templates\MySQL\MySQL4_CSharp_BusinessEntity.vbgen ..\src\templates\MySql\MySQL4_CSharp_BusinessEntity.vbgen
  File /oname=Templates\MySQL\MySQL4_VBNet_BusinessEntity.vbgen ..\src\templates\MySql\MySQL4_VBNet_BusinessEntity.vbgen
  File /oname=Templates\MySQL\MySQL4_CSharp_BusinessView.vbgen ..\src\templates\MySql\MySQL4_CSharp_BusinessView.vbgen
  File /oname=Templates\MySQL\MySQL4_VBNet_BusinessView.vbgen ..\src\templates\MySql\MySQL4_VBNet_BusinessView.vbgen  
  File /oname=Templates\SQLite\SQLite_CSharp_BusinessEntity.vbgen ..\src\templates\SQLite\SQLite_CSharp_BusinessEntity.vbgen
  File /oname=Templates\SQLite\SQLite_VBNet_BusinessEntity.vbgen ..\src\templates\SQLite\SQLite_VBNet_BusinessEntity.vbgen
  File /oname=Templates\SQLite\SQLite_CSharp_BusinessView.vbgen ..\src\templates\SQLite\SQLite_CSharp_BusinessView.vbgen
  File /oname=Templates\SQLite\SQLite_VBNet_BusinessView.vbgen ..\src\templates\SQLite\SQLite_VBNet_BusinessView.vbgen

  File /oname=Templates\Tutorials\Chapter1(VBScript).zeus ..\src\templates\Tutorials\Chapter1(VBScript).zeus
  File /oname=Templates\Tutorials\Chapter1(JScript).zeus ..\src\templates\Tutorials\Chapter1(JScript).zeus
  File /oname=Templates\Tutorials\Chapter1(VB.NET).zeus ..\src\templates\Tutorials\Chapter1(VB.NET).zeus
  File "/oname=Templates\Tutorials\Chapter1(C#).zeus" "..\src\templates\Tutorials\Chapter1(C#).zeus"
  File /oname=Templates\Tutorials\Chapter2(VBScript).zeus ..\src\templates\Tutorials\Chapter2(VBScript).zeus
  File /oname=Templates\Tutorials\Chapter2(JScript).zeus ..\src\templates\Tutorials\Chapter2(JScript).zeus
  File /oname=Templates\Tutorials\Chapter2(VB.NET).zeus ..\src\templates\Tutorials\Chapter2(VB.NET).zeus
  File "/oname=Templates\Tutorials\Chapter2(C#).zeus" "..\src\templates\Tutorials\Chapter2(C#).zeus"

  File /oname=Templates\PostgreSQL\PostgreSQL_StoredProcs.vbgen ..\src\templates\PostgreSQL\PostgreSQL_StoredProcs.vbgen
  File "/oname=Templates\Samples\GuiTest.zeus" "..\src\templates\Samples\GuiTest.zeus"

  Delete Templates\Firebird\FirebirdStoredProcs_Dialect1.vbgen
  Delete Templates\Firebird\FirebirdStoredProcs_Dialect3.vbgen

  ; Rename files if they already exist
  Delete Settings\Languages.xml.4.old
  Rename Settings\Languages.xml.3.old Settings\Languages.xml.4.old
  Rename Settings\Languages.xml.2.old Settings\Languages.xml.3.old
  Rename Settings\Languages.xml.1.old Settings\Languages.xml.2.old
  Rename Settings\Languages.xml Settings\Languages.xml.1.old

  Delete Settings\DbTargets.xml.4.old
  Rename Settings\DbTargets.xml.3.old Settings\DbTargets.xml.xml.4.old
  Rename Settings\DbTargets.xml.2.old Settings\DbTargets.xml.3.old
  Rename Settings\DbTargets.xml.1.old Settings\DbTargets.xml.2.old
  Rename Settings\DbTargets.xml Settings\DbTargets.xml.1.old

  ; Copy config files into Settings folder
  File /oname=Settings\DbTargets.xml ..\src\mygeneration\MyGeneration\Settings\DbTargets.xml
  File /oname=Settings\Languages.xml ..\src\mygeneration\MyGeneration\Settings\Languages.xml
  File /oname=Settings\MyGeneration.xml ..\src\mygeneration\MyGeneration\Settings\MyGeneration.xml
  File /oname=Settings\ScintillaNET.xml ..\src\mygeneration\MyGeneration\Settings\ScintillaNET.xml
 
  ; Delete old config files
  Delete Settings\ZeusScriptingEngines.zcfg
  Delete Settings\ZeusScriptingObjects.zcfg

  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\MyGeneration13 "Install_Dir" "$INSTDIR"

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyGeneration13" "DisplayName" "MyGeneration 1.3 (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyGeneration13" "UninstallString" '"$INSTDIR\uninstall.exe"'

  WriteUninstaller "uninstall.exe"

SectionEnd

;--------------------------------------------------------
; Register Shell Extensions
;--------------------------------------------------------
Section "Register Shell Extensions"

  ; MyGeneration Development Shell Extensions - JGEN
  WriteRegStr HKCR ".jgen" "" "JGenMyGenFile"
  WriteRegStr HKCR "JGenMyGenFile" "" "JGen Template File"
  WriteRegStr HKCR "JGenMyGenFile\shell" "" "open"
  WriteRegStr HKCR "JGenMyGenFile\DefaultIcon" "" $INSTDIR\MyGeneration.ico
  WriteRegStr HKCR "JGenMyGenFile\shell\open\command" "" '"$INSTDIR\MyGeneration.exe" "%1"'

  ; MyGeneration Development Shell Extensions - VBGEN
  WriteRegStr HKCR ".vbgen" "" "VBGenMyGenFile"
  WriteRegStr HKCR "VBGenMyGenFile" "" "VBGen Template File"
  WriteRegStr HKCR "VBGenMyGenFile\shell" "" "open"
  WriteRegStr HKCR "VBGenMyGenFile\DefaultIcon" "" $INSTDIR\MyGeneration.ico
  WriteRegStr HKCR "VBGenMyGenFile\shell\open\command" "" '"$INSTDIR\MyGeneration.exe" "%1"'

  ; MyGeneration Development Shell Extensions - CSGEN
  WriteRegStr HKCR ".csgen" "" "CSGenMyGenFile"
  WriteRegStr HKCR "CSGenMyGenFile" "" "CSGen Template File"
  WriteRegStr HKCR "CSGenMyGenFile\shell" "" "open"
  WriteRegStr HKCR "CSGenMyGenFile\DefaultIcon" "" $INSTDIR\MyGeneration.ico
  WriteRegStr HKCR "CSGenMyGenFile\shell\open\command" "" '"$INSTDIR\MyGeneration.exe" "%1"'

  ; MyGeneration Development Shell Extensions - ZEUS
  WriteRegStr HKCR ".zeus" "" "ZeusMyGenFile"
  WriteRegStr HKCR "ZeusMyGenFile" "" "Zeus Template File"
  WriteRegStr HKCR "ZeusMyGenFile\shell" "" "open"
  WriteRegStr HKCR "ZeusMyGenFile\DefaultIcon" "" $INSTDIR\MyGeneration.ico
  WriteRegStr HKCR "ZeusMyGenFile\shell\open\command" "" '"$INSTDIR\MyGeneration.exe" "%1"'

  ; MyGeneration Development Shell Extensions - ZINP
  WriteRegStr HKCR ".zinp" "" "ZeusInputMyGenFile"
  WriteRegStr HKCR "ZeusInputMyGenFile" "" "MyGeneration Input File"
  WriteRegStr HKCR "ZeusInputMyGenFile\DefaultIcon" "" $INSTDIR\MyGeneration.ico

  ; MyGeneration Development Shell Extensions - ZPRJ
  WriteRegStr HKCR ".zprj" "" "ProjectMyGenFile"
  WriteRegStr HKCR "ProjectMyGenFile" "" "MyGeneration Project File"
  WriteRegStr HKCR "ProjectMyGenFile\shell" "" "open"
  WriteRegStr HKCR "ProjectMyGenFile\DefaultIcon" "" $INSTDIR\ZeusProject.ico
  WriteRegStr HKCR "ProjectMyGenFile\shell\open\command" "" '"$INSTDIR\MyGeneration.exe" "%1"'

  ; MyGeneration Development Shell Extensions - ZPRJ
  WriteRegStr HKCR ".zprjusr" "" "ProjectMyGenFile"
  WriteRegStr HKCR "ProjectMyGenFile" "" "MyGeneration Project (User) File"
  ;WriteRegStr HKCR "ProjectMyGenFile\shell" "" "open"
  WriteRegStr HKCR "ProjectMyGenFile\DefaultIcon" "" $INSTDIR\ZeusProject.ico
  ;WriteRegStr HKCR "ProjectMyGenFile\shell\open\command" "" '"$INSTDIR\MyGeneration.exe" "%1"'

SectionEnd

; *** We will just have to add this in later, there are too many bugs ***
;Section /o "Visual Studio 2005 Add-In"
  ; Set output path to the installation directory.
  ;SetOutPath $INSTDIR
    
  ;File /nonfatal ..\ideplugins\visualstudio2005\MyGenVS2005\MyGenVS2005.AddIn
  ;File /nonfatal ..\ideplugins\visualstudio2005\MyGenVS2005\bin\MyGenVS2005.dll

  ;ExecWait `"$INSTDIR\ZeusCmd.exe" -installvs2005`
  
;SectionEnd

;--------------------------------------------------------
; Install Xsd3b Provider
;--------------------------------------------------------
Section "Install Xsd3b Provider"
  
  ; Set output path to the installation directory
  SetOutPath $INSTDIR
    
  File /nonfatal /oname=MyMeta.Plugins.Xsd3b.dll ..\src\plugins\MyMetaXsd3bPlugin\bin\Release\MyMeta.Plugins.Xsd3b.dll
  File /nonfatal ..\src\lib\thirdparty\Dl3bak.*.dll
  File /nonfatal ..\docs\help\*xsd3b*.chm

  SetOutPath "$INSTDIR\Templates\Xsd3b"
  
  File ..\src\plugins\MyMetaXsd3bPlugin\templates\xsd3b\*.*
  
  SetOutPath $INSTDIR

  WriteUninstaller "uninstall.exe"

SectionEnd

;--------------------------------------------------------
; Reset MSDTC Log
;--------------------------------------------------------
Section "Reset MSDTC Log"

    DetailPrint "Resetting the MSDTC Log"
    ExecWait `"$WINDIR\system32\msdtc.exe" -resetlog`

SectionEnd

;--------------------------------------------------------
; Register MyMeta Assembly
;--------------------------------------------------------
Section "Register MyMeta Assembly"

    DetailPrint "Register the MyMeta DLL into the Global Assembly Cache"
    ExecWait `"$WINDIR\Microsoft.NET\Framework\v2.0.50727\regasm.exe" "$INSTDIR\MyMeta.dll" /tlb:MyMeta.tlb`

SectionEnd

;--------------------------------------------------------
; Optional section
;--------------------------------------------------------
Section /o "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\MyGeneration 1.3"
  CreateShortCut "$SMPROGRAMS\MyGeneration 1.3\MyGeneration.lnk" "$INSTDIR\MyGeneration.exe" "" "$INSTDIR\MyGeneration.exe" 0
  CreateShortCut "$SMPROGRAMS\MyGeneration 1.3\MyGeneration Website.lnk" "https://github.com/khaister/mygeneration" "" "$INSTDIR\MyGeneration.exe" 0
  CreateShortCut "$SMPROGRAMS\MyGeneration 1.3\MyGeneration Source.lnk" "https://github.com/khaister/mygeneration" "" "$INSTDIR\MyGeneration.exe" 0
  CreateShortCut "$SMPROGRAMS\MyGeneration 1.3\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\MyGeneration 1.3\MyGeneration Help.lnk" "$INSTDIR\MyGeneration.chm"
  CreateShortCut "$SMPROGRAMS\MyGeneration 1.3\MyMeta Reference.lnk" "$INSTDIR\MyMeta.chm"
  CreateShortCut "$SMPROGRAMS\MyGeneration 1.3\dOOdads Reference.lnk" "$INSTDIR\dOOdads.chm"
  CreateShortCut "$SMPROGRAMS\MyGeneration 1.3\Zeus Reference.lnk" "$INSTDIR\Zeus.chm"
  CreateShortCut "$SMPROGRAMS\MyGeneration 1.3\DNP Utils Reference.lnk" "$INSTDIR\Dnp.Utils.chm"

SectionEnd

;--------------------------------------------------------
; Launch MyGeneration After Install
;--------------------------------------------------------
Section

  MessageBox MB_YESNO|MB_ICONQUESTION "Do you want to launch MyGeneration after installation?" IDNO DoNotLaunch
  	ExecShell open "$INSTDIR\MyGeneration.exe" SW_SHOWNORMAL
  	Quit
  DoNotLaunch:

SectionEnd

;--------------------------------------------------------
; Uninstall
;--------------------------------------------------------
UninstallText "This will uninstall the MyGeneration Code Generator."
UninstallIcon ".\icos\modern-uninstall.ico"

Section "Uninstall"
    
  IfFileExists "$INSTDIR\MyMeta.dll" 0 +2
	ExecWait `"$WINDIR\Microsoft.NET\Framework\v2.0.50727\regasm.exe" /u "$INSTDIR\MyMeta.dll" /tlb:MyMeta.tlb`
  
  ; remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyGeneration13"
  DeleteRegKey HKLM SOFTWARE\MyGeneration13
  DeleteRegKey HKCR ".vbgen"
  DeleteRegKey HKCR ".jgen"
  DeleteRegKey HKCR ".zeus"
  DeleteRegKey HKCR ".csgen"
  DeleteRegKey HKCR "JGenMyGenFile"
  DeleteRegKey HKCR "CSGenMyGenFile"
  DeleteRegKey HKCR "VBGenMyGenFile"
  DeleteRegKey HKCR "ZeusMyGenFile"

  ; must remove uninstaller too
  Delete $INSTDIR\uninstall.exe
 
  ; remove shortcuts, if any
  Delete "$SMPROGRAMS\MyGeneration 1.3\*.*"
  
  ; remove directories used
  RMDir "$SMPROGRAMS\MyGeneration 1.3"
  
  ;RMDir /r "$INSTDIR"
  Delete $INSTDIR\*.exe
  Delete $INSTDIR\*.dll
  
  ; get rid of new config files, but back them up first.
  Rename $INSTDIR\Settings\Languages.xml $INSTDIR\Settings\Languages.xml.downgrade.backup
  Delete $INSTDIR\Settings\Languages.xml
  
  Rename $INSTDIR\Settings\DbTargets.xml $INSTDIR\Settings\DbTargets.xml.downgrade.backup
  Delete $INSTDIR\Settings\DbTargets.xml
  
  Rename $INSTDIR\Settings\MyGeneration.xml $INSTDIR\Settings\MyGeneration.xml.downgrade.backup
  Delete $INSTDIR\Settings\MyGeneration.xml
  
  Rename $INSTDIR\Settings\ZeusConfig.xml $INSTDIR\Settings\ZeusConfig.xml.downgrade.backup
  Delete $INSTDIR\Settings\ZeusConfig.xml
  
  Rename $INSTDIR\Settings\DefaultSettings.xml $INSTDIR\Settings\DefaultSettings.xml.downgrade.backup
  Delete $INSTDIR\Settings\DefaultSettings.xml
  
  Rename $INSTDIR\Settings\ScintillaNET.xml $INSTDIR\Settings\ScintillaNET.xml.downgrade.backup
  Delete $INSTDIR\Settings\ScintillaNET.xml
  
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
