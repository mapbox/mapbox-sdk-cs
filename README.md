# mapbox-sdk-unity

Tools for using Mapbox APIs with C# / Unity. If you'd like to contribute to the project, [read CONTRIBUTING.md](https://github.com/mapbox/mapbox-sdk-unity/blob/master/CONTRIBUTING.md).
Tools for using Mapbox with Unity.

## Build

You need to have `Unity` installed for a successful build.

With an IDE:
* Open `MapboxSDKUnity.sln` with Visual Studio or Xamarin Studio
* `Build -> Rebuild Solution`
* DLLs will be in `bin`

On the command line:
* `nuget restore MapboxSDKUnity.sln`
* Windows: `msbuild MapboxSDKUnity.sln`
* Linux/OSX: `xbuild MapboxSDKUnity.sln`
* DLLs will be in `bin`

## Tests

To run the tests you need to have the `MAPBOX_ACCESS_TOKEN` environment variable set.

Log into your Mapbox account at https://www.mapbox.com/studio to obtain an access token.
