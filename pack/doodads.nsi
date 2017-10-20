;-----------------------------------------
; dOOdads Installation Script
;-----------------------------------------

!define OUT_PATH ".\installers"
!system 'mkdir "${OUT_PATH}"'

;--------------------------------------------------------
; Configuration
;--------------------------------------------------------
SetCompressor lzma ; Set the compressions to lzma, which is always the best compression!
Name "dOOdads" ; The name of the installer
OutFile "${OUT_PATH}\dOOdads-installer.exe" ; The file to write
Icon ".\icos\modern-install.ico"
XPStyle on
ShowInstDetails show
LicenseText "Liscence Agreement"
LicenseData "..\LICENSE"

; The default installation directory
InstallDir $PROGRAMFILES\MyGeneration

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM SOFTWARE\MyGeneration "Install_Dir"

; The text to prompt the user to enter a directory
ComponentText "This will install MyGeneration's dOOdads .NET Architecture on your computer."

; The text to prompt the user to enter a directory
DirText "Select the directory that MyGeneration was installed to."

;--------------------------------------------------------
; Install
;--------------------------------------------------------
Section "Install dOOdads"

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  File /oname=dOOdads.chm ..\src\doodads\dOOdads.chm

  CreateDirectory "$INSTDIR\Architectures"
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

SectionEnd
