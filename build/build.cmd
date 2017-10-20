if exist ".\build.log" del ".\build.log"

set BUILD=msbuild /t:Rebuild /p:Configuration=Release /flp:logfile=build.log;Append

%BUILD% "..\src\plugins\MyMetaPlugins.sln"
%BUILD% "..\src\plugins\ZeusPlugins.sln"

COPY "..\src\plugins\Dnp.Utils\bin\Release\Dnp.Utils.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyGeneration.UI.Plugins.CodeSmith2MyGen\bin\Release\MyGeneration.UI.Plugins.CodeSmith2MyGen.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyGeneration.UI.Plugins.SqlTool\bin\release\MyGeneration.UI.Plugins.SqlTool.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaEffiprozPlugin\bin\release\MyMeta.Plugins.EffiProz.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaIngres2006Plugin\bin\release\MyMeta.Plugins.Ingres2006.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaSqlCePlugin\bin\release\MyMeta.Plugins.SqlCe.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaSybaseASAPlugin\bin\release\MyMeta.Plugins.SybaseASA.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaSybaseASEPlugin\bin\release\MyMeta.Plugins.SybaseASE.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaTextFilePlugin\bin\release\MyMeta.Plugins.DelimitedText.dll" "..\src\lib\plugins"
COPY "..\src\plugins\MyMetaXsd3bPlugin\bin\release\MyMeta.Plugins.Xsd3b.dll" "..\src\lib\plugins"
COPY "..\src\plugins\SampleUIPlugin\bin\release\MyGeneration.UI.Plugins.Sample.dll" "..\src\lib\plugins"

%BUILD% "..\src\mygeneration\ZeusCmd.sln"
%BUILD% "..\src\mygeneration\MyGeneration.sln"

set BUILD=
