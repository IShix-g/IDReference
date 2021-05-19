#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace IDRef.Internal
{
    public static class IDReferenceConfig
    {
        public static SystemLanguage Language = Application.systemLanguage;

        // Assets
        public const string AssetRootPath = "Assets/Editor/IDReferences/";
        public const string AssetName = "IDReferenceList.asset";
        // ID
        public const string IDCharacters = "0123456789abcdefghijklmnopqrstuvwxyz";
        public const int IDRandomStringLength = 5;
        public const string IDStart = "IDRef";
        public const char IDPrfx = '-';
        
        public static string CreateID(string category, string id) => $"{IDStart}{IDPrfx}{category}{IDPrfx}{id}";
        
        public static bool IsID(string stg) => stg.StartsWith(IDStart);
        
        // Logo
        public static Texture2D Logo
        {
            get
            {
                var path = AssetDatabase.GUIDToAssetPath("dce4ef1f4566549748c72eb3a0048859");
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
        }
    }
}
#endif