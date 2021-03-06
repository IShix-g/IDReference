#if UNITY_EDITOR
using System;
using System.IO;
using IDRef.Internal;

namespace IDRef
{
    public sealed class IDReferenceTable : IDisposable
    {
        public readonly string TableID;
        public readonly string AssetPath;
        public readonly bool DisableRemoveButton;
        public readonly bool DisableDropDownAddID;
        
        IDReferenceList asset;

        public IDReferenceTable(string tableID, bool disableRemoveButton, bool disableDropDownAddID)
        {
            TableID = tableID;
            AssetPath = Path.Combine(IDReferenceConfig.AssetRootPath, $"{tableID}{IDReferenceConfig.AssetName}");
            DisableRemoveButton = disableRemoveButton;
            DisableDropDownAddID = disableDropDownAddID;
        }

        public void SetAsset(IDReferenceList asset)
        {
            this.asset = asset;
        }
        
        /// <summary>
        /// アセットを取得
        /// </summary>
        /// <returns></returns>
        public IDReferenceList GetAsset()
        {
            return asset;
        }
        
        /// <summary>
        /// idからIDReferenceに変換
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IDReference IDToIDReference(string id)
        {
            if (asset != default)
            {
                return asset.IDToIDReference(id);
            }
            return default;
        }

        /// <summary>
        /// 設定 Windowを表示
        /// </summary>
        public void ShowSettingDialog()
        {
            IDReferenceEditorWindow.ShowDialog(GetAsset(), asset.GetListTitle());
        }

        public void Dispose()
        {
            asset = null;
        }
    }
}
#endif