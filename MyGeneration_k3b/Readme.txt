I created a new Solution MyGeneration_k3b\MyGeneration_k3b.sln
that includes the subprojects *_k3b.csproj

*_k3b.csproj are similar to their originals but they place their output into a common build/debug directory.

there is no more need for a post-bat that copies dll-s

currently excluded are 
- dOOdad\*.* :
- vistadb (i cannot compile because of missing dlls)
- MyMetaSqlCePlugin (i cannot compile because of missing dlls)

ToDo update the install-script


