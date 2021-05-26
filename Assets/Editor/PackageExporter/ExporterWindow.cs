
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PackageManagement
{
    public sealed class ExporterWindow : EditorWindow
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
        
        Editor editor;
        string root;
        string fileName;
        Package package;

        public static void ShowDialog(string root, string fileName)
        {
            var window = GetWindow<ExporterWindow>("PackageExporter");
            window.root = root;
            window.fileName = fileName;
            window.package = Utils.LoadPackage();
            window.editor = Editor.CreateEditor(window.package);
            window.Show();
        }

        void OnGUI()
        {
            if (editor == default)
            {
                Close();
                return;
            }
            
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
                GUILayout.Space(10);
            }
            
            GUILayout.BeginVertical( GUI.skin.box );
            editor.OnInspectorGUI();
            GUILayout.EndVertical();
            
            {
                GUILayout.Space(10);
                GUILayout.BeginVertical( GUI.skin.box );
                var style = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 14,
                    padding = new RectOffset(0, 0, 15, 15),
                };
                if (GUILayout.Button("Export", style))
                {
                    EditorUtility.SetDirty(package);
                    AssetDatabase.SaveAssets();
                    
                    var exportPath = EditorUtility.SaveFilePanel
                    (
                        "保存先を選択",
                        CurrentDirectory,
                        Utils.CreateFileName(package.version, fileName),
                        "unitypackage"
                    );
                    
                    if (!string.IsNullOrEmpty(exportPath))
                    {
                        CurrentDirectory = exportPath;
                        PackageExporter.Export(package, root, exportPath);
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}