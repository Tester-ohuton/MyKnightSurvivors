using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 右クリックメニューに表示する、filenameはデフォルトのファイル名
[CreateAssetMenu(fileName = "CharacterSettings", menuName = "ScriptableObjects/CharacterSettings")]
public class CharacterSettings : ScriptableObject
{
    // キャラクターデータ
    public List<CharacterStats> datas;

    public static CharacterSettings instance;
    public static CharacterSettings Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<CharacterSettings>(nameof(CharacterSettings));
            }

            return instance;
        }
    }

    // リストのIDからデータを検索する
    public CharacterStats Get(int id)
    {
        return (CharacterStats)datas.Find(item => item.Id == id).GetCopy();
    }

}

// 敵の動き
public enum MoveType
{
    // プレイヤーに向かって進む
    TargetPlayer,
    // 一方向に進む
    TargetDirection
}

[Serializable]
public class CharacterStats : BaseStats
{
    // キャラクターのプレハブ
    public GameObject Prefab;
    // 初期装備武器ID
    public List<int> DefaultWeaponIds;
    // 装備可能武器ID
    public List<int> UsableWeaponIds;
    // 装備可能数
    public int UsableWeaponMax;
    // 移動タイプ
    public MoveType MoveType;

    // TODO アイテム追加
}
