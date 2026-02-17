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

        CsvWriter _csv;
        StreamWriter _streamWriter;
        string _fileName;
        readonly DateTime _creationTime = DateTime.Now;

        public CSVLogger(string fileName)
        {
            _fileName = fileName;
            Directory.CreateDirectory(_logBasePath);

            _streamWriter = new StreamWriter(FullPathGenerator(_fileName));
            _csv = new CsvWriter(_streamWriter, CultureInfo.InvariantCulture);

            _csv.WriteHeader<T>();
            _csv.NextRecord();
        }

        string FullPathGenerator(string fileName)
        {
            return Path.Join(_logBasePath, FileNameGenerator(fileName));
        }

        string FileNameGenerator(string baseName)
        {
            return $"{baseName}_{_creationTime:yyyyMMdd-HHmm}_{typeof(T).Name}.csv";
        }

        public void Rename(string newName)
        {
            _csv.Flush();
            _streamWriter.Flush();

            _csv.Dispose();
            _streamWriter.Dispose();

            File.Move(
            FullPathGenerator(_fileName),
            FullPathGenerator(newName));
            _fileName = newName;

            _streamWriter = new StreamWriter(FullPathGenerator(_fileName));
            _csv = new CsvWriter(_streamWriter, CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            _csv.Dispose();
            _streamWriter.Dispose();
        }

        public void Log(T record)
        {
            _csv.WriteRecord(record);
            _csv.NextRecord();
            _csv.Flush();
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
