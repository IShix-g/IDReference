
using UnityEditor;
using UnityEngine;

namespace IDRef.Internal
{
    public sealed class IDReferenceTextWindow : EditorWindow
    {
        string dialogTitleTxt;
        string detailsTxt;
        Vector2 scrollPosition;
        
        public static void ShowDialog(string title, string dialogTitle, string details)
        {
            var window = GetWindow<IDReferenceTextWindow>(title);
            window.dialogTitleTxt = dialogTitle;
            window.detailsTxt = details;
            window.Show();
            window.minSize = new Vector2(480, 300);
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
                GUILayout.Label(dialogTitleTxt, style);
                GUILayout.EndVertical();
            }

            {
                GUILayout.BeginVertical( GUI.skin.box );
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                GUILayout.TextArea(detailsTxt);
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
        }
    }
}