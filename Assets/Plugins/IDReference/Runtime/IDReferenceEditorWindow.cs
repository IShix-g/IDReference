#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace IDRef.Internal
{
    public sealed class IDReferenceEditorWindow : EditorWindow
    {
        readonly static List<IDReferenceEditorWindow> Windows = new List<IDReferenceEditorWindow>();
        
        IDReferenceList asset;
        Editor editor;
        bool initialized;

        public static void ShowDialog(IDReferenceList asset, string dialogName)
        {
            var window = GetWindow<IDReferenceEditorWindow>(dialogName);

            var idx = Windows.FindIndex(x => x.asset == asset);
            if(idx < 0)
            {
                if (Windows.Count > 0)
                {
                    window = CreateWindow<IDReferenceEditorWindow>(dialogName);
                }
                window.asset = asset;
                window.editor = Editor.CreateEditor(asset);
                window.initialized = true;
                window.Show();
                Windows.Add(window);
            }
            else
            {
                Windows[idx].Focus();
            }
        }

        void OnGUI()
        {
            if (!initialized)
            {
                Close();
                return;
            }
            
            editor.OnInspectorGUI();
        }

        void OnDestroy()
        {
            Windows.Remove(this);
        }
    }
}
#endif