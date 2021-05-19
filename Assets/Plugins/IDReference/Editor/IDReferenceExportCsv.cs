
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace IDRef.Internal
{
    public sealed class IDReferenceExportCsv
    {
        
        public static void Export(string fileName, IEnumerable<string> list)
        {
            var path = EditorUtility.SaveFilePanel("Save CSV",  "", fileName, "csv");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            
            try
            {
                using (var file = new StreamWriter(path, false, Encoding.UTF8))
                {
                    foreach (var obj in list)
                    {
                        file.WriteLine(obj);
                    }
                    file.Close();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}