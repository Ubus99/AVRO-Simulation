using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using UnityEngine;
using Utils.Objects;

namespace Utils
{
    public class CSVLogger : MonoBehaviour
    {
        readonly DataTable _data = new() { Columns = { "timestamp" } };
        readonly Dictionary<string, string> _frameData = new();
        bool _empty;
        string _fileName;

        string _filePath;

        void Awake()
        {
            _filePath = Path.Combine(Application.dataPath, "logs");
            Directory.CreateDirectory(_filePath);

            ServiceLocator.instance.TryRegister<CSVLogger>(this);
            enabled = false;
        }

        void LateUpdate()
        {
            if (_empty) return;

            // ensure only logs when there is data
            _frameData["timestamp"] = DateTime.Now.ToString("HH:mm:ss:ffff");
            JoinDataTable(_data, _frameData);
            // remove old data
            foreach (var k in _frameData.Keys.ToList())
            {
                _frameData[k] = null;
            }
            _empty = true;

            using var writer = new StreamWriter(Path.Join(_filePath, $"{_fileName}.csv"));
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            var columnNames = _data.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
            foreach (var colName in columnNames)
            {
                csv.WriteField(colName);
            }
            csv.NextRecord();

            foreach (DataRow row in _data.Rows)
            {
                var fields = row.ItemArray.Select(field => field.ToString());
                foreach (var field in fields)
                {
                    csv.WriteField(field);
                }
                csv.NextRecord();
            }
        }

        static void JoinDataTable(DataTable table, Dictionary<string, string> dictionary)
        {
            var row = table.NewRow();
            foreach (var kvp in dictionary)
            {
                row[kvp.Key] = kvp.Value;
            }
            table.Rows.Add(row);
        }

        public void RestartLogging(string newName)
        {
            _fileName = newName;
            _data.Clear();
            
            RegistrationEvent?.Invoke();
            
            enabled = true;
        }

        public bool TryRegister(string key)
        {
            if (!_frameData.TryAdd(key, null))
                return false;

            _data.Columns.Add(key, typeof(string));
            return true;
        }

        public bool TryRegister(string[] keys)
        {
            return keys.All(key => _frameData.TryAdd(key, null));
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
