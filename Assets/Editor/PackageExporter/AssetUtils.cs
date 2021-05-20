
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PackageManagement
{
    public static class AssetUtils
    {
        public static string[] GetAllAssetsPath(string rootPath)
        {
            return Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories)
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
        
        public static Package LoadPackage(string packagePath)
        {
            var stg = File.ReadAllText(packagePath);
            var obj = JsonUtility.FromJson<Package>(stg);
            if (obj == default)
            {
                obj = new Package();
            }
            return obj;
        }
        
        public static void SavePackage(Package package, string packagePath)
        {
            var packageStg = JsonUtility.ToJson(package, true);
            if (!string.IsNullOrEmpty(packageStg) && packageStg != "{}")
            {
                File.WriteAllText(packagePath, packageStg, Encoding.UTF8);
            }
        }

        public static string CreateExportPath(string version, string fileName, string exportPath)
        {
            if (Path.HasExtension(exportPath))
            {
                var path = Path.GetDirectoryName(exportPath);
                if (!string.IsNullOrEmpty(path))
                {
                    exportPath = path;
                }
            }

            fileName = CreateFileName(version, fileName);
            return Path.Combine(exportPath, fileName);
        }
        
        public static string CreateFileName(string version, string fileName)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
            {
                fileName += ".unitypackage";
            }
            
            if(!fileName.Contains("version"))
            {
                var idx = fileName.IndexOf(".unitypackage", StringComparison.Ordinal);
                fileName = fileName.Insert(idx, $"_v{version}");
            }

            return fileName;
        }
    }
}