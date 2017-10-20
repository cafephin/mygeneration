# MyGeneration

MyGeneration is an extremely flexible template based code generator written in Microsoft.NET. MyGeneration is great at generating code for ORM architectures. The meta-data from your database is made available to the templates through the MyMeta API.

This repository is a fork from [sourceforge.net/projects/mygeneration](https://sourceforge.net/projects/mygeneration).

## Build

```bash
git clone https://github.com/khaister/mygeneration
cd mygeneration\build

# ensure msbuild is in your PATH
build.cmd release    # build with release configuration
build.cmd debug      # build with debug configuration
```