#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace IDRef.Internal
{
    [HelpURL(IDReferenceConfig.DocumentUrl)]
    public sealed class IDReferenceList : ScriptableObject
    {
        [SerializeField] List<IDReference> references = new List<IDReference>();
        [SerializeField] string tableID;

        public IReadOnlyList<IDReference> References => references;
        public string TableID => tableID;
        
        internal void SetReference(string tableID, IDReference[] required)
        {
            this.tableID = tableID;

            if (required == null)
            {
                return;
            }
            
            foreach (var obj in required)
            {
                var index = references.FindIndex(id => id == obj);
                if (index >= 0)
                {
                    var current = references[index];
                    current.Name = obj.Name;
                    references[index] = current;
                }
                else
                {
                    references.Add(obj);
                }
            }
        }
        
        public void AddNewID(string name = "")
        {
            var id = IDReferenceConfig.CreateID(TableID, GenerateID());
            var reference = new IDReference(name, id);
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

        internal string GetListTitle() => $"{TableID} ID Reference.";

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
                
                sb.Length = 0;
            }
            return sb.ToString();
        }
    }    
}
#endif