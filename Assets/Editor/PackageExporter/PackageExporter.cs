
using System.IO;
using UnityEditor;

namespace PackageManagement
{
    public static class PackageExporter
    {
        public static void Export(string rootDir, string exportPath)
        {
            Export(default, rootDir, exportPath);
        }
        
        public static void Export(Package package, string rootDir, string exportPath)
        {
            var assetsPaths = Utils.GetAllAssetsPath(rootDir);
            if (package != default)
            {
                Utils.SavePackage(package, rootDir);
            }

            Utils.Print($"Export below files:\n{string.Join("\n", assetsPaths)}");
            AssetDatabase.ExportPackage(assetsPaths, exportPath, ExportPackageOptions.Default);
            Utils.Print($"Export complete: {Path.GetFullPath(exportPath)}");
        }
    }
}
