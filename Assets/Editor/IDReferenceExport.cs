
using System;
using UnityEditor;
using PackageManagement;
using UnityEngine;

public static class IDReferenceExport
{
    [MenuItem("Tools/Export Unitypackage")]
    public static void OpenExportDialog()
    {
        PackageExporterWindow.ShowDialog(
            "Assets/Plugins/IDReference",
            "IDReference",
            "Assets/Plugins/IDReference/package.json"
        );
    }
    
    public static void Export(string version, string exportPath)
    {
        Version.Parse(version);
        if (string.IsNullOrEmpty(exportPath))
        {
            Debug.Log($"empty export path: {exportPath}");
            return;
        }
        
        var package = AssetUtils.LoadPackage("Assets/Plugins/IDReference/package.json");
        package.version = version;
        PackageExporter.Export(package, "IDReference", "Assets/Plugins/IDReference", exportPath);
    }
}
