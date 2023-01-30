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
        
        public bool Equals(IDReference other) => ID == other.ID;
        public override bool Equals(object obj) => obj is IDReference other && Equals(other);
        public override int GetHashCode() => (ID != null ? ID.GetHashCode() : 0);
        public static bool operator ==(IDReference lhs, IDReference rhs) => lhs.ID == rhs.ID;
        public static bool operator !=(IDReference lhs, IDReference rhs) => !(lhs == rhs);
        public override string ToString() => $"IDReference (Name:{Name} ID:{ID})";
    }
}
#endif