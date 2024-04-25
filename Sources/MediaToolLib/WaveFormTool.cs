using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.Drawing;

namespace MediaToolLib
{
    public static class WaveFormTool
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">filename and path</param>
        /// <param name="height">height in px</param>
        /// <param name="width">width in px</param>
        /// <param name="color">color in lower case, will be changed to lowercase otherwise</param>
        /// <returns></returns>
        /// <exception cref="NoFileException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InvalidExtensionException"></exception>
        public static Image MakeWaveForm(string input,
                                                int height = 400,
                                                int width = 600,
                                                string color = "red",
                                                int audioStreamId = 0)
        {

            TemporaryFile temporaryFile = new(Path.GetTempPath(), ".png");
            string name = Path.GetFileName(input);            
            string ext = Path.GetExtension(name);   
            
            if (string.IsNullOrEmpty(name))
            {
                throw new NoFileException($"No file input.");
            }

            if (!File.Exists(input))
            {
                throw new FileNotFoundException($"The input file {input} does not exist.");
            }

            if (string.IsNullOrEmpty(ext))
            {
                throw new InvalidExtensionException($"The file has no extension.");
            }

            ProcessStartInfo processStartInfo = new()
            {
                FileName = "ffmpeg",
                CreateNoWindow = true,
                //RedirectStandardInput = true,
                //RedirectStandardOutput = true,
                //RedirectStandardError = true,
                UseShellExecute = false,
                Arguments = $"-y -i \"{input}\" -filter_complex \"[0:a:0]compand,showwavespic=colors={color!.ToLowerInvariant()}:s={width}x{height}\" -frames:v 1 \"{temporaryFile.FilePath}\"", 
            };

            //{audioStreamId}

            Process process = new()
            {
                StartInfo = processStartInfo//,
                //EnableRaisingEvents = true
            };
            process.Start();
            process.WaitForExit();
            
            Debug.WriteLine($"File {temporaryFile.FilePath} created");

            Image? image = Image.FromFile(temporaryFile.FilePath);            
            Image imageClone = new Bitmap(image);
            image?.Dispose();
            temporaryFile.Dispose();
            temporaryFile = null;
            return imageClone;
        }
    }
}