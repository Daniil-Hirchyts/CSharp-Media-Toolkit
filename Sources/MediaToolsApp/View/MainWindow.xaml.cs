using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Tools;
using System.Text.RegularExpressions;


namespace MediaToolsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string color = "";
        private string path_to_shots = "";
        public int NumberOfFrames = 0;
        public string language = "";

    private void FloatValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void IntValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    public ObservableCollection<string> accepted_languages = new ObservableCollection<string>();
    private void InitializeLanguages()
    {
        accepted_languages.Add("Français");
        accepted_languages.Add("English (US)");
    }
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = this;
        InitializeLanguages();
    }




        ///////////////////////////// WaveForm
        private void Button_WaveForm_AddName_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".mp3"; // Default file extension
            // dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                Path_waveform.Text = filename;
            }
        }

        private void ButtonWaveForm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path_to_waveform = AudioTools.WaveForm(Path_waveform.Text, color: color, ismixed: mixed.IsChecked);
                Console.WriteLine(path_to_waveform);

                Uri uripath = new Uri(path_to_waveform);
                WaveForm_image.Source = new BitmapImage(uripath);
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("Please select an audio or video file");
            }
            catch (Tools.InvalidExtensionException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Colors_ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            
            if (item != null)
            {
                color = item.Content.ToString();
            }
        }





        ///////////////////////////// Shots
        private void Button_Shots_AddName_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".mp4"; // Default file extension
            // dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                Path_shots.Text = filename;
            }
        }

        private void ButtonShots_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                path_to_shots = VideoTools.ShotDetection(Path_shots.Text, Threshold.Text);

                try
                {
                    List<float> time_stamps = VideoTools.ShotTimeStamps(path_to_shots + "metadata.txt");

                    NumberOfFrames = time_stamps.Count;
                    NbFrames.Text = NumberOfFrames.ToString();
                }
                catch (NoFileException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (System.IO.FileNotFoundException)
                {
                    MessageBox.Show("Please select an audio or video file. " + path_to_shots);
                }
                catch (Tools.InvalidExtensionException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("Please select an audio or video file. " + path_to_shots);
            }
            catch (Tools.InvalidExtensionException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Shot(object sender, RoutedEventArgs e)
        {
            int num_shot = 0;
            try
            {
                num_shot = int.Parse(NumeroShot.Text);
                if (num_shot < 1 || num_shot > NumberOfFrames)
                {
                    MessageBox.Show($"The numero of the frame should be between 1 and {NumberOfFrames}");
                }
                else
                {
                    Uri uripath = new Uri(path_to_shots + $"{num_shot:000}.jpg");
                    Shot_image.Source = new BitmapImage(uripath);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The numero of the shot must be an integer.");
            }
        }




        /////////////// Transcription
        private void Button_Transcription_AddName_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".wav"; // Default file extension
            // dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                Path_transcription.Text = filename;
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected_language = e.AddedItems[0].ToString();
            switch (selected_language)
            {
                case "Français":
                    language = "fr-FR";
                    break;
                case "English (US)":
                    language = "en-US";
                    break;
            }
        }

        private void ButtonTranscription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path_to_waveform = AudioTools.WaveForm(Path_waveform.Text, color: color, ismixed: mixed.IsChecked);
                Console.WriteLine(path_to_waveform);

                Uri uripath = new Uri(path_to_waveform);
                WaveForm_image.Source = new BitmapImage(uripath);
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("Please select an audio or video file");
            }
            catch (Tools.InvalidExtensionException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
