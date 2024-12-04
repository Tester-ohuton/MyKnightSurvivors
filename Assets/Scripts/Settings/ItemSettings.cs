using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 右クリックメニューに表示する、filenameはデフォルトのファイル名
[CreateAssetMenu(fileName = "ItemSettings", menuName = "ScriptableObjects/ItemSettings")]
public class ItemSettings : ScriptableObject
{
    // アイテムデータ
    public List<ItemData> datas;

    public static ItemSettings instance;
    public static ItemSettings Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<ItemSettings>(nameof(ItemSettings));
            }

            return instance;
        }
    }

    // リストのIDからデータを検索する
    public ItemData Get(int id)
    {
        return (ItemData)datas.Find(item => item.Id == id).GetCopy();
    }
}

[Serializable]
public class ItemData
{
    public string Title;

    // 固有ID
    public int Id;
    // アイテム名
    public string Name;
    // 説明
    [TextArea] public string Description;
    // アイコン
    public Sprite Icon;
    // ボーナス
    public List<BonusStats> Bonuses;

    // コピーしたデータを返す 
    public ItemData GetCopy()
    {
        return (ItemData)MemberwiseClone();
    }
}

// レベルアップや宝箱のデータ
public class BonusData
{
    // 武器
    public WeaponSpawnerStats WeaponSpawnerStats;
    // アイテム
    public ItemData ItemData;

    // コンストラクタ
    public BonusData(WeaponSpawnerStats data)
    {
        WeaponSpawnerStats = data;
    }

    public BonusData(ItemData data)
    {
        ItemData = data;
    }
}
