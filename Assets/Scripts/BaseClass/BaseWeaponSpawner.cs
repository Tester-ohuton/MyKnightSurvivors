using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponSpawner : MonoBehaviour
{
    // 武器のプレハブ
    [SerializeField] GameObject PrefabWeapon;

    // 武器データ
    public WeaponSpawnerStats Stats;
    // 与えたダメージ
    public float TotalDamage;
    // 稼働タイマー
    public float TotalTimer;

    // 生成タイマー
    protected float spawnTimer;
    // 生成した武器のリスト
    protected List<BaseWeapon> weapons;
    // 武器生成装置
    protected EnemySpawnerController enemySpawner;

    // 初期化
    public void Init(EnemySpawnerController enemySpawner,WeaponSpawnerStats stats)
    {
        weapons = new List<BaseWeapon>();
        this.enemySpawner = enemySpawner;
        this.Stats = stats;
    }

    // 稼働タイマー
    private void FixedUpdate()
    {
        TotalTimer += Time.fixedDeltaTime;
    }

    // 武器生成
    protected BaseWeapon createWeapon(Vector3 position,Vector2 forward,Transform parent = null)
    {
        // 生成
        GameObject obj =Instantiate(PrefabWeapon,position,PrefabWeapon.transform.rotation,parent);
        // 共通データセット
        BaseWeapon weapon = obj.GetComponent<BaseWeapon>();
        // データ初期化
        weapon.Init(this,forward);
        weapons.Add(weapon);

        return weapon;
    }

    // 武器生成（簡易版）
    protected BaseWeapon createWeapon(Vector3 position,Transform parent = null)
    {
        return createWeapon(position,Vector2.zero,parent);
    }

    // 武器のアップデートを停止する
    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
        // オブジェクトを削除
        weapons.RemoveAll(item => !item);
        // 生成した武器を停止
        foreach(var item in weapons)
        {
            item.enabled = enabled;
            item.GetComponent<Rigidbody2D>().simulated = enabled;
        }
    }

    // タイマー消化チェック
    protected bool isSpawnTimerNotElapsed()
    {
        // タイマー消化
        spawnTimer -= Time.deltaTime;
        if (0 < spawnTimer) return true;

        return false;
    }

    // レベルアップ時のデータを返す
    public WeaponSpawnerStats GetLevelUpStats(bool isNextLevel = false)
    {
        // 次のレベル
        int nextLv = Stats.Lv + 1;

        // 次のレベルがあるかどうか調べて、あれば上書き
        WeaponSpawnerStats ret = WeaponSpawnerSettings.Instance.Get(Stats.Id,nextLv);

        // 上書きデータあり
        if(Stats.Lv < ret.Lv)
        {

        }
        else
        {
            // 説明をアイテムのものに書き換える
            ItemData itemData =ItemSettings.Instance.Get(Stats.LevelUpItemId);
            ret.Description = itemData.Description;
        }

        // レベルを1あげて返すかどうか
        if(isNextLevel)
        {
            ret.Lv = nextLv;
        }

        return ret;
    }

    // レベルアップ
    public void LevelUp()
    {
        // 現在のレベル
        int lv = Stats.Lv;

        // 次のレベルのデータ
        WeaponSpawnerStats nextData = GetLevelUpStats();

        // 現在のレベルと違えば上書き
        if (Stats.Lv < nextData.Lv)
        {
            Stats = nextData;
        }
        // なければアイテムデータを追加
        else
        {
            // 説明をアイテムのものに書き換える
            ItemData itemData = ItemSettings.Instance.Get(Stats.LevelUpItemId);
            Stats.AddItemData(itemData);
        }

        Stats.Lv = lv + 1;
    }
}
