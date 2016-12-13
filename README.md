# mapbox-sdk-unity

Tools for using Mapbox APIs with C# / Unity. If you'd like to contribute to the project, [read CONTRIBUTING.md](https://github.com/mapbox/mapbox-sdk-unity/blob/master/CONTRIBUTING.md).
Tools for using Mapbox with Unity.

## Cake build system

We use cake for running tests, building the SDK and documentation. The following files are used by cake:

- `tools/packages.config`: This is the package configuration that tells the bootstrapper script what NuGet packages to install in the tools folder.

- `build.ps1` / `build.sh`: Bootstrapper powershell/bash scripts that ensure you have Cake and required dependencies installed. The bootstrapper script is also responsible for invoking Cake.

- `build.cake`: The actual cake scripts. If you want to add a new command, do it here.

### Cake commands

- __Mac OS and Linux__: `./build.sh`
- __Windows__: `powershell .\build.ps1`

Use the argument `-Target` followed by the name of the task you want to run if you don't want to run all tasks. For example, on a mac, `./build.sh -Target "Build-Docs"` will generate docs.
