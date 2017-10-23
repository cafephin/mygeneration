# MyGeneration

MyGeneration is a flexible template-based code generator written in Microsoft .NET Framework. MyGeneration is great at generating code for ORM architectures. The metadata from your database is made available to the templates through the MyMeta API.

This repository is a fork from [sourceforge.net/projects/mygeneration](https://sourceforge.net/projects/mygeneration).

## Download

Download the latest release from the [release page](https://github.com/cafephin/mygeneration/releases).

## Build

```cmd
:: ensure msbuild is in your PATH

cmd> git clone https://github.com/khaister/mygeneration
cmd> cd mygeneration\build

:: build with release configuration
cmd> build.cmd release

:: build with debug configuration
cmd> build.cmd debug
```

## Package

```cmd
:: this should be performed only after build step with Release configuration
:: ensure that NSIS is installed on your machine
:: output installer files are placed in pack\installers

cmd> cd pack
cmd> pack.cmd
```