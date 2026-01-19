using System;
using System.IO;
using UnityEngine;
using Utils.Types;
using Object = UnityEngine.Object;

namespace Utils
{
    /// <summary>
    ///     Inspired by:
    ///     https://medium.com/@ahmetfarukgntrkn/scalable-multi-target-logging-system-for-unity-console-file-api-and-beyond-scriptableobject-5550549a4fc3
    ///     and https://docs.unity3d.com/6000.3/Documentation/ScriptReference/ILogHandler.html
    /// </summary>
    public class Logger : AbstractSingleton<Logger>, ILogHandler, IDisposable
    {
        const string FileExtension = ".log";
        static readonly string LogBasePath = Path.Combine(Application.persistentDataPath, "logs");

        readonly ILogHandler _defaultLogHandler = Debug.unityLogger.logHandler;

        string _logFilePath;
        string _newFilePath;
        StreamWriter _streamWriter;

        public void Dispose()
        {
            _streamWriter.Dispose();
            File.Move(_logFilePath, _newFilePath);
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            _streamWriter.WriteLine(format, args);
            _streamWriter.Flush();
            _defaultLogHandler.LogFormat(logType, context, format, args); // log to console also
        }

        public void LogException(Exception exception, Object context)
        {
            _defaultLogHandler.LogException(exception, context);
        }

        public void Init(string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = $"{DateTime.Now:yyyy-MM-dd_HH-mm}";
            }

            try
            {
                _logFilePath = _newFilePath = Path.Combine(LogBasePath, name + FileExtension);
                Directory.CreateDirectory(LogBasePath);
                _streamWriter = File.AppendText(_logFilePath);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }

            Debug.unityLogger.logHandler = this;

            Debug.Log($"Log File created @{_logFilePath}");
        }

        public void RenameLog(string name)
        {
            _newFilePath = Path.Combine(LogBasePath, name, FileExtension);
        }
    }
}
