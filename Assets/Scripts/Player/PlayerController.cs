using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // 移動とアニメーション
    Rigidbody2D rigidbody2d;
    Animator animator;

    // Initでセットされる
    GameSceneDirector sceneDirector;
    Slider sliderHP;
    Slider sliderXP;

    public CharacterStats Stats;

    // 攻撃のクールダウン
    float attackCoolDownTimer;
    float attackCoolDownTimerMax = 0.5f;

    // 必要XP
    List<int> levelRequirements;
    // 敵生成装置
    EnemySpawnerController enemySpawner;
    // 向き
    public Vector2 Forward;
    // レベルテキスト
    Text textLv;
    // 現在装備中の武器
    public List<BaseWeaponSpawner> WeaponSpawners;

    // 追加したアイテムと個数
    public Dictionary<ItemData, int> ItemDatas;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        updateTimer();

        movePlayer();

        moveCamera();

        moveSliderHP();
    }

    // 初期化
    public void Init(GameSceneDirector sceneDirector, EnemySpawnerController enemySpawner,
        CharacterStats characterStats, Text textLv, Slider sliderHP, Slider sliderXP)
    {
        // 変数初期化
        levelRequirements = new List<int>();
        WeaponSpawners = new List<BaseWeaponSpawner>();
        ItemDatas = new Dictionary<ItemData, int>();

        this.sceneDirector = sceneDirector;
        this.enemySpawner = enemySpawner;
        this.Stats = characterStats;
        this.textLv = textLv;
        this.sliderHP = sliderHP;
        this.sliderXP = sliderXP;

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // プレイヤーの向き
        Forward = Vector2.right;

        // 経験値の閾値リスト作成
        levelRequirements.Add(0);
        for (int i = 1; i < 1000; i++)
        {
            // 1つ前の閾値
            int prevxp = levelRequirements[i - 1];
            // 41以降はレベル毎に16XPずつ増加
            int addxp = 16;

            // レベル2までレベルアップするのに5XP
            if (i == 1)
            {
                addxp = 5;
            }
            else if (20 >= i)
            {
                addxp = 10;
            }
            else if (40 >= i)
            {
                addxp = 13;
            }

            // 必要経験値
            levelRequirements.Add(prevxp + addxp);
        }

        // LV2の必要経験値
        Stats.MaxXP = levelRequirements[1];

        // UI初期化
        setTextLv();
        setSliderHP();
        setSliderXP();

        moveSliderHP();

        // 武器データセット
        foreach (var item in Stats.DefaultWeaponIds)
        {
            addWeaponSpawner(item);
        }

    }

    // プレイヤーの移動に関する処理
    void movePlayer()
    {
        // 移動する方向
        Vector2 dir = Vector2.zero;
        // 再生するアニメーション
        string trigger = "";

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            dir += Vector2.up;
            trigger = "isUp";
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            dir -= Vector2.up;
            trigger = "isDown";
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            dir += Vector2.right;
            trigger = "isRight";
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            dir -= Vector2.right;
            trigger = "isLeft";
        }

        // 入力がなければ抜ける
        if (Vector2.zero == dir) return;

        // プレイヤー移動
        rigidbody2d.position += dir.normalized * Stats.MoveSpeed * Time.deltaTime;

        // アニメーションを再生する
        animator.SetTrigger(trigger);

        // 移動範囲制御
        // 始点
        if (rigidbody2d.position.x < sceneDirector.WorldStart.x)
        {
            Vector2 pos = rigidbody2d.position;
            pos.x = sceneDirector.WorldStart.x;
            rigidbody2d.position = pos;
        }
        if (rigidbody2d.position.y < sceneDirector.WorldStart.y)
        {
            Vector2 pos = rigidbody2d.position;
            pos.y = sceneDirector.WorldStart.y;
            rigidbody2d.position = pos;
        }
        // 終点
        if (sceneDirector.WorldEnd.x < rigidbody2d.position.x)
        {
            Vector2 pos = rigidbody2d.position;
            pos.x = sceneDirector.WorldEnd.x;
            rigidbody2d.position = pos;
        }
        if (sceneDirector.WorldEnd.y < rigidbody2d.position.y)
        {
            Vector2 pos = rigidbody2d.position;
            pos.y = sceneDirector.WorldEnd.y;
            rigidbody2d.position = pos;
        }

        Forward = dir;
    }

    // カメラ移動
    void moveCamera()
    {
        Vector3 pos = transform.position;
        pos.z = Camera.main.transform.position.z;

        //始点
        if (pos.x < sceneDirector.TileMapStart.x)
        {
            pos.x = sceneDirector.TileMapStart.x;
        }
        if (pos.y < sceneDirector.TileMapStart.y)
        {
            pos.y = sceneDirector.TileMapStart.y;
        }
        // 終点
        if (sceneDirector.TileMapEnd.x < pos.x)
        {
            pos.x = sceneDirector.TileMapEnd.x;
        }
        if (sceneDirector.TileMapEnd.y < pos.y)
        {
            pos.y = sceneDirector.TileMapEnd.y;
        }

        // カメラの位置を更新する
        Camera.main.transform.position = pos;
    }

    // HPスライダー移動
    void moveSliderHP()
    {
        // ワールド座標をスクリーン座標に変換
        Vector3 pos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        pos.y -= 50;
        sliderHP.transform.position = pos;
    }

    // ダメージ
    public void Damage(float attack)
    {
        // 非アクティブなら抜ける
        if (!enabled) return;

        float damage = Mathf.Max(0, attack - Stats.Defense);
        Stats.HP -= damage;

        // ダメージ表示
        sceneDirector.DispDamage(gameObject, damage);

        // ゲームオーバー
        if (0 > Stats.HP)
        {
            // 操作できないようにする
            SetEnabled(false);

            // アニメーション
            transform.DOScale(new Vector2(5, 0), 2).SetUpdate(true)
                .OnComplete(() =>
                {
                    sceneDirector.DispPanelGameOver();
                });
        }

        if (0 > Stats.HP) Stats.HP = 0;
        setSliderHP();
    }

    // HPスライダーの値を更新
    void setSliderHP()
    {
        sliderHP.maxValue = Stats.MaxHP;
        sliderHP.value = Stats.HP;
    }

    // XPスライダーの値を更新
    void setSliderXP()
    {
        sliderXP.maxValue = Stats.MaxXP;
        sliderXP.value = Stats.XP;
    }

    // 衝突した時
    private void OnCollisionEnter2D(Collision2D collision)
    {
        attackEnemy(collision);
    }

    // 衝突している間
    private void OnCollisionStay2D(Collision2D collision)
    {
        attackEnemy(collision);
    }

    // 衝突が終わった時
    private void OnCollisionExit2D(Collision2D collision)
    {

    }

    // プレイヤーへ攻撃する
    void attackEnemy(Collision2D collision)
    {
        // エネミー以外
        if (!collision.gameObject.TryGetComponent<EnemyController>(out var enemy)) return;
        // タイマー未消化
        if (0 < attackCoolDownTimer) return;

        enemy.Damage(Stats.Attack);
        attackCoolDownTimer = attackCoolDownTimerMax;
    }

    // 各種タイマー更新
    void updateTimer()
    {
        if (0 < attackCoolDownTimer)
        {
            attackCoolDownTimer -= Time.deltaTime;
        }
    }

    // レベルテキスト更新
    void setTextLv()
    {
        textLv.text = "LV " + Stats.Lv;
    }

    // 武器を追加
    void addWeaponSpawner(int id)
    {
        // 装備済みならレベルアップ
        BaseWeaponSpawner spawner = WeaponSpawners.Find(item => item.Stats.Id == id);

        if (spawner)
        {
            spawner.LevelUp();
            return;
        }

        // 新規追加
        spawner = WeaponSpawnerSettings.Instance.CreateWeaponSpawner(id, enemySpawner, transform);

        if (null == spawner)
        {
            Debug.LogError("武器データがありません");
            return;
        }

        // 装備済みリストへ追加
        WeaponSpawners.Add(spawner);
    }

    // 経験値取得
    public void GetXP(float xp)
    {
        Stats.XP += xp;

        // レベル上限
        if (levelRequirements.Count - 1 < Stats.Lv) return;

        // レベルアップ
        if (levelRequirements[Stats.Lv] <= Stats.XP)
        {
            Stats.Lv++;

            // 次の経験値
            if (Stats.Lv < levelRequirements.Count)
            {
                Stats.XP = 0;
                Stats.MaxXP = levelRequirements[Stats.Lv];
            }

            // レベルアップパネル表示
            sceneDirector.DispPanelLevelUp();

            setTextLv();
        }

        // 表示更新
        setSliderXP();
    }

    // 装備可能な武器リスト
    public List<int> GetUsableWeaponIds()
    {
        List<int> ret = new List<int>(Stats.UsableWeaponIds);

        // 装備可能数を超える場合は装備している武器のIDを返す
        if (Stats.UsableWeaponMax - 1 < WeaponSpawners.Count)
        {
            ret.Clear();
            foreach (var item in WeaponSpawners)
            {
                ret.Add(item.Stats.Id);
            }
        }

        return ret;
    }

    // 装備可能な武器をランダムで返す
    public WeaponSpawnerStats GetRandomSpawnerStats()
    {
        // 装備可能な武器ID
        List<int> usableIds = GetUsableWeaponIds();

        // 装備可能な武器がない(一応)
        if (1 > usableIds.Count)
        {
            return null;
        }

        // 抽選
        int rnd = Random.Range(0, usableIds.Count);
        int id = usableIds[rnd];

        // 装備済みなら次のレベルのデータ
        BaseWeaponSpawner spawner = WeaponSpawners.Find(item => item.Stats.Id == id);
        if (spawner)
        {
            return spawner.GetLevelUpStats(true);
        }

        // 新規ならレベル1のデータ
        return WeaponSpawnerSettings.Instance.Get(id, 1);
    }

    // アイテムを追加
    void addItemData(int id)
    {
        ItemData itemData = ItemSettings.Instance.Get(id);

        if (null == itemData)
        {
            Debug.LogError("アイテムデータが見つかりませんでした");
            return;
        }

        // データ追加
        Stats.AddItemData(itemData);

        // 取得済みリストへ追加
        ItemData key = null;
        foreach (var item in ItemDatas)
        {
            if (item.Key.Id == itemData.Id)
            {
                key = item.Key;
                break;
            }
        }

        if (null == key)
        {
            ItemDatas.Add(itemData, 0);
            key = itemData;
        }

        ItemDatas[key]++;
    }

    // レベルアップやアイテム取得時
    public void AddBonusData(BonusData bonusData)
    {
        if (null == bonusData) return;

        // 武器データ
        if (null != bonusData.WeaponSpawnerStats)
        {
            addWeaponSpawner(bonusData.WeaponSpawnerStats.Id);
        }

        // アイテムデータ
        if (null != bonusData.ItemData)
        {
            addItemData(bonusData.ItemData.Id);
        }

        // 表示更新
        setSliderHP();
    }

    // アップデート停止
    public void SetEnabled(bool enabled = true)
    {
        this.enabled = enabled;

        // 武器
        foreach (var item in WeaponSpawners)
        {
            item.SetEnabled(enabled);
        }
    }
}
