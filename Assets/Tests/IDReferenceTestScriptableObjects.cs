
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IDReferenceTestScriptableObject", menuName = "IDReferenceTestScriptableObject", order = 0)]
public class IDReferenceTestScriptableObjects : ScriptableObject
{
    [SerializeField] List<IDReferenceTestScriptableObject> characterIDs;
}

[Serializable]
public class IDReferenceTestScriptableObject
{
    [SerializeField, CharacterIDReference] string characterID;
}