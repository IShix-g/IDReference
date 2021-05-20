
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Csv;
using UnityEditor;
using UnityEngine;

namespace IDRef.Internal
{
    public sealed class IDReferenceImportCsvWindow : EditorWindow
    {
        int toggleIdx;
        Vector2 scrollPosition;
        Action<List<string>> importAction;
        ICsvLine[] csvLines;
        int skipRow;
        string currentPath;

        public static void ShowDialog(Action<List<string>> importAction)
        {
            var window = GetWindow<IDReferenceImportCsvWindow>("Import CSV");
            window.Show();
            window.minSize = new Vector2(400, 300);
            window.importAction = importAction;
        }

        void OnGUI()
        {
            if (importAction == default)
            {
                Close();
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
                GUILayout.Label("Import CSV", style);
                GUILayout.EndVertical();
            }

            {
                var style = new GUIStyle(GUI.skin.button)
                {
                    padding = new RectOffset(0,0,10,10)
                };
                
                GUILayout.BeginVertical( GUI.skin.box );
                if (GUILayout.Button("Load from file.", style))
                {
                    currentPath = EditorUtility.OpenFilePanel("Select CSV file",  IDReferenceConfig.LatestOpenDirectory, "csv");
                    if (string.IsNullOrEmpty(currentPath))
                    {
                        return;
                    }

                    IDReferenceConfig.LatestOpenDirectory = currentPath;
                    csvLines = null;
                    
                    try
                    {
                        var bs = File.ReadAllBytes(currentPath);
                        var enc = bs.GetEncodingCode();
                        var stg = enc.GetString(bs);
                        
                        var options = new CsvOptions
                        {
                            HeaderMode = HeaderMode.HeaderAbsent
                        };
                        csvLines = CsvReader.ReadFromText(stg, options).ToArray();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
                GUILayout.EndVertical();
            }
            
            if (csvLines == null || csvLines.Length == 0)
            {
                return;
            }

            {
                GUILayout.BeginHorizontal( GUI.skin.box );
                GUILayout.Label("Skip row");
                var text = GUILayout.TextField(skipRow.ToString());
                try
                {
                    skipRow = Mathf.Clamp(int.Parse(text), 0, csvLines.Length - 1);
                }
                catch
                {
                    skipRow = 0;
                }
                
                GUILayout.EndHorizontal();
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            {
                var styleToggle = new GUIStyle(GUI.skin.toggle)
                {
                    alignment = TextAnchor.MiddleCenter,
                    padding = new RectOffset(0,0,10,10)
                };

                GUILayout.BeginVertical(GUI.skin.box);
                var width = position.size.x / csvLines.Length;
                
                GUILayout.BeginHorizontal();
                for (var i = 0; i < csvLines[0].Values.Length; i++)
                {
                    var selected = EditorGUILayout.Toggle(toggleIdx == i, styleToggle, GUILayout.MinWidth(width), GUILayout.MinHeight(35));
                    if (selected)
                    {
                        toggleIdx = i;
                    }
                    if (i < csvLines[0].Values.Length - 1)
                    {
                        GUILayout.Space(5);
                    }
                }
                GUILayout.EndHorizontal();
                
                var style = new GUIStyle(GUI.skin.textField)
                {
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };
                for (var i = skipRow; i < csvLines.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    for (var j = 0; j < csvLines[i].Values.Length; j++)
                    {
                        GUILayout.Label(csvLines[i].Values[j], style, GUILayout.MinWidth(width));
                        if (j < csvLines[i].Values.Length - 1)
                        {
                            GUILayout.Space(5);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }
                
                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
            
            {
                GUILayout.Space(15);
                var style = new GUIStyle(GUI.skin.button)
                {
                    padding = new RectOffset(0,0,15,15),
                    wordWrap = true
                };
                
                GUILayout.BeginVertical(GUI.skin.box);
                if (GUILayout.Button("Import", style))
                {
                    var titleTxt = IDReferenceConfig.Language == SystemLanguage.Japanese
                        ? "CSVのインポート"
                        : "Importing CSV";

                    var contentsTxt = IDReferenceConfig.Language == SystemLanguage.Japanese
                        ? "インポートしますか？この処理は元に戻せません。重複は破棄されます。"
                        : "Do you want to import it? This process is irreversible. Duplicates will be discarded.";

                    var btnTxt = IDReferenceConfig.Language == SystemLanguage.Japanese
                        ? "インポート"
                        : "Importing.";
                    
                    if (EditorUtility.DisplayDialog(titleTxt, contentsTxt, btnTxt, "Close"))
                    {
                        var results = new List<string>();
                        for (var i =  skipRow; i < csvLines.Length; i++)
                        {
                            results.Add(csvLines[i].Values[toggleIdx]);
                        }
                        importAction?.Invoke(results);
                        Close();
                    }
                }
                GUILayout.EndVertical();
                GUILayout.Space(15);
            }
        }

        void OnDestroy()
        {
            importAction = null;
        }
    }
}