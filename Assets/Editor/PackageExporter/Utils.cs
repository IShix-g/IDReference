
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace PackageManagement
{
    public static class Utils
    {
        public static string[] GetAllAssetsPath(string root)
        {
            return Directory.GetFiles(root, "*", SearchOption.AllDirectories)
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Where(x => !x.EndsWith(".meta"))
                        .Where(x => !x.EndsWith(".DS_Store"))
                        .Select(x => x.Replace(@"\", "/"))
                        .ToArray();
        }
        
        public static string[] Include(this IEnumerable<string> current, IEnumerable<string> target)
        {
            return current.Where(x => target.Any(x.Contains)).ToArray();
        }
        
        public static string[] Exclude(this IEnumerable<string> current, IEnumerable<string> target)
        {
            return current.Where(x => !target.Any(x.Contains)).ToArray();
        }
        
        public static Package LoadPackage()
        {
            var path = "Assets/Editor/PackageManagement/Package.asset";
            Package package = default;
            if (File.Exists(path))
            {
                package = AssetDatabase.LoadAssetAtPath<Package>(path);
            }
            else
            {
                CreateFolderRecursively(path);
                package = ScriptableObject.CreateInstance<Package>();
                AssetDatabase.CreateAsset(package, path);
                Print($"Create an path:{path}");
            }

            return package;
        }
        
        public static void SavePackage(Package package, string root)
        {
            var path = Path.Combine(root, "package.json");
            if (!File.Exists(path))
            {
                Debug.LogError("[PackageExporter] package.json not found.");
                return;
            }
            
            var packageStg = package.ToJson();
            File.WriteAllText(path, packageStg);
        }

        public static string CreateFileName(string version, string fileName)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
            {
                fileName += ".unitypackage";
            }
            
            if(!fileName.Contains(version))
            {
                var idx = fileName.IndexOf(".unitypackage", StringComparison.Ordinal);
                fileName = fileName.Insert(idx, $"_v{version}");
            }

            return fileName;
        }

        public static void Print( string msg)
        {
            msg = $"[PackageExporter] {msg}";
            if (Application.isBatchMode)
            {
                Console.WriteLine(msg);
            }
            else
            {
                Debug.Log(msg);
            }
        }
        
        public static void CreateFolderRecursively(string path)
        {
            Debug.Assert(path.StartsWith("Assets/"), "The `path` should be specified from `Assets/`");

            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }
            
            if (path[path.Length - 1] == '/')
            {
                path = path.Substring(0, path.Length - 1);
            }

            var names = path.Split('/');
            for (int i = 1; i < names.Length; i++)
            {
                var parent = string.Join("/", names.Take(i).ToArray());
                var target = string.Join("/", names.Take(i + 1).ToArray());
                var child = names[i];
                if (!AssetDatabase.IsValidFolder(target))
                {
                    AssetDatabase.CreateFolder(parent, child);
                }
            }
        }
    }
}