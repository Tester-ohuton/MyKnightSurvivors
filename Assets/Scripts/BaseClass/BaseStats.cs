using UnityEngine;
using System;

// 追加ステータスタイプ
public enum BonusType
{
    // 定数で追加
    Bonus,
    // %で追加
    Boost,
}

// 追加データタイプ
public enum StatsType
{
    Attack,
    Defence,
    MoveSpeed,
    HP,
    MaxHP,
    XP,
    MaxXP,
    PickUpRange,
    AliveTime,
    // 武器生成用の拡張タイプ
    SpawnCount,
    SpawnTimerMin,
    SpawnTimerMax,
}

// 追加データタイプ
[Serializable]
public class BonusStats
{
    // 追加タイプ
    public BonusType Type;
    // 追加するプロパティ
    public StatsType Key;
    // 追加する値
    public float Value;
}

// キャラクターと武器で共通のステータス
public class BaseStats
{
    // Inspectorで表示されるタイトル
    public string title;

    // データID
    public int Id;

    // 設定レベル
    public int Lv;

    // 名前
    public string Name;

    // 説明文
    [TextArea] public string Description;

    // 攻撃力
    public float Attack;

    // 防御力
    public float Defence;

    // 体力
    public float HP;

    // 体力最大
    public float MaxHP;

    // 経験値
    public float XP;

    // 経験値最大
    public float MaxXP;

    // 移動速度
    public float MoveSpeed;

    // 経験値取得範囲
    public float PickUpRange;

    // 生存時間
    public float AliveTime;

    // StatsTypeとの紐づけ インデクサを利用する
    public float this[StatsType key]
    {
        get
        {
            if (key == StatsType.Attack) return Attack;
            else if (key == StatsType.Defence) return Defence;
            else if (key == StatsType.MoveSpeed) return MoveSpeed;
            else if (key == StatsType.HP) return HP;
            else if (key == StatsType.MaxHP) return MaxHP;
            else if (key == StatsType.XP) return XP;
            else if (key == StatsType.MaxXP) return MaxXP;
            else if (key == StatsType.PickUpRange) return PickUpRange;
            else if (key == StatsType.AliveTime) return AliveTime;
            else return 0;
        }

        set
        {
            if (key == StatsType.Attack) Attack = value;
            else if (key == StatsType.Defence) Defence = value;
            else if (key == StatsType.MoveSpeed) MoveSpeed = value;
            else if (key == StatsType.HP) HP = value;
            else if (key == StatsType.MaxHP) MaxHP = value;
            else if (key == StatsType.XP) XP = value;
            else if (key == StatsType.MaxXP) MaxXP = value;
            else if (key == StatsType.PickUpRange) PickUpRange = value;
            else if (key == StatsType.AliveTime) AliveTime = value;
        }
    }

    // ボーナス値を計算
    protected float applyBonus(float currentValue, float value, BonusType type)
    {
        // 固定値を加算
        if (BonusType.Bonus == type)
        {
            return currentValue + value;
        }

        // %で加算
        else if (BonusType.Boost == type)
        {
            return currentValue * (1 + value);
        }

        return currentValue;
    }

    // ボーナスを追加する関数
    protected void addBonus(BonusStats bonus)
    {
        float value = applyBonus(this[bonus.Key], bonus.Value, bonus.Type);

        // 最大値があるもの
        if(StatsType.HP==bonus.Key)
        {
            value = Mathf.Clamp(value, 0, MaxHP);
        }
        else if (StatsType.XP == bonus.Key)
        {
            value = Mathf.Clamp(value, 0, MaxXP);
        }

        this[bonus.Key] = value;
    }

    // コピーしたデータを返す
    public BaseStats GetCopy()
    {
        return (BaseStats)MemberwiseClone();
    }
}
