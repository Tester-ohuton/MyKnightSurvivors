using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// 右クリックメニューに表示する、filenameはデフォルトのファイル名
[CreateAssetMenu(fileName = "WeaponSpawnerSettings", menuName = "ScriptableObjects/WeaponSpawnerSettings")]
public class WeaponSpawnerSettings : ScriptableObject
{
    // キャラクターデータ
    public List<WeaponSpawnerStats> datas;

    public static WeaponSpawnerSettings instance;
    public static WeaponSpawnerSettings Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<WeaponSpawnerSettings>(nameof(WeaponSpawnerSettings));
            }

            return instance;
        }
    }

    // リストのIDからデータを検索する
    public WeaponSpawnerStats Get(int id,int lv)
    {
        // 指定されたレベルのデータがなければ1番高いレベルのデータを返す
        WeaponSpawnerStats ret = null;

        foreach (var item in datas)
        {
            if (id != item.Id) continue;

            // 指定レベルと一致
            if(lv == item.Lv)
            {
                return (WeaponSpawnerStats)item.GetCopy();
            }

            // 仮のデータがセットされていないか、それを超えるレベルがあったら入れ替える
            if (null == ret)
            {
                ret = item;
            }
            // 探しているレベルより下で、暫定データより大きい
            else if (item.Lv < lv && ret.Lv < item.Lv)
            {
                ret = item;
            }
        }

        return (WeaponSpawnerStats)ret.GetCopy();
    }

    // 作成
    public BaseWeaponSpawner CreateWeaponSpawner(int id,EnemySpawnerController enemySpawner,Transform parent = null)
    {
        // データ取得
        WeaponSpawnerStats stats = Instance.Get(id,1);
        //オブジェクト作成
        GameObject obj = Instantiate(stats.PrefabSpawner,parent);
        // データセット
        BaseWeaponSpawner spawner = obj.GetComponent<BaseWeaponSpawner>();
        spawner.Init(enemySpawner,stats);
    
        return spawner;
    }
}

// 武器生成装置
[Serializable]
public class WeaponSpawnerStats : BaseStats
{
    // 生成装置のプレハブ
    public GameObject PrefabSpawner;
    // 武器のアイコン
    public Sprite Icon;
    // レベルアップ時に追加されるアイテムID
    public int LevelUpItemId;

    // 一度に生成する数
    public float SpawnCount;
    // 生成タイマー
    public float SpawnTimerMin;
    public float SpawnTimerMax;

    // 生成時間取得
    public float GetRandomSpawnTimer()
    {
        return Random.Range(SpawnTimerMin, SpawnTimerMax);
    }

    // アイテムを追加
    public void AddItemData(ItemData itemData)
    {
        foreach(var item in itemData.Bonuses)
        {
            // 武器固有パラメータ
            if(item.Key == StatsType.SpawnCount)
            {
                SpawnCount = applyBonus(SpawnCount,item.Value,item.Type);
            }
            // 生成時間最小
            else if(item.Key == StatsType.SpawnTimerMin)
            {
                SpawnTimerMin = applyBonus(SpawnTimerMin, item.Value, item.Type);
            }
            // 生成時間最大
            else if (item.Key == StatsType.SpawnTimerMax)
            {
                SpawnTimerMax = applyBonus(SpawnTimerMax, item.Value, item.Type);
            }
            // 通常ボーナス
            else
            {
                addBonus(item);
            }
        }
    }
}
