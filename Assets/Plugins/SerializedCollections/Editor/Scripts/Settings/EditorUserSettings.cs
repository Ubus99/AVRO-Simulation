using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.Editor
{
    public sealed class EditorUserSettings : ScriptableObject
    {
        const string _filePath = "UserSettings/SerializedCollectionsEditorSettings.asset";

        static EditorUserSettings _instance;

        [SerializeField]
        bool _alwaysShowSearch;

        [SerializeField]
        [Range(1, 10)]
        int _pageCountForSearch = 1;

        [SerializeField]
        [Min(1)]
        int _elementsPerPage = 10;

        public bool AlwaysShowSearch
        {
            get { return _alwaysShowSearch; }
        }

        public int PageCountForSearch
        {
            get { return _pageCountForSearch; }
        }

        public int ElementsPerPage
        {
            get { return _elementsPerPage; }
        }

        public static EditorUserSettings Get()
        {
            if (_instance == null)
            {
                _instance = CreateInstance<EditorUserSettings>();
                LoadInto(_instance);
            }
            return _instance;
        }

        static void LoadInto(EditorUserSettings settings)
        {
            if (!File.Exists(_filePath)) return;

            try
            {
                var json = File.ReadAllText(_filePath);
                EditorJsonUtility.FromJsonOverwrite(json, settings);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        internal static void Save()
        {
            var contents = EditorJsonUtility.ToJson(Get());
            File.WriteAllText(_filePath, contents);
        }
    }
}
