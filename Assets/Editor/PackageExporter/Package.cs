
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PackageManagement
{
    [Serializable]
    public class Package
    {
        public string name;
        public string displayName;
        public string version;
        public string unity;
        public string description;
        public Author author;
        public string[] keywords;
        public string license;
        public Dictionary<string, string> dependencies;

        public void MargePackage(Package newObj)
        {
            if (!string.IsNullOrEmpty(newObj.name))
            {
                name = newObj.name;
            }
            if (!string.IsNullOrEmpty(newObj.displayName))
            {
                displayName = newObj.displayName;
            }
            if (!string.IsNullOrEmpty(newObj.version))
            {
                version = newObj.version;
            }
            if (!string.IsNullOrEmpty(newObj.unity))
            {
                unity = newObj.unity;
            }
            if (!string.IsNullOrEmpty(newObj.description))
            {
                description = newObj.description;
            }
            if (newObj.author != default)
            {
                author = newObj.author;
            }
            if (newObj.keywords != default)
            {
                keywords = newObj.keywords;
            }
            if (!string.IsNullOrEmpty(newObj.license))
            {
                license = newObj.description;
            }
            if (newObj.dependencies != default)
            {
                dependencies = newObj.dependencies;
            }
        }
        
        public override string ToString() => JsonUtility.ToJson(this);
    }

    [Serializable]
    public class Author
    {
        public string name;
        public string url;
    }
}