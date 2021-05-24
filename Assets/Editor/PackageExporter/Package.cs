
using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace PackageManagement
{
    [Serializable]
    public class Package : ScriptableObject
    {
#pragma warning disable 0108
        public string name = "com.example.name";
#pragma warning restore 0108
        public string version = "1.0.0";
        public string displayName;
        public string description;
        public string unity = "2019.4";
        public Author author = new Author()
        {
            name = "Your name",
            url = "https://"
        };
        public string[] keywords;
        public Dependencie[] dependencies;
        public string license = "MIT";

        public string ToJson()
        {
            var serializedObject = new SerializedObject(this);
            var iterator = serializedObject.GetIterator();

            var sb = new StringBuilder();
            sb.Append("{\n");
            iterator.NextVisible(true);
            while(iterator.NextVisible(false))
            {
                if (iterator.propertyPath == "dependencies")
                {
                    sb.Append($"\"{iterator.propertyPath}\": ");
                    if (dependencies.Length > 0)
                    {
                        sb.Append("{\n");
                        foreach (var dependency in dependencies)
                        {
                            if (string.IsNullOrEmpty(dependency.PackageName) ||
                                string.IsNullOrEmpty(dependency.Version))
                            {
                                continue;
                            }
                            
                            sb.Append($"    \"{dependency.PackageName}\": \"{dependency.Version.Trim()}\",\n");
                        }
                        
                        var index = sb.ToString().LastIndexOf(',');
                        if (index >= 0)
                        {
                            sb.Remove(index, 1);
                        }
                        
                        sb.Append("},\n");
                    }
                    else
                    {
                        sb.Append("{},\n");
                    }
                }
                else if (iterator.propertyPath == "author")
                {
                    sb.Append($"\"{iterator.propertyPath}\": {JsonUtility.ToJson(author, true)},\n");
                }
                else if (iterator.propertyPath == "keywords")
                {
                    sb.Append($"\"{iterator.propertyPath}\": ");

                    if (keywords.Length > 0)
                    {
                        sb.Append("[\n");
                        foreach (var keyword in keywords)
                        {
                            if (string.IsNullOrEmpty(keyword))
                            {
                                continue;
                            }
                            
                            sb.Append($"    \"{keyword.Trim()}\",\n");
                        }
                        var index = sb.ToString().LastIndexOf(',');
                        if (index >= 0)
                        {
                            sb.Remove(index, 1);
                        }
                        sb.Append("],\n");
                    }
                    else
                    {
                        sb.Append("[],\n");
                    }
                }
                else
                {
                    if (iterator.propertyType == SerializedPropertyType.String)
                    {
                        sb.Append($"\"{iterator.propertyPath}\": \"{iterator.stringValue.Trim()}\",\n");
                    }
                    else if (iterator.propertyType == SerializedPropertyType.Integer)
                    {
                        sb.Append($"\"{iterator.propertyPath}\": \"{iterator.intValue}\",\n");
                    }
                }
            }

            {
                var index = sb.ToString().LastIndexOf(',');
                if (index >= 0)
                {
                    sb.Remove(index, 1);
                }
            }

            sb.Append("}");
            return sb.ToString();
        }
        
        public override string ToString() => JsonUtility.ToJson(this);
    }

    [Serializable]
    public class Author
    {
        public string name;
        public string url;
    }
    
    [Serializable]
    public class Dependencie
    {
        public string PackageName;
        public string Version;
    }
}