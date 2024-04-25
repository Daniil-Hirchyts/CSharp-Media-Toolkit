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
    public static class SceneDetectionTool
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input_filename">filename and path </param>
        /// <param name="outdir">folder where scene shots will be stored</param>
        /// <param name="threshold"></param>
        /// <param name="scale">to resize png created, default set at 500</param>
        /// <exception cref="NoFileException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InvalidExtensionException"></exception>
        /// <exception cref="ThresholdException"></exception>
        public static void GetSceneShots(string input_filename,
            string outdir,
            string threshold,
            uint scale = 500)
        {
            char[] separators = new char[] { '/', '\\' };
            string fname = input_filename.Split(separators).Last();

            // Check file
            if (fname == "")
            {
                throw new NoFileException($"No file input.");
            }

            if (!File.Exists(input_filename))
            {
                throw new FileNotFoundException($"The input file {input_filename} does not exist.");
            }

            // Check extension
            string ext = FileFolderTool.GetExtension(fname);

            if (ext == "")
            {
                throw new InvalidExtensionException($"The file has no extension.");
            }

            // Check threshold
            double th;
            bool th_good = double.TryParse(threshold, NumberStyles.Any, CultureInfo.InvariantCulture, out th);
            if (!th_good)
            {
                throw new ThresholdException($"The threshold must be a double number (Format 0.00).");
            }

            if (th < 0 || th > 1)
            {
                throw new ThresholdException($"The threshold must be a percentage (between 0 and 1 included).");
            }

            string name = fname + "_" + threshold;

            outdir = outdir + "frames_" + name + "/";
            outdir = FileFolderTool.CreateDirectory(outdir);

            // Needed for the process to differentiate ":" from a parameters separator.
            string outdir_meta = outdir.Replace("\\", "/").Replace(":", "\\:");

            /*using (var process = new Process
            {
                StartInfo =
                        {
                            FileName = "ffmpeg", Arguments = "-y -i \"" + input_filename + "\" -vf \"select='gt(scene," + threshold + ")',scale=" + scale + ":-1,metadata=print:file='" + outdir_meta + "metadata.txt'\" -vsync vfr \"" + outdir + "%03d.jpg\"",
                            UseShellExecute = false, CreateNoWindow = true,
                            RedirectStandardOutput = true, RedirectStandardError = true
                        },
                EnableRaisingEvents = true

            })
            {

            }*/

            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                FileName = "ffmpeg",
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                Arguments =
                    $"-i {input_filename} -vf select='gt(scene\\,0.4)',scale=500:-1,metadata=print:file='{outdir_meta}metadata.txt' -fps_mode vfr {outdir}\\frames%d.png",
            };

            Process process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };
        }
    }
}