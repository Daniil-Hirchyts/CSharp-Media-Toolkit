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
    public static class FileFolderTool
    {
        /// <summary>
        /// All the possible audio extension supported.
        /// </summary>
        public static readonly string[] all_audio_extensions = new string[] { "mp3",
                                                                              "wav",};

        /// <summary>
        /// All the possible video extension supported.
        /// </summary>
        public static readonly string[] all_video_extensions = new string[] { "mp4",
                                                                              "mov",
                                                                              "mxf"};

        /// <summary>
        /// All the possible audio and video extension supported.
        /// </summary>
        public static readonly string[] all_extensions = all_audio_extensions.Concat(all_video_extensions).ToArray<string>();

        /// <summary>
        /// All the possible data extension supported.
        /// </summary>
        public static readonly string[] data_extensions = new string[] { "txt",
                                                                         "",
                                                                         "csv"};

        /// <summary>
        /// Create a directory if it does not already exist. The path is either relative ("./...") or absolute (C://...)
        /// </summary>
        /// <param name="path">Path to create.</param>
        /// <returns>Void</returns>
        public static string CreateDirectory(string path)
        {
            // Create the directory with the path path if it does not exist
            Console.WriteLine($"Creation of the directory {path}");

            char[] file_separators = new char[] { '/', '\\' };
            string[] files = path.Split(file_separators);
            bool first_dir = true;
            string created_folder = @"";
            foreach (string f in files)
            {
                if (first_dir)
                {
                    created_folder = @"" + f + "\\";
                    if (f == "." || f == "..")
                    {
                        FileInfo fi = new FileInfo(created_folder);
                        created_folder = fi.FullName;
                    }

                    first_dir = false;
                }
                else
                {
                    if (f != "")
                    {
                        created_folder = created_folder + f + "\\";
                        System.IO.Directory.CreateDirectory(created_folder);
                    }

                }
            }

            if (created_folder == "")
            {
                Console.WriteLine($"Empty folder, none created.");
            }
            else
            {
                Console.WriteLine($"Created folder {created_folder}");
            }

            return created_folder;
        }

        /// <summary>
        /// Computes the name of the filename in the path.
        /// </summary>
        /// <param name="filename">Path to the file</param>
        /// <returns>String</returns>
        public static string GetName(string filename)
        {
            char[] separators = new char[] { '/', '\\' };
            return filename.Split(separators).Last();
        }

        /// <summary>
        /// Computes the extension of the filename.
        /// </summary>
        /// <param name="filename">Path to the file</param>
        /// <returns>String</returns>
        public static string GetExtension(string filename)
        {
            if (filename.Contains('.'))
            {
                return filename.Split('.').Last();
            }
            return "";

        }

        /// <summary>
        /// Computes the extension of the filename and returns also the name part.
        /// </summary>
        /// <param name="filename">Path to the file</param>
        /// <returns>String</returns>
        public static string[] GetNameAndExtension(string filename)
        {
            int n = filename.Length;
            int i = n - 1;
            while (i > 0)
            {
                if (filename[i] == '.')
                {
                    return new string[] { filename.Substring(0, i), filename.Substring(i + 1) };
                }
                i--;
            }
            return new string[] { filename, "" };
        }


        public static string CheckFileAndExt(string filename,
                                           string[]? accepted_extensions = null)
        {
            char[] separators = new char[] { '/', '\\' };
            string fname = filename.Split(separators).Last();

            // Check file
            if (fname == "")
            {
                throw new NoFileException($"No file input.");
            }

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"The input file {filename} does not exist.");
            }

            // Check extension
            if (accepted_extensions != null)
            {
                string ext = GetExtension(fname);

                /*if (ext == "")
                {
                    throw new InvalidExtensionException($"The file has no extension.");
                }*/

                if (!Array.Exists(accepted_extensions, e => e == ext))
                {
                    throw new InvalidExtensionException($"The extension \"{ext}\" is not supported here (yet?).");
                }
            }


            return fname;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class InvalidExtensionException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public InvalidExtensionException() : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public InvalidExtensionException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public InvalidExtensionException(string message, Exception inner) : base(message, inner) { }


        /// <summary>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InvalidExtensionException(SerializationInfo info,
                                                StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NoFileException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public NoFileException() : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NoFileException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public NoFileException(string message, Exception inner) : base(message, inner) { }


        /// <summary>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected NoFileException(SerializationInfo info,
                                    StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ThresholdException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public ThresholdException() : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ThresholdException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public ThresholdException(string message, Exception inner) : base(message, inner) { }


        /// <summary>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ThresholdException(SerializationInfo info,
                                        StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class LanguageException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public LanguageException() : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public LanguageException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public LanguageException(string message, Exception inner) : base(message, inner) { }


        /// <summary>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected LanguageException(SerializationInfo info,
                                        StreamingContext context) : base(info, context) { }
    }

    public class TemporaryFile : IDisposable
    {
        public TemporaryFile() : this(Path.GetTempPath()) { }

        public TemporaryFile(string directory)
        {
            Create(Path.Combine(directory, Path.GetRandomFileName()));
        }

        public TemporaryFile(string directory, string extension)
        {
            var filename = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + extension;
            Create(Path.Combine(directory, filename));
        }


        ~TemporaryFile()
        {
            Delete();
        }

        public void Dispose()
        {
            Delete();
            GC.SuppressFinalize(this);
        }

        public string? FilePath { get; private set; }

        private void Create(string path)
        {
            FilePath = path;
            using (File.Create(FilePath)) { };
        }

        private void Delete()
        {
            if (FilePath == null) return;
            File.Delete(FilePath);
            FilePath = null;
        }
    }
}
