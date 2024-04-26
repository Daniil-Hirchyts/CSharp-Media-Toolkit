# Build and configuration

## Project targets

| Project | Target |
| --- | --- |
| `MediaToolsApplication` | .NET 6 Windows, WPF |
| `WaveForm` | .NET 6 Windows |
| `MediaToolLib` | .NET 6 |
| `MediaToolsTest` | .NET 6 Windows, NUnit |
| `legacy/MediaToolsApp` | .NET Framework 4.8, WPF |

The Windows projects use the target framework `net6.0-windows10.0.22000.0`.

## Build

From the repository root on Windows:

```powershell
dotnet restore Media_Tools.sln
dotnet build src/MediaToolsApplication/MediaToolsApplication.csproj --configuration Release
dotnet build src/MediaToolLib/MediaToolLib.csproj --configuration Release
```

Run the desktop application:

```powershell
dotnet run --project src/MediaToolsApplication/MediaToolsApplication.csproj --configuration Release
```

## FFmpeg

Use the FFmpeg 6.1 shared build, including `ffmpeg.exe` and its native DLLs. The library references `FFmpeg.AutoGen` 6.1.0.

- **Desktop app:** select `ffmpeg.exe` in the Settings tab. Its directory becomes the native-library search path and the selection is saved through `AppSettings`.
- **Library code:** set `FFmpeg.AutoGen.ffmpeg.RootPath` to the directory containing the DLLs before calling native media operations.
- **Process-based conversion:** `AudioTools.ConvertToAudio` starts `ffmpeg` by name, so the executable must also be available on `PATH`.

The application setting is named `ffmpegPath` in [App.config](../src/MediaToolsApplication/App.config).

## Speech and translation

| Feature | Configuration |
| --- | --- |
| **Vosk transcription** | Set the model directory in `Button_Transcribe_Click` in [MainWindow.xaml.cs](../src/MediaToolsApplication/MainWindow.xaml.cs). The current handler creates a recogniser with a 44,100 Hz sample rate. |
| **Google Cloud Speech** | Configure authentication for the Google client SDK and a Cloud Storage bucket. The desktop handler sets the bucket in `ButtonTranscription_Click`; the library's `SpeechToText` methods accept a `gs://` audio URI. |
| **DeepL translation** | Configure the translator in the `Translation` class in [Tools.cs](../src/WaveForm/Tools.cs). The application calls its `TranslateAsync` method from `ButtonTranslation_Click`. |

Cloud credentials, local recognition models and media files are environment-specific inputs. They are separate from the application binaries.

## Generated files

Library defaults write to these paths, relative to the process working directory:

| Operation | Default output directory |
| --- | --- |
| MP3/WAV conversion | `Data/Audios/<extension>/` |
| Waveform generation | `WaveForm/PNGs/` |
| Scene detection | `ShotDetection/JPGs/` |

The conversion, waveform and scene-detection methods accept an output-directory argument. Native build output, generated media and local editor settings are covered by `.gitignore`.

## Tests

The NUnit project contains file-helper tests and integration cases for media processing and external services.

```powershell
dotnet test tests/MediaToolsTest/MediaToolsTest.csproj --configuration Release
```

The test project currently does not compile: several tests call older waveform, scene-detection and Amazon transcription APIs. As a result, a build of the entire solution also fails at the test project. The application and library build commands above compile those projects separately.

Media integration cases also reference local fixtures and models; cloud cases require their provider configuration.

## Legacy interface

`legacy/MediaToolsApp` targets .NET Framework 4.8 and includes a reference to the .NET 6 processing library. Those framework targets are incompatible. This project is outside the root solution's build and requires a dependency/framework update before it can be built independently.
