
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Csv;
using UnityEditor;
using UnityEngine;

namespace IDRef.Internal
{
    public sealed class IDReferenceExportCsv
    {
        public static void Export(string fileName, IEnumerable<IDReference> references)
        {
            var path = EditorUtility.SaveFilePanel("Export CSV",  IDReferenceConfig.LatestOpenDirectory, fileName, "csv");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            
            IDReferenceConfig.LatestOpenDirectory = path;
            
            try
            {
                var columnNames = new[] {"Id", "Name"};
                var rows = references.Select(x => new[] {x.ID, x.Name});
                var csv = CsvWriter.WriteToText(columnNames, rows, ',');
                File.WriteAllText(path, csv, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}