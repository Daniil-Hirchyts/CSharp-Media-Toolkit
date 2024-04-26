# C# Media Toolkit

A Windows desktop application and .NET libraries for audio conversion, waveform generation, scene detection, speech transcription and translation.

## Features

| Tool | Function |
| --- | --- |
| **Waveforms** | Generate PNG waveforms from audio or video, with control over colour, dimensions and channel mixing. |
| **Scene detection** | Detect scene changes using a configurable threshold, export JPEG previews and read shot timestamps. |
| **Frame extraction** | Extract still images from video and browse detected shots in the desktop interface. |
| **Audio conversion** | Convert between MP3 and WAV and extract audio from video. |
| **Transcription** | Transcribe audio using a local Vosk model or Google Cloud Speech. |
| **Translation** | Translate text through DeepL. |

![Example waveform output](assets/samples/sheep.mp4-waveform-mixed.png)

## Run the desktop application

Requirements:

- Windows x64.
- A .NET SDK that supports the project's .NET 6 targets, plus the .NET 6 Windows Desktop Runtime.
- FFmpeg 6.1 shared binaries, matching the `FFmpeg.AutoGen` dependency.

```powershell
git clone https://github.com/Daniil-Hirchyts/CSharp-Media-Toolkit.git
cd CSharp-Media-Toolkit
dotnet run --project src/MediaToolsApplication/MediaToolsApplication.csproj --configuration Release
```

In **Settings**, select `ffmpeg.exe` from the folder containing the FFmpeg shared libraries. The waveform and scene-detection tabs then accept a media file and the relevant processing options.

Speech and translation tools require additional model or provider configuration. See [setup and configuration](docs/setup.md).

## Use the library

The main library is built from `src/WaveForm` as `MediaTools.dll`, with public types in the `Tools` namespace.

```csharp
using FFmpeg.AutoGen;
using Tools;

ffmpeg.RootPath = @"C:\ffmpeg\bin";

string waveform = AudioTools.WaveForm(
    file: @"media\clip.mp4",
    color: "blue",
    ismixed: true,
    height: 400,
    width: 1200,
    outDir: @"output\waveforms\");
```

`WaveForm` returns the path to the generated PNG. See the [API reference](docs/api-reference.md) for conversion, scene detection and file utilities.

## Project structure

| Project | Purpose |
| --- | --- |
| [MediaToolsApplication](src/MediaToolsApplication/) | WPF desktop interface. |
| [WaveForm](src/WaveForm/) | Main audio, video, speech and translation library. |
| [MediaToolLib](src/MediaToolLib/) | Additional filesystem, waveform and scene helpers. |
| [MediaToolsTest](tests/MediaToolsTest/) | NUnit helper and integration tests. |
| [Legacy application](legacy/) | .NET Framework 4.8 interface and its dependencies. |

[Build, configuration and test details](docs/setup.md) describe the environment requirements and current test limitations.

## License

[MIT](LICENSE) for project code. Third-party dependencies use their respective licenses.
