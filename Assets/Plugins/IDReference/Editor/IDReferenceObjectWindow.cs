
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace IDRef.Internal
{
    public sealed class IDReferenceObjectWindow : EditorWindow
    {
        class ObjectDetails
        {
            public string Category;
            public Object Obj;
            public GlobalObjectId ID;
        }
        
        public int ReferenceLength { get; private set; }
        HashSet<ObjectDetails> objectDetailsSet = new HashSet<ObjectDetails>();
        IDReference reference;
        bool isInitialized;
        Vector2 scrollPosition;
        string lengthString = "-";
        
        public static void ShowDialog(IDReference reference, Action<IDReferenceObjectWindow> loaded)
        {
            var window = GetWindow<IDReferenceObjectWindow>($"Reference to \"{reference.Name}\"");
            window.reference = reference;
            window.Show();
            
            {
                var sceneName = SceneManager.GetActiveScene().name;
                var objs = FindObjectsOfType<MonoBehaviour>()
                        .Select(x => x as Object)
                        .Where(x => x != default)
                        .ToArray();
                window.objectDetailsSet.UnionWith(CreateTargets($"Objects or Prefabs\n( {sceneName} scene )", reference, objs));
            }
            
            {
                var objs = new HashSet<Object>();
                var ids = AssetDatabase.FindAssets("t:GameObject");
                foreach (var id in ids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(id);
                    var obj = AssetDatabase.LoadAssetAtPath<MonoBehaviour>(path);
                    if (obj != default)
                    {
                        objs.Add(obj);
                    }
                }
                window.objectDetailsSet.UnionWith(CreateTargets("Prefabs\n( Assets )", reference, objs));
            }
            
            {
                var objs = new HashSet<Object>();
                var ids = AssetDatabase.FindAssets("t:ScriptableObject");
                foreach (var id in ids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(id);
                    var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    if (obj != default && !(obj is IDReferenceList))
                    {
                        objs.Add(obj);
                    }
                }
                window.objectDetailsSet.UnionWith(CreateTargets("ScriptableObjects", reference, objs));
            }
            
            window.ReferenceLength = window.objectDetailsSet.Count;
            window.lengthString = window.objectDetailsSet.Count.ToString();
            window.isInitialized = true;
            loaded?.Invoke(window);
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
                GUILayout.Label(reference.Name, style);
                GUILayout.EndVertical();
            }

            if (!isInitialized)
            {
                GUILayout.BeginVertical( GUI.skin.box );
                var style = new GUIStyle(GUI.skin.label)
                {
                    padding = new RectOffset(0, 0, 5, 5),
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };
                GUILayout.Label("Now Loading...", style);
                GUILayout.EndVertical();
                return;
            }

            if (objectDetailsSet == null)
            {
                GUILayout.BeginVertical( GUI.skin.box );
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    padding = new RectOffset(0, 0, 5, 5),
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };
                
                var btnTxt = IDReferenceConfig.Language == SystemLanguage.Japanese
                    ? "一旦閉じて開き直してください。"
                    : "Close it and reopen it.";
                GUILayout.Label(btnTxt, style);
                GUILayout.EndVertical();
                return;
            }

            if (objectDetailsSet.Count == 0)
            {
                GUILayout.BeginVertical( GUI.skin.box );
                var style = new GUIStyle(GUI.skin.label)
                {
                    padding = new RectOffset(0, 0, 5, 5),
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };

                var btnTxt = IDReferenceConfig.Language == SystemLanguage.Japanese
                    ? "未使用のIDです\nもし使用している場合は一旦閉じて開き直してください。"
                    : "This is an unused ID\nIf you are using it, please close it and reopen it.";
                GUILayout.Label(btnTxt, style);
                GUILayout.EndVertical();
                return;
            }

            {
                GUILayout.BeginVertical( GUI.skin.box );
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 10,
                    alignment = TextAnchor.MiddleCenter
                };
                GUILayout.Label($"{lengthString} objects", style);
                GUILayout.EndVertical();
            }
            
            {
                GUILayout.BeginHorizontal( GUI.skin.box );
                var style = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };
                
                var btnTxt = IDReferenceConfig.Language == SystemLanguage.Japanese
                                    ? "詳細を書き出す"
                                    : "Export details.";
                
                if (GUILayout.Button(btnTxt, style))
                {
                    var builder = new StringBuilder();
                    builder.Append($"Name : {reference.Name}\nID : {reference.ID}\nLength : {lengthString}\n\n");
                    var idx = 0;
                    foreach (var result in objectDetailsSet)
                    {
                        idx++;
                        builder.Append($"{idx:000} - {result.Obj.name}  ( {AssetDatabase.GUIDToAssetPath(result.ID.assetGUID.ToString())} )\n");
                    }
                    IDReferenceTextWindow.ShowDialog($"Reference details of \"{reference.Name}\"", reference.Name, builder.ToString());
                }
                
                GUILayout.Label("ID :", GUILayout.MaxWidth(22));
                GUILayout.TextField(reference.ID);
                GUILayout.EndHorizontal();
            }
            
            {
                GUILayout.BeginVertical( GUI.skin.box );
                var style = new GUIStyle(GUI.skin.label)
                {
                    padding = new RectOffset(0, 0, 5, 5),
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };

                var btnTxt = IDReferenceConfig.Language == SystemLanguage.Japanese
                    ? "シーン内のオブジェクトを検索するには\nシーンを開いた後、開き直してください。"
                    : "To search for an object in a scene,\nfirst open the scene.";
                GUILayout.Label(btnTxt, style);
                GUILayout.EndVertical();
            }

            GUILayout.BeginVertical( GUI.skin.box );
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            var categoryTitle = string.Empty;
            foreach (var result in objectDetailsSet)
            {
                if (result.Category != categoryTitle)
                {
                    categoryTitle = result.Category;
                    GUILayout.BeginHorizontal( GUI.skin.box );
                    var titleStyle = new GUIStyle(GUI.skin.label)
                    {
                        padding = new RectOffset(0, 0, 5, 5),
                        alignment = TextAnchor.MiddleCenter,
                        wordWrap = true
                    };
                    GUILayout.Label(categoryTitle, titleStyle);
                    GUILayout.EndHorizontal();
                }
                var buttonTitle = $"{result.Obj.name}";
                var style = new GUIStyle(GUI.skin.button)
                {
                    padding = new RectOffset(0, 0, 10, 10),
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };
                
                if (GUILayout.Button(buttonTitle, style))
                {
                    Selection.activeObject = result.Obj;
                    EditorGUIUtility.PingObject(result.Obj);
                }
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        static HashSet<ObjectDetails> CreateTargets(string categoryName, IDReference reference, IEnumerable<Object> objs)
        {
            var results = new HashSet<ObjectDetails>();

            foreach (var obj in objs)
            {
                var serializedObject = new SerializedObject(obj);
                var iterator = serializedObject.GetIterator();
                while (iterator.NextVisible(true))
                {
                    if (iterator.hasVisibleChildren)
                    {
                        continue;
                    }

                    if (iterator.propertyType == SerializedPropertyType.String)
                    {
                        if (reference.IsMyID(iterator.stringValue))
                        {
                            var gId = GlobalObjectId.GetGlobalObjectIdSlow(obj);
                            var result = new ObjectDetails()
                            {
                                Category = categoryName,
                                Obj = obj,
                                ID = gId
                            };
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }
        
        void OnDestroy()
        {
            objectDetailsSet = null;
        }
    }
}