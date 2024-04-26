# Media processing API

- Project: [`src/WaveForm`](../src/WaveForm/)
- Assembly: `MediaTools.dll`
- Namespace: `Tools`

The examples use Windows paths. Set the FFmpeg native-library directory before calling waveform or scene-processing methods:

```csharp
using FFmpeg.AutoGen;
using Tools;

ffmpeg.RootPath = @"C:\ffmpeg\bin";
```

## AudioTools.ConvertToAudio

```csharp
public static string ConvertToAudio(
    string input_filename,
    string extension = "wav",
    string outdir = "./Data/Audios/");
```

Converts MP3 or WAV input into the requested MP3/WAV format through the FFmpeg executable. Returns the output-file path. Files are written beneath an extension subdirectory of `outdir`.

```csharp
string audio = AudioTools.ConvertToAudio(
    input_filename: @"media\voice.mp3",
    extension: "wav",
    outdir: @"output\audio\");
```

The FFmpeg executable must be on `PATH` for this method.

## AudioTools.WaveForm

```csharp
public static string WaveForm(
    string file,
    string color = "brown",
    bool? ismixed = true,
    int height = 400,
    int width = 600,
    string outDir = "./WaveForm/PNGs/");
```

Decodes an input's audio stream and writes a PNG waveform. The result is the path to the generated image.

| Parameter | Meaning |
| --- | --- |
| `file` | Audio or video input path. |
| `color` | Colour passed to FFmpeg's waveform filter. |
| `ismixed` | Channel-mixing selection; `null` uses the default mixed behaviour. |
| `height`, `width` | Output image dimensions in pixels. |
| `outDir` | Parent directory for the generated waveform folder. |

```csharp
string waveform = AudioTools.WaveForm(
    file: @"media\clip.mp4",
    color: "blue",
    ismixed: true,
    height: 400,
    width: 1200,
    outDir: @"output\waveforms\");
```

## VideoTools.ShotDetection

```csharp
public static string ShotDetection(
    string inputfile,
    string threshold,
    IProgress<int> progress,
    uint scale = 500,
    string outdir = "./ShotDetection/JPGs/");
```

Detects scene changes, writes numbered JPEG frames and creates `metadata.txt` containing shot information. Returns the generated output directory.

`threshold` is an invariant-culture decimal string between `0` and `1`. `scale` sets the output-frame width; FFmpeg calculates the corresponding height. `progress` receives a processing percentage.

```csharp
var progress = new Progress<int>(value => Console.WriteLine($"{value}%"));

string shots = VideoTools.ShotDetection(
    inputfile: @"media\clip.mp4",
    threshold: "0.2",
    progress: progress,
    scale: 500,
    outdir: @"output\shots\");

List<float> timestamps = VideoTools.ShotTimeStamps(
    Path.Combine(shots, "metadata.txt"));
```

## File helpers

| Method | Result |
| --- | --- |
| `UseTools.GetName(path)` | Final path component, handling `/` and `\` separators. |
| `UseTools.GetExtension(filename)` | Extension without the dot, or an empty string. |
| `UseTools.GetNameAndExtension(filename)` | Two-element array containing the name without its final extension and the extension. |
| `UseTools.CheckFileAndExt(path, acceptedExtensions)` | Checks that a file exists and optionally validates its extension. |
| `UseTools.CreateDirectory(path)` | Creates a directory path and returns the resulting path. |

## Speech and translation APIs

- `SpeechToText.TranscribeAsync(gcsUri, languageCode, progress)` transcribes audio from Google Cloud Storage.
- `SpeechToText.TranscribeAndSaveSubtitles(gcsUri, languageCode, outputPath, progress)` writes subtitle output.
- `new CloudStorageService(bucketName).UploadFileAsync(localPath, objectName)` uploads an input to a storage bucket; `objectName` is optional.
- `new Translation().TranslateAsync(text, from_language, to_language)` translates text through DeepL.

These APIs require provider configuration. See [setup](setup.md#speech-and-translation) and the implementations in [Tools.cs](../src/WaveForm/Tools.cs).
