#if UNITY_EDITOR
using System;

namespace IDRef
{
    [Serializable]
    public struct IDReference : IEquatable<IDReference>
    {
        public string ID;
        public string Name;
        
        public IDReference(string name, string id)
        {
            Name = name;
            ID = id;
        }

        public bool IsValid() => !string.IsNullOrEmpty(ID);
        
        public bool IsMyID(string stg) => ID == stg;

        public override string ToString() => $"Name:{Name} ID:{ID}";

        public bool Equals(IDReference other) => ID == other.ID && Name == other.Name;

        public override bool Equals(object obj) => obj is IDReference other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ID != null ? ID.GetHashCode() : 0) * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }
}
#endif