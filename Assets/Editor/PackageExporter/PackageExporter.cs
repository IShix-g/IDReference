
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PackageManagement
{
    public static class PackageExporter
    {
        public static void Export(Package package, string fileName, string rootDir, string exportPath)
        {
            exportPath = AssetUtils.CreateExportPath(package.version, fileName, exportPath);
            var assetsPaths = AssetUtils.GetAllAssetsPath(rootDir);
            var packagePath = assetsPaths.FirstOrDefault(x => x.Contains("package.json"));
            AssetUtils.SavePackage(package, packagePath);
            Debug.Log("Export below files: " + string.Join("\n", assetsPaths));
            Export(assetsPaths, exportPath);
        }

        public static void Export(string[] assetsPaths, string exportPath)
        {
             AssetDatabase.ExportPackage(assetsPaths, exportPath, ExportPackageOptions.Default);
             Debug.Log("Export complete: " + Path.GetFullPath(exportPath));
        }
    }
}
