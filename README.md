# mapbox-sdk-cs

SDK for using Mapbox APIs with C#. If you'd like to contribute to the project, [read CONTRIBUTING.md](https://github.com/mapbox/mapbox-sdk-cs/blob/master/CONTRIBUTING.md).


`master` branch build status: [![Build status](https://ci.appveyor.com/api/projects/status/mh7ad8p1qonkbnwr/branch/master?svg=true)](https://ci.appveyor.com/project/Mapbox/mapbox-sdk-cs/branch/master)

`master` branch code coverage: [![Coverage Status](https://coveralls.io/repos/github/mapbox/mapbox-sdk-cs/badge.svg?branch=master&t=jR0cza)](https://coveralls.io/github/mapbox/mapbox-sdk-cs?branch=master)

## Build

With an IDE:
* Open `MapboxSdkCs.sln` with Visual Studio or Xamarin Studio
* Select configuration to build:
  * `DebugNet` for targeting .NetFramework >=3.5
  * `DebugUWP` for targeting UWP
* `Build -> Rebuild Solution`
* DLLs will be in
  * `bin\Debug\net35\`
  * or `bin\Debug\uap10`

On the command line:
* Windows
  * `nuget restore MapboxSdkCs.sln`
  * `msbuild MapboxSdkCs.sln /p:Configuration=<DebugNet|DebugUWP>`
* Linux/OSX
  * `mono nuget.exe restore MapboxSdkCs.sln`
  * `xbuild MapboxSdkCs.sln /p:Configuration=<DebugNet|DebugUWP>`
* DLLs will be in
  * `bin\Debug\net35\`
  * or `bin\Debug\uap10`

## Tests

Currently tests are only available with configuration `DebugNet`.

To run the tests you need to have the `MAPBOX_ACCESS_TOKEN` environment variable set.

Log into your Mapbox account at https://www.mapbox.com/studio to obtain an access token.

## Automation and Publishing

**Before publishing verify that there no build or test errors locally and on AppVeyor and Travis!**

### Publishing to nuget.org

* Increment the versions in `versions.txt`
  * `dlls`: will patch the `AssemblyVersion` attribute in `src\SharedAssemblyInfo.cs`, e.g.: `dlls:1.0.0.1`. Convention is `<major version>.<minor version>.<build number>.<revision>`.
  * `nupkg`: defines the version of the nuget package, e.g.: `nupkg:1.0.0-alpha04`.
  This may be different from `dlls` as Nuget allows for custom postfixes to identify pre-releases, see https://docs.nuget.org/ndocs/create-packages/prerelease-packages#semantic-versioning
* Commit with a message containing `[publish nuget]` (can be anywhere within the commit message)
* Tag the commit with the `nupkg` version
* Push.
  * AppVeyor build will publish to nuget.org
  * Verify the build itself and publishing finished successfully: https://ci.appveyor.com/project/Mapbox/mapbox-sdk-cs

### Publishing Docs

* Commit with a message containing `[publish docs]`
  * can be anywhere within the commit message
  * may be combined with `[publish nuget]`
* Push
  * AppVeyor build will publish to `gh-pages`
  * Verify the build itself and publishing finished successfully:
    * https://ci.appveyor.com/project/Mapbox/mapbox-sdk-cs
    * https://mapbox.github.io/mapbox-sdk-cs/

### Publishing locally (currently Windows only)

**This should not be done unless there is an emergency (e.g. AppVeyor is down)**

* `SET MAPBOX_ACCESS_TOKEN=<MAPBOX-ACCESS-TOKEN>`
* `SET NUGET_API_KEY=<NUGET-API-KEY-WITH-PERMISSION-TO-PUSH-NUPKG>`
* `SET GITHUB_TOKEN=<GITHUB-TOKEN-WITH-PERMISSION-TO-PUSH-TO-GHPAGES>`
* Increment the versions in `versions.txt`
* `build-local.bat "APPVEYOR_REPO_COMMIT_MESSAGE=[publish nuget] [publish docs]"`
