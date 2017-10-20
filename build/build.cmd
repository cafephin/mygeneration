@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

IF EXIST ".\build.log" DEL ".\build.log"

SET config=%1
IF [%config%]==[] (
    ECHO A build configuration ^(debug or release^) must be specified
    EXIT /b 1
)

SET BUILD=msbuild /t:Rebuild /p:Configuration=%config% /flp:logfile=build.log;Append

%BUILD% "..\src\plugins\MyMetaPlugins.sln"
%BUILD% "..\src\plugins\ZeusPlugins.sln"
COPY "..\src\plugins\Dnp.Utils\bin\%config%\Dnp.Utils.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyGeneration.UI.Plugins.CodeSmith2MyGen\bin\%config%\MyGeneration.UI.Plugins.CodeSmith2MyGen.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyGeneration.UI.Plugins.SqlTool\bin\%config%\MyGeneration.UI.Plugins.SqlTool.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaEffiprozPlugin\bin\%config%\MyMeta.Plugins.EffiProz.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaIngres2006Plugin\bin\%config%\MyMeta.Plugins.Ingres2006.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaSqlCePlugin\bin\%config%\MyMeta.Plugins.SqlCe.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaSybaseASAPlugin\bin\%config%\MyMeta.Plugins.SybaseASA.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaSybaseASEPlugin\bin\%config%\MyMeta.Plugins.SybaseASE.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaTextFilePlugin\bin\%config%\MyMeta.Plugins.DelimitedText.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaXsd3bPlugin\bin\%config%\MyMeta.Plugins.Xsd3b.dll" "..\src\lib\plugins"
COPY "..\src\plugins\SampleUIPlugin\bin\%config%\MyGeneration.UI.Plugins.Sample.dll" "..\src\lib\plugins"

%BUILD% "..\src\mygeneration\ZeusCmd.sln"
%BUILD% "..\src\mygeneration\MyGeneration.sln"

SET BUILD=

ECHO ON
