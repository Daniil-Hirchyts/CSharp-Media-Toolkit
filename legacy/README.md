# .NET Framework interface

`MediaToolsApp` contains the .NET Framework 4.8 WPF interface, with waveform, scene-detection and transcription tabs, plus MVVM examples for commands and file services.

Its bundled package files are in `packages/`. The project references the .NET 6 `WaveForm` library, which is incompatible with its .NET Framework target. It is therefore excluded from the root solution.

The desktop application used by the main solution is [MediaToolsApplication](../src/MediaToolsApplication/).
