using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

namespace Utils
{
    public class CSVLogger<T> : IDisposable where T : BaseRecord
    {
#if UNITY_EDITOR
        readonly string _logBasePath = Path.Combine(Application.dataPath, "logs");
#else
        readonly string _logBasePath = Path.Combine(Application.persistentDataPath, "logs");
#endif

        readonly CsvWriter _csv;
        StreamWriter _streamWriter;
        string _fileName;

        public CSVLogger(string fileName)
        {
            _fileName = $"{fileName}_{DateTime.Now:yyyyMMdd-HHmm}_{typeof(T).Name}";
            Directory.CreateDirectory(_logBasePath);

            _streamWriter = new StreamWriter(fullPath);
            _csv = new CsvWriter(_streamWriter, CultureInfo.InvariantCulture);
        }

        string fullPath
        {
            get { return Path.Join(_logBasePath, $"{_fileName}.csv"); }
        }

        public void Rename(string newName)
        {
            _streamWriter.Dispose();
            File.Move(fullPath, Path.Join(_logBasePath, $"{newName}.csv"));
            _fileName = newName;
            _streamWriter = new StreamWriter(fullPath);
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
            _csv.Dispose();
        }

        public void Log(T record)
        {
            _csv.WriteRecord(record);
            _csv.NextRecord();
        }
    }

    public class BaseRecord
    {
        public string timestamp
        {
            get { return DateTime.Now.ToString("HH:mm:ss:ffff"); }
        }
    }
}
