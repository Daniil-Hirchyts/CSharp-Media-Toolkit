# Media Tools

**C# media-processing libraries and Windows desktop interfaces · 2024**

I developed **Media Tools** to explore audio and video processing through reusable C# libraries and WPF interfaces. The work included audio conversion, waveform rendering, scene-change detection, frame browsing, speech transcription and translation integrations.

The repository retained the .NET 6 application and libraries, the original NUnit tests, an earlier .NET Framework interface and the original library reference.

## Repository structure

| Location | Contents |
| --- | --- |
| [Media_Tools.sln](Media_Tools.sln) | Root solution for the .NET 6 application, libraries and tests. |
| [src/MediaToolsApplication](src/MediaToolsApplication/) | WPF application with waveform, shot-detection and transcription controls. |
| [src/WaveForm](src/WaveForm/) | The main media-processing library, built as the `MediaTools` assembly. |
| [src/MediaToolLib](src/MediaToolLib/) | Additional filesystem, waveform and scene-detection helpers. |
| [tests/MediaToolsTest](tests/MediaToolsTest/) | NUnit tests and media/service integration cases. |
| [legacy](legacy/) | The earlier .NET Framework 4.8 interface and its bundled dependencies. |
| [assets/samples](assets/samples/) | The original example waveform image, shared by the interfaces. |
| [docs](docs/) | Development notes and the archived library API reference. |

## Media-processing work

The libraries implemented file and folder utilities alongside FFmpeg-based processing:

- Converting audio and video inputs into audio files.
- Rendering waveform images with configurable dimensions, colours and channel mixing.
- Detecting scene changes and extracting preview frames.
- Reading shot timestamps and navigating extracted frames through the desktop interface.
- Exploring voice activity, silence detection and speech transcription.

The code also contained integrations and experiments using Vosk, Google Cloud Speech, Microsoft speech services, Amazon Transcribe/S3 and DeepL. Those paths used provider-specific configuration or local speech models.

## Desktop application

The main WPF interface grouped the tools into tabs and exposed file selection, waveform settings, scene thresholds, frame previews and transcription controls. Its settings interface stored the selected FFmpeg location.

An earlier .NET Framework 4.8 application was retained under [legacy/MediaToolsApp](legacy/MediaToolsApp/). It included an earlier media interface and MVVM examples. The root solution grouped the .NET 6 projects separately from that historical application.

## Technical environment

The main projects targeted **.NET 6**, with the desktop application and media/test projects targeting Windows. They used **WPF**, **FFmpeg**, **FFmpeg.AutoGen**, **Vosk**, **System.Drawing** and the speech/translation SDKs recorded in their project files.

The original tests covered file helpers and included cases that depended on media fixtures, native tools or external services. The original model locations and integration configuration remained part of the historical source.

The application and processing library compiled after reorganisation. The preserved test project still referenced earlier APIs; its existing compilation errors were recorded in the development notes.

The [development notes](docs/development.md) recorded the solution layout and environment requirements. The [archived API reference](docs/archive/library-reference.md) retained the original library documentation.

## Author and history

Developed by **[Daniil Hirchyts](https://github.com/Daniil-Hirchyts)**. The original commits were recorded in April 2024, and their authorship and commit objects were retained.

## License

Original code by Daniil Hirchyts is licensed under the [MIT License](LICENSE). Bundled NuGet packages and other third-party components retain their own licenses and notices.
