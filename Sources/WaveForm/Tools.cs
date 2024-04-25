using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;
using DeepL;
using FFmpeg.AutoGen;
using Google.Cloud.Speech.V1;
using Google.Cloud.Storage.V1;
using Newtonsoft.Json;
using Vosk;

namespace Tools;

/// <summary>
/// </summary>
[Serializable]
public class InvalidExtensionException : Exception
{
    /// <summary>
    /// </summary>
    public InvalidExtensionException()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    public InvalidExtensionException(string message) : base(message)
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public InvalidExtensionException(string message, Exception inner) : base(message, inner)
    {
    }


    /// <summary>
    ///     A constructor is needed for serialization when an
    ///     exception propagates from a remoting server to the client.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected InvalidExtensionException(SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}

/// <summary>
/// </summary>
[Serializable]
public class NoFileException : Exception
{
    /// <summary>
    /// </summary>
    public NoFileException()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    public NoFileException(string message) : base(message)
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public NoFileException(string message, Exception inner) : base(message, inner)
    {
    }


    /// <summary>
    ///     A constructor is needed for serialization when an
    ///     exception propagates from a remoting server to the client.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected NoFileException(SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}

/// <summary>
/// </summary>
[Serializable]
public class ThresholdException : Exception
{
    /// <summary>
    /// </summary>
    public ThresholdException()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    public ThresholdException(string message) : base(message)
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public ThresholdException(string message, Exception inner) : base(message, inner)
    {
    }


    /// <summary>
    ///     A constructor is needed for serialization when an
    ///     exception propagates from a remoting server to the client.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected ThresholdException(SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}

/// <summary>
/// </summary>
[Serializable]
public class LanguageException : Exception
{
    /// <summary>
    /// </summary>
    public LanguageException()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    public LanguageException(string message) : base(message)
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public LanguageException(string message, Exception inner) : base(message, inner)
    {
    }


    /// <summary>
    ///     A constructor is needed for serialization when an
    ///     exception propagates from a remoting server to the client.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected LanguageException(SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}

/// <summary>
///     UseTools Class that contains useful functions about files and folders.
/// </summary>
public unsafe class UseTools
{
    /// <summary>
    ///     All the possible audio extension supported.
    /// </summary>
    public static readonly string[] all_audio_extensions =
    {
        "mp3",
        "wav"
    };

    /// <summary>
    ///     All the possible video extension supported.
    /// </summary>
    public static readonly string[] all_video_extensions =
    {
        "mp4",
        "mov",
        "mxf"
    };

    /// <summary>
    ///     All the possible audio and video extension supported.
    /// </summary>
    public static readonly string[] all_extensions =
        all_audio_extensions.Concat(all_video_extensions).ToArray();

    /// <summary>
    ///     All the possible data extension supported.
    /// </summary>
    public static readonly string[] data_extensions =
    {
        "txt",
        "",
        "csv"
    };

    /// <summary>
    ///     Create a directory if it does not already exist. The path is either relative ("./...") or absolute (C://...)
    /// </summary>
    /// <param name="path">Path to create.</param>
    /// <returns>Void</returns>
    public static string CreateDirectory(string path)
    {
        // Create the directory with the path path if it does not exist
        Console.WriteLine($"Creation of the directory {path}");

        char[] file_separators = { '/', '\\' };
        var files = path.Split(file_separators);
        var first_dir = true;
        var created_folder = @"";
        foreach (var f in files)
            if (first_dir)
            {
                created_folder = @"" + f + "\\";
                if (f == "." || f == "..")
                {
                    var fi = new FileInfo(created_folder);
                    created_folder = fi.FullName;
                }

                first_dir = false;
            }
            else
            {
                if (f != "")
                {
                    created_folder = created_folder + f + "\\";
                    Directory.CreateDirectory(created_folder);
                }
            }

        if (created_folder == "")
            Console.WriteLine("Empty folder, none created.");
        else
            Console.WriteLine($"Created folder {created_folder}");

        return created_folder;
    }

    /// <summary>
    ///     Computes the name of the filename in the path.
    /// </summary>
    /// <param name="filename">Path to the file</param>
    /// <returns>String</returns>
    public static string GetName(string filename)
    {
        char[] separators = { '/', '\\' };
        return filename.Split(separators).Last();
    }

    /// <summary>
    ///     Computes the extension of the filename.
    /// </summary>
    /// <param name="filename">Path to the file</param>
    /// <returns>String</returns>
    public static string GetExtension(string filename)
    {
        if (filename.Contains('.')) return filename.Split('.').Last();

        return "";
    }

    /// <summary>
    ///     Computes the extension of the filename and returns also the name part.
    /// </summary>
    /// <param name="filename">Path to the file</param>
    /// <returns>String</returns>
    public static string[] GetNameAndExtension(string filename)
    {
        var n = filename.Length;
        var i = n - 1;
        while (i > 0)
        {
            if (filename[i] == '.') return new[] { filename.Substring(0, i), filename.Substring(i + 1) };

            i--;
        }

        return new[] { filename, "" };
    }

    public static string CheckFileAndExt(string filename, string[]? accepted_extensions = null)
    {
        char[] separators = { '/', '\\' };
        var fname = filename.Split(separators).Last();

        // Check file
        if (fname == "") throw new NoFileException("No file input.");

        if (!File.Exists(filename)) throw new FileNotFoundException($"The input file {filename} does not exist.");

        // Check extension
        if (accepted_extensions != null)
        {
            var ext = GetExtension(fname);

            /*if (ext == "")
            {
                throw new InvalidExtensionException($"The file has no extension.");
            }*/

            if (!Array.Exists(accepted_extensions, e => e == ext))
                throw new InvalidExtensionException($"The extension \"{ext}\" is not supported here (yet?).");
        }


        return fname;
    }

    public static void SaveFrameAsImage(AVFrame* frame, int width, int height, string outputPath,
        ImageFormat format)
    {
        var pFrameARGB = ffmpeg.av_frame_alloc();
        if (pFrameARGB == null) throw new Exception("Could not allocate memory for ARGB frame.");

        var buffer_size = ffmpeg.av_image_get_buffer_size(AVPixelFormat.AV_PIX_FMT_BGRA, width, height, 1);
        var buffer = (byte*)ffmpeg.av_malloc((ulong)buffer_size);

        var data = new byte_ptrArray4();
        var linesize = new int_array4();

        ffmpeg.av_image_fill_arrays(ref data, ref linesize, buffer, AVPixelFormat.AV_PIX_FMT_BGRA, width,
            height, 1);

        var data8 = new byte_ptrArray8
        {
            [0] = data[0],
            [1] = data[1],
            [2] = data[2],
            [3] = data[3]
        };

        pFrameARGB->data = data8;

        var linesize8 = new int_array8
        {
            [0] = linesize[0],
            [1] = linesize[1],
            [2] = linesize[2],
            [3] = linesize[3]
        };

        pFrameARGB->linesize = linesize8;


        var sws_ctx = ffmpeg.sws_getContext(
            width,
            height,
            (AVPixelFormat)frame->format,
            width,
            height,
            AVPixelFormat.AV_PIX_FMT_BGRA,
            ffmpeg.SWS_FAST_BILINEAR,
            null,
            null,
            null
        );

        if (sws_ctx == null) throw new Exception("Could not initialize the conversion context.");

        ffmpeg.sws_scale(sws_ctx, frame->data, frame->linesize, 0, height, pFrameARGB->data, pFrameARGB->linesize);

        using (var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            var srcPtr = pFrameARGB->data[0];
            var destPtr = (byte*)bitmapData.Scan0;
            var srcStride = pFrameARGB->linesize[0];

            for (var y = 0; y < height; y++)
            {
                Buffer.MemoryCopy(srcPtr, destPtr, width * 4, width * 4);
                srcPtr += srcStride;
                destPtr += bitmapData.Stride;
            }

            bitmap.UnlockBits(bitmapData);
            bitmap.Save(outputPath, format);
        }

        // Cleanup
        ffmpeg.av_free(buffer);
        ffmpeg.av_frame_free(&pFrameARGB);
        ffmpeg.sws_freeContext(sws_ctx);
    }
}

public static unsafe class FFmpegExtensions
{
    /// <summary>
    ///     Throws an exception if the result indicates an error.
    /// </summary>
    /// <param name="code">The result code returned by an FFmpeg function.</param>
    public static void ThrowExceptionIfError(this int code)
    {
        if (code < 0)
        {
            var buff = new byte[255];
            var pinnedArray = GCHandle.Alloc(buff, GCHandleType.Pinned);
            var ptr = pinnedArray.AddrOfPinnedObject();
            ffmpeg.av_strerror(code, (byte*)ptr, 255);
            pinnedArray.Free();

            var s = Encoding.UTF8.GetString(buff, 0, buff.Length);
            var pos = s.IndexOf('\0');
            if (pos >= 0)
                s = s.Substring(0, pos);
            Debug.WriteLine(s);
        }
    }
}

/// <summary>
///     AudioTools Class that contains useful methods about audio files.
/// </summary>
public unsafe class AudioTools
{
    public const int AV_BUFFERSRC_FLAG_KEEP_REF = 1;

    private static AVFormatContext* fmtCtx;
    private static AVCodecContext* decCtx;
    private static AVFilterContext* buffersinkCtx;
    private static AVFilterContext* buffersrc_ctx;
    private static AVFilterGraph* filterGraph;

    /// <summary>
    ///     Converts any audio or video file to a specified extension.
    /// </summary>
    /// <param name="input_filename">Path to the audio or video file to convert.</param>
    /// <param name="extension">(Optionnal) Extension of the output audio or video file. Default wav.</param>
    /// <param name="outdir"> (Optionnal) Path of the folder to save the created file.</param>
    /// <returns>Path to the created file.</returns>
    public static string ConvertToAudio(string input_filename,
        string extension = "wav",
        string outdir = @"./Data/Audios/")
    {
        char[] separators = { '/', '\\' };

        var filename = input_filename.Split(separators).Last();

        if (filename == "") throw new NoFileException("No file input.");

        var name_and_ext = UseTools.GetNameAndExtension(filename);
        var name = name_and_ext[0];
        var ext = name_and_ext[^1].ToLower();

        extension = Regex.Replace(extension, "[^a-zA-Z0-9]", "", RegexOptions.Compiled);
        extension = extension.ToLower();

        if (ext == "") throw new InvalidExtensionException("The file has no extension.");

        if (!Array.Exists(UseTools.all_audio_extensions, e => e == ext))
            throw new InvalidExtensionException(
                $"The audio extension of the file {input_filename} is not supported (yet?).");

        if (!Array.Exists(UseTools.all_audio_extensions, e => e == extension))
            throw new InvalidExtensionException($"The target audio extension {extension} is not supported (yet?).");

        outdir = outdir + extension + "/";

        // We convert only if the output extension is different than the input extension.
        /*if (!(ext == extension))
        {*/
        outdir = UseTools.CreateDirectory(outdir);
        Console.WriteLine("\nConverting {0} to {1} in {2}...", name, extension, outdir);
        var output_filename_ext = string.Format(outdir + "{0}.{1}", name, extension);
        if (File.Exists(output_filename_ext))
            Console.WriteLine("File already converted in \"" + outdir + "\", continue...");
        else
            using (var process = new Process
                   {
                       StartInfo =
                       {
                           FileName = "ffmpeg",
                           Arguments = "-i \"" + input_filename /*+ " -ac 1 " + output_mono*/ + "\" \"" +
                                       output_filename_ext + "\"",
                           UseShellExecute = false, CreateNoWindow = true,
                           RedirectStandardOutput = true, RedirectStandardError = true
                       },
                       EnableRaisingEvents = true
                   })
            {
                //store outcome of process
                var errors = new StringBuilder();
                var output = new StringBuilder();
                var hadErrors = false;

                // capture normal output
                process.OutputDataReceived += (s, d) => { output.Append(d.Data); };

                // Capture error output
                process.ErrorDataReceived += (s, d) =>
                {
                    if (!hadErrors)
                        if (d.Data != null)
                            if (d.Data.EndsWith("Invalid data found when processing input"))
                                hadErrors = true;

                    errors.Append(d.Data);
                };

                // Console.WriteLine(process.StartInfo.Arguments);
                if (!process.Start())
                {
                    Console.WriteLine("Error starting");
                    Environment.Exit(0);
                }

                // start listening on the stream
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.WaitForExit();
                var stdout = output.ToString();
                var stderr = errors.ToString();

                if (process.ExitCode != 0 || hadErrors) throw new ArgumentException("Error: " + stderr);

                Console.WriteLine("File converted in " + outdir);
            }

        Console.WriteLine("");
        input_filename = output_filename_ext;
        /*}
        else
        {
            Console.WriteLine("The file is already {0} is the correct extension", input_filename);
        }*/
        return input_filename;
    }

    public static string WaveForm(string file, string color = "brown", bool? ismixed = true, int height = 400,
        int width = 600, string outDir = @"./WaveForm/PNGs/")
    {
        AVFormatContext* formatContext = null;
        ffmpeg.avformat_open_input(&formatContext, file, null, null).ThrowExceptionIfError();
        ffmpeg.avformat_find_stream_info(formatContext, null).ThrowExceptionIfError();

        AVCodec* codec = null;
        var streamIndex =
            ffmpeg.av_find_best_stream(formatContext, AVMediaType.AVMEDIA_TYPE_AUDIO, -1, -1, &codec, 0);
        if (streamIndex < 0)
            throw new ApplicationException("Could not find any audio stream in the file");

        var stream = formatContext->streams[streamIndex];
        var codecContext = ffmpeg.avcodec_alloc_context3(codec);
        ffmpeg.avcodec_parameters_to_context(codecContext, stream->codecpar);
        ffmpeg.avcodec_open2(codecContext, codec, null).ThrowExceptionIfError();

        var filterGraph = ffmpeg.avfilter_graph_alloc();
        if (filterGraph == null)
            throw new ApplicationException("Could not allocate filter graph");

        AVFilterContext* buffersrc_ctx = null;
        AVFilterContext* buffersink_ctx = null;

        var sample_rate = codecContext->sample_rate;
        var sample_fmt = codecContext->sample_fmt;
        var channel_layout = codecContext->channel_layout;

        if (channel_layout == 0 && codecContext->channels > 0)
            channel_layout = (ulong)ffmpeg.av_get_default_channel_layout(codecContext->channels);

        var abuffer_args =
            $"time_base={1}/{sample_rate}:sample_rate={sample_rate}:sample_fmt={ffmpeg.av_get_sample_fmt_name(sample_fmt)}:channel_layout={channel_layout}";
        ffmpeg.avfilter_graph_create_filter(&buffersrc_ctx, ffmpeg.avfilter_get_by_name("abuffer"), "src",
            abuffer_args, null, filterGraph).ThrowExceptionIfError();
        ffmpeg.avfilter_graph_create_filter(&buffersink_ctx, ffmpeg.avfilter_get_by_name("buffersink"), "sink",
            null, null, filterGraph).ThrowExceptionIfError();

        var filterSpec = ismixed ?? true
            ? $"showwavespic=colors={color}:s={width}x{height}"
            : $"aformat=channel_layouts=stereo,showwavespic=colors={color}:s={width}x{height}";
        var outputs = ffmpeg.avfilter_inout_alloc();
        var inputs = ffmpeg.avfilter_inout_alloc();
        outputs->name = ffmpeg.av_strdup("in");
        outputs->filter_ctx = buffersrc_ctx;
        outputs->pad_idx = 0;
        outputs->next = null;
        inputs->name = ffmpeg.av_strdup("out");
        inputs->filter_ctx = buffersink_ctx;
        inputs->pad_idx = 0;
        inputs->next = null;

        ffmpeg.avfilter_graph_parse_ptr(filterGraph, filterSpec, &inputs, &outputs, null).ThrowExceptionIfError();
        ffmpeg.avfilter_graph_config(filterGraph, null).ThrowExceptionIfError();

        var packet = ffmpeg.av_packet_alloc();
        try
        {
            while (ffmpeg.av_read_frame(formatContext, packet) >= 0)
            {
                if (packet->stream_index != streamIndex)
                {
                    ffmpeg.av_packet_unref(packet);
                    continue;
                }

                ffmpeg.avcodec_send_packet(codecContext, packet).ThrowExceptionIfError();
                var frame = ffmpeg.av_frame_alloc();
                try
                {
                    while (ffmpeg.avcodec_receive_frame(codecContext, frame) == 0)
                    {
                        ffmpeg.av_buffersrc_add_frame_flags(buffersrc_ctx, frame, AV_BUFFERSRC_FLAG_KEEP_REF)
                            .ThrowExceptionIfError();
                        ffmpeg.av_frame_unref(frame);
                    }
                }
                finally
                {
                    ffmpeg.av_frame_free(&frame);
                }

                ffmpeg.av_packet_unref(packet);
            }

            // Flush the filter graph
            ffmpeg.av_buffersrc_add_frame_flags(buffersrc_ctx, null, AV_BUFFERSRC_FLAG_KEEP_REF);

            // Retrieve the processed frames
            var outDirectory = Path
                .Combine(outDir, $"{Path.GetFileNameWithoutExtension(file)}-waveform-{width}x{height}")
                .Replace("\\", "/");
            Directory.CreateDirectory(outDirectory);
            var outputFile = Path.Combine(outDirectory,
                    $"{Path.GetFileNameWithoutExtension(file)}-waveform-{width}x{height}-{color}.png")
                .Replace("\\", "/");

            outputFile = Path.GetFullPath(outputFile);

            var filteredFrame = ffmpeg.av_frame_alloc();
            try
            {
                while (ffmpeg.av_buffersink_get_frame(buffersink_ctx, filteredFrame) >= 0)
                {
                    UseTools.SaveFrameAsImage(filteredFrame, width, height, outputFile, ImageFormat.Png);
                    ffmpeg.av_frame_unref(filteredFrame);
                }
            }
            finally
            {
                ffmpeg.av_frame_free(&filteredFrame);
            }

            return outputFile;
        }
        finally
        {
            ffmpeg.avfilter_graph_free(&filterGraph);
            ffmpeg.avcodec_free_context(&codecContext);
            ffmpeg.avformat_close_input(&formatContext);
        }
    }
}

/// <summary>
///     VideoTools Class that contains useful methods about video files.
/// </summary>
public unsafe class VideoTools
{
    public const int AV_BUFFERSRC_FLAG_KEEP_REF = 1;
    public const int AVSEEK_FLAG_BACKWARD = 1;

    private static AVFormatContext* fmt_ctx;
    private static AVCodecContext* dec_ctx;
    private static AVFilterContext* buffersink_ctx;
    private static AVFilterContext* buffersrc_ctx;
    private static AVFilterGraph* filter_graph;
    public static int video_stream_index = -1;


    /// <summary>
    ///     Detects shot changes in a video, and computes jpg images for each change.
    /// </summary>
    /// <param name="inputfile"></param>
    /// <param name="threshold"></param>
    /// <param name="scale"></param>
    /// <param name="outdir"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="ApplicationException"></exception>
    public static string ShotDetection(string inputfile, string threshold, IProgress<int> progress, uint scale = 500,
        string outdir = @"./ShotDetection/JPGs/")
    {
        if (string.IsNullOrEmpty(inputfile)) throw new ArgumentException("Filename cannot be null or empty");
        if (!File.Exists(inputfile)) throw new FileNotFoundException($"The input file {inputfile} does not exist.");
        if (!double.TryParse(threshold, NumberStyles.Any, CultureInfo.InvariantCulture, out var th) || th < 0 ||
            th > 1)
            throw new ArgumentException("Threshold is not a valid percentage (0 to 1).");

        var fname = inputfile.Split('/', '\\').Last();
        var outDirFullPath = Path.Combine(outdir, $"frames_{fname}_{threshold}/");
        var filter_desc = $"select='gt(scene,{threshold})',scale={scale}:-1";
        Directory.CreateDirectory(outDirFullPath);

        fmt_ctx = ffmpeg.avformat_alloc_context();
        var frame = ffmpeg.av_frame_alloc();
        if (frame == null) throw new ApplicationException("Could not allocate frame.");
        var filt_frame = ffmpeg.av_frame_alloc();
        var packet = ffmpeg.av_packet_alloc();

        Open_input_file(inputfile);

        Init_filters(filter_desc);
        var imgCount = 0;
        var metadataPath = Path.Combine(outDirFullPath, "metadata.txt");

        int totalFrames = (int)fmt_ctx->streams[video_stream_index]->nb_frames;
        int framesProcessed = 0;

        using (var metadataFile = new StreamWriter(metadataPath, false))
        {
            while (true)
            {
                if (ffmpeg.av_read_frame(fmt_ctx, packet) < 0) break;

                if (packet->stream_index == video_stream_index)
                {
                    if (ffmpeg.avcodec_send_packet(dec_ctx, packet) < 0) break;

                    while (ffmpeg.avcodec_receive_frame(dec_ctx, frame) >= 0)
                    {
                        framesProcessed++;
                        progress?.Report((int)((double)framesProcessed / totalFrames * 100));
                        if (ffmpeg.av_buffersrc_add_frame_flags(buffersrc_ctx, frame, AV_BUFFERSRC_FLAG_KEEP_REF) <
                            0) break;

                        while (ffmpeg.av_buffersink_get_frame(buffersink_ctx, filt_frame) >= 0)
                        {
                            var imgFilename = Path.Combine(outDirFullPath, $"{++imgCount:000}.jpg");
                            UseTools.SaveFrameAsImage(filt_frame, filt_frame->width, filt_frame->height,
                                imgFilename,
                                ImageFormat.Jpeg);
                            // Écriture des métadonnées pour chaque scène détectée
                            Write_metadata(metadataFile, filt_frame, imgCount);

                            ffmpeg.av_frame_unref(filt_frame);
                        }
                    }

                    ffmpeg.av_frame_unref(frame);
                }

                ffmpeg.av_packet_unref(packet);
            }
        }

        fixed (AVFilterGraph** pfilter = &filter_graph)
        {
            ffmpeg.avfilter_graph_free(pfilter);
        }

        fixed (AVCodecContext** pdec_ctx = &dec_ctx)
        {
            ffmpeg.avcodec_free_context(pdec_ctx);
        }

        fixed (AVFormatContext** pfmt_ctx = &fmt_ctx)
        {
            ffmpeg.avformat_close_input(pfmt_ctx);
        }

        ffmpeg.av_frame_free(&frame);
        ffmpeg.av_frame_free(&filt_frame);
        ffmpeg.av_packet_free(&packet);

        return outDirFullPath;
    }

    private static void Write_metadata(StreamWriter metadataFile, AVFrame* filt_frame, int imgCount)
    {
        metadataFile.WriteLine(
            $"frame:{imgCount} pts:{filt_frame->pts} pts_time:{filt_frame->pts * ffmpeg.av_q2d(fmt_ctx->streams[video_stream_index]->time_base)}");
        var tag = ffmpeg.av_dict_get(filt_frame->metadata, "lavfi.scene_score", null, 0);
        if (tag != null)
        {
            var metadataValue = Marshal.PtrToStringAnsi((IntPtr)tag->value);
            metadataFile.WriteLine($"lavfi.scene_score={metadataValue}");
        }
    }

    private static int Open_input_file(string filename)
    {
        AVCodec* dec;
        int ret;

        fixed (AVFormatContext** pfmt_ctx = &fmt_ctx)
        {
            if ((ret = ffmpeg.avformat_open_input(pfmt_ctx, filename, null, null)) < 0)
            {
                ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot open input file");
                return ret;
            }
        }

        if ((ret = ffmpeg.avformat_find_stream_info(fmt_ctx, null)) < 0)
        {
            ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot find stream information");
            return ret;
        }

        ret = ffmpeg.av_find_best_stream(fmt_ctx, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &dec, 0);
        if (ret < 0)
        {
            ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot find a video stream in the input file");
            return ret;
        }

        video_stream_index = ret;

        dec_ctx = ffmpeg.avcodec_alloc_context3(dec);
        if (dec_ctx == null) return ffmpeg.AVERROR(ffmpeg.ENOMEM);

        // Copie les paramètres du codec du flux vers dec_ctx
        ffmpeg.avcodec_parameters_to_context(dec_ctx, fmt_ctx->streams[video_stream_index]->codecpar);

        dec_ctx->thread_count = Environment.ProcessorCount;

        if ((ret = ffmpeg.avcodec_open2(dec_ctx, dec, null)) < 0)
        {
            ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot open video decoder");
            return ret;
        }

        return ret;
    }

    private static int Init_filters(string filter_descr)
    {
        var ret = 0;
        var buffersrc = ffmpeg.avfilter_get_by_name("buffer");
        var buffersink = ffmpeg.avfilter_get_by_name("buffersink");
        var outputs = ffmpeg.avfilter_inout_alloc();
        var inputs = ffmpeg.avfilter_inout_alloc();
        var time_base = fmt_ctx->streams[video_stream_index]->time_base;
        Span<int> pix_fmts = stackalloc int[]
        {
            (int)AVPixelFormat.AV_PIX_FMT_RGB24
        };

        filter_graph = ffmpeg.avfilter_graph_alloc();

        var args = $"video_size={dec_ctx->width}x{dec_ctx->height}:pix_fmt={(int)dec_ctx->pix_fmt}:"
                   + $"time_base={time_base.num}/{time_base.den}:pixel_aspect={dec_ctx->sample_aspect_ratio.num}/{dec_ctx->sample_aspect_ratio.den}";

        fixed (AVFilterContext** pbuffersrc_ctx = &buffersrc_ctx)
        {
            ret = ffmpeg.avfilter_graph_create_filter(pbuffersrc_ctx, buffersrc, "in", args, null, filter_graph);
        }

        fixed (AVFilterContext** pbuffersink_ctx = &buffersink_ctx)
        {
            ret = ffmpeg.avfilter_graph_create_filter(pbuffersink_ctx, buffersink, "out", null, null, filter_graph);
        }

        fixed (int* pfmts = pix_fmts)
        {
            ret = ffmpeg.av_opt_set_bin(buffersink_ctx, "pix_fmts", (byte*)pfmts, pix_fmts.Length * sizeof(int),
                ffmpeg.AV_OPT_SEARCH_CHILDREN);
        }

        outputs->name = ffmpeg.av_strdup("in");
        outputs->filter_ctx = buffersrc_ctx;
        outputs->pad_idx = 0;
        outputs->next = null;

        inputs->name = ffmpeg.av_strdup("out");
        inputs->filter_ctx = buffersink_ctx;
        inputs->pad_idx = 0;
        inputs->next = null;

        ret = ffmpeg.avfilter_graph_parse_ptr(filter_graph, filter_descr, &inputs, &outputs, null);

        ret = ffmpeg.avfilter_graph_config(filter_graph, null);

        ffmpeg.avfilter_inout_free(&inputs);
        ffmpeg.avfilter_inout_free(&outputs);

        return ret;
    }

    /// <summary>
    ///     Extracts a frame from a video file at a specified time and saves it as an image.
    /// </summary>
    /// <param name="inputFileName"></param>
    /// <param name="outputFileName"></param>
    /// <param name="seconde"></param>
    /// <param name="largeur"></param>
    /// <param name="hauteur"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public static void ExtractFrameToImage(string inputFileName, string outputFileName, int second, int width,
        int height)
    {
        if (string.IsNullOrEmpty(inputFileName)) throw new ArgumentNullException(nameof(inputFileName));
        if (string.IsNullOrEmpty(outputFileName)) throw new ArgumentNullException(nameof(outputFileName));
        if (!File.Exists(inputFileName))
            throw new FileNotFoundException($"Fichier d'entrée introuvable : {inputFileName}");

        Open_input_file(inputFileName);

        var pFrame = ffmpeg.av_frame_alloc();
        var packet = ffmpeg.av_packet_alloc();

        var streamTimeBase = (double)fmt_ctx->streams[video_stream_index]->time_base.num /
                             fmt_ctx->streams[video_stream_index]->time_base.den;
        var desiredTimestamp = (long)(second / streamTimeBase);
        ffmpeg.av_seek_frame(fmt_ctx, video_stream_index, desiredTimestamp, AVSEEK_FLAG_BACKWARD)
            .ThrowExceptionIfError();

        var frameExported = false;
        while (!frameExported && ffmpeg.av_read_frame(fmt_ctx, packet) >= 0)
        {
            if (packet->stream_index != video_stream_index)
            {
                ffmpeg.av_packet_unref(packet);
                continue;
            }

            ffmpeg.avcodec_send_packet(dec_ctx, packet).ThrowExceptionIfError();

            while (ffmpeg.avcodec_receive_frame(dec_ctx, pFrame) == 0)
            {
                var ptsSeconds = pFrame->pts * streamTimeBase;
                if (ptsSeconds > second + 1) break;
                if (ptsSeconds >= second)
                {
                    UseTools.SaveFrameAsImage(pFrame, pFrame->width, pFrame->height, outputFileName,
                        ImageFormat.Bmp);
                    frameExported = true;
                    break;
                }
            }

            ffmpeg.av_packet_unref(packet);
        }

        ffmpeg.av_frame_free(&pFrame);
        ffmpeg.av_packet_free(&packet);
        fixed (AVFormatContext** pfmt_ctx = &fmt_ctx)
        {
            ffmpeg.avformat_close_input(pfmt_ctx);
        }

        fixed (AVCodecContext** pdec_ctx = &dec_ctx)
        {
            ffmpeg.avcodec_free_context(pdec_ctx);
        }
    }

    /// <summary>
    ///     Extracts time stamps from the metadata file containing informations on the shot changes.
    /// </summary>
    /// <param name="filename">Path to the file containing metadata.</param>
    /// <returns>List of floats</returns>
    public static List<float> ShotTimeStamps(string filename)
    {
        var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.NumberDecimalSeparator = ".";

        char[] separators = { '/', '\\' };
        var fname = filename.Split(separators).Last();

        // Check file
        if (fname == "") throw new NoFileException("No file input.");

        if (!File.Exists(filename)) throw new FileNotFoundException($"The input file {filename} does not exist.");

        // Check extension
        var ext = UseTools.GetExtension(fname);

        if (!Array.Exists(UseTools.data_extensions, e => e == ext))
            throw new InvalidExtensionException($"The data extension {ext} is not supported (yet?).");

        var fsr = new FileStream(filename, FileMode.Open, FileAccess.Read);
        var sr = new StreamReader(fsr);
        var timestamps = new List<float>();
        string line;
        while ((line = sr.ReadLine()) != null)
            if (line.StartsWith("frame"))
                timestamps.Add(float.Parse(line.Split(' ', '\t').Last().Split(':').Last(),
                    NumberStyles.Any, ci));

        sr.Close();
        fsr.Close();

        return timestamps;
    }
}

public class SpeechToText
{
    public unsafe static string ExtractAudioFromVideoToWAV(string videoPath, string audioOutputPath)
    {
        Debug.WriteLine("Extracting audio from video...");
        if (!File.Exists(videoPath)) throw new FileNotFoundException("The video file does not exist");

        if (!Directory.Exists("./SpeechToText/")) UseTools.CreateDirectory("./SpeechToText/");

        ffmpeg.avformat_network_init();

        AVFormatContext* formatContext = null;
        if (ffmpeg.avformat_open_input(&formatContext, videoPath, null, null) != 0)
            throw new ApplicationException("Could not open video file");

        if (ffmpeg.avformat_find_stream_info(formatContext, null) < 0)
            throw new ApplicationException("Could not get the stream info");

        int audioStreamIndex = -1;
        AVCodecParameters* codecParams = null;
        for (int i = 0; i < formatContext->nb_streams; i++)
        {
            if (formatContext->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
            {
                audioStreamIndex = i;
                codecParams = formatContext->streams[i]->codecpar;
                break;
            }
        }

        if (audioStreamIndex == -1)
            throw new ApplicationException("Could not find any audio stream");

        var codec = ffmpeg.avcodec_find_decoder(codecParams->codec_id);
        if (codec == null)
            throw new ApplicationException("Unsupported codec");
        AVCodecContext* codecContext = ffmpeg.avcodec_alloc_context3(codec);
        if (ffmpeg.avcodec_parameters_to_context(codecContext, codecParams) < 0)
            throw new ApplicationException("Failed to copy codec parameters to codec context");

        if (ffmpeg.avcodec_open2(codecContext, codec, null) < 0)
            throw new ApplicationException("Could not open codec");

        var outputPath = Path.Combine("./SpeechToText/", audioOutputPath);
        using var outputFile = new FileStream(outputPath, FileMode.Create);

        WriteWavHeader(outputFile, 1, 44100);

        AVPacket* pkt = ffmpeg.av_packet_alloc();
        AVFrame* frame = ffmpeg.av_frame_alloc();

        var swrContext = ffmpeg.swr_alloc_set_opts(null,
            (long)ffmpeg.AV_CH_LAYOUT_MONO, AVSampleFormat.AV_SAMPLE_FMT_S16, 44100,
            (long)codecContext->channel_layout, codecContext->sample_fmt, codecContext->sample_rate,
            0, null);

        ffmpeg.swr_init(swrContext);

        while (ffmpeg.av_read_frame(formatContext, pkt) >= 0)
        {
            if (pkt->stream_index == audioStreamIndex)
            {
                if (ffmpeg.avcodec_send_packet(codecContext, pkt) < 0)
                    continue;

                while (ffmpeg.avcodec_receive_frame(codecContext, frame) == 0)
                {
                    int dst_nb_samples = (int)ffmpeg.av_rescale_rnd(
                        ffmpeg.swr_get_delay(swrContext, codecContext->sample_rate) + frame->nb_samples,
                        codecContext->sample_rate, codecContext->sample_rate, AVRounding.AV_ROUND_UP);
                    int buffer_size = ffmpeg.av_samples_get_buffer_size(null, 1, dst_nb_samples,
                        AVSampleFormat.AV_SAMPLE_FMT_S16, 0);
                    byte[] buffer = new byte[buffer_size];
                    fixed (byte* out_buf = buffer)
                    {
                        byte* out_buf_ptr = out_buf;
                        int frame_count = ffmpeg.swr_convert(swrContext, &out_buf_ptr, dst_nb_samples,
                            frame->extended_data, frame->nb_samples);

                        if (frame_count > 0)
                        {
                            int actual_size = ffmpeg.av_samples_get_buffer_size(null, 1, frame_count,
                                AVSampleFormat.AV_SAMPLE_FMT_S16, 0);
                            var data = new Span<byte>(out_buf, actual_size);
                            outputFile.Write(data);
                        }
                    }

                    ffmpeg.av_frame_unref(frame);
                }
            }

            ffmpeg.av_packet_unref(pkt);
        }


        ffmpeg.av_frame_free(&frame);
        ffmpeg.av_packet_free(&pkt);
        ffmpeg.avcodec_free_context(&codecContext);
        ffmpeg.avformat_close_input(&formatContext);

        UpdateWavHeader(outputFile);

        Debug.WriteLine("Audio extracted from video.");
        return outputPath;
    }

    public unsafe static void WriteWavHeader(FileStream file, int channels, int sampleRate)
    {
        int blockAlign = channels * 2;
        int byteRate = sampleRate * blockAlign;

        // L'en-tête du fichier WAV
        file.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, 4);
        file.Write(BitConverter.GetBytes(0), 0, 4); // Taille du fichier, à mettre à jour
        file.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, 4);

        // Chunk de format
        file.Write(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, 4);
        file.Write(BitConverter.GetBytes(16), 0, 4); // Taille du sous-chunk
        file.Write(BitConverter.GetBytes((short)1), 0, 2); // Format audio (1 = PCM)
        file.Write(BitConverter.GetBytes((short)channels), 0, 2); // Nombre de canaux
        file.Write(BitConverter.GetBytes(sampleRate), 0, 4); // Fréquence d'échantillonnage
        file.Write(BitConverter.GetBytes(byteRate), 0, 4); // ByteRate
        file.Write(BitConverter.GetBytes((short)blockAlign), 0, 2); // BlockAlign
        file.Write(BitConverter.GetBytes((short)16), 0, 2); // Bits par échantillon

        // Chunk de données
        file.Write(System.Text.Encoding.UTF8.GetBytes("data"), 0, 4);
        file.Write(BitConverter.GetBytes(0), 0, 4); // Taille des données, à mettre à jour
    }

    public unsafe static void UpdateWavHeader(FileStream file)
    {
        long fileSize = file.Length;

        // Mettre à jour la taille du fichier dans l'en-tête RIFF
        file.Seek(4, SeekOrigin.Begin);
        file.Write(BitConverter.GetBytes((uint)(fileSize - 8)), 0, 4);

        // Mettre à jour la taille des données dans le chunk de données
        file.Seek(40, SeekOrigin.Begin);
        file.Write(BitConverter.GetBytes((uint)(fileSize - 44)), 0, 4);
    }

    public unsafe static string ExtractAudioFromVideoToMP3(string videoPath, string audioOutputPath)
    {
        Debug.WriteLine("Extraction de l'audio de la vidéo...");
        if (!Directory.Exists("./SpeechToText/")) Directory.CreateDirectory("./SpeechToText/");

        string outputPath = Path.Combine("./SpeechToText/", audioOutputPath);
        using FileStream outputFile = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

        AVFormatContext* formatContext = null;
        if (ffmpeg.avformat_open_input(&formatContext, videoPath, null, null) != 0)
        {
            Debug.WriteLine("Failed to open input file.");
            return null;
        }

        if (ffmpeg.avformat_find_stream_info(formatContext, null) < 0)
        {
            Debug.WriteLine("Failed to find stream information.");
            return null;
        }

        AVCodec* codec = null;
        AVCodecContext* codecContext = null;
        AVStream* stream = null;
        int audioStreamIndex = -1;
        for (int i = 0; i < formatContext->nb_streams; i++)
        {
            if (formatContext->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
            {
                audioStreamIndex = i;
                stream = formatContext->streams[i];
                codec = ffmpeg.avcodec_find_decoder(stream->codecpar->codec_id);
                if (codec == null)
                {
                    Debug.WriteLine("Failed to find the codec.");
                    continue;
                }

                codecContext = ffmpeg.avcodec_alloc_context3(codec);
                ffmpeg.avcodec_parameters_to_context(codecContext, stream->codecpar);
                if (ffmpeg.avcodec_open2(codecContext, codec, null) < 0)
                {
                    Debug.WriteLine("Failed to open codec.");
                    continue;
                }

                break;
            }
        }

        if (audioStreamIndex == -1 || codecContext == null)
        {
            Debug.WriteLine("No suitable audio stream found.");
            return null;
        }

        Debug.WriteLine(
            $"Selected audio stream index: {audioStreamIndex}, sample rate: {codecContext->sample_rate}, channels: {codecContext->channels}");

        AVCodecContext* mp3Context = InitializeMp3Encoder(codecContext->sample_rate, codecContext->channels);
        if (mp3Context == null)
        {
            Debug.WriteLine("Failed to initialize MP3 encoder.");
            return null;
        }

        // Initialize resampler
        SwrContext* swrCtx = InitializeResampler(codecContext, mp3Context);
        if (swrCtx == null)
        {
            Debug.WriteLine("Failed to initialize resampler.");
            return null;
        }

        AVPacket* pkt = ffmpeg.av_packet_alloc();
        AVFrame* frame = ffmpeg.av_frame_alloc();


        int retryAllocation = 3;

        while (ffmpeg.av_read_frame(formatContext, pkt) >= 0)
        {
            if (pkt->stream_index == audioStreamIndex)
            {
                ffmpeg.avcodec_send_packet(codecContext, pkt);
                while (ffmpeg.avcodec_receive_frame(codecContext, frame) == 0)
                {
                    AVFrame* resampledFrame = ffmpeg.av_frame_alloc();

                    resampledFrame->channel_layout = (ulong)ffmpeg.av_get_default_channel_layout(mp3Context->channels);
                    resampledFrame->sample_rate = mp3Context->sample_rate;
                    resampledFrame->format = (int)mp3Context->sample_fmt;

                    // Calculate the number of samples for resampled output
                    int dst_nb_samples = (int)ffmpeg.av_rescale_rnd(
                        ffmpeg.swr_get_delay(swrCtx, codecContext->sample_rate) + frame->nb_samples,
                        mp3Context->sample_rate, codecContext->sample_rate, AVRounding.AV_ROUND_UP);
                    resampledFrame->nb_samples = dst_nb_samples;

                    if (ffmpeg.av_frame_get_buffer(resampledFrame, 0) < 0)
                    {
                        Debug.WriteLine("Failed to allocate buffer for resampled frame.");
                        ffmpeg.av_frame_free(&resampledFrame);
                        break;
                    }

                    if (ffmpeg.swr_convert(swrCtx, (byte**)&resampledFrame->data, dst_nb_samples, (byte**)frame->extended_data, frame->nb_samples) < 0)
                    {
                        Debug.WriteLine("Error while converting.");
                        ffmpeg.av_frame_free(&resampledFrame);
                        break;
                    }

                    resampledFrame->pts = frame->pts;
                    resampledFrame->nb_samples = mp3Context->frame_size;

                    AVPacket* enc_pkt = EncodeFrameToMp3(mp3Context, resampledFrame);
                    if (enc_pkt != null)
                    {
                        Debug.WriteLine($"Encoded packet: pts={enc_pkt->pts}, dts={enc_pkt->dts}, duration={enc_pkt->duration}, size={enc_pkt->size}");
                        WritePacketToFile(outputFile, enc_pkt);
                        ffmpeg.av_packet_free(&enc_pkt);
                    }
                    ffmpeg.av_frame_free(&resampledFrame);
                }
                ffmpeg.av_packet_unref(pkt);
            }
        }

        // Ensure to handle flushing the encoder for any remaining frames
        ffmpeg.avcodec_send_frame(mp3Context, null);
        while (ffmpeg.avcodec_receive_packet(mp3Context, pkt) == 0)
        {
            WritePacketToFile(outputFile, pkt);
            ffmpeg.av_packet_free(&pkt);
        }

        ffmpeg.av_frame_free(&frame);
        ffmpeg.avcodec_free_context(&codecContext);
        ffmpeg.avcodec_free_context(&mp3Context);
        ffmpeg.avformat_close_input(&formatContext);

        Debug.WriteLine("Audio extracted from video.");

        return outputPath;
    }

    public unsafe static AVCodecContext* InitializeMp3Encoder(int sampleRate, int channels)
    {
        AVCodec* codec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MP3);
        if (codec == null)
        {
            Debug.WriteLine("MP3 codec not found.");
            return null;
        }

        AVCodecContext* codecContext = ffmpeg.avcodec_alloc_context3(codec);
        if (codecContext == null)
        {
            Debug.WriteLine("Failed to allocate the MP3 codec context.");
            return null;
        }

        codecContext->bit_rate = 128000;
        codecContext->sample_rate = sampleRate;
        codecContext->sample_fmt = AVSampleFormat.AV_SAMPLE_FMT_S16P;
        codecContext->channel_layout = (ulong)ffmpeg.av_get_default_channel_layout(channels);
        codecContext->channels = channels;


        if (ffmpeg.avcodec_open2(codecContext, codec, null) < 0)
        {
            Debug.WriteLine("Could not open the MP3 codec.");
            ffmpeg.avcodec_free_context(&codecContext);
            return null;
        }

        return codecContext;
    }

    public unsafe static AVPacket* EncodeFrameToMp3(AVCodecContext* codecContext, AVFrame* frame)
    {

        AVPacket* packet = ffmpeg.av_packet_alloc();
        if (packet == null)
        {
            Debug.WriteLine("Failed to allocate packet.");
            return null;
        }

        if (ffmpeg.av_frame_get_buffer(frame, 0) < 0)
        {
            Debug.WriteLine("Failed to allocate buffer for frame.");
            ffmpeg.av_packet_free(&packet);
            return null;
        }

        int error = ffmpeg.avcodec_send_frame(codecContext, frame);
        if (error < 0)
        {
            Debug.WriteLine($"Error sending a frame for encoding: {error}");
            ffmpeg.av_packet_free(&packet);
            return null;
        }

        error = ffmpeg.avcodec_receive_packet(codecContext, packet);
        if (error < 0)
        {
            if (error == ffmpeg.AVERROR(ffmpeg.EAGAIN) || error == ffmpeg.AVERROR_EOF)
            {
                ffmpeg.av_packet_free(&packet);
                return null;
            }

            Debug.WriteLine($"Error receiving encoded packet: {error}");
            ffmpeg.av_packet_free(&packet);
            return null;
        }

        return packet;
    }

    public unsafe static void WritePacketToFile(FileStream outputFile, AVPacket* packet)
    {
        if (packet == null || outputFile == null)
        {
            Debug.WriteLine("Invalid packet or output file stream.");
            return;
        }

        try
        {
            int dataLength = packet->size;
            byte[] buffer = new byte[dataLength];
            System.Runtime.InteropServices.Marshal.Copy((IntPtr)packet->data, buffer, 0, dataLength);
            outputFile.Write(buffer, 0, dataLength);
            outputFile.Flush();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to write packet to file: {ex.Message}");
        }
    }

    public unsafe static SwrContext* InitializeResampler(AVCodecContext* decCtx, AVCodecContext* encCtx)
    {
        SwrContext* swr = ffmpeg.swr_alloc();
        if (swr == null) return null;

        // Set the options for the resampler
        ffmpeg.av_opt_set_int(swr, "in_channel_layout", (long)decCtx->channel_layout, 0);
        ffmpeg.av_opt_set_int(swr, "out_channel_layout", (long)encCtx->channel_layout, 0);
        ffmpeg.av_opt_set_int(swr, "in_sample_rate", decCtx->sample_rate, 0);
        ffmpeg.av_opt_set_int(swr, "out_sample_rate", encCtx->sample_rate, 0);
        ffmpeg.av_opt_set_sample_fmt(swr, "in_sample_fmt", decCtx->sample_fmt, 0);
        ffmpeg.av_opt_set_sample_fmt(swr, "out_sample_fmt", encCtx->sample_fmt, 0);

        if (ffmpeg.swr_init(swr) < 0)
        {
            ffmpeg.swr_free(&swr);
            return null;
        }

        return swr;
    }

    public static async Task<string> TranscribeAsync(string gcsUri, string languageCode, IProgress<int> progress)
    {
        var speechClient = SpeechClient.Create();
        var config = new RecognitionConfig
        {
            Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
            SampleRateHertz = 44100,
            LanguageCode = languageCode,
            EnableAutomaticPunctuation = true,
            EnableWordTimeOffsets = true
        };
        var audio = RecognitionAudio.FromStorageUri(gcsUri);

        var operation = await speechClient.LongRunningRecognizeAsync(config, audio);
        var completedOperation = await operation.PollUntilCompletedAsync();

        if (completedOperation.IsFaulted)
        {
            Console.WriteLine($"Error: {completedOperation.Exception}");
            return null;
        }

        var response = completedOperation.Result;
        var transcript = new StringBuilder();

        foreach (var result in response.Results)
        {
            foreach (var alternative in result.Alternatives)
            {
                foreach (var wordInfo in alternative.Words)
                {
                    transcript.AppendLine(
                        $"{wordInfo.Word} [{wordInfo.StartTime.ToTimeSpan()} - {wordInfo.EndTime.ToTimeSpan()}]");
                }

                transcript.AppendLine();
            }
        }

        return transcript.ToString();
    }

    public static async Task TranscribeAndSaveSubtitles(string gcsUri, string languageCode, string outputPath,
        IProgress<int> progress)
    {
        var speechClient = SpeechClient.Create();
        var config = new RecognitionConfig
        {
            Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
            SampleRateHertz = 44100,
            LanguageCode = languageCode,
            EnableAutomaticPunctuation = true,
            EnableWordTimeOffsets = true
        };
        var audio = RecognitionAudio.FromStorageUri(gcsUri);

        try
        {
            var operation = await speechClient.LongRunningRecognizeAsync(config, audio);


            var progressTask = SimulateProgress(progress);


            var completedOperation = await operation.PollUntilCompletedAsync();
            progressTask.Cancel();

            if (completedOperation.IsFaulted)
            {
                Debug.WriteLine($"Error: {completedOperation.Exception}");
                throw new InvalidOperationException("Transcription failed to complete.", completedOperation.Exception);
            }

            var response = completedOperation.Result;
            var subtitleBuilder = new StringBuilder();
            int index = 1;

            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    foreach (var wordInfo in alternative.Words)
                    {
                        subtitleBuilder.AppendFormat("{0}\n", index++);
                        subtitleBuilder.AppendFormat("{0} --> {1}\n",
                            wordInfo.StartTime.ToTimeSpan().ToString(@"hh\:mm\:ss\,fff"),
                            wordInfo.EndTime.ToTimeSpan().ToString(@"hh\:mm\:ss\,fff"));
                        subtitleBuilder.AppendLine(wordInfo.Word);
                        subtitleBuilder.AppendLine(); // Empty line to separate entries
                    }
                }
            }

            SaveSubtitles(subtitleBuilder.ToString(), outputPath);
            progress?.Report(100);
            Debug.WriteLine("Subtitles saved successfully to " + outputPath);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during transcription or saving subtitles: {ex}");
            throw;
        }
    }

    private static CancellationTokenSource SimulateProgress(IProgress<int> progress)
    {
        var cts = new CancellationTokenSource();
        Task.Run(async () =>
        {
            int fakeProgress = 0;
            while (!cts.Token.IsCancellationRequested && fakeProgress < 100)
            {
                progress?.Report(fakeProgress);
                await Task.Delay(2000);
                fakeProgress += 1;
            }
        }, cts.Token);
        return cts;
    }

    public static void SaveSubtitles(string subtitleText, string outputPath)
    {
        try
        {
            File.WriteAllText(outputPath, subtitleText, Encoding.UTF8);
            Debug.WriteLine("Subtitles saved successfully to " + outputPath);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error saving subtitles: " + ex.Message);
        }
    }
}

public class CloudStorageService
{
    private readonly StorageClient storageClient;
    private readonly string bucketName;

    public CloudStorageService(string bucketName)
    {
        this.bucketName = bucketName;
        storageClient = StorageClient.Create();
    }

    public async Task<string> UploadFileAsync(string localPath, string objectName = null)
    {
        objectName ??= Path.GetFileName(localPath);

        using var fileStream = File.OpenRead(localPath);
        var storageObject = await storageClient.UploadObjectAsync(bucketName, objectName, null, fileStream);
        Console.WriteLine($"File uploaded to {storageObject.MediaLink}");

        return $"gs://{bucketName}/{objectName}";
    }
}

/// <summary>
///     Amazon SpeechToText class.
/// </summary>
// public class SpeechToText : IDisposable
// {
//     /// <summary>
//     ///     Initialization of the SpeechToText class object.
//     /// </summary>
//     /// <param name="regionEndpoint">(Optionnal) Default EUWest3.</param>
//     public SpeechToText(RegionEndpoint regionEndpoint = null)
//     {
//         RegionEndpoint = regionEndpoint ?? RegionEndpoint.EUWest3;
//         //todo add region endpoint for AWS Face
//         S3Client = new AmazonS3Client(RegionEndpoint);
//         TranscribeClient = new AmazonTranscribeServiceClient(RegionEndpoint);
//     }
//
//     private static RegionEndpoint RegionEndpoint { get; set; }
//     private static AmazonTranscribeServiceClient TranscribeClient { get; set; }
//     private static AmazonS3Client S3Client { get; set; }
//
//     /// <summary>
//     ///     Dispose.
//     /// </summary>
//     public void Dispose()
//     {
//         //TODO remember to call
//         S3Client.Dispose();
//         TranscribeClient.Dispose();
//         //TODO dispose for faceClient
//     }
//
//     /// <summary>
//     ///     Speech detection using Vosk model and demo
//     /// </summary>
//     /// <param name="model"></param>
//     /// <param name="filePath"></param>
//     public static /*void*/ string DemoBytes(Model model, string filePath)
//     {
//         // Demo byte buffer
//         var rec = new VoskRecognizer(model, 16000.0f);
//         rec.SetMaxAlternatives(0);
//         rec.SetWords(true);
//         using (Stream source = File.OpenRead(filePath))
//         {
//             var buffer = new byte[4096];
//             int bytesRead;
//             while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
//                 if (rec.AcceptWaveform(buffer, bytesRead))
//                     Console.WriteLine(rec.Result());
//                 else
//                     Console.WriteLine(rec.PartialResult());
//         }
//
//         /*Console.WriteLine(rec.FinalResult());*/
//         return rec.FinalResult();
//     }
//
//     /// <summary>
//     ///     Should get voice detection with filename and model = French
//     /// </summary>
//     /// <param name="filename"></param>
//     /// <returns></returns>
//     public static /*async*/ string TranscribeInputVosk(string filename)
//     {
//         Vosk.Vosk.SetLogLevel(0);
//
//         var Model = new Model(@"C:\Travail\Media_Tools\Sources\vosk-model-fr-0.22");
//         return DemoBytes(Model, filename);
//     }
//
//     /// <summary>
//     ///     Asynchronous Transcription of an audio with Amazon API.
//     /// </summary>
//     /// <param name="fileName">
//     ///     Path to the video or audio file to transcribe. Must be in format FLAC, MP3, MP4, Ogg, WebM, AMR
//     ///     or WAV
//     /// </param>
//     /// <param name="targetLanguageCode">(Optionnal) Language of the audio in the format "en-US". Default "fr-FR"</param>
//     /// <param name="bucketName">(Optionnal) Name of the bucket if specified. Default "".</param>
//     /// <param name="outdir">(Optionnal) Path to the folder where to save the results. Default "./AmazonSpeechToText/".</param>
//     /// <param name="diarization">(Optionnal) True if the API should separate the different speakers. Default true.</param>
//     /// <param name="MaxSpeakers">(Optionnal) Maximum number of speakers to recognize in the audio. Default 2.</param>
//     /// <returns></returns>
//     public static async Task<ResponseJSON[]?> TranscribeInputFile(string fileName,
//         string targetLanguageCode = "fr-FR",
//         string? bucketName = null,
//         string outdir = "./AmazonSpeechToText/",
//         bool diarization = true,
//         int MaxSpeakers = 2,
//         bool transcribe = false)
//     {
//         var name = UseTools.GetName(fileName);
//
//         if (diarization) name = name + "_diarization_" + MaxSpeakers;
//
//         UseTools.CreateDirectory(outdir + name + "/");
//
//         // Initialization
//         ResponseJSON? resp = new();
//         var result = "";
//         PutBucketResponse putBucketResponse = new();
//         putBucketResponse.HttpStatusCode = HttpStatusCode.OK;
//
//         Console.WriteLine("Searching for reponse...");
//         // If the response has already been saved, we extract it
//         if (File.Exists(outdir + name + "/response.json"))
//         {
//             FileStream fsr = new(outdir + name + "/response.json", FileMode.Open, FileAccess.Read);
//             StreamReader sr = new(fsr);
//             result = sr.ReadLine();
//             sr.Close();
//             fsr.Close();
//             Console.WriteLine($"   {outdir}{name}/response.json already exists and is open.");
//         }
//         // Otherwise we ask the Amazon server.
//         else
//         {
//             // We get the name of the bucket in the Amazon Storage, or we create it if it does not exist.
//             if (bucketName == null)
//                 try
//                 {
//                     FileStream fsr = new(outdir + name + "/bucketName.txt", FileMode.Open, FileAccess.Read);
//                     StreamReader sr = new(fsr);
//                     bucketName = sr.ReadLine();
//                     sr.Close();
//                     fsr.Close();
//                     if (bucketName == null) throw new ArgumentNullException("bucketName");
//
//                     Console.WriteLine($"   BucketName extracted from {outdir}{name}/bucketName.txt");
//                 }
//                 catch (Exception)
//                 {
//                     bucketName = "transcribe-" + Guid.NewGuid();
//                     FileStream fsb = new(outdir + name + "/bucketName.txt", FileMode.Create, FileAccess.Write);
//                     StreamWriter sb = new(fsb);
//                     sb.WriteLine(bucketName);
//                     sb.Close();
//                     fsb.Close();
//
//                     putBucketResponse = await CreateBucket(bucketName);
//                     Console.WriteLine($"   BucketName created and saved in {outdir}{name}/bucketName.txt");
//                 }
//
//             var keyName = $"transcribe-job-{bucketName}.json";
//
//             // We check if the response is already stored in the Amazon storage
//             try
//             {
//                 result = await GetFileFromS3(keyName, bucketName);
//                 Console.WriteLine("   Json response already exists on the S3 storage.");
//                 Console.WriteLine($"   {outdir}{name}/response.json has been created and is open.");
//             }
//             // Otherwise we launch the transcription.
//             catch (Exception)
//             {
//                 if (transcribe)
//                 {
//                     if (putBucketResponse.HttpStatusCode == HttpStatusCode.OK)
//                     {
//                         var uploadInputFileToS3 = await UploadInputFileToS3(fileName, bucketName);
//                         if (uploadInputFileToS3.HttpStatusCode == HttpStatusCode.OK)
//                         {
//                             Console.WriteLine("   Transcribing...");
//                             result = await TranscribeInputFile(fileName, bucketName, targetLanguageCode,
//                                 diarization = diarization, MaxSpeakers = MaxSpeakers);
//
//                             //todo
//                             //todo delete bucket
//                         }
//                         else
//                         {
//                             Console.WriteLine(
//                                 $"\nFail to transcribe {fileName} because cannot upload {name} to {bucketName}",
//                                 uploadInputFileToS3);
//                             Environment.Exit(0);
//                         }
//                     }
//                     else
//                     {
//                         Console.WriteLine(
//                             $"\nFail to transcribe {fileName} because cannot create bucket {bucketName}",
//                             putBucketResponse);
//                         Environment.Exit(0);
//                     }
//                 }
//
//                 else
//                 {
//                     Console.WriteLine(
//                         "\nFile not already transcribed on Amazon Server, and option \"Transcribe\" not checked. Aborting.");
//                     Environment.Exit(0);
//                 }
//             }
//
//             var fsw = new FileStream(outdir + name + "/response.json", FileMode.OpenOrCreate,
//                 FileAccess.Write);
//             var sw = new StreamWriter(fsw);
//             sw.WriteLine(result);
//             sw.Close();
//             fsw.Close();
//         }
//
//         Console.WriteLine("\nTranscription:\n--------------------------------------");
//         resp = DeserializeJSON(result);
//
//         if (resp == null) return null;
//
//         Console.WriteLine(resp.results.transcripts[0].transcript);
//         Console.WriteLine("--------------------------------------\n");
//
//         resp.filename = name;
//         resp.language = targetLanguageCode;
//
//         ResponseJSON[] all_resps = { resp };
//
//         if (diarization)
//         {
//             var diar_resps = DiarizationResponse(resp);
//             var speaker = 0;
//             foreach (var res in diar_resps)
//             {
//                 // JSON
//                 FileStream fsw_d = new(outdir + name + $"/speaker_{speaker}.json", FileMode.OpenOrCreate,
//                     FileAccess.Write);
//                 StreamWriter sw_d = new(fsw_d);
//                 sw_d.WriteLine(SerializeJSON(res));
//                 sw_d.Close();
//                 fsw_d.Close();
//                 _ = all_resps.Append(res);
//                 speaker++;
//             }
//         }
//
//         return all_resps;
//     }
//
//     private static async Task<string> TranscribeInputFile(string fileName, string bucketName,
//         string targetLanguageCode, bool diarization = true, int MaxSpeakers = 2)
//     {
//         var objectName = Path.GetFileName(fileName);
//
//         var media = new Media
//         {
//             MediaFileUri = $"https://s3.{RegionEndpoint.SystemName}.amazonaws.com/{bucketName}/{objectName}"
//         };
//
//         var transcriptionJobName = $"transcribe-job-{bucketName}";
//         var transcriptionJobRequest = new StartTranscriptionJobRequest
//         {
//             LanguageCode = targetLanguageCode,
//             Media = media,
//             MediaFormat = MediaFormat.Wav,
//             TranscriptionJobName = transcriptionJobName,
//             OutputBucketName = bucketName,
//             Settings = new Settings
//             {
//                 ShowSpeakerLabels = diarization,
//                 MaxSpeakerLabels = MaxSpeakers
//             }
//         };
//
//         var startTranscriptionJobResponse =
//             await TranscribeClient.StartTranscriptionJobAsync(transcriptionJobRequest);
//         if (startTranscriptionJobResponse.HttpStatusCode == HttpStatusCode.OK)
//             return await WaitForTranscriptionJob(startTranscriptionJobResponse.TranscriptionJob, bucketName);
//         //todo
//         throw new NotImplementedException();
//     }
//
//     private static async Task<string> WaitForTranscriptionJob(TranscriptionJob transcriptionJob,
//         string bucketName, int delayTime = 16000)
//     {
//         var transcriptionJobTranscriptionJobStatus = transcriptionJob.TranscriptionJobStatus;
//         Console.WriteLine($"   transcriptionJobTranscriptionJobStatus={transcriptionJobTranscriptionJobStatus}");
//         if (transcriptionJobTranscriptionJobStatus ==
//             TranscriptionJobStatus.COMPLETED)
//         {
//             var keyName = $"{transcriptionJob.TranscriptionJobName}.json";
//             Console.WriteLine($"   Downloading {keyName}");
//             var result = await GetFileFromS3(keyName, bucketName);
//             return result;
//             /*using var stringReader = new StringReader(result);
//             using var jsonTextReader = new JsonTextReader(stringReader);*/
//         }
//
//         if (transcriptionJobTranscriptionJobStatus == TranscriptionJobStatus.FAILED)
//             //TODO
//             throw new NotImplementedException();
//
//         await Task.Delay(delayTime);
//         var getTranscriptionJobResponse = await TranscribeClient.GetTranscriptionJobAsync(
//             new GetTranscriptionJobRequest
//             {
//                 TranscriptionJobName = transcriptionJob.TranscriptionJobName
//             });
//         return await WaitForTranscriptionJob(getTranscriptionJobResponse.TranscriptionJob, bucketName,
//             delayTime * 2);
//     }
//
//     public static async Task<PutBucketResponse> CreateBucket(string bucketName)
//     {
//         var putBucketRequest = new PutBucketRequest
//         {
//             BucketName = bucketName
//         };
//         return await S3Client.PutBucketAsync(putBucketRequest);
//     }
//
//     public static async Task<PutObjectResponse> UploadInputFileToS3(string fileName,
//         string bucketName)
//     {
//         var objectName = Path.GetFileName(fileName);
//         var extension = UseTools.GetExtension(fileName);
//
//         var putObjectRequest = new PutObjectRequest
//         {
//             BucketName = bucketName,
//             Key = objectName,
//             ContentType = "audio/" + extension,
//             FilePath = fileName
//         };
//
//         return await S3Client.PutObjectAsync(putObjectRequest);
//     }
//
//     public static async Task<string> GetFileFromS3(string keyName,
//         string bucketName)
//     {
//         var request = new GetObjectRequest
//         {
//             BucketName = bucketName,
//             Key = keyName
//         };
//         using var response = await S3Client.GetObjectAsync(request);
//         using var responseStream = response.ResponseStream;
//         using var reader = new StreamReader(responseStream);
//         /*string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
//         string contentType = response.Headers["Content-Type"];
//         Console.WriteLine("Object metadata, Title: {0}", title);
//         Console.WriteLine("Content type: {0}", contentType);*/
//
//         return await reader.ReadToEndAsync(); // Now you process the response body.
//     }
//
//     public static ResponseJSON? DeserializeJSON(string? json_string)
//     {
//         if (json_string == null) return null;
//
//         return JsonConvert.DeserializeObject<ResponseJSON>(json_string);
//     }
//
//     public static string SerializeJSON(ResponseJSON resp)
//     {
//         return JsonConvert.SerializeObject(resp);
//     }
//
//     private static uint LabelToIndex(string label)
//     {
//         return uint.Parse(label[^1].ToString());
//     }
//
//     public static ResponseJSON[] DiarizationResponse(ResponseJSON response)
//     {
//         var nb_speakers = response.results.speaker_labels.speakers;
//
//         var diar_responses = new ResponseJSON[nb_speakers];
//         for (var i = 0; i < nb_speakers; i++)
//             diar_responses[i] = new ResponseJSON
//             {
//                 results = new ResponseJSON.Results
//                 {
//                     items = new List<ResponseJSON.Results.Items>()
//                 }
//             };
//
//         foreach (var segment in response.results.speaker_labels.segments)
//         {
//             var speaker = LabelToIndex(segment.speaker_label);
//             foreach (var item in segment.items)
//             {
//                 var item_found =
//                     response.results.items.Find(x => x.start_time == item.start_time);
//                 diar_responses[speaker].results.items.Add(item_found);
//             }
//         }
//
//         return diar_responses;
//     }
//
//     /// <summary>
//     ///     Dummy converter from JSON to SRT.
//     /// </summary>
//     /// <param name="resp">Response of the API converted into a C# class.</param>
//     /// <param name="name">Name of the file.</param>
//     /// <param name="outdir"></param>
//     public static void CreateSRTFromJSON(ResponseJSON[] resps,
//         string filename,
//         string shot_metadata_file,
//         string outdir = "./AmazonSpeechToText/",
//         uint CPS = 13,
//         uint CPL = 35,
//         uint MaxLines = 2,
//         double GapMin = 0.17,
//         double TimeMin = 1,
//         double TimeMax = 5)
//     {
//         // Shot time stamps
//         var ShotTimeStamps = new List<float>();
//         if (shot_metadata_file != @"")
//         {
//             ShotTimeStamps = VideoTools.ShotTimeStamps(shot_metadata_file);
//             Console.WriteLine($"Shot Time Stamps extracted from {shot_metadata_file}");
//         }
//         else
//         {
//             Console.WriteLine("No shot changes time stamps.");
//         }
//
//         // Information about transcripts, words and time stamps.
//         var resp_global = resps[0];
//         // Information about transcripts, words and time stamps of each speaker.
//         var resp_speaker = new ArraySegment<ResponseJSON>(resps, 1, resps.Length - 1);
//
//
//         char[] file_separators = { '/', '\\' };
//         var name = filename.Split(file_separators).Last();
//         var outname = outdir + name + "/subtitles.srt";
//
//         UseTools.CreateDirectory(outdir);
//         UseTools.CreateDirectory(outdir + name + "/");
//
//         var fileStream = new FileStream(outname, FileMode.Create, FileAccess.Write);
//         var sw = new StreamWriter(fileStream);
//
//         var nb_words = resp_global.results.items.Count();
//         var i = 1;
//         var cur_item = resp_global.results.items[0];
//         var cur_word = cur_item.alternatives[0].content;
//         var cur_time_start = TimeSpan.FromSeconds(cur_item.start_time);
//         var cur_time_end = TimeSpan.FromSeconds(cur_item.end_time);
//
//         var phrase = cur_word + " ";
//         var phrase_start = cur_time_start;
//         var phrase_end = cur_time_end;
//         var phrase_length = phrase_end - phrase_start;
//
//         var new_phrase = false;
//
//         var num_st = 1;
//         var sub_duration = TimeSpan.FromSeconds(6);
//         var speaker = 0;
//
//         while (i < nb_words)
//         {
//             cur_item = resp_global.results.items[i];
//             cur_word = cur_item.alternatives[0].content;
//             cur_time_start = TimeSpan.FromSeconds(cur_item.start_time);
//             cur_time_end = TimeSpan.FromSeconds(cur_item.end_time);
//
//             if (cur_item.type == "punctuation")
//             {
//                 phrase = phrase.Remove(phrase.Length - 1) + cur_word + " ";
//             }
//
//             else
//             {
//                 if (phrase_length > sub_duration - TimeSpan.FromSeconds(1)) new_phrase = true;
//
//                 if (new_phrase || i == nb_words - 1)
//                 {
//                     if (phrase != "")
//                     {
//                         sw.WriteLine(num_st);
//                         var format = @"hh\:mm\:ss\,fff";
//                         sw.WriteLine("{0:" + format + "} --> {1:" + format + "}\n{2}\n", phrase_start, phrase_end,
//                             phrase);
//                         Console.WriteLine("   {0} - {1}, speaker {2}: {3}", phrase_start.ToString(format),
//                             phrase_end.ToString(format), speaker, phrase);
//
//                         num_st++;
//                     }
//
//                     phrase = "";
//                     phrase_start = cur_time_start;
//                     new_phrase = false;
//                 }
//
//                 phrase = phrase + cur_word + " ";
//                 phrase_end = cur_time_end;
//                 phrase_length = phrase_end - phrase_start;
//             }
//
//             i++;
//         }
//
//         sw.Close();
//         fileStream.Close();
//     }
//
//     /// <summary>
//     ///     Converter from JSON to XML.
//     /// </summary>
//     /// <param name="resp">Response of the API converted into a C# class.</param>
//     /// <param name="name">Name of the file.</param>
//     /// <param name="outdir"></param>
//     public static void CreateXMLFromJSON(string path_to_json,
//         string shot_folder = "",
//         string outdir = "./AmazonSpeechToText/")
//     {
//         Console.WriteLine("Creating XML file...");
//
//         // Check the file
//         UseTools.CheckFileAndExt(path_to_json, new[] { "json" });
//
//         // Deserialize Json file
//         var fsr = new FileStream(path_to_json, FileMode.Open, FileAccess.Read);
//         var sr = new StreamReader(fsr);
//         var json_string = "";
//         string line;
//         while ((line = sr.ReadLine()) != null) json_string = json_string + line;
//
//         sr.Close();
//         fsr.Close();
//         var resp = DeserializeJSON(json_string);
//         var nb_words = resp.results.items.Count();
//         var i = 0;
//
//
//         var name = "";
//         try
//         {
//             name = resp.filename;
//         }
//         catch (Exception)
//         {
//             name = "Oui";
//         }
//
//         var language = "";
//         try
//         {
//             name = resp.language;
//         }
//         catch (Exception)
//         {
//             name = "fr-FR";
//         }
//
//         // Creating outdir
//         char[] file_separators = { '/', '\\' };
//         name = name.Split(file_separators).Last(); // Should not change anything
//         var outname = outdir + name + "/subtitles.xml";
//         UseTools.CreateDirectory(outdir + name + "/");
//
//
//         // Shot time stamps
//         // The exceptions about the file are thrown in the ShotTimeStamps method
//         var shot_metadata_file = shot_folder + "metadata.txt";
//         UseTools.CheckFileAndExt(shot_metadata_file);
//
//         var ShotTimeStamps = VideoTools.ShotTimeStamps(shot_metadata_file); // new List<float>();
//         var nb_shots = ShotTimeStamps.Count;
//         var j = 0;
//
//
//         var fileStream = new FileStream(outname, FileMode.Create, FileAccess.Write);
//         var sw = new StreamWriter(fileStream);
//
//         sw.WriteLine("<Subtitles>");
//         {
//             // Language
//             sw.WriteLine("<Language>" + language + "</Language>");
//
//             // Words
//             sw.WriteLine("<Words>");
//             {
//                 while (i < nb_words)
//                 {
//                     var word = resp.results.items[i].alternatives[0].content;
//                     var start_time = TimeSpan.FromSeconds(resp.results.items[i].start_time);
//                     sw.WriteLine("<Word>");
//                     {
//                         sw.WriteLine("<Content>" + word + "</Content>");
//                         sw.WriteLine("<TimeStamp>" + start_time + "</TimeStamp>");
//                     }
//                     sw.WriteLine("</Word>");
//                     i++;
//                 }
//             }
//             sw.WriteLine("</Words>");
//
//             // Shot changes
//             sw.WriteLine("<Shots>");
//             {
//                 while (j < nb_shots)
//                 {
//                     var file = File.ReadAllBytes(shot_folder + $"{j + 1:000}.jpg");
//                     var img = Convert.ToBase64String(file);
//                     sw.WriteLine("<Shot>");
//                     {
//                         sw.WriteLine("<Img>" + img + "</Img>");
//                         sw.WriteLine("<TimeStamp>" + ShotTimeStamps[j] + "</TimeStamp>");
//                     }
//                     sw.WriteLine("</Shot>");
//                     j++;
//                 }
//             }
//             sw.WriteLine("</Shots>");
//         }
//         sw.WriteLine("</Subtitles>");
//
//         sw.Close();
//         fileStream.Close();
//
//         Console.WriteLine($"   XML file created and saved in {outname}\n");
//     }
//
//     public class ResponseJSON
//     {
//         public string jobName { get; set; }
//         public string accountId { get; set; }
//         public Results results { get; set; }
//         public string status { get; set; }
//         public string filename { get; set; }
//         public string language { get; set; }
//
//         public class Results
//         {
//             public List<Transcripts> transcripts { get; set; }
//             public List<Items> items { get; set; }
//             public Speakers speaker_labels { get; set; }
//
//             public class Transcripts
//             {
//                 public string transcript { get; set; }
//             }
//
//             public class Items
//             {
//                 public float start_time { get; set; }
//                 public float end_time { get; set; }
//                 public IList<Alternatives> alternatives { get; set; }
//                 public string type { get; set; }
//
//                 public class Alternatives
//                 {
//                     public float confidence { get; set; }
//                     public string content { get; set; }
//                 }
//             }
//
//             public class Speakers
//             {
//                 public int speakers { get; set; }
//                 public List<Segment> segments { get; set; }
//
//                 public class Segment
//                 {
//                     public string speaker_label { get; set; }
//                     public float start_time { get; set; }
//                     public float end_time { get; set; }
//                     public List<SpeakerItems> items { get; set; }
//
//                     public class SpeakerItems
//                     {
//                         public float start_time { get; set; }
//                         public float end_time { get; set; }
//                         public string speaker_label { get; set; }
//                     }
//                 }
//             }
//         }
//     }
// }
public class Translation : IDisposable
{
    private static readonly string[][] _possible_source_languages_code =
    {
        new[] { "en", "en" },
        new[] { "fr-FR", "fr" }
    };

    private static readonly string[][] _possible_target_languages_code =
    {
        new[] { "en", "en" },
        new[] { "en-GB", "en-GB" },
        new[] { "en-US", "en-US" },
        new[] { "fr-FR", "fr" }
    };

    /// <summary>
    ///     Initialization of the Translation class object.
    /// </summary>
    public Translation()
    {
        AuthKey = Environment.GetEnvironmentVariable("DEEPL_AUTH_KEY");
        try
        {
            Translator = new Translator(AuthKey);
        }
        catch
        {
            throw new ArgumentException($"The Authentification Key {AuthKey} is wrong.");
        }
    }

    private static string? AuthKey { get; set; }
    private static Translator Translator { get; set; }

    /// <summary>
    /// </summary>
    public void Dispose()
    {
        Translator.Dispose();
    }

    /// <summary>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="from_language"></param>
    /// <param name="to_language"></param>
    /// <returns></returns>
    public async Task<string> TranslateAsync(string text,
        string from_language,
        string to_language)
    {
        if (text == "") return "";

        var source_language = CheckSourceLanguage(from_language);
        var target_language = CheckTargetLanguage(to_language);
        Console.WriteLine("Src: " + source_language + ", Targ: " + target_language);

        var translatedText = await Translator.TranslateTextAsync(
            text,
            source_language,
            target_language);

        return translatedText.Text;
    }

    public static string CheckSourceLanguage(string language)
    {
        if (Array.Exists(_possible_source_languages_code, e => e[0] == language || e[1] == language))
        {
            var lang_res = Array.Find(_possible_source_languages_code,
                e => e[0] == language || e[1] == language);
            Console.WriteLine("0: " + lang_res[0] + ", 1: " + lang_res[1]);
            return lang_res[1];
        }

        throw new LanguageException(
            $"The language code {language} does not exist. Try a language with the format \"fr-FR\".");
    }

    public static string CheckTargetLanguage(string language)
    {
        if (Array.Exists(_possible_target_languages_code, e => e[0] == language || e[1] == language))
        {
            var lang_res = Array.Find(_possible_target_languages_code,
                e => e[0] == language || e[1] == language);
            Console.WriteLine("0: " + lang_res[0] + ", 1: " + lang_res[1]);
            return lang_res[1];
        }

        throw new LanguageException(
            $"The language code {language} does not exist. Try a language with the format \"fr-FR\".");
    }
}

internal class Launch
{
    private static async Task Main(string[] args)
    {
        var filename = args[0];

        /////////////////////////////////////////////////////////////


        /*Create_srt(filename);*/


        /////////////////////////////////////////////////////////////


        /*HumanVoiceRecognition(filename);*/


        /////////////////////////////////////////////////////////////


        /*string threshold = args[1];
        uint scale;
        try
        {
            scale = uint.Parse(args[2]);
        }
        catch (Exception)
        {
            scale = 500;
        }
        ShotDetection(filename, threshold, scale);*/


        /////////////////////////////////////////////////////////////


        /*bool ismixed;
        try
        {
            ismixed = bool.Parse(args[1]);
        }
        catch (Exception)
        {
            ismixed = true;
        }

        bool is_create_csv;
        try
        {
            is_create_csv = bool.Parse(args[2]);
        }
        catch (Exception)
        {
            is_create_csv = false;
        }

        WaveForm(filename, ismixed, is_create_csv: is_create_csv);*/


        /////////////////////////////////////////////////////////////


        /*string n;
        try
        {
            n = args[1];
        }
        catch (Exception)
        {
            n = "0.01";
        }

        float d;
        try
        {
            d = float.Parse(args[2]);
        }
        catch (Exception)
        {
            d = 2;
        }

        SilenceDetect(filename, n, d);*/


        /////////////////////////////////////////////////////////////


        /*string language;
        try
        {
            language = args[1];
        }
        catch (Exception)
        {
            language = "";
        }
        SpeechToText.stt(filename, language);*/


        /////////////////////////////////////////////////////////////

        /*string language = "";
        try
        {
            language = args[1];
        }
        catch (Exception)
        { }

        string phrases = "";

        Main2(filename);*/

        /////////////////////////////////////////////////////////////

        /*string language = "";
        string phrases = "";
        bool overwrite_csv;
        try
        {
            language = args[1];
        }
        catch (Exception)
        {
            language = "en-US";
        }

        try
        {
            phrases = args[2];
        }
        catch (Exception)
        {
            phrases = "";
        }

        try
        {
            overwrite_csv = bool.Parse(args[3]);
        }
        catch (Exception)
        {
            overwrite_csv = false;
        }

        SpeechToTextGoogle(filename, language, phrases, overwrite_csv);*/

        /////////////////////////////////////////////////////////////

        /*string language;
        string path_shot;
        string bucketName;

        bool diarization = true;
        int MaxSpeakers = 2;

        try
        {
            language = args[1];
        }
        catch (Exception)
        {
            language = "en-US";
        }

        try
        {
            path_shot = args[2];
        }
        catch (Exception)
        {
            path_shot = @"";
        }

        try
        {
            bucketName = args[3];
        }
        catch (Exception)
        {

            bucketName = "";
        }

        // Transcription Service
        SpeechToText awss = new SpeechToText();
        SpeechToText.ResponseJSON[] resps = await awss.TranscribeInputFile(filename, targetLanguageCode: language, bucketName: bucketName, diarization: diarization, MaxSpeakers: MaxSpeakers);
        awss.Dispose();

        // Convert to XML
        string name = resps[0].filename;
        SpeechToText.CreateXMLFromJSON(resps[0], name, language, shot_folder: path_shot);
*/
    }
}

/*
/// <summary>
/// SpeechToText class that gathers all speech to text functions.
/// </summary>
///
internal class SpeechToText
{
static bool completed;
private static int lineCount = 0;
private static StringBuilder output = new StringBuilder();
static StreamWriter sw;
static FileStream filew;


public static async Task ContinuousRecognitionWithFileAsync(string filename)
{
    // <recognitionContinuousWithFile>
    // Creates an instance of a speech config with specified subscription key and service region.
    // Replace with your own subscription key and service region (e.g., "westus").
    var config = SpeechConfig.FromSubscription("127b7c94d7e74734b9d43064986acbaa", "francecentral");

    var stopRecognition = new TaskCompletionSource<int>();

    // Creates a speech recognizer using file as audio input.
    // Replace with your own audio file name.
    Console.WriteLine($"Create audioInput");
    using (var audioInput = AudioConfig.FromWavFileInput(@"" + filename))
    {
        using (var recognizer = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(config, audioInput))
        {
            // Subscribes to events.
            recognizer.Recognizing += (s, e) =>
            {
                Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
            };

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
            };

            recognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }

                stopRecognition.TrySetResult(0);
            };

            recognizer.SessionStarted += (s, e) =>
            {
                Console.WriteLine("\n    Session started event.");
            };

            recognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\n    Session stopped event.");
                Console.WriteLine("\nStop recognition.");
                stopRecognition.TrySetResult(0);
            };

            // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            // Waits for completion.
            // Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });

            // Stops recognition.
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
        }
    }
    // </recognitionContinuousWithFile>
}

/// <summary>
/// Asynchronous ffmpeg process.
/// </summary>
/// <param name="input_filename">Path to the audio or video file</param>
/// <param name="outdir">(Optional) Path of the folder to save the created file.</param>
/// <returns>Void</returns>
public static async Task<int> RunProcessAsync(string input_filename, string name, string outdir = @"./Data/Audios/wav/")
{
    using (var process = new Process
    {
        StartInfo =
        {
            FileName = "ffmpeg", Arguments = "-i " + input_filename + " " + outdir + name + ".wav",
            UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true
        },
        EnableRaisingEvents = true
    })
    {
        return await RunProcessAsync(process).ConfigureAwait(false);
    }
}


private static Task<int> RunProcessAsync(Process process)
{
    var tcs = new TaskCompletionSource<int>();

    process.Exited += (s, ea) => tcs.SetResult(process.ExitCode);
    process.OutputDataReceived += (s, ea) => Console.WriteLine(ea.Data);
    process.ErrorDataReceived += (s, ea) => Console.WriteLine("ERR: " + ea.Data);

    bool started = process.Start();
    if (!started)
    {
        //you may allow for the process to be re-used (started = false)
        //but I'm not sure about the guarantees of the Exited event in such a case
        throw new InvalidOperationException("Could not start process: " + process);
    }

    process.BeginOutputReadLine();
    process.BeginErrorReadLine();

    return tcs.Task;
}

static Task<int> RunProcessAsync(string input_filename, string name)
{

    var tcs = new TaskCompletionSource<int>();

    var proc = new Process
    {
        StartInfo = { FileName = "ffmpeg",
          Arguments = "-i " + input_filename + " .\\temp\\" + name + ".wav"},
        EnableRaisingEvents = true
    };


    proc.StartInfo.FileName = "ffmpeg";
    proc.StartInfo.Arguments = "-i " + input_filename + " .\\temp\\" + name + ".wav";
    proc.StartInfo.RedirectStandardError = true;
    proc.StartInfo.RedirectStandardOutput = true;
    proc.StartInfo.UseShellExecute = false;

    proc.Exited += (sender, args) =>
    {
        tcs.SetResult(proc.ExitCode);
        proc.Dispose();
    };

    if (!proc.Start())
    {
        Console.WriteLine("Error starting");
        System.Environment.Exit(0);
    }

    StreamReader reader = proc.StandardError;
    string line;
    while ((line = reader.ReadLine()) != null)
    {

    }

    proc.Close();

    Console.WriteLine(tcs.Task);
    return tcs.Task;

}
static async Task<string> ConvertToWavAsync(string input_filename, string outdir = @"./Data/Audios/wav/")
{
    Console.WriteLine($"{nameof(ConvertToWavAsync)}: About to convert.");
    char[] separators = new char[] { '/', '\\' };
    string name = input_filename.Split(separators).Last();
    int size_name = name.Count();

    String format = "";
    int p = size_name - 1;
    while (!(name[p] == '.') && p > 0)
    {
        format = name[p] + format;
        p--;
    }

    if (!(format == "wav"))
    {
        CreateDirectory(outdir);

        Console.WriteLine("\n   Converting {0} to wav", name);

        FileInfo f = new FileInfo(String.Format(outdir + "{0}.wav", name));
        string fullname = f.FullName;
        string input_filename_wav = string.Format(@"{0}", fullname);

        if (File.Exists(input_filename_wav))
        {
            Console.WriteLine("\n   File already converted in \"" + outdir + "\", continue...");
        }

        else
        {
            await RunProcessAsync(input_filename, name);
            Console.WriteLine($"{nameof(ConvertToWavAsync)}: Launched converter.");


            Console.WriteLine($"{nameof(ConvertToWavAsync)}: File converted.");
            Console.WriteLine("   File converted in " + outdir, name);
        }

        input_filename = input_filename_wav;
    }
    return input_filename;
}

public static async Task stt(string path, string language, string outdir = @"./SystemSpeech/")
{
    string culture;
    switch (language)
    {
        case "en":
            culture = "en-GB";
            break;
        case "fr":
            culture = "fr-FR";
            break;
        default:
            culture = "en-US";
            break;
    }

    Console.WriteLine("Language set to {0}", culture);

    FileInfo f = new FileInfo(path);
    string fullname = f.FullName;
    string input_filename = string.Format(@"{0}", fullname);
    char[] separators = new char[] { '/', '\\' };
    string name = path.Split(separators).Last();

    Task<string> converting = ConvertToWavAsync(path);
    Console.WriteLine($"{nameof(stt)}: Launched converter.");
    input_filename = await converting;

    Console.WriteLine(input_filename);

    string outdircsv = outdir + "CSVs/";
    CreateDirectory(outdircsv);

    string csv_filename = String.Format(outdircsv + "{0}-speechtotext-{1}.csv", name, culture);

    using (SpeechRecognitionEngine recognizer =
      new SpeechRecognitionEngine(new CultureInfo(culture)))
    {


        filew = new FileStream(csv_filename, FileMode.Create, FileAccess.Write);
        sw = new StreamWriter(filew);

        System.Speech.Recognition.Grammar dictation = new DictationGrammar("grammar:dictation");
        dictation.Name = "Dictation Grammar";
        dictation.Enabled = true;

        recognizer.LoadGrammar(dictation);

        // Configure the audio input.
        recognizer.SetInputToNull();
        recognizer.SetInputToAudioStream(
          File.OpenRead(@"" + path),
          new SpeechAudioFormatInfo(
            44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono));
        recognizer.SetInputToWaveFile(input_filename);

        // Add event handlers for the events raised by the
        // EmulateRecognizeAsync method.
        recognizer.SpeechDetected +=
          new EventHandler<SpeechDetectedEventArgs>(
            SpeechDetectedHandler);
        recognizer.SpeechHypothesized +=
          new EventHandler<SpeechHypothesizedEventArgs>(
            SpeechHypothesizedHandler);
        recognizer.SpeechRecognitionRejected +=
          new EventHandler<SpeechRecognitionRejectedEventArgs>(
            SpeechRecognitionRejectedHandler);
        recognizer.SpeechRecognized +=
          new EventHandler<SpeechRecognizedEventArgs>(
            SpeechRecognizedHandler);
        recognizer.RecognizeCompleted +=
          new EventHandler<RecognizeCompletedEventArgs>(
            RecognizeCompletedHandler);


        Console.WriteLine("Starting asynchronous recognition...");
        completed = false;
        recognizer.RecognizeAsync(RecognizeMode.Multiple);

        while (!completed)
        {
            Thread.Sleep(333);
        }
        Console.WriteLine("Done.");
    }

    sw.Close();
    filew.Close();

    Console.WriteLine("Creating subtitles...");
    string outdirsrt = outdir + "SRTs/";
    Create_srt(csv_filename, outdirsrt);

    Console.WriteLine("press any key to exit...");
    Console.ReadKey(true);
}

static void SpeechDetectedHandler(
object sender, SpeechDetectedEventArgs e)
{
    Console.WriteLine(" SpeechDetected event raised.");

}

static void SpeechHypothesizedHandler(
object sender, SpeechHypothesizedEventArgs e)
{
    if (e.Result != null)
    {

    }
    else
    {
        Console.WriteLine("  No recognition result available.");
    }
}

static void SpeechRecognitionRejectedHandler(
object sender, SpeechRecognitionRejectedEventArgs e)
{
    if (e.Result != null)
    {

    }
    else
    {
        Console.WriteLine("  No recognition result available.");
    }
}
static void SpeechRecognizedHandler(object sender, SpeechRecognizedEventArgs e)
{
    if (e.Result == null) return;

    // The following code illustrates some of the information available
    // in the recognition result.
    //

    Console.WriteLine(e.Result.Text);

    Console.WriteLine("Recognition result summary:");
    Console.WriteLine(
      "  Recognized phrase: {0}\n" +
      "  Confidence score {1}\n" +
      "  Grammar used: {2}\n",
      e.Result.Text, e.Result.Confidence, e.Result.Grammar.Name);

    // Display the semantic values in the recognition result.
    Console.WriteLine("  Semantic results:");
    foreach (KeyValuePair<String, SemanticValue> child in e.Result.Semantics)
    {
        Console.WriteLine("    The {0} city is {1}",
          child.Key, child.Value.Value ?? "null");
    }
    Console.WriteLine();

    // Display information about the words in the recognition result.
    Console.WriteLine("  Word summary: ");
    foreach (RecognizedWordUnit word in e.Result.Words)
    {
        Console.WriteLine(
          "    Lexical form ({1})" +
          " Pronunciation ({0})" +
          " Display form ({2})",
          word.Pronunciation, word.LexicalForm, word.DisplayAttributes);
    }

    // Display information about the audio in the recognition result.
    Console.WriteLine("  Input audio summary:\n" +
      "    Candidate Phrase at:       {0} mSec\n" +
      "    Phrase Length:             {1} mSec\n" +
      "    Input State Time:          {2}\n" +
      "    Input Format:              {3}\n",
      e.Result.Audio.AudioPosition,
      e.Result.Audio.Duration,
      e.Result.Audio.StartTime,
      e.Result.Audio.Format.EncodingFormat);

    // Display information about the alternate recognitions in the recognition result.
    Console.WriteLine("  Alternate phrase collection:");
    foreach (RecognizedPhrase phrase in e.Result.Alternates)
    {
        Console.WriteLine("    Phrase: " + phrase.Text);
        Console.WriteLine("    Confidence score: " + phrase.Confidence);
    }

    sw.WriteLine("{0};{1};{2};{3}", e.Result.Audio.AudioPosition, e.Result.Audio.Duration, e.Result.Confidence, e.Result.Text);
}

static void RecognizeCompletedHandler(
object sender, RecognizeCompletedEventArgs e)
{
    Console.WriteLine(" RecognizeCompleted event raised.");

    if (e.Error != null)
    {
        Console.WriteLine("  {0} exception encountered: {1}:",
          e.Error.GetType().Name, e.Error.Message);
    }
    else if (e.Cancelled)
    {
        Console.WriteLine("  Operation cancelled.");
    }
    else if (e.Result != null)
    {
        Console.WriteLine(e.Result.Text);
    }
    else
    {
        Console.WriteLine("  No recognition result available.");
    }

    completed = true;
}


public static async Task SpeechContinuousRecognitionAsync()
{
    // Creates an instance of a speech config with specified subscription key and region.
    // Replace with your own subscription key and service region (e.g., "westus").
    var config = SpeechConfig.FromSubscription("ea0310fa136348368a1795df4913bb4f", "francecentral");

    // Creates a speech recognizer from microphone.
    using (var recognizer = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(config))
    {
        // Subscribes to events.
        recognizer.Recognizing += (s, e) =>
        {
            Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
        };

        recognizer.Recognized += (s, e) =>
        {
            var result = e.Result;
            Console.WriteLine($"Reason: {result.Reason.ToString()}");
            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                Console.WriteLine($"Final result: Text: {result.Text}.");
            }
        };

        recognizer.Canceled += (s, e) =>
        {
            Console.WriteLine($"\n    Canceled. Reason: {e.Reason.ToString()}, CanceledReason: {e.Reason}");
        };

        recognizer.SessionStarted += (s, e) =>
        {
            Console.WriteLine("\n    Session started event.");
        };

        recognizer.SessionStopped += (s, e) =>
        {
            Console.WriteLine("\n    Session stopped event.");
        };

        // Starts continuous recognition.
        // Uses StopContinuousRecognitionAsync() to stop recognition.
        await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

        do
        {
            Console.WriteLine("Press Enter to stop");
        } while (Console.ReadKey().Key != ConsoleKey.Enter);

        // Stops recognition.
        await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
    }
}
}



/// <summary>
/// Upload the file objectName in localPath in the bucket BucketName
/// </summary>
/// <param name="bucketName">Name of the bucket</param>
/// <param name="localPath">Path of the folder of the object to upload.</param>
/// <param name="objectName">The name of the object to upload.</param>
/// <returns>Void</returns>
public static void UploadFile(string bucketName, string localPath, string objectName)
{
var storage = StorageClient.Create();
using var fileStream = File.OpenRead(localPath);
storage.UploadObject(bucketName, objectName, null, fileStream);
Console.WriteLine($"Uploaded {objectName}.");
}


/// <summary>
/// Creates a csv file of the decoded audio values (Obsolete, too long), and returns the number of channels of the audio input.
/// </summary>
/// <param name="filename">Path to the audio or video file</param>
/// <param name="shortfilename">Name of the file to create, without the csv extension</param>
/// <param name="is_create_csv"> (Optionnal) Boolean. If true, decodes all audio values (too long). Default false.</param>
/// <param name="overwrite_csv"> (Optionnal) Boolean. If true, overwrite the decoded audio values if they already exist (too long). Default false.</param>
/// <param name="outdir"> (Optionnal) Path of the folder to save the created mp3 file.</param>
/// <returns>The number of channels of the audio input.</returns>
static unsafe uint Create_csv(string filename, string shortfilename, bool is_create_csv = false, bool overwrite_csv = false, string outdir = @"./WaveForm/CSVs/")
{
// If is_create_csv is true, decodes all the audio values of the file filename into a csv file and return the number of channels that compose the audio
// Otherwise, return the number of channels only
ffmpeg.RootPath = @"C:\ffmpeg\bin\";
Console.WriteLine("\nDecoding audio...");
int size_name = shortfilename.Count();

string[] name_and_ext = GetExtension(shortfilename);
string ext = name_and_ext[1];

// Convert the input file into mp3 format
if (!(ext == "mp3"))
{
    filename = ConvertToMp3(filename, shortfilename);
}

// Open file
AVFormatContext* container = ffmpeg.avformat_alloc_context();
if (ffmpeg.avformat_open_input(&container, filename, null, null) < 0)
{
    Console.WriteLine("\n   Could not open the file {0}", filename);
    System.Environment.Exit(0);
}

// Getting info
if (ffmpeg.avformat_find_stream_info(container, null) < 0)
{
    Console.WriteLine("\n   Could not find file info");
    System.Environment.Exit(0);
}

ffmpeg.av_dump_format(container, 0, filename, 0);

// Getting stream id of the audio stream
int stream_id = -1;
int i;
for (i = 0; i < container->nb_streams; i++)
{
    if (container->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
    {
        stream_id = i;
        break;
    }
}
if (stream_id == -1)
{
    Console.WriteLine("\n   Could not find audio stream");
    System.Environment.Exit(0);
}

// Getting the codec
AVDictionary* metadata = container->metadata;
AVCodecParameters* c_params = container->streams[stream_id]->codecpar;
AVCodec* codec = ffmpeg.avcodec_find_decoder(c_params->codec_id);

if (codec == null)
{
    Console.WriteLine("\n   Cannot find codec");
    System.Environment.Exit(0);
}


AVCodecContext* ctx = ffmpeg.avcodec_alloc_context3(codec);
AVDictionary* opts;
ffmpeg.av_dict_set(&opts, "ac", string.Format("{0}", (int)c_params->channels), 0);
ffmpeg.av_dict_set(&opts, "ab", string.Format("{0}", (int)c_params->bit_rate), 0);
ffmpeg.av_dict_set(&opts, "ar", string.Format("{0}", (int)c_params->sample_rate), 0);
if (ffmpeg.avcodec_open2(ctx, codec, &opts) < 0)
{
    Console.WriteLine("\n   Codec cannot be found");
    System.Environment.Exit(0);
}

uint nb_channels = (uint)ctx->channels;

if (is_create_csv)
{

    List<int> channels_to_csv = new List<int>();
    if (!overwrite_csv)
    {
        for (int k = 0; k < nb_channels; k++)
        {
            if (!File.Exists(String.Format(outdir + "{0}-{1}.csv", shortfilename, k)))
            {
                channels_to_csv.Add(k);
            }
        }
    }
    else
    {
        channels_to_csv = new List<int>(from number in Enumerable.Range(0, ctx->channels) select number);
    }

    int nb_channels_to_csv = channels_to_csv.Count;

    if (nb_channels_to_csv == 0)
    {
        Console.WriteLine("------\nAll channels already decoded");
    }
    else
    {
        Console.WriteLine("------\nDecoding {0} channel(s) over all {1}", nb_channels_to_csv, nb_channels);
    }

    // Compute the csv files only when necessary
    if (channels_to_csv.Any())
    {
        // Getting the parser
        AVCodecParserContext* parser = ffmpeg.av_parser_init((int)codec->id);
        if (parser == null)
        {
            Console.WriteLine("\n   Parser not found");
            System.Environment.Exit(0);
        }

        AVPacket* packet = ffmpeg.av_packet_alloc();
        AVFrame* frame = ffmpeg.av_frame_alloc();

        int buffer_size = 20480 + 64; // AVCODEC_MAX_AUDIO_FRAME_SIZE + FF_INPUT_BUFFER_PADDING_SIZE;

        uint[] buffer = new uint[buffer_size];
        fixed (uint* data = &buffer[0])
        {
            packet->data = (byte*)data;
            packet->size = buffer_size;
        }

        int plane_size;

        FileStream[] fileChs = new FileStream[nb_channels_to_csv];
        StreamWriter[] srs = new StreamWriter[nb_channels_to_csv];


        CreateDirectory(outdir);

        for (int k = 0; k < nb_channels_to_csv; k++)
        {
            fileChs[k] = new FileStream(String.Format(outdir + "{0}-{1}.csv", shortfilename, channels_to_csv[k]), FileMode.Create, FileAccess.Write);
            srs[k] = new StreamWriter(fileChs[k]);
        }


        AVSampleFormat sfmt = ctx->sample_fmt;

        uint j = 1;
        uint nb_frames = 0;
        uint count = 0;
        while (ffmpeg.av_read_frame(container, packet) >= 0)
        {
            j++;
            if (packet->stream_index == stream_id)
            {
                nb_frames++;
                int len = ffmpeg.av_parser_parse2(parser, ctx, &packet->data, &packet->size,
                    packet->buf->data, packet->buf->size, unchecked((long)0x8000000000000000), unchecked((long)0x8000000000000000), 0);

                if (len < 0)
                {
                    Console.WriteLine("\n   Error while parsing");
                    System.Environment.Exit(0);
                }


                int ret = ffmpeg.avcodec_send_packet(ctx, packet);

                if (ret < 0)
                {
                    Console.WriteLine("\n   Error submitting the packet to the decoder (ret = {0})", ret);
                    System.Environment.Exit(0);
                }

                ret = ffmpeg.avcodec_receive_frame(ctx, frame);

                if (ret == ffmpeg.AVERROR(ffmpeg.EAGAIN) || ret == ffmpeg.AVERROR_EOF)
                {
                    Console.WriteLine("\n   Error while receiving frame (ret = {0})", ret);
                    System.Environment.Exit(0);
                }
                else if (ret < 0)
                {
                    Console.WriteLine("\n   Error during decoding");
                    System.Environment.Exit(0);
                }
                int data_size = ffmpeg.av_samples_get_buffer_size(&plane_size, ctx->channels,
                                                    frame->nb_samples,
                                                    ctx->sample_fmt, 1);

                if (data_size > 0)
                {
                    switch (sfmt)
                    {
                        case AVSampleFormat.AV_SAMPLE_FMT_FLTP:

                            for (int nb = 0; nb < plane_size / sizeof(float); nb++)
                            {
                                for (int ch = 0; ch < nb_channels_to_csv; ch++)
                                {
                                    srs[ch].WriteLine("{0};{1};{2}", ((float*)frame->extended_data[channels_to_csv[ch]])[nb], frame->nb_samples, frame->pkt_dts);
                                }
                            }

                            count = count + (uint)(plane_size / sizeof(float));

                            break;

                    }
                }
                else
                {
                    Console.WriteLine("\n   Frame failed");
                    System.Environment.Exit(0);
                }
            }
            ffmpeg.av_packet_unref(packet);
        }

        for (int k = 0; k < nb_channels_to_csv; k++)
        {
            srs[k].Close();
            fileChs[k].Close();
        }

        Console.WriteLine($"{count}");

        Console.WriteLine("\n------\nAll Frames decoded");
    }
}

ffmpeg.avcodec_close(ctx);
ffmpeg.avformat_close_input(&container);

return nb_channels;
}

/// <summary>
/// Detects human voices in an audio, and creates a csv file that contains time stamps.
/// </summary>
/// <param name="filename">Path to the audio or video file</param>
/// <param name="outdir">(Optional) Path of the folder to save the created jpg file.</param>
/// <returns>Void</returns>
public static void HumanVoiceRecognition(string filename, string outdir = @"./HumanVoices/")
{
// VAD(Voice Audio Detection), detects voices in the audio file and cut the video into pieces of speech
Console.WriteLine("Human voices detection...\n\n");

// Converting to PCM format if it does not already exist
FileInfo f = new FileInfo(filename);
char[] separators = new char[] { '/', '\\' };
string name = filename.Split(separators).Last();
string ext = name.Split('.').Last();

string outdirpcm = outdir + "PCMs/";
CreateDirectory(outdirpcm);

if (!File.Exists(outdirpcm + name + ".pcm"))
{
    Process proc = new Process();
    proc.StartInfo.FileName = "ffmpeg";
    proc.StartInfo.Arguments = "-y -i " + filename + " -acodec pcm_s16le -f s16le -ac 1 -ar 16000 " + outdirpcm + name + ".pcm";

    proc.StartInfo.RedirectStandardError = true;
    proc.StartInfo.UseShellExecute = false;
    if (!proc.Start())
    {
        Console.WriteLine("Error starting");
        return;
    }

    StreamReader reader = proc.StandardError;
    string line;
    while ((line = reader.ReadLine()) != null)
    {
        Console.WriteLine(line);
    }

    proc.Close();
}

string to_pcm = string.Format(outdirpcm + "{0}.pcm", name);
FileInfo f1 = new FileInfo(to_pcm);
string path_to_pcm = f1.FullName;

// Parametrization of the VAD
using var vad = new WebRtcVad()
{
    OperatingMode = OperatingMode.LowBitrate,
    FrameLength = FrameLength.Is10ms,
    SampleRate = SampleRate.Is16kHz,
};

var frameSize = ((int)vad.SampleRate / 1000) * 2 32 * (int)vad.FrameLength;
var buffer = new byte[frameSize];
byte[] audio = File.ReadAllBytes(path_to_pcm);

string outdirvoices = outdir + name + "/";
CreateDirectory(outdirvoices);

FileStream file = new FileStream(String.Format(outdirvoices + "data.csv"), FileMode.Create, FileAccess.Write);
StreamWriter sr = new StreamWriter(file);

bool hasSpeech_prec = false;
int start_voice = 0;
int end_voice = 0;

float time_per_i = (float)vad.FrameLength / frameSize;
int time_start_voice = 0;
int time_end_voice = 0;
uint count = 0;

bool wait = false;
int wait_until = 0;
int time_wait = 2000; // 2000ms = 2s
int nb_i_wait = (int)(time_wait / time_per_i);

int total_time_extracted = 0;

// For each frame, checking wether there is a speech or not
for (int i = 0; i < audio.Length - frameSize; i += frameSize)
{
    Buffer.BlockCopy(audio, i, buffer, 0, buffer.Length);
    bool hasSpeech = vad.HasSpeech(buffer);

    // If this frame contains a voice and the previous one did not,
    // then a speech begins here
    if (hasSpeech && !hasSpeech_prec)
    {
        if (wait)
        {
            wait = false;
        }
        else
        {
            start_voice = i;
            time_start_voice = (int)(start_voice * time_per_i);
        }


    }
    // If this frame does not contain a voice and the previous one did,
    // then a speech ends here.To avoid small cuts in sentences for example,
    // we wait for a specified time before ending the speech, in case an other

    // comes along.
    else if (!hasSpeech && hasSpeech_prec)
    {
        wait = true;
        end_voice = i;
        time_end_voice = (int)(end_voice * time_per_i);
        wait_until = end_voice + nb_i_wait;
    }

    // If we reach an end of speech after having waited a specified time, we cut the
    // part of the audio that contains the speech and save it into a file.
    if (wait && (i > wait_until || i + frameSize >= audio.Length - frameSize))
    {
        wait = false;

        // At least 1 second of voice audio to save it
        if (time_end_voice - time_start_voice > 1000)
        {
            count++;
            total_time_extracted += time_end_voice - time_start_voice;
            TimeSpan ts = TimeSpan.FromMilliseconds(time_start_voice);
            string time_start = ts.ToString();
            TimeSpan te = TimeSpan.FromMilliseconds(time_end_voice);
            string time_end = te.ToString();
            sr.WriteLine($"{time_start};{time_end}");

            Process proc_trim = new Process();
            proc_trim.StartInfo.FileName = "ffmpeg";
            proc_trim.StartInfo.Arguments = "-y -i " + filename + " -ss " + time_start + " -to " + time_end + " -c:v copy -c:a copy " + outdirvoices + String.Format("{0:000}", count) + "." + ext;

            proc_trim.StartInfo.RedirectStandardError = true;
            proc_trim.StartInfo.UseShellExecute = false;
            if (!proc_trim.Start())
            {
                Console.WriteLine("Error starting");
                return;
            }

            StreamReader reader_trim = proc_trim.StandardError;
            string line_trim;
            while ((line_trim = reader_trim.ReadLine()) != null)
            {
                Console.WriteLine(line_trim);

            }

            proc_trim.Close();
        }

    }
    hasSpeech_prec = hasSpeech;

    // Progress
    if (i % 10000 == 0)
    {
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.WriteLine("{0:P}", (float)(i / 1000) / (float)((audio.Length - frameSize) / 1000));
    }
}
Console.SetCursorPosition(0, Console.CursorTop - 1);
Console.WriteLine("{0:P}", 1);

sr.Close();
file.Close();

Console.WriteLine("\n------\nTotal time extracted: {0} over {1}\n------", TimeSpan.FromMilliseconds(total_time_extracted).ToString(), TimeSpan.FromMilliseconds(audio.Length * time_per_i).ToString());
}

/// <summary>
/// Detects silences in a video, and writes time stamps into acsv file.
/// </summary>
/// <param name="filename">Path to the audio or video file</param>
/// <param name="n">Noise tolerance (in dB or in percentage). Default 0.01.</param>
/// <param name="d">Minimum duration of silence. Default 2.</param>
/// <param name="outdir">(Optional) Path of the folder to save the created csv file.</param>
/// <returns>Void</returns>
public static void SilenceDetect(string filename, string n = "0.01", float d = 2, string outdir = @"./Silences/")
{
FileInfo f = new FileInfo(filename);
char[] separators = new char[] { '/', '\\' };
string name = filename.Split(separators).Last();

Process proc = new Process();
proc.StartInfo.FileName = "ffmpeg";
proc.StartInfo.Arguments = "-i " + filename + " -af silencedetect=n=" + n + ":d=" + d + " -f null -";

proc.StartInfo.RedirectStandardError = true;
proc.StartInfo.UseShellExecute = false;
if (!proc.Start())
{
    Console.WriteLine("Error starting");
    return;
}

StreamReader reader = proc.StandardError;
string line;
int i = 0;
bool completeline = false;
string startsilence = " ";
string endsilence = " ";
string duration = " ";

outdir = outdir + name + "/";
CreateDirectory(outdir);

FileStream file = new FileStream(String.Format(outdir + "Silences-n={0}-d={1}.csv", n, d), FileMode.Create, FileAccess.Write);
StreamWriter sw = new StreamWriter(file);
while ((line = reader.ReadLine()) != null)
{
    if (line.StartsWith("[silencedetect"))
    {
        var sep = line.Split(' ');

        // If the duration is written, then it means the end is written
        if (sep.Length > 7)
        {
            endsilence = sep[4];
            duration = sep[7];
            completeline = true;
        }

        else
        {
            startsilence = sep[4];
        }

        if (completeline)
        {
            sw.WriteLine(startsilence + ";" + endsilence + ";" + duration);
            Console.WriteLine(startsilence + ";" + endsilence + ";" + duration);
            completeline = false;
        }
    }
    i++;
}
sw.Close();
file.Close();
proc.Close();
}

/// <summary>
/// Creates SRT file from csv response from Google API.
/// </summary>
/// <param name="csv_file">Path to the csv file</param>
/// <param name="outdir">(Optional) Path of the folder to save the created jpg file.</param>
/// <returns>Void</returns>
static void Create_srt(string csv_file, string outdir = @"./")
{
// Csv file like time_start;duration;confidence;text
char[] file_separators = new char[] { '/', '\\' };
string name = csv_file.Split(file_separators).Last() + ".srt";

CreateDirectory(outdir);

FileStream fileStream = new FileStream(String.Format(outdir + name), FileMode.Create, FileAccess.Write);
StreamWriter sw = new StreamWriter(fileStream);

using (StreamReader sr = new StreamReader(csv_file))
{
    char[] separators = { ';' };
    string line;
    // Read and display lines from the file until the end of
    // the file is reached.
    uint num_st = 1;
    while ((line = sr.ReadLine()) != null)
    {
        string[] read = line.Split(separators);
        TimeSpan time_start = TimeSpan.Parse(read[0]);
        TimeSpan duration = TimeSpan.Parse(read[1]);
        float confidence = float.Parse(read[2]);
        string text = read[3];

        sw.WriteLine(num_st);
        string format = @"hh\:mm\:ss\,fff";
        sw.WriteLine("{0:" + format + "} --> {1:" + format + "}\n{2}\n", time_start, time_start + duration, text);
        num_st++;
    }
}
sw.Close();
fileStream.Close();
}

/// <summary>
/// Writes phrases context for a better comprehension for the API
/// </summary>
/// <param name="phrases_csv">Path to the csv file</param>
/// <returns>Void</returns>
private static SpeechContext PhraseContext(string phrases_csv)
{
SpeechContext speechContext = new SpeechContext();
using (StreamReader sr = new StreamReader(phrases_csv))
{
    char[] separators = { ';' };
    string line;
    while ((line = sr.ReadLine()) != null)
    {
        string[] read = line.Split(separators);
        int bo;
        try
        {
            bo = int.Parse(read[1]);
        }
        catch (Exception)
        {
            bo = 10;
        }

        foreach (string word in read)
        {
            speechContext.Phrases.Add(word);
            speechContext.Boost = bo;
        }

    }
}
return speechContext;

}

/// <summary>
/// Writes phrases set into the project on the Google API.
/// </summary>
/// <param name="projectId">Id of the project</param>
/// <param name="location">Location in the project.</param>
/// <param name="PhraseSetId">PhraseSet Id</param>
/// <param name="phrases_csv">Path to the csv file containing phrases.</param>
/// <param name="overwrite_csv">True if we need to overwrite phrases on the Google API.</param>
/// <returns>Void</returns>
private static PhraseSet PhraseHelper(string projectId, string location, string PhraseSetId, string phrases_csv, bool overwrite_csv)
{
AdaptationClientBuilder adap_clt_bd = new AdaptationClientBuilder();
AdaptationClient adap_clt = adap_clt_bd.Build();

if (overwrite_csv)
{
    try
    {
        Console.WriteLine("Overwriting, deleting existing phrases set...");
        DeletePhraseSetRequest delrequest = new DeletePhraseSetRequest
        {
            PhraseSetName = PhraseSetName.FromProjectLocationPhraseSet($"{projectId}", $"{location}", $"{PhraseSetId}"),
        };
        // Make the request
        adap_clt.DeletePhraseSet(delrequest);
    }
    catch (Exception)
    {
        Console.WriteLine("No phrases set to delete, continue");
    }

}

Console.WriteLine("Requesting phrases set...");
try
{
    GetPhraseSetRequest getrequest = new GetPhraseSetRequest
    {
        PhraseSetName = PhraseSetName.FromProjectLocationPhraseSet($"{projectId}", $"{location}", $"{PhraseSetId}"),
    };

    Console.WriteLine("Request success !");
    return adap_clt.GetPhraseSet(getrequest);
}
catch (Exception)
{
    Console.WriteLine("Can't request, creating phrases set...");
    PhraseSet ps = new PhraseSet();

    using (StreamReader sr = new StreamReader(phrases_csv))
    {
        char[] separators = { ';' };
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            string[] read = line.Split(separators);
            int bo;
            try
            {
                bo = int.Parse(read[1]);
            }
            catch (Exception)
            {
                bo = 10;
            }

            foreach (string word in read)
            {
                PhraseSet.Types.Phrase p = new PhraseSet.Types.Phrase();
                p.Value = word;
                p.Boost = bo;
                ps.Phrases.Add(p);
            }

        }
    }

    LocationName parent = LocationName.FromProjectLocation(projectId, location);
    CreatePhraseSetRequest request = new CreatePhraseSetRequest
    {
        ParentAsLocationName = parent,
        PhraseSetId = PhraseSetId,
        PhraseSet = ps,
    };

    Console.WriteLine("Phrases set created !");
    return adap_clt.CreatePhraseSet(request);
}
}

/// <summary>
/// Google API for SpeechToText. Writes the results into a csv file and creates a SRT file.
/// </summary>
/// <param name="filename">Path to the audio or video file.</param>
/// <param name="language">Language of the audio.</param>
/// <param name="phrases_csv">(Optional) Path to the csv file containing phrases. Default ""</param>
/// <param name="overwrite_csv">(Optional) True if we need to overwrite the phrases set on the Google platform.</param>
/// <param name="speaker_diarization"> (Optional) True if we want to apply the diarization.</param>
/// <param name="bucket_name">(Optional) Name of the bucket.</param>
/// <param name="outdir">(Optional) Path of the folder to save the created jpg file.</param>
/// <returns>Void</returns>
public static void SpeechToTextGoogle(string filename, string language = "en-US", string phrases_csv = "", bool overwrite_csv = false, bool speaker_diarization = false, string outdir = @"./GoogleSpeech/", string bucket_name = "example_ecosm_bucket")
{
// Create a SRT file of the subtitles of the input file

char[] file_separators = new char[] { '/', '\\' };
string name = filename.Split(file_separators).Last();
string namecsv = name + "_" + language + ".csv";
string namejson_global = name + "_" + language + "_all_alternatives.json";

// If the file is in the Google Storage
if (filename.Substring(0, 5) != "gs://")
{

    bool already_in_storage = false;
    try
    {
        var storage = StorageClient.Create();
        var obj = storage.GetObject(bucket_name, name + ".wav");
        already_in_storage = true;
        Console.WriteLine("Wav file already uploaded on the storage");
    }
    catch (Exception)
    {
    }

    if (!already_in_storage)
    {
        string filename_wav = ConvertToWav(filename);
        while (!(File.Exists(filename_wav)))
        {
            Console.WriteLine("Waiting for the wav file to be created...");
            Thread.Sleep(2000);
        }
        Thread.Sleep(2000);
        string name_wav = filename_wav.Split(file_separators).Last();
        Console.WriteLine("Uploading the wav file to the Google Storage");
        UploadFile(bucket_name, filename_wav, name_wav);
    }
    filename = "gs://" + bucket_name + "/" + name + ".wav";

}

string name_phrase = phrases_csv.Split(file_separators).Last();
Regex rgx = new Regex("[^a-zA-Z0-9 -]");
name_phrase = rgx.Replace(name_phrase, "");


string projectId = "xenon-crossbar-332213";
string location = "global";
string PhraseSetId = name_phrase;

SpeechClient speechClient = SpeechClient.Create();

RecognitionConfig config = new RecognitionConfig();
config.Encoding = RecognitionConfig.Types.AudioEncoding.Linear16;
config.SampleRateHertz = 16000;
config.LanguageCode = language;
config.EnableWordTimeOffsets = true;
config.EnableAutomaticPunctuation = true;
config.AudioChannelCount = 2;

// Configure phrases helper
if (phrases_csv != "")
{
    PhraseSet phrasesetresponse = PhraseHelper(projectId, location, PhraseSetId, phrases_csv, overwrite_csv);
    SpeechAdaptation speechAdapt = new SpeechAdaptation();
    speechAdapt.PhraseSets.Add(phrasesetresponse);

    Console.WriteLine("Phrases set response from {0}", LocationName.FormatProjectLocation(projectId, location) + "/phraseSets/" + PhraseSetId);
    config.Adaptation = speechAdapt;

    config.SpeechContexts.Add(PhraseContext(phrases_csv));

}

SpeakerDiarizationConfig speakerDiarizationConfig = new SpeakerDiarizationConfig
{
    EnableSpeakerDiarization = speaker_diarization,
    MinSpeakerCount = 2,
    MaxSpeakerCount = 2,
};

config.DiarizationConfig = speakerDiarizationConfig;


RecognitionAudio audio = new RecognitionAudio();
if (filename.Substring(0, 5) == "gs://")
{
    Console.WriteLine("Using Google Storage file " + filename);
    audio.Uri = @"" + filename;
}
else
{
    Console.WriteLine("Using local file " + filename);
    // As ConvertToWav works asynchronously (why ?), we wait until the file is written

    Byte[] bytes = File.ReadAllBytes(filename);
    String file = Convert.ToBase64String(bytes);
    audio.Content = Google.Protobuf.ByteString.FromBase64(file);
    if (audio.Content.Length > 10485760)
    {
        Console.WriteLine("Local audio to long.");
        System.Environment.Exit(0);
    }
}

Console.WriteLine("Transcribing on GoogleSpeech API...");
Google.Protobuf.Collections.RepeatedField<Google.Cloud.Speech.V1P1Beta1.SpeechRecognitionResult> results;
try
{
    RecognizeResponse response = speechClient.Recognize(config, audio);
    results = response.Results;
}
catch (Exception)
{
    Console.WriteLine("Long file");
    Google.LongRunning.Operation<LongRunningRecognizeResponse, LongRunningRecognizeMetadata> response_wait = speechClient.LongRunningRecognize(config, audio);
    Console.WriteLine("   Waiting for response...");
    Google.LongRunning.Operation<LongRunningRecognizeResponse, LongRunningRecognizeMetadata> response = response_wait.PollUntilCompleted();
    results = response.Result.Results;
}

string outdircsv = outdir + "CSVs/";
CreateDirectory(outdircsv);

FileStream fileStream = new FileStream(outdircsv + namecsv, FileMode.Create, FileAccess.Write);
StreamWriter sw = new StreamWriter(fileStream);

FileStream fs = new FileStream(outdircsv + namejson_global, FileMode.Create, FileAccess.Write);
StreamWriter sw_global = new StreamWriter(fs);

Google.Protobuf.CodedOutputStream cos = new Google.Protobuf.CodedOutputStream(fs);

Console.WriteLine("\n----------\nTranscription:");

foreach (Google.Cloud.Speech.V1P1Beta1.SpeechRecognitionResult result in results)
{
    // There can be several alternative transcripts for a given chunk of speech. Just use the
    // first (most likely) one here.
    SpeechRecognitionAlternative alternative = result.Alternatives[0];
    Console.WriteLine(result.Alternatives.Count);
    sw_global.WriteLine(result.ToString());

    result.WriteTo(cos);
    try
    {
        int nb_words = alternative.Words.Count;
        int currentSpeakerTag = alternative.Words[0].SpeakerTag;
        TimeSpan word_start_time = alternative.Words[0].StartTime.ToTimeSpan();
        TimeSpan word_end_time = alternative.Words.Last().EndTime.ToTimeSpan();

        TimeSpan word_final_time = alternative.Words.Last().EndTime.ToTimeSpan();
        TimeSpan diff_last_beg = word_final_time - word_start_time;
        string format = @"mm\:ss\.fff";
        string transcript = "";
        string prev_transcript = "";

        int i = 0;
        WordInfo prev_word = alternative.Words[0];
        while (diff_last_beg > TimeSpan.FromSeconds(7) && i < nb_words)
        {
            bool written = false;
            while (!written && i < nb_words)
            {
                TimeSpan next_time_cut = word_start_time + TimeSpan.FromSeconds(6);
                WordInfo cur_word = alternative.Words[i];
                transcript = transcript + cur_word.Word + " ";
                if (cur_word.EndTime.ToTimeSpan() >= next_time_cut && cur_word.EndTime.ToTimeSpan() < next_time_cut + TimeSpan.FromSeconds(1))
                {
                    written = true;
                    word_end_time = cur_word.EndTime.ToTimeSpan();

                    Console.WriteLine("   {0} - {1}, Speaker {2}: {3}", word_start_time.ToString(format), word_end_time.ToString(format), currentSpeakerTag, transcript);
                    sw.WriteLine("{0};{1};{2};{3}", word_start_time, word_end_time - word_start_time, currentSpeakerTag, transcript);
                }
                else if (cur_word.EndTime.ToTimeSpan() >= next_time_cut + TimeSpan.FromSeconds(1))
                {
                    word_end_time = prev_word.EndTime.ToTimeSpan();

                    Console.WriteLine("   {0} - {1}, Speaker {2}: {3}", word_start_time.ToString(format), word_end_time.ToString(format), currentSpeakerTag, prev_transcript);
                    sw.WriteLine("{0};{1};{2};{3}", word_start_time, word_end_time - word_start_time, currentSpeakerTag, prev_transcript);

                    word_start_time = cur_word.StartTime.ToTimeSpan();
                    transcript = cur_word.Word + " ";
                }
                prev_word = cur_word;
                prev_transcript = transcript;
                i++;
            }

            transcript = "";
            if (i < nb_words)
            {
                word_start_time = alternative.Words[i].StartTime.ToTimeSpan();
                diff_last_beg = word_final_time - word_start_time;
            }
        }

        if (i < nb_words)
        {
            while (i < nb_words)
            {
                WordInfo cur_word = alternative.Words[i];
                transcript = transcript + cur_word.Word + " ";
                i++;
            }
            Console.WriteLine("   {0} - {1}, Speaker {2}: {3}", word_start_time.ToString(format), word_final_time.ToString(format), currentSpeakerTag, transcript);
            sw.WriteLine("{0};{1};{2};{3}", word_start_time, word_final_time - word_start_time, currentSpeakerTag, transcript);
        }

    }
    catch (Exception)
    {
        Console.WriteLine("   00:00:00.000 - 00:00:00.000 : {0}", alternative.Transcript);
        sw.WriteLine("00:00:00.000;00:00:00.000;1;{0}", alternative.Transcript);
    }

}
Console.WriteLine("----------");
sw.Close();
fileStream.Close();

cos.Flush();
sw_global.Close();
fs.Close();


string outdirsrt = outdir + "SRTs/";
Create_srt(outdircsv + namecsv, outdirsrt);

Console.WriteLine("\nSRT created and saved in " + outdirsrt + "\n");
}

//todo move
public class TranscriptionJobResult
{
public string jobName { get; set; }
public string accountId { get; set; }
public string status { get; set; }
public TranscriptionResult results { get; set; }
}

public class TranscriptionResult
{
public List<Transcript> transcripts { get; set; }
public List<TranscriptItem> items { get; set; }
public Speakers speaker_labels { get; set; }
}

public class Speakers
{
public int speakers { get; set; }
List<Segment> segments { get; set; }
}
public class Segment
{
string speaker_label { get; set; }
float start_time { get; set; }
float end_time { get; set; }
List<SpeakerItems> items { get; set; }
}
public class SpeakerItems
{
float start_time { get; set; }
float end_time { get; set; }
string speaker_label { get; set; }
}
public class Transcript
{
public string transcript { get; set; }
}

public class TranscriptItem
{
public string start_time { get; set; }
public string end_time { get; set; }
public List<AlternativeTranscription> alternatives { get; set; }
public string type { get; set; }
}

public class AlternativeTranscription
{
public string confidence { get; set; }
public string content { get; set; }
}
*/