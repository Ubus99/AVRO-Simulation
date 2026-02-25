using System;
using System.IO;
using UnityEngine;

namespace Logging
{
    public static class LogUtils
    {
        static readonly string LogBasePath =
            Path.Combine(Application.dataPath, "logs");

        static void EnsureDirectory(string subdirectory = null)
        {
            var dir = string.IsNullOrEmpty(subdirectory)
                ? LogBasePath
                : Path.Combine(LogBasePath, subdirectory);

            Directory.CreateDirectory(dir);
        }

        // Single source of truth for file paths
        static string GetFilePath(
            string name,
            string extension,
            DateTime time,
            string subdirectory = null)
        {
            var dir = string.IsNullOrEmpty(subdirectory)
                ? LogBasePath
                : Path.Combine(LogBasePath, subdirectory);

            var fileName = $"{time:yyyy-MM-dd_HH-mm-ss}_{name}.{extension.TrimStart('.')}";
            return Path.Combine(dir, fileName);
        }

        static StreamWriter Open(string fullPath)
        {
            return File.AppendText(fullPath);
        }

        public static (StreamWriter writer, string path) CreateLogWriter(
            string name,
            string extension,
            string subdirectory = null,
            DateTime? time = null)
        {
            var t = time ?? DateTime.Now;

            EnsureDirectory(subdirectory);
            var path = GetFilePath(name, extension, t, subdirectory);

            return (Open(path), path);
        }

        public static (StreamWriter writer, string path) RenameLogFileAndReopen(
            StreamWriter writer,
            string currentPath,
            string newName,
            string extension,
            string subdirectory = null,
            DateTime? time = null)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (!File.Exists(currentPath)) throw new FileNotFoundException(currentPath);

            try
            {
                writer.Flush();
                writer.Dispose();
            }
            catch (ObjectDisposedException)
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var t = time ?? DateTime.Now;

            EnsureDirectory(subdirectory);
            var newPath = GetFilePath(newName, extension, t, subdirectory);

            File.Move(currentPath, newPath);

            return (Open(newPath), newPath);
        }
    }
}
