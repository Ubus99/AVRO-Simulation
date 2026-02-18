using System;
using System.IO;
using UnityEngine;

namespace Logging
{
    public static class LogUtils
    {
#if UNITY_EDITOR
        static readonly string LogBasePath = Path.Combine(Application.dataPath, "logs");
#else
        static readonly string LogBasePath = Path.Combine(Application.dataPath, "logs");
#endif

        public static void CreateLogDirectory()
        {
            Directory.CreateDirectory(LogBasePath);
        }

        public static void CreateLogDirectory(string directory)
        {
            var path = Path.Combine(LogBasePath, directory);
            Directory.CreateDirectory(path);
        }

        public static string GenerateFileName(string baseName, DateTime creationTime, string extension)
        {
            return $"{baseName}_{creationTime:yyyyMMdd-HHmm}{extension}";
        }

        public static string GenerateFileName(string baseName, DateTime creationTime, Type identifier, string extension)
        {
            return $"{baseName}_{creationTime:yyyyMMdd-HHmm}_{identifier.Name}{extension}";
        }

        public static string GenerateFilePath(string baseName, DateTime creationTime, string extension)
        {
            return Path.Combine(LogBasePath, GenerateFileName(baseName, creationTime, extension));
        }

        public static string GenerateFilePath(string baseName, DateTime creationTime, Type identifier, string extension)
        {
            return Path.Combine(LogBasePath, GenerateFileName(baseName, creationTime, identifier, extension));
        }
    }
}
