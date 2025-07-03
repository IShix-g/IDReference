![Unity](https://img.shields.io/badge/Unity-2021.3%2B-black)

# ID Reference

![IDReference](ReadMeImages/image1.png)

## IDの管理を簡単に

プログラマーは、簡単に`Name`（名前）を変更したいと考えていますが、システムは変更されない一意の`ID`が必要です。
`IDReference`は、プログラマーとシステムの正反対の要件を満たします。

![manage your ID](ReadMeImages/image10.png)

### プルダウンで簡単に選択

プルダウンメニューから選ぶだけで、間違えません。

```c#
[SerializeField, CharacterIDReference] string characterID;
//Inspector:Cat 猫 characterID:IDRef-Character-iwp05
```
![Easy setting](ReadMeImages/image2.png)

### 変更が容易

設定画面で簡単に修正できます。

- 名前を変更しても参照先には影響ありません。
- 順序を変更しても参照先には影響ありません。
- 英語以外の言語を使用しても問題ありません。

![Changeable](ReadMeImages/image3.png)

### IDの使用箇所を確認

設定画面で確認できます。

- すべての参照を表示
- 参照にジャンプ
- 参照を出力

![where to use ID](ReadMeImages/image4.png)

## Getting started

### パッケージマネージャー

#### URL
```
https://github.com/IShix-g/IDReference.git?path=Assets/Plugins/IDReference
```

#### [Unity 2019.3 higher] Install via git URL
パッケージマネージャーにURLを追加します。

![Package Manager](ReadMeImages/image5.png)

## Quick Start

### [Step1] 初期化

```c#
#if UNITY_EDITOR
using UnityEditor;
using IDRef;

// [Step1] Initialization (editor only)
[InitializeOnLoad]
public sealed class IDReferenceSetting
{
    static IDReferenceTable characterTable;

    static IDReferenceSetting()
    {
        characterTable = new IDReferenceTable("Character", false, false);
        IDReferenceProvider.SetTable(characterTable);
    }
}
#endif
```

### [Step2] 属性を設定

```c#
using IDRef;

public sealed class CharacterIDReferenceAttribute : IDReferenceAttribute
{
    public override string GetTableID() => "Character";
}
```

### [Step3] ### スクリプトに属性を追加

```c#
using UnityEngine;

public sealed class IDReferenceTest : MonoBehaviour
{
    [SerializeField, CharacterIDReference] string characterID;
```

### [Step4] IDの追加

![Adding an ID](ReadMeImages/image3.png)

### [Option] カスタムメニュー

`IDリファレンスリスト`にはカスタムメニューを追加してアクセスできます。設定しておくと便利です。

![Custom menu](ReadMeImages/image9.png)

```c#
static IDReferenceTable characterTable;

static IDReferenceSetting()
{
    characterTable = new IDReferenceTable("Character", false, false);
    IDReferenceProvider.SetTable(characterTable);
}

// Add to menu
[MenuItem("IDReference/Character")]
public static void CharacterCustomMenu()
{
    characterTable.ShowSettingDialog();
}
```

### 注意点

- 複数の登録が可能です。
- 本機能はUnity Editorでのみ使用可能です。
- 初期化コードは必ず`UNITY_EDITOR`で囲んでください。
- 他クラスで参照されるため、属性コードは`UNITY_EDITOR`で囲まないでください。

## オプション

### 削除ボタンの無効化

IDを削除すると、そのIDを参照できなくなります。これは`IDReference`の唯一の弱点ですが、削除ボタンを無効化することで安心して使用できます。

```c#
characterTable = new IDReferenceTable("Character", disableRemoveButton: true);
```

![Delete button](ReadMeImages/image7.png)

### ドロップダウンでのID追加を無効化
ドロップダウンでのID追加を無効化することで、編集できる人を制限できます。

```c#
characterTable = new IDReferenceTable("Character", disableDropDownAddID: true);
```

![add ID in dropdown](ReadMeImages/image8.png)

### 初期値を設定
初期値を設定することで、独自のIDを定義できます。追加されたIDは削除や編集ができず、青い文字で表示されます。

```c#
characterTable = new IDReferenceTable("Character", required: new []{ new IDReference("Mob モブ", "Mob") });
```

![ID list](ReadMeImages/image14.png)

## ID Reference オブジェクト

**Editorのみで有効**

![IDReference](ReadMeImages/image11.png)

IDのみでは理解しづらい場合、`ToIDReferenceEditorOnly()`を使用して、文字列を`IDReference Object`に変換できます。

```c#

using UnityEngine;

public sealed class IDReferenceTest : MonoBehaviour
{
    [SerializeField, CharacterIDReference] string characterID;

    void Start()
    {
#if UNITY_EDITOR
        // convert to IdReference
        var idReference = characterID.ToIDReferenceEditorOnly();
        
        if (idReference.IsValid())
        {
            var name = idReference.Name;
            var id = idReference.ID;
    
            Debug.Log($"Name:{name} ID:{id}");
            // Name:Cat 猫 ID:IDRef-Character-iwp05
        }
#endif
    }
}
```

## CSV インポート / エクスポート

`Shift_JIS` に対応しています。

![ID list](ReadMeImages/image13.png)

## ID管理の比較
Unityインスペクターから設定することを前提としています。

|        | IDReference | int |  string  |  enum  |
|--------|-------------|-----| ---- | ---- |
| 名前の変更  | ◎           | ×   |  ×  | ◎ |
| 並び替え   | ◎   | ○   |  ○  | ○ |
| 使用感    | ◎   | ×   |  ×  | ◎ |
| 可読性    | ◎   | ×   |  ◎  | ◎ |

## ID リストのサンプル

Character / Item / Monster / Story

![ID list](ReadMeImages/image12.png)

## 使用ライブラリ

- [stevehansen/csv](https://github.com/stevehansen/csv)  
  CSVのインポート/エクスポートで使用.
