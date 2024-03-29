#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IDRef.Internal
{
    public static class IDReferenceConfig
    {
        public static SystemLanguage Language = Application.systemLanguage;
        public const string DocumentUrl = "https://github.com/IShix-g/IDReference";
        public const string AssetRootPath = "Assets/Editor/IDReferences/";
        public const string AssetName = "IDReferenceList.asset";
        public const string IDCharacters = "0123456789abcdefghijklmnopqrstuvwxyz";
        public const int IDRandomStringLength = 5;
        public const string IDUniq = "IDRef";
        public const char IDPrefix = '-';

        public static Texture2D Logo
        {
            get
            {
                var path = AssetDatabase.GUIDToAssetPath("dce4ef1f4566549748c72eb3a0048859");
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
        }
        
        internal static string CreateID(string category, string id) => $"{id}{IDPrefix}{category}{IDPrefix}{IDUniq}";

        public static string LatestOpenDirectory
        {
            get
            {
                return EditorPrefs.GetString("IDReferenceConfig_LatestOpenDirectory", Application.dataPath);
            }
            set
            {
                var path = Path.GetDirectoryName(value);
                EditorPrefs.SetString("IDReferenceConfig_LatestOpenDirectory", path);
            }
        }
    }
}
#endif