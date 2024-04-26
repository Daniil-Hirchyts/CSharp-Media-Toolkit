# Development notes

## Solution layout

The root `Media_Tools.sln` groups four .NET 6 projects:

- `src/MediaToolsApplication`: the WPF desktop application.
- `src/WaveForm`: the `MediaTools` processing library.
- `src/MediaToolLib`: additional processing and filesystem helpers.
- `tests/MediaToolsTest`: the NUnit test project.

The earlier .NET Framework 4.8 project was retained separately in `legacy/MediaToolsApp`. Its original package files remained in `legacy/packages`.

## Original environment requirements

The desktop application and Windows-targeted library/test projects were developed for Windows. The project files retain their original .NET 6 target frameworks, including `net6.0-windows10.0.22000.0` where specified.

Media-processing paths used FFmpeg binaries and native libraries. Speech functionality also used a local Vosk model or provider-specific service configuration. The original source retained environment-specific model and fixture paths.

The shared waveform preview was embedded as a WPF resource from `assets/samples`, replacing the old working-directory and machine-specific sample-image references.

## Build entry points

The .NET 6 application and helper-library build entry points on Windows are:

```powershell
dotnet restore Media_Tools.sln
dotnet build src/MediaToolsApplication/MediaToolsApplication.csproj --configuration Release
dotnet build src/MediaToolLib/MediaToolLib.csproj --configuration Release
```

The test project's entry point is:

```powershell
dotnet test tests/MediaToolsTest/MediaToolsTest.csproj --configuration Release
```

The original test suite included both helper tests and integration cases. Media files, speech models and service configuration were required by the corresponding integration paths; those requirements were retained with the original tests.

## Verification of the reorganised snapshot

The main WPF application and its processing library compiled successfully with Windows targeting enabled. The shared sample PNG and its resource name were verified in the application assembly.

The complete solution build reached nine compilation errors in the preserved NUnit project. Those tests referenced older waveform, scene-detection and Amazon transcription APIs. Building the untouched original test project reproduced the same nine errors. The test source was retained unchanged.

The Windows interface and integration tests were not executed on macOS.

## Historical interface

The legacy project targeted .NET Framework 4.8 and retained its original reference to the .NET 6 processing library. It was kept outside the active solution because that historical framework combination was not part of the current .NET 6 build group.

The bundled legacy packages and their notices were preserved. Generated build output and personal editor settings were excluded from version control.
