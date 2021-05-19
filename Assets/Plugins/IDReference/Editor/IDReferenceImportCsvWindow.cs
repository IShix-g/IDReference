
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IDRef.Internal
{
    public sealed class IDReferenceImportCsvWindow : EditorWindow
    {
        readonly List<string> wordList = new List<string>();
        IDReferenceImportCsvWindow window;
        int toggleIdx;
        Vector2 scrollPosition;
        Action<List<string>> importAction;
        
        public static void ShowDialog(Action<List<string>> importAction)
        {
            var window = GetWindow<IDReferenceImportCsvWindow>("Import CSV");
            window.Show();
            window.window = window;
            window.minSize = new Vector2(400, 300);
            window.importAction = importAction;
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
                    var path = EditorUtility.OpenFilePanel("Select CSV",  "", "csv");
                    if (string.IsNullOrEmpty(path))
                    {
                        return;
                    }

                    wordList.Clear();
                    
                    try
                    {
                        using (var sr = new StreamReader(path))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                wordList.Add(line);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
                GUILayout.EndVertical();
            }

            if (wordList.Count == 0)
            {
                return;
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            var map = wordList.Select(s => s.Split(',')).ToArray();
            
            if (map.Length == 0)
            {
                return;
            }

            {
                var style = new GUIStyle(GUI.skin.textField)
                {
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };
                
                var styleToggle = new GUIStyle(GUI.skin.toggle)
                {
                    alignment = TextAnchor.MiddleCenter,
                    padding = new RectOffset(0,0,10,10)
                };

                GUILayout.BeginVertical(GUI.skin.box);
                var width = window.position.size.x / map.Length;
                
                GUILayout.BeginHorizontal();
                for (var j = 0; j < map[0].Length; j++)
                {
                    var selected = EditorGUILayout.Toggle(toggleIdx == j, styleToggle, GUILayout.MinWidth(width), GUILayout.MinHeight(35));
                    if (selected)
                    {
                        toggleIdx = j;
                    }
                    if (j != map[0].Length - 1)
                    {
                        GUILayout.Space(5);
                    }
                }
                GUILayout.EndHorizontal();
                
                for (var i = 0; i < map.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    for (var j = 0; j < map[i].Length; j++)
                    {
                        GUILayout.TextField(map[i][j], style, GUILayout.MinWidth(width));
                        if (j != map[i].Length - 1)
                        {
                            GUILayout.Space(5);
                        }
                    }
                    GUILayout.EndHorizontal();
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
                        for (var i = 0; i < map.Length; i++)
                        {
                            results.Add(map[i][toggleIdx]);
                        }
                        importAction?.Invoke(results);
                        window.Close();
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