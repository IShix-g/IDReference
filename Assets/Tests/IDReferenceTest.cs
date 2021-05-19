
using UnityEngine;

public sealed class IDReferenceTest : MonoBehaviour
{
    [SerializeField, CharacterIDReference] string characterID;

    void Start()
    {
#if UNITY_EDITOR
        var idReference = characterID.ToIDReferenceEditorOnly();
        if (idReference.IsValid())
        {
            var id = idReference.ID;
            var name = idReference.Name;
    
            Debug.Log($"ID:{id} Name:{name}");
            // or
            Debug.Log(idReference);
        }
#endif
    }
}