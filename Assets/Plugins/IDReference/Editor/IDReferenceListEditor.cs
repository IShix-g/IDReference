
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace IDRef.Internal
{
    [CustomEditor(typeof(IDReferenceList))]
    public sealed class IDReferenceListEditor : Editor
    {
        IDReferenceList list;
        IDReferenceTable table;
        ReorderableList reorderableList;
        Dictionary<int,int> referenceLengths = new Dictionary<int, int>();
        Texture2D logo;
        Vector2 scrollPosition;
        
        void OnEnable()
        {
            list = target as IDReferenceList;
            if (list != default)
            {
                table = IDReferenceProvider.GetTable(list.TableID);
            }
            
            if (list == default || table == default)
            {
                return;
            }

            logo = IDReferenceConfig.Logo;
            
            var prop = serializedObject.FindProperty("references");
            var waitingForOpenWindow = false;
            reorderableList = new ReorderableList (serializedObject, prop);
            reorderableList.drawElementCallback =
                (rect, index, isActive, isFocused) =>
                {
                    if (!referenceLengths.ContainsKey(index))
                    {
                        referenceLengths[index] = 0;
                    }
                    var reference = list.References[index];
                    var isRequired = table.IsRequires(reference);
                    var btnPos = new Rect( rect.x + rect.width - 30f, rect.y, 30f, rect.height );
                    var textPos = new Rect( rect.x, rect.y, rect.width - 36f, rect.height );
                    var textFieldStyle = new GUIStyle(GUI.skin.textField);
                    if (isRequired)
                    {
                        textFieldStyle.normal.textColor = Color.cyan;
                    }
                    var newName = EditorGUI.TextField(textPos, reference.Name, textFieldStyle);
                    if (newName != reference.Name
                        && !isRequired)
                    {
                        reference.Name = newName;
                        list.UpdateID(index, reference);
                    }
                    
                    var buttonStyle = new GUIStyle(GUI.skin.button);
                    if (referenceLengths[index] > 0)
                    {
                        buttonStyle.normal.textColor = Color.green;
                    }
                    if (GUI.Button(btnPos, referenceLengths[index] == 0 ? "ref" : $"{referenceLengths[index]}", buttonStyle) && !waitingForOpenWindow)
                    {
                        waitingForOpenWindow = true;
                        IDReferenceObjectWindow.ShowDialog(reference, window =>
                        {
                            referenceLengths[index] = window.ReferenceLength;
                            waitingForOpenWindow = false;
                        });
                    }
                };

            reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "");
            reorderableList.onAddCallback = _ => AddDirection(list);
            reorderableList.onRemoveCallback = reorderableList => RemoveDirection(list, table, reorderableList);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            {
                GUILayout.BeginVertical( GUI.skin.box );
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    padding = new RectOffset(0, 0, 10, 10),
                    alignment = TextAnchor.MiddleCenter,
                };
                GUILayout.Label(logo, style, GUILayout.MinWidth(50), GUILayout.MaxHeight(70));
                GUILayout.EndVertical();
            }

            {
                var style = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 10
                };
                GUILayout.BeginHorizontal( GUI.skin.box );
                if (GUILayout.Button("Import CSV", style))
                {
                    IDReferenceImportCsvWindow.ShowDialog(results =>
                    {
                        foreach (var result in results)
                        {
                            if (string.IsNullOrEmpty(result))
                            {
                                continue;
                            }

                            var has = list.References.Any(x => x.Name == result || x.ID == result);
                            if (!has)
                            {
                                list.AddNewID(result);
                            }
                        }
                        
                    });
                }
                
                if (GUILayout.Button("Export CSV", style))
                {
                    IDReferenceExportCsv.Export(list.TableID, list.References);
                }
                
                GUILayout.Space(5);
                
                if (GUILayout.Button("Documents", style, GUILayout.Width(72)))
                {
                    Application.OpenURL(IDReferenceConfig.DocumentUrl);
                }
                GUILayout.EndHorizontal();
            }
            
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("List", list, typeof(ScriptableObject), true);   
            }
            
            if (list == default || table == default)
            {
                OnEnable();
                GUILayout.BeginVertical( GUI.skin.box );
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    padding = new RectOffset(0, 0, 10, 10)
                };
                GUILayout.Label("Load Error...", style);
                GUILayout.EndVertical();
                return;
            }
            
            {
                GUILayout.BeginVertical( GUI.skin.box );
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    padding = new RectOffset(0, 0, 10, 10),
                    alignment = TextAnchor.MiddleCenter
                };
                GUILayout.Label(list.TableID, style);
                GUILayout.EndVertical();
            }

            EditorGUI.BeginChangeCheck();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            reorderableList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            
            if ( EditorGUI.EndChangeCheck() )
                serializedObject.ApplyModifiedProperties();
        }

        void AddDirection(IDReferenceList list) => list.AddNewID();
        
        void RemoveDirection(IDReferenceList list, IDReferenceTable table, ReorderableList reorderableList)
        {
            var reference = list.References[reorderableList.index];
            var isRequired = table.IsRequires(reference);
            var title = string.Empty;
            var contents = string.Empty;
            var deleteBtn = string.Empty;

            if (isRequired)
            {
                title = IDReferenceConfig.Language == SystemLanguage.Japanese
                    ? "削除できません"
                    : "Cannot delete";
                contents = IDReferenceConfig.Language == SystemLanguage.Japanese
                    ? "管理者によって追加されたため、削除することができません。削除を希望される場合は、管理者にご連絡ください。"
                    : "Cannot be removed because it was added by an administrator. Please contact the administrator if you wish to remove it.";
            }
            else if (table.DisableRemoveButton)
            {
                title = IDReferenceConfig.Language == SystemLanguage.Japanese
                    ? "削除は無効です"
                    : "Delete is disabled.";

                contents = IDReferenceConfig.Language == SystemLanguage.Japanese
                    ? "管理者により削除機能が無効になっています。削除機能をご利用になりたい場合は、管理者にお問い合わせください。"
                    : "The delete function has been disabled by the administrator. If you want to use the delete function, please contact the administrator.";
            }
            else
            {
                title = IDReferenceConfig.Language == SystemLanguage.Japanese
                    ? "削除しても良いですか?"
                    : "Can I delete it?";

                contents = IDReferenceConfig.Language == SystemLanguage.Japanese
                    ? $"{reference.Name} を削除します。"
                    : $"Delete the \"{reference.Name}\".";

                deleteBtn = IDReferenceConfig.Language == SystemLanguage.Japanese
                    ? "削除"
                    : "Delete it.";   
            }

            if (isRequired
                || table.DisableRemoveButton)
            {
                EditorUtility.DisplayDialog(title, contents, "Close");
            }
            else if (EditorUtility.DisplayDialog(title, contents, deleteBtn, "Close"))
            {
                list.RemoveID(reorderableList.index);
            }
        }
    }
}