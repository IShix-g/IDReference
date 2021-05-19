#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using IDRef.Internal;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

namespace IDRef
{
    public sealed class IDReferenceProvider : IDisposable
    {
        static Stack<IDReferenceTable> idReferenceTables = new Stack<IDReferenceTable>();

        public static void SetTable(IDReferenceTable table)
        {
            idReferenceTables.Push(table);

            IDReferenceList referenceList = null;
            if (File.Exists(table.AssetPath))
            {
                referenceList = AssetDatabase.LoadAssetAtPath<IDReferenceList>(table.AssetPath);
            }
            else
            {
                CreateFolderRecursively(table.AssetPath);
                referenceList = ScriptableObject.CreateInstance<IDReferenceList>();
                referenceList.SetTableID(table.TableID);
                AssetDatabase.CreateAsset(referenceList, table.AssetPath);
                Debug.Log($"[ID reference provider] Create an {table.TableID} path:{table.AssetPath}");
            }

            table.SetAsset(referenceList);
        }

        public static IEnumerable<IDReferenceTable> GetTables()
        {
            return idReferenceTables;
        }
        
        public static IDReferenceTable GetTable(string tableID)
        {
            var table = idReferenceTables.FirstOrDefault(x => x.TableID == tableID);
            Assert.IsNotNull(table, $"[ID reference provider] Nonexistent ID:{tableID}");
            return table;
        }

        /// <summary>
        /// 複数階層のフォルダを作成する
        /// </summary>
        /// <param name="path">一番子供のフォルダまでのパスe.g.)Assets/Resources/Sound/</param>
        /// <remarks>パスは"Assets/"で始まっている必要があります。Splitなので最後のスラッシュ(/)は不要です</remarks>
        public static void CreateFolderRecursively(string path)
        {
            Debug.Assert(path.StartsWith("Assets/"), "The `path` should be specified from `Assets/`");

            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }
            
            if (path[path.Length - 1] == '/')
            {
                path = path.Substring(0, path.Length - 1);
            }

            var names = path.Split('/');
            for (int i = 1; i < names.Length; i++)
            {
                var parent = string.Join("/", names.Take(i).ToArray());
                var target = string.Join("/", names.Take(i + 1).ToArray());
                var child = names[i];
                if (!AssetDatabase.IsValidFolder(target))
                {
                    AssetDatabase.CreateFolder(parent, child);
                }
            }
        }

        public void Dispose()
        {
            idReferenceTables = default;
        }
    }
}
#endif