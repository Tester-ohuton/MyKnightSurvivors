using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    // 親の生成装置
    protected BaseWeaponSpawner spawner;
    // 武器ステータス
    protected WeaponSpawnerStats stats;
    // 物理挙動
    protected Rigidbody2D rigidbody2D;
    // 方向
    protected Vector2 forward;

    // 初期化
    public void Init(BaseWeaponSpawner spawner,Vector2 forward)
    {
        // 親の生成位置
        this.spawner = spawner;
        // 武器データセット
        this.stats = (WeaponSpawnerStats)spawner.Stats.GetCopy();
        // 進む方向
        this.forward = forward;
        // 物理挙動
        this.rigidbody2D = rigidbody2D;

        // 生存時間があれば設定する
        if(-1 < stats.AliveTime)
        {
            Destroy(gameObject, stats.AliveTime);
        }
    }

    // 敵へ攻撃
    protected void attackEnemy(Collider2D collider2D,float attack)
    {
        // 敵じゃない
        if (!collider2D.gameObject.TryGetComponent<EnemyController>(out var enemy)) return;
        // 攻撃
        float damage = enemy.Damage(attack);
        // 総ダメージ計算
        spawner.TotalDamage += damage;

        // HPがあれば自分もダメージ
        if (stats.HP < 0) return;
        stats.HP--;
        if (stats.HP < 0) Destroy(gameObject);
    }

    // 敵へ攻撃（デフォルトの攻撃力）
    protected void attackEnemy(Collider2D collider2D)
    {
        attackEnemy(collider2D,stats.Attack);
    }
}
