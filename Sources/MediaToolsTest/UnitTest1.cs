using NUnit.Framework;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Tools;

namespace MediaToolsTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGetExtension()
        {
            string[] filenames = { "file.mp3", "file.mp3.wav", "mp3.wav.mp4", "file", "" };
            string[] true_results = { "mp3", "wav", "mp4", "", "" };
            int i = 0;
            foreach (var file in filenames)
            {
                string ext = Tools.UseTools.GetExtension(file);
                Assert.IsTrue(ext == true_results[i], string.Format("Expected for '{0}': true; Actual: {1}",
                                     true_results[i], ext));
                i++;
            }
        }

        [Test]
        public void TestCreateDirectory()
        {
            string[] paths = { "C://1/", "./2/3/4", "./../5", "../.././.././6", "" };
            string[] corrected_paths = { "C:/1/", "./2/3/4", "./../5", "../.././.././6", "" };
            int i = 0;
            foreach (var path in paths)
            {
                Tools.UseTools.CreateDirectory(path);
                
                if (path != "")
                {
                    FileInfo fi = new FileInfo(corrected_paths[i]);
                    string fif = fi.FullName;
                    Assert.IsTrue(Directory.Exists(fif), string.Format("Expected the path '{0}' to be created. Got {1}",
                                     path, fif));
                }

                else
                {
                    Assert.True(true);
                }
                
                i++;
            }
        }

        [Test]
        public void TestNotCreateDirectory()
        {
            string[] paths = { "D://1/", ".../2/3/4", "5", "/.././.././6" };
            string[] corrected_paths = { "D:/1/", ".../2/3/4", "5", "/.././.././6" };
            int i = 0;
            foreach (var path in paths)
            {
                try
                {
                    Tools.UseTools.CreateDirectory(path);
                    Assert.False(false, $"The path {path} has been created.");
                }
                catch (System.IO.DirectoryNotFoundException)
                {
                    Assert.True(true, $"The path {path} could not have been created");
                }

                i++;
            }
        }

        [Test]
        public void TestGetNameAndExtension()
        {
            string[] names = { "hey.mp3", "az.er.ty", "ab;cd!ef", "" };
            string[][] corrected_answers = { new string[] { "hey", "mp3" }, 
                                           new string[] { "az.er", "ty" }, 
                                           new string[] { "ab;cd!ef", "" },
                                           new string[] { "", "" } 
                                         };
            int i = 0;
            foreach (var name in names)
            {
                string[] res = Tools.UseTools.GetNameAndExtension(name);
                Assert.IsTrue(res[0] == corrected_answers[i][0] && res[1] == corrected_answers[i][1], $"Expected name: {corrected_answers[i][0]}, ext: {corrected_answers[i][1]}\nGot name: {res[0]}, ext: {res[1]}");
                i++;
            }
        }

        [Test]
        public void TestConvertToAudio()
        {
            string[] input_filenames = { @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Audios\mp3\Baker.mp4.mp3",
                                         @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Audios\mp3\Baker.mp4.mp3",
                                         @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Audios\mp3\a.txt",
                                         @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Audios\mp3\Baker.mp4.mp3",
                                         @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Audios\mp3\Baker.mp4.mp3",
                                         @"./../oui.mp3",
                                         @"./../vzrn",
                                         @"C:\Users\",
                                         @""};

            string[] extensions = {"mp3", ".wav", "mp3", "szrg", "mov", "wav", "mov", "mp3", "mov"};
            string outdir = @".\Tests\ConvertToAudio\";

            string[] correct_answers = { @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Audios\mp3\Baker.mp4.mp3",
                                         @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\ConvertToAudio\wav\Baker.mp4.wav",
                                         "The audio extension of the file",
                                         "The target audio extension",
                                         "The target audio extension",
                                         "Error",
                                         "The file has no extension.",
                                         "No file input.",
                                         "No file input."};
            int i = 0;
            foreach (var name in input_filenames)
            {
                System.Console.WriteLine("\n" + i + "\nInput: " + name + "\nExt: " + extensions[i] + "\nCorrect: " + correct_answers[i]);
                try
                {
                    string res = Tools.AudioTools.ConvertToAudio(name, extensions[i], outdir);
                    System.Console.WriteLine("Results: " + res);

                    Assert.IsTrue(res == correct_answers[i], $"Expected outfile path {correct_answers[i]}, got {res}");
                }
                catch (Tools.InvalidExtensionException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                catch (System.ArgumentException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                catch (Tools.NoFileException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                i++;
            }
        }

        [Test]
        public void TestWaveForm()
        {
            string[] files = { @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Audios\mp3\Baker.mp4.mp3",
                               @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Audios\mp3\Baker.mp4.mp3",
                               @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Audios\mp3\a.txt",
                               @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Videos\Baker.mp4",
                               @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Videos\Steel.mov",
                               @"C:\Users\",
                               @"",
                               @"./zfezf.mp4" };

            bool[] is_mixeds = { true, false, true, true, false, true, true, false };
            int[] heights = { 400, 800, 100, 400, 800, 100, 100, 100 };
            int[] widths = { 600, 2000, 100, 600, 2000, 100, 100, 100 };
            string[] colors = { "brown", "red", "yellow", "blue", "green", "yellow", "yellow", "yellow" };
            string outdir = @".\Tests\WaveForm\";

            string[] correct_answers = { @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\WaveForm\Baker.mp4.mp3-mixed-400x600-brown.png",
                                         @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\WaveForm\Baker.mp4.mp3-800x2000-red.png",
                                         "The video or audio extension",
                                         @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\WaveForm\Baker.mp4-mixed-400x600-blue.png",
                                         @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\WaveForm\Steel.mov-800x2000-green.png",
                                         "No file input.",
                                         "No file input.",
                                         "The input file"};

            int i = 0;
            foreach (var file in files)
            {
                System.Console.WriteLine("\n" + i + "\nInput: " + file + "\nMixed: " + is_mixeds[i] + "\nHxW: " + heights[i] + ", " + widths[i] + "\nColor: " + colors[i] + "\nCorrect: " + correct_answers[i]);
                try
                {
                    string res = Tools.AudioTools.WaveForm(file,
                                              ismixed: is_mixeds[i],
                                              height: heights[i],
                                              width: widths[i],
                                              color: colors[i],
                                              outdir: outdir);

                    System.Console.WriteLine(res);
                }
                catch (FileNotFoundException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                catch (Tools.NoFileException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                catch (Tools.InvalidExtensionException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                catch (System.ArgumentException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                i++;
            }
        }

        [Test]
        public void TestShotDetection()
        {
            string[] filenames = { @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Videos\Baker_short.mp4",
                                   @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Videos\Baker_short.mp4",
                                   @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Videos\Baker_short.mp4",
                                   @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Videos\Baker_short.mp4",
                                   @"",
                                   @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Videos\fehbi.mp4",
                                   @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Videos\sheep_short.mp4",
                                   @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Videos\Steel_short.mov",
                                   @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Audios\mp3\oui"};

            double[] thresholds = { 0.2, 0.3, 80, 2, 0.5, 0.5, 0.5, 0.2, 0.3, 0.5 };
            uint[] scales = { 400, 800, 100, 100, 100, 100, 100, 400, 800, 100 };
            string outdir = @".\Tests\ShotDetection\";

            string[] correct_answers = { @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\ShotDetection\frames_Baker_short.mp4_0.2\",
                                         @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\ShotDetection\frames_Baker_short.mp4_0.3\",
                                         "The threshold must be a double number (Format 0.00).",
                                         "The threshold must be a percentage",
                                         "No file input.",
                                         "The input file", 
                                         @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\ShotDetection\frames_sheep_short.mp4_0.2\",
                                         @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\ShotDetection\frames_Steel_short.mov_0.3\",
                                         "The file has no extension."
                                       };

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            int i = 0;
            foreach (var file in filenames)
            {
                System.Console.WriteLine("\n" + i + "\nInput: " + file + "\nThreshold: " + thresholds[i] + "\nScales: " + scales[i] + "\nCorrect: " + correct_answers[i]);
                try
                {
                    string res;
                    if (thresholds[i] == 80)
                    {
                        res = Tools.VideoTools.ShotDetection(file,
                                              threshold: "fr",
                                              scale: scales[i],
                                              outdir: outdir);
                    }
                    else
                    {
                        res = Tools.VideoTools.ShotDetection(file,
                                              threshold: thresholds[i].ToString(nfi),
                                              scale: scales[i],
                                              outdir: outdir);
                    }
                    

                    System.Console.WriteLine(res);
                }
                catch (System.ArgumentException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                catch (Tools.InvalidExtensionException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                catch (Tools.NoFileException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                catch (Tools.ThresholdException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                catch (FileNotFoundException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                System.Threading.Thread.Sleep(2000);
                i++;
            }
        }

        [Test]
        public void TestDeserializeJSON()
        {
            string[] json_strings = { @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Audios\mp3\a.txt" };

            // string outdir = @".\Tests\DeserializeJSON\";

            string[] correct_answers = { "null",
                                       };

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            int i = 0;
            foreach (var file in json_strings)
            {
                System.Console.WriteLine("\n" + i + "\nInput: " + file + "\nCorrect: " + correct_answers[i]);
                try
                {
                    Tools.SpeechToText.ResponseJSON res = Tools.SpeechToText.DeserializeJSON(file);
                    Console.WriteLine(res.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Assert.IsTrue(1==1, $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                i++;
            }
        }

        [Test]
        public void TestShotTimeStamps()
        {
            string[] filenames = { @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\ShotDetection\frames_Baker_short.mp4_0.2\metadata.txt",
                                   @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\oui_meta",
                                   @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Videos\Baker_short.mp4",
                                   @"",
                                   @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\Data\Videos\fehbi.mp4"};

            List<float> correct = new List<float> { 5.6f, 8.84f, 11.48f, 20.84f, 29.8f };

            string[] correct_answers = { @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\ShotDetection\frames_Baker_short.mp4_0.2\",
                                         @"C:\Users\Ce PC\Documents\Ecosm\Sources\MediaToolsTest\bin\Debug\net6.0-windows10.0.22000.0\Tests\ShotDetection\frames_Baker_short.mp4_0.3\",
                                         "The data extension",
                                         "No file input.",
                                         "The input file"
                                       };

            int i = 0;
            foreach (var file in filenames)
            {
                Console.WriteLine("\n" + i + "\nInput: " + file + "\nCorrect: " + correct_answers[i]);
                try
                {
                    List<float> res = VideoTools.ShotTimeStamps(file);
                    string s_res = "";
                    foreach (var item in res)
                    {
                        s_res += item + " ";
                    }
                    string s_correct = "";
                    foreach (var item in correct)
                    {
                        s_correct += item + " ";
                    }
                    Console.WriteLine("Results: " + s_res);
                    Console.WriteLine("Correct: " + s_correct);
                    Assert.IsTrue(s_res == s_correct, $"Expected the good result.");
                }
                catch (ArgumentException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                catch (NoFileException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                catch (FileNotFoundException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                catch (InvalidExtensionException e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                i++;
            }
        }

        [Test]
        public async void TestTranscriptInputFiles()
        {
            // CAUTION: payant
            string[] filenames = { ".mp4", ".mov" };
            string[] targetLanguageCode = { };
            string[] bucketName = { };
            string outdir = "";
            bool[] diarization = { };
            int[] MaxSpeakers = { };
            bool transcribe = false;

            string[] correct_answers = { };

            int i = 0;
            foreach (var filename in filenames)
            {
                System.Console.WriteLine("\n" + i +
                                         "\nInput: " + filename +
                                         "\nLanguage: " + targetLanguageCode[i] +
                                         "\nBucketName: " + bucketName[i] +
                                         "\nDiarization: " + diarization[i] +
                                         "\nMaxSpeakers: " + MaxSpeakers[i] + 
                                         "\nCorrect: " + correct_answers[i]);
                try
                {
                    Tools.SpeechToText.ResponseJSON[]? resps = await SpeechToText.TranscribeInputFile(
                                                                         fileName: filename,
                                                                         targetLanguageCode: targetLanguageCode[i],
                                                                         bucketName: bucketName[i],
                                                                         outdir:outdir,
                                                                         diarization: diarization[i],
                                                                         MaxSpeakers: MaxSpeakers[i],
                                                                         transcribe: transcribe
                                                                        );
                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }

                i++;
            }
        }


        [Test]
        public void TestUploadInputFileToS3()
        {
            string[] filenames = { ".mp4", ".mov" };
            string[] bucketNames = { };

            string[] correct_answers = { };
            /*int i = 0;
            foreach (var name in names)
            {
                string[] res = Tools.UseTools.GetNameAndExtension(name);
                Assert.IsTrue(res[0] == corrected_answers[i][0] && res[1] == corrected_answers[i][1], $"Expected name: {corrected_answers[i][0]}, ext: {corrected_answers[i][1]}\nGot name: {res[0]}, ext: {res[1]}");
                i++;
            }*/
        }

        [Test]
        public void TestDiarizationResponse()
        {
            Tools.SpeechToText.ResponseJSON[] responses = { };

            string[] correct_answers = { };
            /*int i = 0;
            foreach (var name in names)
            {
                string[] res = Tools.UseTools.GetNameAndExtension(name);
                Assert.IsTrue(res[0] == corrected_answers[i][0] && res[1] == corrected_answers[i][1], $"Expected name: {corrected_answers[i][0]}, ext: {corrected_answers[i][1]}\nGot name: {res[0]}, ext: {res[1]}");
                i++;
            }*/
        }

        [Test]
        public void TestCreateXMLFromJSON()
        {
            string[] path_to_jsons = { @"./oui.mp3",
                                       @"C:\Users\Ce PC\Documents\Ecosm\Sources\WaveForm\AmazonSpeechToText\Steel_short.mov.wav_diarization_2\response.json"};

            string[] shot_folders = { "" };
            string outdir = @".\Tests\CreateXMLFromJSON\";

            string[] correct_answers = { "The input file"};

            int i = 0;
            foreach (var path_to_json in path_to_jsons)
            {
                System.Console.WriteLine("\n" + i + 
                                         "\nInput: " + path_to_json +
                                         "\nShots: " + shot_folders[i] +
                                         "\nCorrect: " + correct_answers[i]);
                try
                {
                    Tools.SpeechToText.CreateXMLFromJSON(path_to_json:path_to_json,
                                                         shot_folder: shot_folders[i],
                                                         outdir: outdir);
                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }
                
                i++;
            }

        }

        [Test]
        public async Task TestTranslation()
        {
            Translation translation = new Translation();

            string[] texts = { "Hello", "Bonjour", "Bonjour", "Bonjour", "" };

            string[] from_language = { "en", "fr-FR", "fr-FR", "zer", "en-US" };
            string[] to_language = { "fr", "en-GB", "erv", "en-US", "fr-FR" };

            string[] correct_answers = { "Bonjour",
                                         "Hello",
                                         "The language code",
                                         "The language code",
                                         ""};

            int i = 0;
            foreach (var text in texts)
            {
                System.Console.WriteLine("\n" + i +
                                         "\nInput: " + text +
                                         "\nFrom: " + from_language[i] +
                                         "\nTo: " + to_language[i] +
                                         "\nCorrect: " + correct_answers[i]);
                try
                {
                    string res = await translation.TranslateAsync(text: text,
                                                                        from_language: from_language[i],
                                                                        to_language: to_language[i]);
                    Assert.IsTrue(res == correct_answers[i], $"Expected answer \"{correct_answers[i]}\" but got \"{res}\".");
                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.Message.StartsWith(correct_answers[i]), $"Expected error message \"{correct_answers[i]}\", but got \"{e.Message}\".");
                }

                i++;
            }

            translation.Dispose();
        }
    }
}