
using System.IO;
using UnityEditor;
using PackageManagement;

public static class IDReferenceExport
{
    public const string Root = "Assets/Plugins/IDReference";
    public const string FileName = "IDReference";

    [MenuItem("Tools/Export Unitypackage")]
    public static void OpenExportDialog()
    {
        ExporterWindow.ShowDialog( Root, FileName );
    }

    public static void ExportGithub()
    {
        var exportPath = Path.Combine("build", $"{FileName}.unitypackage");
        PackageExporter.Export(Root, exportPath);
    }
}
