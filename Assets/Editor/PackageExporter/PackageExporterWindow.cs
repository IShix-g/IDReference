
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PackageManagement
{
    public sealed class PackageExporterWindow : EditorWindow
    {
        public static string CurrentDirectory
        {
            get{ return EditorPrefs.GetString("PackageExporterWindow_CurrentDirectory", Application.dataPath); }
            set
            {
                var path = Path.GetDirectoryName(value);
                EditorPrefs.SetString("PackageExporterWindow_CurrentDirectory", path);
            }
        }
        
        string rootDir;
        string fileName;
        Package package;

        public static void ShowDialog(string rootDir, string fileName, string packagePath)
        {
            var window = GetWindow<PackageExporterWindow>("PackageExporter");
            window.rootDir = rootDir;
            window.fileName = fileName;
            window.package = AssetUtils.LoadPackage(packagePath);

            window.Show();
        }

        void OnGUI()
        {
            {
                GUILayout.BeginVertical( GUI.skin.box );
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14,
                    padding = new RectOffset(0, 0, 15, 15),
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };
                GUILayout.Label("PackageExporter", style);
                GUILayout.EndVertical();
            }

            {
                GUILayout.BeginHorizontal( GUI.skin.box );
                GUILayout.Label("version");
                var newVersion = GUILayout.TextField(package.version);
                try
                {
                    Version.Parse(newVersion);
                    package.version = newVersion;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }

                GUILayout.EndHorizontal();
            }

            {
                GUILayout.BeginVertical( GUI.skin.box );
                var style = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 14,
                    padding = new RectOffset(0, 0, 15, 15),
                };
                if (GUILayout.Button("Export", style))
                {
                    var exportPath = EditorUtility.SaveFilePanel
                    (
                        "保存先を選択",
                        CurrentDirectory,
                        AssetUtils.CreateFileName(package.version, fileName),
                        "unitypackage"
                    );
                    
                    if (!string.IsNullOrEmpty(exportPath))
                    {
                        CurrentDirectory = exportPath;
                        Debug.Log(CurrentDirectory);
                        PackageExporter.Export(package, fileName, rootDir, CurrentDirectory);
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}