using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public CharacterStats Stats;

    [SerializeField] GameSceneDirector sceneDirector;
    Rigidbody2D rigidbody2d;

    // SE
    [SerializeField] SoundPlayer soundPlayer;

    // 攻撃のクールダウン
    float attackCoolDownTimer;
    float attackCoolDownTimerMax = 0.5f;
    // 向き
    Vector2 forward;

    // 状態
    enum State
    {
        Alive,
        Dead
    }
    State state;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer();

        moveEnemy();
    }

    // 初期化
    public void Init(GameSceneDirector sceneDirector, CharacterStats characterStats)
    {
        this.sceneDirector = sceneDirector;
        this.Stats = characterStats;

        rigidbody2d = GetComponent<Rigidbody2D>();

        // アニメーション
        // ランダムで緩急をつける
        float random = Random.Range(0.8f, 1.2f);
        float speed = 1 / Stats.MoveSpeed * random;

        // サイズ
        float addx = 0.8f;
        float x = addx * random;
        transform.DOScale(x, speed)
            .SetRelative()
            .SetLoops(-1, LoopType.Yoyo);

        // 回転
        float addz = 10f;
        float z = Random.Range(-addz, addz) * random;
        // 初期値
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = z;
        // 目標値
        transform.eulerAngles = rotation;
        transform.DORotate(new Vector3(0, 0, -z), speed)
            .SetLoops(-1, LoopType.Yoyo);

        // 進む方向
        PlayerController player = sceneDirector.Player;
        Vector2 dir = player.transform.position - transform.position;
        forward = dir;

        state = State.Alive;
    }

    // プレイヤーを追いかける
    void moveEnemy()
    {
        if (State.Alive != state) return;

        // 目的がプレイヤーなら進む方向を更新する
        if (MoveType.TargetPlayer == Stats.MoveType)
        {
            PlayerController player = sceneDirector.Player;
            Vector2 dir = player.transform.position - transform.position;
            forward = dir;
        }

        // 移動
        rigidbody2d.position += forward.normalized * Stats.MoveSpeed * Time.deltaTime;
    }

    // 各種タイマー更新
    void updateTimer()
    {
        if (0 < attackCoolDownTimer)
        {
            attackCoolDownTimer -= Time.deltaTime;
        }

        // 生存時間が設定されていたらタイマー消化
        if (0 < Stats.AliveTime)
        {
            Stats.AliveTime -= Time.deltaTime;
            if (0 > Stats.AliveTime) setDead(false);
        }
    }

    // 敵が死んだ時に呼び出される
    void setDead(bool createXP = true)
    {
        if (State.Alive != state) return;

        // 物理挙動を停止
        rigidbody2d.simulated = false;

        // アニメーションを停止
        transform.DOKill();

        // 縦に潰れるアニメーション
        transform.DOScaleY(0, 0.5f).OnComplete(() => Destroy(gameObject));

        // 経験値を作成
        if (createXP)
        {
            // 経験値生成
            sceneDirector.CreateXP(this);
        }

        state = State.Dead;
    }

    // 衝突した時
    private void OnCollisionEnter2D(Collision2D collision)
    {
        attackPlayer(collision);
    }

    // 衝突している間
    private void OnCollisionStay2D(Collision2D collision)
    {
        attackPlayer(collision);
    }

    // 衝突が終わった時
    private void OnCollisionExit2D(Collision2D collision)
    {

    }

    // プレイヤーへ攻撃する
    void attackPlayer(Collision2D collision)
    {
        // プレイヤー以外
        if(!collision.gameObject.TryGetComponent<PlayerController>(out var player)) return;
        // タイマー未消化
        if (0 < attackCoolDownTimer) return;
        // 非アクティブ
        if(State.Alive != state) return;

        player.Damage(Stats.Attack);

        attackCoolDownTimer = attackCoolDownTimerMax;
    }

    // ダメージ
    public float Damage(float attack)
    {
        if(State.Alive != state) return 0;

        float damage = Mathf.Max(0, attack - Stats.Defense);

        /*
         // 2024/12/04 =になっていたのを修正
         敵のHPが減らないバグ修正
         */
        Stats.HP -= damage;

        sceneDirector.DispDamage(gameObject, damage);

        // 消滅
        if (Stats.HP < 0)
        {
            sceneDirector.AddDefeatedEnemy();
            setDead();
        }

        // 計算後のダメージを返す
        return damage;
    }
}
