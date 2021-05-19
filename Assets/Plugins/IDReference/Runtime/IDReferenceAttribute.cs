
using System;
using IDRef.Internal;
using UnityEngine;

namespace IDRef
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class IDReferenceAttribute : PropertyAttribute
    {
        public abstract string GetTableID();
        
#if UNITY_EDITOR
        IDReferenceTable table;
        
        IDReferenceTable GetTable()
        {
            if (table == null)
            {
                table = IDReferenceProvider.GetTable(GetTableID());
            }
            return table;
        }
    
        public virtual string GetAssetPath()
        {
            return GetTable().AssetPath;
        }

        public virtual IDReferenceList GetAsset()
        {
            return GetTable().GetAsset();
        }

        public virtual void ShowSettingDialog()
        {
            GetTable().ShowSettingDialog();
        }

        public bool DisableDropDownAddKey()
        {
            return GetTable().DisableDropDownAddID;
        }
#endif
    }
}