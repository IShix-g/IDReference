
using IDRef;

#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public sealed class IDReferenceSetting
{
    static readonly IDReferenceTable characterTable;
    static readonly IDReferenceTable storyTable;
    static readonly IDReferenceTable monsterTable;
    static readonly IDReferenceTable itemTable;
    
    static IDReferenceSetting()
    {
        characterTable = new IDReferenceTable("Character", disableRemoveButton: false, disableDropDownAddID: false, required: new []{ new IDReference("Mob モブ", "Mob") });
        storyTable = new IDReferenceTable("Story", disableRemoveButton: true, disableDropDownAddID: true);
        monsterTable = new IDReferenceTable("Monster", disableRemoveButton: false, disableDropDownAddID: false);
        itemTable = new IDReferenceTable("Item", disableRemoveButton: false, disableDropDownAddID: false);
        
        IDReferenceProvider.SetTable(characterTable);
        IDReferenceProvider.SetTable(storyTable);
        IDReferenceProvider.SetTable(monsterTable);
        IDReferenceProvider.SetTable(itemTable);
    }
    
    /*
     * CustomMenu ------------
     */
    [MenuItem("IDReference/Character")]
    public static void CharacterCustomMenu()
    {
        characterTable.ShowSettingDialog();
    }
    
    [MenuItem("IDReference/Story")]
    public static void StoryCustomMenu()
    {
        storyTable.ShowSettingDialog();
    }
    
    [MenuItem("IDReference/Monster")]
    public static void MonsterCustomMenu()
    {
        monsterTable.ShowSettingDialog();
    }
    
    [MenuItem("IDReference/Item")]
    public static void ItemCustomMenu()
    {
        itemTable.ShowSettingDialog();
    }
}
#endif

/*
 * Attribute ------------
 */
public sealed class CharacterIDReferenceAttribute : IDReferenceAttribute
{
    public override string GetTableID() => "Character";
}

public sealed class StoryIDReferenceAttribute : IDReferenceAttribute
{
    public override string GetTableID() => "Story";
}

public sealed class MonsterIDReferenceAttribute : IDReferenceAttribute
{
    public override string GetTableID() => "Monster";
}

public sealed class ItemIDReferenceAttribute : IDReferenceAttribute
{
    public override string GetTableID() => "Item";
}
