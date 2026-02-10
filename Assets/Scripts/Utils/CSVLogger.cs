using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Unity.Cecil.Awesome.Ordering;
using UnityEngine;
using Utils.Objects;

namespace Utils
{
    public class CSVLogger : MonoBehaviour
    {
        readonly List<Dictionary<string, string>> _data = new();
        readonly Dictionary<string, string> _frameData = new();
        bool _empty;

        string _filePath;

        void Awake()
        {
            _filePath = Path.Combine(Application.dataPath, "logs", "log.csv");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath) ?? string.Empty);

            ServiceLocator.instance.TryRegister<CSVLogger>(this);
            enabled = false;
        }

        void LateUpdate()
        {
            if (_empty) return;

            // ensure only logs when there is data
            _frameData["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:zzz");
            _data.Add(new Dictionary<string, string>(_frameData));
            foreach (var k in _frameData.Keys.ToList()) // remove old data
            {
                _frameData[k] = null;
            }
            _empty = true;

            using var writer = new StreamWriter(_filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            foreach (var stream in _data[0])
            {
                csv.WriteField(stream.Key);
            }
            csv.NextRecord();

            foreach (var record in _data)
            {
                foreach (var stream in record)
                {
                    csv.WriteField(stream.Value);
                }
                csv.NextRecord();
            }
        }

        public void RestartLogging(string newName)
        {
            enabled = true;
            _data.Clear();
            RegistrationEvent?.Invoke();
        }
        
        public bool TryRegister(string key)
        {
            return _frameData.TryAdd(key, null);
        }

        public bool TryRegister(string[] keys)
        {
            return keys.Select(key => _frameData.TryAdd(key, null)).All(success => success);
        }

        public bool TryLog(string key, string message)
        {
            if (!_frameData.ContainsKey(key))
                return false;

            _frameData[key] = message;
            _empty = false;
            return true;
        }

        public event Action RegistrationEvent;
    }
}
