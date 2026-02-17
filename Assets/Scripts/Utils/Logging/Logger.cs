using System;
using System.IO;
using UnityEngine;
using Utils.Types;
using Object = UnityEngine.Object;

namespace Utils.Logging
{
    /// <summary>
    ///     Inspired by:
    ///     https://medium.com/@ahmetfarukgntrkn/scalable-multi-target-logging-system-for-unity-console-file-api-and-beyond-scriptableobject-5550549a4fc3
    ///     and https://docs.unity3d.com/6000.3/Documentation/ScriptReference/ILogHandler.html
    /// </summary>
    public class Logger : AbstractSingleton<Logger>, ILogHandler, IDisposable
    {
        const string FileExtension = ".log";

        readonly DateTime _creationTime = DateTime.Now;

        readonly ILogHandler _defaultLogHandler = Debug.unityLogger.logHandler;
        string _fileName;

        string _logFilePath;
        StreamWriter _streamWriter;

        public void Dispose()
        {
            _streamWriter.Dispose();
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            _streamWriter.WriteLine($"[{DateTime.Now:hh:mm:ss:ff}] {string.Format(format, args)}");
            _streamWriter.Flush();
            _defaultLogHandler.LogFormat(logType, context, format, args); // log to console also
        }

        public void LogException(Exception exception, Object context)
        {
            _defaultLogHandler.LogException(exception, context);
        }

        public void Init(string name = "log")
        {
            _fileName = name;
            try
            {
                _logFilePath = LogUtils.GenerateFilePath(_fileName, _creationTime, FileExtension);
                LogUtils.CreateLogDirectory();
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

        public void RenameLog(string newName)
        {
            Dispose();

            var newPath = LogUtils.GenerateFilePath(newName, _creationTime, FileExtension);
            var oldPath = LogUtils.GenerateFilePath(_fileName, _creationTime, FileExtension);
            File.Move(oldPath, newPath);
            _fileName = newName;

            _streamWriter = new StreamWriter(newPath, true);
        }
    }
}
