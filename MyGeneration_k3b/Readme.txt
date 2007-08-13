I created a new Solution MyGeneration_k3b\MyGeneration_k3b.sln
that includes the subprojects *_k3b.csproj

*_k3b.csproj are similar to their originals but they place their output into a common build/debug directory.

there is no more need for a post-bat that copies dll-s

currently excluded are 
- dOOdad\*.* :
- vistadb (i cannot compile because of missing dlls)
- MyMetaSqlCePlugin (i cannot compile because of missing dlls)

2007-08-11
- if mymetaXXX.csproj contains a define
	- ENTERPRISE	mymeta becomes a com-object
		enabled in MyMeta.csproj
		enabled in MyMeta_k3b.csproj
	- IGNORE_VISTA  mymeta.dll will not contain the vista driver that require
			dlls to compile
		MyMeta.csproj will include vistadb
		MyMeta_k3b.csproj will exclude vistadb

2007-08-12
	installer\mygeneration.nsi original my generation installer based on output of buildall.bat
	installer\mygeneration_k3b.nsi
		the same as mygeneration.nsi except that the source comes from 
		..\build\Release instead of the local ...bin directories of buildall.bat


