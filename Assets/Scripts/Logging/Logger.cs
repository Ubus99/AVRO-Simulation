using System;
using System.IO;
using UnityEngine;
using Utils.Types;
using Object = UnityEngine.Object;

namespace Logging
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
        bool _closed;

        string _logFilePath;
        StreamWriter _streamWriter;
        string _subDirectory;

        public void Dispose()
        {
            _streamWriter.Dispose();
            _closed = true;
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            if (_closed)
            {
                throw new ObjectDisposedException(nameof(Logger));
            }
            _streamWriter.WriteLine($"[{DateTime.Now:hh:mm:ss:ff}] {string.Format(format, args)}");
            _streamWriter.Flush();
            _defaultLogHandler.LogFormat(logType, context, format, args); // log to console also
        }

        public void LogException(Exception exception, Object context)
        {
            _defaultLogHandler.LogException(exception, context);
        }

        public void Init(string directory, string name)
        {
            _subDirectory = directory;
            (_streamWriter, _logFilePath) =
                LogUtils.CreateLogWriter(name, FileExtension, directory, _creationTime);

            Debug.unityLogger.logHandler = this;

            _closed = false;
            Debug.Log($"Log File created @{_logFilePath}");
        }

        public void RenameLog(string newDirectory, string newName)
        {
            _subDirectory = newDirectory;
            (_streamWriter, _logFilePath) =
                LogUtils.RenameLogFileAndReopen(
                _streamWriter,
                _logFilePath,
                newName,
                FileExtension,
                _subDirectory,
                _creationTime);

            _closed = false;
        }
    }
}
