
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IDRef.Internal
{
    [CustomPropertyDrawer(typeof(IDReferenceAttribute), true)]
    public sealed class IDReferencePropertyDrawer : PropertyDrawer
    {
        const string EmptyID = "Not Selected";

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.red;
                EditorGUI.LabelField(position, $"{property.name}  [string Only]", style);
                return;
            }

            var attr = attribute as IDReferenceAttribute;
            var asset = attr?.GetAsset();
            if (asset == default)
            {
                EditorGUI.PropertyField(position, property, true);
                var style  = new GUIStyle();
                style.normal.textColor  = Color.red;
                var assetPath = attr != default ? attr.GetAssetPath() : "";
                EditorGUILayout.TextArea($"{GetTxt(Application.systemLanguage)}{assetPath}", style);
                return;
            }
            
            var ids = new List<string>();
            var keys = new List<string>();
            
            if (asset.References != default)
            {
                ids = asset.References.Select(x => x.Name).ToList();
                keys = asset.References.Select(x => x.ID).ToList();
            }

            ids.Insert(0, EmptyID);
            keys.Insert(0, EmptyID);

            var disableDropDownAddKey = attr.DisableDropDownAddKey();
            if (!disableDropDownAddKey)
            {
                ids.Add("Add ID...");
                keys.Add("Add ID...");
            }
            
            var popUpLabel = EditorGUI.BeginProperty(position, label, property);
            var curValue = string.IsNullOrEmpty(property.stringValue) ? EmptyID : property.stringValue;
            var optionsArray = ids.Select(o => new GUIContent(o)).ToArray();
            var curIndex = keys.IndexOf(curValue);

            if (curIndex < 0)
            {
                curIndex = 0;
                ids[0] = "*Unknown";
            }
            
            EditorGUI.BeginChangeCheck();
            
            var newIndex = EditorGUI.Popup(position, popUpLabel, curIndex, optionsArray);
            
            if (!disableDropDownAddKey
                && newIndex == ids.Count - 1)
            {
                newIndex = curIndex;
                attr.ShowSettingDialog();
            }
            
            var newValue = IsIndexValid(ids, newIndex) ? keys[newIndex] : keys[0];
            if (newValue == EmptyID)
            {
                newValue = string.Empty;
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = newValue;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 22;
        
        bool IsIndexValid<T> (List<T> list, int index) => list.Count > 0 && index >= 0 && index < list.Count;
        
        static string GetTxt(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.Japanese:
                    return "***** 重要 *****\n内容が分からない場合は開発担当者に連絡ください。\n[データが読み取れませんでした]\nIDReferenceListデータが見当たらず、ドロップダウンを生成できませんでした。\nデータが存在するはずのパス : ";
                default:
                    return "***** Caution *****\nIf you do not understand the content, please contact the development staff.\n[The data could not be read.]\nThe IDReferenceList data could not be found and the drop-down could not be generated.\nThe path where the data should reside. : ";
            }
        }
    }
}