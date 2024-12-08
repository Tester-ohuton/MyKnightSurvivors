using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : BaseWeapon
{
    // 状態
    enum State
    {
        Bomb,
        Explosion,
        DamageFloor,
        Destroy,
    }
    State state;

    // アニメーター
    Animator animator;
    // アニメーション毎のタイマー
    Dictionary<State, float> animationTimer;
    // ダメージフロア滞在時間
    float damageFloorCoolDownTime = 0.5f;
    // 敵毎のタイマー（ダメージフロア時）
    Dictionary<EnemyController, float> damageFloorTimer;

    // Start is called before the first frame update
    void Start()
    {
        // 初期化
        animationTimer = new Dictionary<State, float>();
        damageFloorTimer = new Dictionary<EnemyController, float>();
        animator = GetComponent<Animator>();

        // 爆弾時
        animationTimer.Add(State.Bomb, Random.Range(0.5f, 1.5f));
        // 爆発時
        animationTimer.Add(State.Explosion, 0.66f);
        // ダメージフロア
        animationTimer.Add(State.DamageFloor, 30f);

        // 初期状態
        state = State.Bomb;
    }

    // Update is called once per frame
    void Update()
    {
        // タイマー消化で次の状態へ
        if (animationTimer.ContainsKey(state))
        {
            animationTimer[state] -= Time.deltaTime;
            if (animationTimer[state] < 0)
            {
                changeState(++state);
            }
        }
    }

    // 爆弾の状態を変える
    void changeState(State next)
    {
        // 爆発
        if (State.Explosion == next)
        {
            animator.SetTrigger("isExplosion");
            rigidbody2d.gravityScale = 0;
            rigidbody2d.velocity = Vector2.zero;
        }
        // ダメージフロアー
        else if (State.DamageFloor == next)
        {
            animator.SetTrigger("isDamageFloor");
            // 下側に入れる
            GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        // 終了
        else if (State.Destroy == next)
        {
            Destroy(gameObject);
        }

        // 現在の状態
        state = next;
    }

    // 衝突時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 敵以外
        if (!collision.gameObject.TryGetComponent<EnemyController>(out var enemy)) return;

        // 爆弾
        if (State.Bomb == state)
        {
            attackEnemy(collision);
            changeState(State.Explosion);
        }
        // 爆発中
        else if (State.Explosion == state)
        {
            attackEnemy(collision);
        }
    }

    // 衝突している間
    private void OnTriggerStay2D(Collider2D collision)
    {
        // ダメージフロアーじゃない
        if (State.DamageFloor != state) return;

        // 敵以外
        if (!collision.gameObject.TryGetComponent<EnemyController>(out var enemy)) return;

        // ターゲットのタイマーをセット
        damageFloorTimer.TryAdd(enemy, damageFloorCoolDownTime);
        // 敵毎にタイマーを消化
        damageFloorTimer[enemy] -= Time.deltaTime;

        // 一定時間でダメージ
        if (damageFloorTimer[enemy] < 0)
        {
            attackEnemy(collision, stats.Attack / 3);
            damageFloorTimer[enemy] = damageFloorCoolDownTime;
        }
    }
}
