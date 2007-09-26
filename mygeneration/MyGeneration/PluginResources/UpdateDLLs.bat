if "%DEVENV%"=="" set DEVENV="%PROGRAMFILES%\Microsoft Visual Studio 8\Common7\IDE\VCSExpress.exe"  /rebuild release

%DEVENV% "..\..\..\plugins\MyMetaPlugins.sln"
%DEVENV% "..\..\..\plugins\ZeusPlugins.sln"
set DEVENV=

COPY "..\..\..\plugins\ContextProcessor\bin\release\*.dll" .
COPY "..\..\..\plugins\Dnp.Utils\bin\release\*.dll" .
COPY "..\..\..\plugins\DotNetScriptingEngine\bin\release\*.dll" .
COPY "..\..\..\plugins\MicrosoftScriptingEngine\bin\Release\*.dll" .
COPY "..\..\..\plugins\MyMetaSqlCePlugin\bin\release\*.dll" .
COPY "..\..\..\plugins\MyMetaTextFilePlugin\bin\release\*.dll" .
COPY "..\..\..\plugins\MyMetaVistaDB3xPlugin\bin\release\*.dll" .
COPY "..\..\..\plugins\MyMetaXsd3bPlugin\bin\release\*.dll" .
COPY "..\..\..\plugins\TypeSerializer\bin\release\*.dll" .
COPY "..\..\..\plugins\SampleUIPlugin\bin\release\*.dll" .

DEL Dnp.Utils.dll
DEL MyGenUtility.dll
DEL MyMeta.dll
DEL PluginInterfaces.dll
DEL ScintillaNET.dll
DEL WeifenLuo.WinFormsUI.Docking.dll
DEL Zeus.dll