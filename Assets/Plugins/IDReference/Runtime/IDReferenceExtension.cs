#if UNITY_EDITOR
using UnityEngine;

public static class IDReferenceExtension
{
    public static IDRef.IDReference ToIDReferenceEditorOnly(this string id)
    {
        if (!IDRef.Internal.IDReferenceConfig.IsID(id))
        {
            Debug.LogError($"[IDReference] Is not ID : {id}");
            return default;
        }
        
        foreach (var table in IDRef.IDReferenceProvider.GetTables())
        {
            var idReference = table.IDToIDReference(id);
            if (idReference.IsValid())
            {
                return idReference;
            }
        }
        
        return default;
    }

    public static string ToIDReferenceValueEditorOnly(this string id)
    {
        var idReference = ToIDReferenceEditorOnly(id);
        if (idReference.IsValid())
        {
            return idReference.Name;
        }
        return string.Empty;
    }
}
#endif