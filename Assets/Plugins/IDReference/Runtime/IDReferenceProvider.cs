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

            var referenceList = default(IDReferenceList);
            if (File.Exists(table.AssetPath))
            {
                referenceList = AssetDatabase.LoadAssetAtPath<IDReferenceList>(table.AssetPath);
            }
            else
            {
                CreateFolderRecursively(table.AssetPath);
                referenceList = ScriptableObject.CreateInstance<IDReferenceList>();
                AssetDatabase.CreateAsset(referenceList, table.AssetPath);
                Debug.Log($"[ID reference provider] Create an {table.TableID} path:{table.AssetPath}");
            }

            referenceList.SetReference(table.TableID, table.Required);
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
        static void CreateFolderRecursively(string path)
        {
            if (!path.StartsWith("Assets"))
            {
                throw new ArgumentException("Specify a path starting with Asset.");
            }

            if (Path.HasExtension(path))
            {
                path = Path.GetDirectoryName(path);
            }
            
            var folders = path.Split('/');
            var parentFolder = string.Empty;

            foreach (var folder in folders.Where(f => !string.IsNullOrEmpty(f)))
            {
                if (string.IsNullOrEmpty(parentFolder))
                {
                    parentFolder = folder;
                }
                else
                {
                    var newFolder = Path.Combine(parentFolder, folder);
                    if (!AssetDatabase.IsValidFolder(newFolder)) 
                    {
                        AssetDatabase.CreateFolder(parentFolder, folder);
                    }
                    parentFolder = newFolder;   
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