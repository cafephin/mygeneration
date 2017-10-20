# MyGeneration

MyGeneration is an extremely flexible template based code generator written in Microsoft.NET. MyGeneration is great at generating code for ORM architectures. The meta-data from your database is made available to the templates through the MyMeta API.

This repository is a fork from [sourceforge.net/projects/mygeneration](https://sourceforge.net/projects/mygeneration).

## Build

```cmd
cmd> git clone https://github.com/khaister/mygeneration
cmd> cd mygeneration\build

:: ensure msbuild is in your PATH
cmd> build.cmd release    # build with release configuration
cmd> build.cmd debug      # build with debug configuration
```

## Package

```cmd
:: This shold be performed only after build step with Release configuration
:: Ensure that NSIS is installed on your machine
cmd> cd package
cmd> package.bat # installers are placed in package\installers
```