#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace IDRef.Internal
{
    [HelpURL("https://github.com/IShix-g/IDReference")]
    public sealed class IDReferenceList : ScriptableObject
    {
        [SerializeField] List<IDReference> references = new List<IDReference>();
        [SerializeField] string tableID;

        public IReadOnlyList<IDReference> References => references;
        public string TableID => tableID;

        public void SetTableID(string tableID) => this.tableID = tableID;
        
        public void AddNewID()
        {
            var id = IDReferenceConfig.CreateID(TableID, GenerateID());
            var reference = new IDReference("", id);
            references.Add(reference);
            EditorUtility.SetDirty(this);
        }

        public void UpdateID(int index, IDReference reference)
        {
            references[index] = reference;
            EditorUtility.SetDirty(this);
        }
        
        public void RemoveID(int index)
        {
            references.RemoveAt(index);
            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// IDをIDReferenceに変換する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IDReference IDToIDReference(string id)
        {
            foreach (var reference in references)
            {
                if (reference.ID == id)
                {
                    return reference;
                }
            }
            return default;
        }

        public string GetListTitle() => $"{TableID} ID Reference.";
        
        void OnEnable()
        {
            if (references != default)
            {
                var referencesNotEmpty = references.Where(x => !string.IsNullOrEmpty(x.Name)).ToArray();
                var length = referencesNotEmpty.Select(x => x.Name).Distinct().Count();
                if (referencesNotEmpty.Length != length)
                {
                    Debug.LogWarning($"[Name Duplication] Resolve the duplication. Name:{TableID} Path:{AssetDatabase.GetAssetPath(this)}");
                }
            }
        }

        /// <summary>
        /// 現在所持中のIDの中に該当のidが存在するか？
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool HasIDReference(string id)
        {
            if (references == null)
            {
                return false;
            }
            
            foreach (var reference in references)
            {
                if (reference.ID.Contains(id))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ランダムな文字列を生成する
        /// </summary>
        /// <returns>生成された文字列</returns>
        string GenerateID()
        {
            var sb = new StringBuilder(IDReferenceConfig.IDRandomStringLength);
            var r = new System.Random();
            
            while (true)
            {
                for (var i = 0; i < IDReferenceConfig.IDRandomStringLength; i++)
                {
                    var pos = r.Next(IDReferenceConfig.IDCharacters.Length);
                    var c = IDReferenceConfig.IDCharacters[pos];
                    sb.Append(c);
                }

                if (!HasIDReference(sb.ToString()))
                {
                    break;
                }
            }
            
            return sb.ToString();
        }
    }    
}
#endif