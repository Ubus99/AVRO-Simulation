using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Time = UnityEngine.Time;

namespace Logging
{
    public class CSVLogger<T> : IDisposable where T : BaseRecord
    {
        const string FileExtension = ".csv";
        readonly DateTime _creationTime = DateTime.Now;

        CsvWriter _csv;
        string _filePath;
        StreamWriter _streamWriter;
        string _subdirectory;

        public CSVLogger(string directory, string name)
        {
            _subdirectory = directory;
            (_streamWriter, _filePath) =
                LogUtils.CreateLogWriter(
                $"{name}_{typeof(T).Name}",
                FileExtension,
                directory,
                _creationTime);
            _csv = new CsvWriter(_streamWriter, CultureInfo.InvariantCulture);

            _csv.WriteHeader<T>();
            _csv.NextRecord();
        }

        public void Dispose()
        {
            try
            {
                _csv.Flush();
                _streamWriter.Flush();

                _csv.Dispose();
                _streamWriter.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Rename(string newDirectory, string newName)
        {
            _subdirectory = newDirectory;
            (_streamWriter, _filePath) =
                LogUtils.RenameLogFileAndReopen(
                _streamWriter,
                _filePath, // old path before assignment
                $"{newName}_{typeof(T).Name}",
                FileExtension,
                _subdirectory,
                _creationTime);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };
            _csv = new CsvWriter(_streamWriter, config);
        }

        public void Log(T record)
        {
            _csv.WriteRecord(record);
            _csv.NextRecord();
            _csv.Flush();
        }
    }

    [Serializable]
    public class BaseRecord
    {
        public string timestamp
        {
            get { return DateTime.Now.ToString("HH:mm:ss:ffff"); }
        }

        public float secondsSinceStart
        {
            get { return Time.timeSinceLevelLoad; }
        }
    }
}
