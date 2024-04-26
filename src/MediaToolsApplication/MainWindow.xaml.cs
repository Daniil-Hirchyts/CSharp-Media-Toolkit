using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FFmpeg.AutoGen;
using MediaToolsApplication;
using Microsoft.Win32;
using Tools;
using Vosk;
using Path = System.IO.Path;


namespace MediaToolsApp;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public ObservableCollection<string> accepted_languages { get; set; } = new ObservableCollection<string>();
    public string language { get; set; } = "";
    private string color = "";

    public int NumberOfFrames;
    private string path_to_shots = "";


    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = this;
        InitializeLanguages();
        CheckFfmpegPath();
        bool isSupported = CheckLibMp3LameSupport();
        Debug.WriteLine("libmp3lame supported: " + isSupported);
    }

    public static bool CheckLibMp3LameSupport()
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "ffmpeg";
            psi.Arguments = "-encoders";
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            Process process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output.Contains("libmp3lame");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error checking libmp3lame support: " + ex.Message);
            return false;
        }
    }

    private void CheckFfmpegPath()
    {
        var ffmpegPath = AppSettings.ReadSetting("ffmpegPath");
        if (File.Exists(ffmpegPath))
        {
            Path_FFMPEG.Text = ffmpegPath;
            ffmpeg.RootPath = Path.GetDirectoryName(ffmpegPath);
            Button_Download.Visibility = Visibility.Hidden;
        }
        else MessageBox.Show("FFmpeg binaries not found. Please select the path to FFmpeg.");
    }

    private void Button_SelectAudio_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Audio Files (*.wav;*.mp3)|*.wav;*.mp3|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            SelectedAudioPath.Text = openFileDialog.FileName;
        }
    }

    private async void Button_Transcribe_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(SelectedAudioPath.Text))
        {
            MessageBox.Show("Please select an audio file first.");
            return;
        }
        //model folder is C:\Users\Daniil\Music\vosk-model-eu
        Model model = new Model(@"C:\Users\Daniil\Music\vosk-model-eu");
        //44.1 kHz
        VoskRecognizer recognizer = new VoskRecognizer(model, 44100.0f);
        using (Stream source = File.OpenRead(SelectedAudioPath.Text))
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (recognizer.AcceptWaveform(buffer, bytesRead))
                {
                    TranscriptionResults.Text += recognizer.Result();
                }
                else
                {
                    TranscriptionResults.Text += recognizer.PartialResult();
                }
            }
            TranscriptionResults.Text += recognizer.FinalResult();
        }
    }


    private void Button_Download_FFMPEG_Click(object sender, RoutedEventArgs e)
    {
        Process.Start("https://www.gyan.dev/ffmpeg/builds/packages/ffmpeg-6.1.1-full_build-shared.7z");
    }

    private void Button_Select_FFMPEG_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.FileName = "ffmpeg";
        dialog.DefaultExt = ".exe";
        dialog.Filter = "Executable files (.exe)|*.exe";
        var result = dialog.ShowDialog();
        if (result != true) return;
        var filename = dialog.FileName;
        Path_FFMPEG.Text = filename;
        AppSettings.UpdateAppSettings("ffmpegPath", filename);
        ffmpeg.RootPath = Path.GetDirectoryName(filename);
    }

    private void IncrementNbFrames_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (int.TryParse(NbFrames.Text, out var nbFrames)) NbFrames.Text = (nbFrames + 1).ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void DecrementNbFrames_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (int.TryParse(NbFrames.Text, out var nbFrames) && nbFrames > 0)
                NbFrames.Text = (nbFrames - 1).ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void IncrementNumeroShot_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!int.TryParse(NumeroShot.Text, out var numeroShot)) return;
            NumeroShot.Text = (numeroShot + 1).ToString();
            Button_Shot(sender, e);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void DecrementNumeroShot_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!int.TryParse(NumeroShot.Text, out var numeroShot) || numeroShot <= 0) return;
            NumeroShot.Text = (numeroShot - 1).ToString();
            Button_Shot(sender, e);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void ShotTimeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var shotIndex = (int)e.NewValue;
        NavigateToShot(shotIndex);
        NumeroShot.Text = shotIndex.ToString();
    }

    private void NavigateToShot(int shotIndex)
    {
        if (string.IsNullOrEmpty(path_to_shots)) return;
        var path = Path.GetFullPath(path_to_shots);
        var fileName = $"{shotIndex:D3}.jpg";

        var fullPath = Path.Combine(path, fileName);

        if (File.Exists(fullPath))
        {
            var uripath = new Uri(fullPath);
            Shot_image.Source = new BitmapImage(uripath);
        }
        else MessageBox.Show($"Shot image file not found: {fullPath}");
    }

    private void Button_Shot(object sender, RoutedEventArgs e)
    {
        NavigateToShot(int.Parse(NumeroShot.Text));
    }

    private void FloatValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void IntValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void InitializeLanguages()
    {
        accepted_languages.Add("Français");
        accepted_languages.Add("English (US)");
    }

    ///////////////////////////// WaveForm
    private void Button_WaveForm_AddName_Click(object sender, RoutedEventArgs e)
    {
        // Configure open file dialog box
        var dialog = new OpenFileDialog();
        dialog.FileName = "Document"; // Default file name
        dialog.DefaultExt = ".mp3"; // Default file extension
        // dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

        // Show open file dialog box
        var result = dialog.ShowDialog();

        // Process open file dialog box results
        if (result == true)
        {
            // Open document
            var filename = dialog.FileName;
            Path_waveform.Text = filename;
        }
    }

    private void ButtonWaveForm_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var pathToWaveform = AudioTools.WaveForm(Path_waveform.Text, color, mixed.IsChecked);
            Debug.WriteLine($"Path to waveform: {pathToWaveform}");

            // Correct URI format for local files
            var uriPath = new Uri("file:///" + pathToWaveform.Replace("\\", "/")); // Ensure correct URI format

            // Ensure BitmapImage is loaded properly
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = uriPath;
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // This option helps with files in use
            bitmap.EndInit();
            bitmap.Freeze(); // Makes the image usable across threads

            WaveForm_image.Source = bitmap;
        }
        catch (FileNotFoundException)
        {
            MessageBox.Show("Please select an audio or video file.");
        }
        catch (InvalidExtensionException ex)
        {
            MessageBox.Show(ex.Message);
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message);
        }
        catch (Exception ex) // Catch other exceptions, might give clues on other errors
        {
            MessageBox.Show("An error occurred: " + ex.Message);
        }
    }


    private void Colors_ListBoxItem_Selected(object sender, RoutedEventArgs e)
    {
        var item = e.OriginalSource as ListBoxItem;

        if (item != null) color = item.Content.ToString();
    }


    ///////////////////////////// Shots
    private void Button_Shots_AddName_Click(object sender, RoutedEventArgs e)
    {
        // Configure open file dialog box
        var dialog = new OpenFileDialog();
        dialog.FileName = "Document"; // Default file name
        dialog.DefaultExt = ".mp4"; // Default file extension
        // dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

        // Show open file dialog box
        var result = dialog.ShowDialog();

        // Process open file dialog box results
        if (result == true)
        {
            // Open document
            var filename = dialog.FileName;
            Path_shots.Text = filename;
        }
    }

    private void ButtonShots_Click(object sender, RoutedEventArgs e)
    {
        var sw = new Stopwatch();
        sw.Start();
        var pathShots = Path_shots.Text;
        var threshold = Threshold.Text;

        var progressIndicator = new Progress<int>(value =>
        {
            ShotDetectionProgress.Value = value;
            ShotDetectionProgressText.Text = value + "%";
        });


        Task.Run(() =>
        {
            try
            {
                Dispatcher.Invoke(() => { ProgressPanel.Visibility = Visibility.Visible; });

                path_to_shots = VideoTools.ShotDetection(pathShots, threshold, progressIndicator);

                Dispatcher.Invoke(() =>
                {
                    var time_stamps = VideoTools.ShotTimeStamps(path_to_shots + "metadata.txt");

                    NumberOfFrames = time_stamps.Count;
                    NbFrames.Text = NumberOfFrames.ToString();
                    ShotTimeline.Maximum = NumberOfFrames;
                    ProgressPanel.Visibility = Visibility.Hidden;
                    sw.Stop();
                    TimeElapsed.Text = $"Elapsed time: {sw.Elapsed}";
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => { MessageBox.Show("Error during shot detection: " + ex.Message); });
            }
        });
    }


    /////////////// Transcription
    private void Button_Transcription_AddName_Click(object sender, RoutedEventArgs e)
    {
        // Configure open file dialog box
        var dialog = new OpenFileDialog();
        dialog.FileName = "Document"; // Default file name
        dialog.DefaultExt = ".wav"; // Default file extension
        // dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

        // Show open file dialog box
        var result = dialog.ShowDialog();

        // Process open file dialog box results
        if (result == true)
        {
            // Open document
            var filename = dialog.FileName;
            Path_transcription.Text = filename;
        }
    }

    private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count <= 0) return;
        var selectedLanguage = e.AddedItems[0]?.ToString();
        language = selectedLanguage switch
        {
            "Français" => "fr-FR",
            "English (US)" => "en-US",
            _ => language
        };
    }


    private async void ButtonTranscription_Click(object sender, RoutedEventArgs e)
    {
        Stopwatch stopwatchTotal = Stopwatch.StartNew();
        Stopwatch stopwatchAction = new Stopwatch();

        CurrentAction.Text = "Preparing files for extraction...";
        var videoPath = Path_transcription.Text;
        var language = this.language;
        string fileName = Path.GetFileNameWithoutExtension(videoPath);
        var outputPath = Path.Combine(Path.GetDirectoryName(videoPath), fileName + ".srt");

        // Create a list to store action timings
        List<string> actionTimings = new List<string>();

        var progressIndicator = new Progress<int>(percent =>
        {
            Dispatcher.Invoke(() =>
            {
                TranscriptionProgress.Value = percent;
                TranscriptionProgressText.Text = $"Progress: {percent}%";
            });
        });

        var cloudStorageService = new CloudStorageService("video-storage-test-v1");

        try
        {
            stopwatchAction.Restart();
            CurrentAction.Text = "Extracting audio from video...";
             //var audioPath = SpeechToText.ExtractAudioFromVideoToWAV(videoPath, fileName + ".wav");
            var audioPath = SpeechToText.ExtractAudioFromVideoToMP3(videoPath, fileName + ".mp3");
            actionTimings.Add($"Extracting audio: {stopwatchAction.ElapsedMilliseconds} ms");

            stopwatchAction.Restart();
            CurrentAction.Text = "Uploading audio file to Google Cloud Storage...";
            // var gcsUri = await cloudStorageService.UploadFileAsync(audioPath);
            actionTimings.Add($"Uploading audio to GCS: {stopwatchAction.ElapsedMilliseconds} ms");

            stopwatchAction.Restart();
            CurrentAction.Text = "Transcribing audio...";
            // await SpeechToText.TranscribeAndSaveSubtitles(gcsUri, language, outputPath, progressIndicator);
            actionTimings.Add($"Transcribing audio: {stopwatchAction.ElapsedMilliseconds} ms");

            CurrentAction.Text = "Transcription completed!";
        }
        catch (Exception ex)
        {
            Dispatcher.Invoke(() =>
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show("Error during transcription: " + ex.Message, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            });

            CurrentAction.Text = "Error.";
        }
        finally
        {
            stopwatchTotal.Stop();
            actionTimings.Add($"Total time: {stopwatchTotal.ElapsedMilliseconds} ms");

            // Save action timings to a file
            string timingsFilePath = Path.Combine(Path.GetDirectoryName(videoPath), fileName + "_timings.txt");
            File.WriteAllLines(timingsFilePath, actionTimings);
        }
    }


////////////////////////// Translation

    private async void ButtonTranslation_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var translation = new Translation();

            var res_text = await translation.TranslateAsync(Text_to_translate.Text,
                LanguageSourceCombo_translation.SelectionBoxItem.ToString(),
                LanguageTargetCombo_translation.SelectionBoxItem.ToString());

            Console.WriteLine(res_text);

            Translation_result.Text = res_text;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        // Configure open file dialog box
        var dialog = new OpenFileDialog();
        dialog.FileName = "A Video file"; // Default file name
        dialog.DefaultExt = ".mp4"; // Default file extension
        // dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

        // Show open file dialog box
        var result = dialog.ShowDialog();

        // Process open file dialog box results
        if (result == true)
        {
            // Open document
            var filename = dialog.FileName;
            videoFile.Text = filename;
        }
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        // Configure save file dialog box
        var dialog = new SaveFileDialog();
        dialog.FileName = "A Video file"; // Default file name
        dialog.DefaultExt = ".bmp"; // Default file extension
        dialog.Filter = "Bitmap (.bmp)|*.bmp|JPEG (*.jpeg)|*.jpeg|PNG (*.png)|*.png"; // Filter files by extension

        // Show open file dialog box
        var result = dialog.ShowDialog();

        // Process open file dialog box results
        if (result == true)
        {
            // Open document
            var filename = dialog.FileName;
            outputFile.Text = filename;
        }
    }

    // private void Button_Click_2(object sender, RoutedEventArgs e)
    // {
    //     logBox.Document.Blocks.Clear();
    //
    //     logBox.AppendText(" *** Grab Image ***" + Environment.NewLine);
    //     logBox.AppendText(" *** Input  : " + videoFile.Text + Environment.NewLine);
    //     logBox.AppendText(" *** Output : " + outputFile.Text + Environment.NewLine);
    //     logBox.AppendText(" *** From second N° : " + frameToGrab.Text + Environment.NewLine);
    //     logBox.AppendText(" *** Width : " + WidthImg.Text + Environment.NewLine);
    //     logBox.AppendText(" *** Height : " + HeightImg.Text + Environment.NewLine);
    //     Stopwatch sw = new();
    //     sw.Start();
    //     logBox.AppendText(" *** START " + Environment.NewLine);
    //
    //
    //     using (var process = new Process
    //            {
    //                StartInfo =
    //                {
    //                    FileName = @"ffmpeg",
    //                    Arguments = "-ss \"" + frameToGrab.Text + "\" -i \"" + videoFile.Text +
    //                                "\" -frames:v 1 -s " + WidthImg.Text + "x" + HeightImg.Text + " -f image2 \"" +
    //                                outputFile.Text + "\"",
    //                    UseShellExecute = false, CreateNoWindow = true,
    //                    RedirectStandardOutput = true, RedirectStandardError = true
    //                },
    //                EnableRaisingEvents = true
    //            })
    //     {
    //         logBox.AppendText(" *** CMD LINE => ffmpeg -ss " + frameToGrab.Text + " -i \"" + videoFile.Text +
    //                           "\" -frames:v 1 -s " + WidthImg.Text + "x" + HeightImg.Text + " -f \"" +
    //                           outputFile.Text + "\"" + Environment.NewLine);
    //         //store outcome of process
    //         var errors = new StringBuilder();
    //         var output = new StringBuilder();
    //         var hadErrors = false;
    //
    //         // capture normal output
    //         process.OutputDataReceived += (s, d) => { output.Append(d.Data); };
    //
    //         // Capture error output
    //         process.ErrorDataReceived += (s, d) =>
    //         {
    //             if (!hadErrors)
    //             {
    //                 if (d.Data != null)
    //                 {
    //                     if (d.Data.EndsWith("Invalid data found when processing input"))
    //                     {
    //                         hadErrors = true;
    //                     }
    //                 }
    //                 // hadErrors = !String.IsNullOrEmpty(d.Data);
    //             }
    //
    //             errors.Append(d.Data);
    //         };
    //
    //         if (!process.Start())
    //         {
    //             logBox.AppendText("Error starting" + Environment.NewLine);
    //         }
    //
    //         // start listening on the stream
    //         process.BeginErrorReadLine();
    //         process.BeginOutputReadLine();
    //
    //         process.WaitForExit();
    //         string stdout = output.ToString();
    //         logBox.AppendText(stdout + Environment.NewLine);
    //         string stderr = errors.ToString();
    //
    //         if (process.ExitCode != 0 || hadErrors)
    //         {
    //             logBox.AppendText(stderr + Environment.NewLine);
    //         }
    //     }
    //
    //     logBox.AppendText(" END *** " + Environment.NewLine);
    //     sw.Stop();
    //     logBox.AppendText(" Execution Time : " + sw.ElapsedMilliseconds + " ms" + Environment.NewLine);
    // }

    /*
     * Cette méthode est appelée lorsque l'événement Button_Click_2 est déclenché.
     * Elle efface la boîte de journalisation (logBox) et affiche des informations sur le processus d'extraction d'image.
     */
    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        // Efface la logBox
        logBox.Document.Blocks.Clear();

        // Affiche des informations sur le processus d'extraction
        logBox.AppendText(" *** Extraction d'Image ***" + Environment.NewLine);
        logBox.AppendText(" *** Entrée  : " + videoFile.Text + Environment.NewLine);
        logBox.AppendText(" *** Sortie : " + outputFile.Text + Environment.NewLine);
        logBox.AppendText(" *** Depuis la seconde N° : " + frameToGrab.Text + Environment.NewLine);
        logBox.AppendText(" *** Largeur : " + WidthImg.Text + Environment.NewLine);
        logBox.AppendText(" *** Hauteur : " + HeightImg.Text + Environment.NewLine);
        var sw = Stopwatch.StartNew();
        logBox.AppendText(" *** DÉBUT " + Environment.NewLine);

        try
        {
            // Extraction des paramètres pour l'extraction d'image
            var videoFileText = videoFile.Text;
            var outputFileText = outputFile.Text;
            var frameToGrabInt = int.Parse(frameToGrab.Text);
            var widthImgInt = int.Parse(WidthImg.Text);
            var heightImgInt = int.Parse(HeightImg.Text);

            VideoTools.ExtractFrameToImage(videoFileText, outputFileText, frameToGrabInt, widthImgInt,
                heightImgInt);
        }
        catch (Exception ex)
        {
            // Affiche un message d'erreur dans la logBox si une exception se produit
            logBox.AppendText("Erreur : " + ex.Message + Environment.NewLine);
        }

        // Affiche la fin du processus et le temps d'exécution
        logBox.AppendText(" FIN *** " + Environment.NewLine);
        sw.Stop();
        logBox.AppendText(" Temps d'Exécution : " + sw.ElapsedMilliseconds + " ms" + Environment.NewLine);
    }
}