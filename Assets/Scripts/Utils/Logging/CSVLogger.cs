using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Time = UnityEngine.Time;

namespace Utils.Logging
{
    public class CSVLogger<T> : IDisposable where T : BaseRecord
    {
        const string FileExtension = ".csv";
        readonly DateTime _creationTime = DateTime.Now;

        CsvWriter _csv;
        string _fileName;
        StreamWriter _streamWriter;

        public CSVLogger(string fileName)
        {
            _fileName = fileName;

            LogUtils.CreateLogDirectory();

            _streamWriter =
                new StreamWriter(LogUtils.GenerateFilePath(_fileName, _creationTime, typeof(T), FileExtension));
            _csv = new CsvWriter(_streamWriter, CultureInfo.InvariantCulture);

            _csv.WriteHeader<T>();
            _csv.NextRecord();
        }

        public void Dispose()
        {
            _csv.Flush();
            _streamWriter.Flush();

            _csv.Dispose();
            _streamWriter.Dispose();
        }

        public void Rename(string newName)
        {
            Dispose();

            var newPath = LogUtils.GenerateFilePath(newName, _creationTime, typeof(T), FileExtension);
            var oldPath = LogUtils.GenerateFilePath(_fileName, _creationTime, typeof(T), FileExtension);
            File.Move(oldPath, newPath);
            _fileName = newName;

            _streamWriter = new StreamWriter(newPath, true);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };
            _csv = new CsvWriter(_streamWriter, config);
            //_csv.NextRecord();
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
