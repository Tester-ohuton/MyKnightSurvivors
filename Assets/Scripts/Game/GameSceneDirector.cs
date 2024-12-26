using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using unityroom.Api;

public class GameSceneDirector : MonoBehaviour
{
    [SerializeField] int playerScore;
    // タイルマップ
    [SerializeField] GameObject grid;
    [SerializeField] Tilemap tilemapCollider;
    // マップ全体座標
    public Vector2 TileMapStart;
    public Vector2 TileMapEnd;
    public Vector2 WorldStart;
    public Vector2 WorldEnd;

    public PlayerController Player;

    [SerializeField] Transform parentTextDamage;
    [SerializeField] GameObject prefabTextDamage;

    // タイマー
    [SerializeField] Text textTimer;
    public float GameTimer;
    public float OldSeconds;

    // 敵生成
    [SerializeField] EnemySpawnerController enemySpawner;

    // プレイヤー生成
    [SerializeField] Slider sliderXP;
    [SerializeField] Slider sliderHP;
    [SerializeField] Text textLv;

    // 背景変更
    [SerializeField] Image imageBackground;

    // 経験値
    [SerializeField] List<GameObject> prefabXP;

    // レベルアップパネル
    [SerializeField] PanelLevelUpController panelLevelUp;
    public ParticleSystem[] particle;

    // 宝箱関連
    [SerializeField] PanelTreasureChestController panelTreasureChest;
    [SerializeField] GameObject[] prefabTreasureChest;
    [SerializeField] List<int> treasureChestItemIds;
    [SerializeField] float treasureChestTimerMin;
    [SerializeField] float treasureChestTimerMax;
    float treasureChestTimer;

    // 左上に表示するアイコン
    [SerializeField] Transform canvas;
    [SerializeField] GameObject prefabImagePlayerIcon;
    Dictionary<BaseWeaponSpawner, GameObject> playerWeaponIcons;
    Dictionary<ItemData, GameObject> playerItemIcons;
    const int PlayerIconStartX = 20;
    const int PlayerIconStartY = -40;

    // 倒した敵のカウント
    [SerializeField] Text textDefeatedEnemy;
    public int DefeatedEnemyCount;

    // ゲームオーバー
    [SerializeField] PanelGameOverController panelGameOver;

    // 終了時間
    [SerializeField] float GameOverTime;

    const string statisticsName = "HighScore";

    // SE
    [SerializeField] SoundPlayer soundPlayer;

    // Start is called before the first frame update
    void Start()
    {
        // 変数初期化
        playerWeaponIcons = new Dictionary<BaseWeaponSpawner, GameObject>();
        playerItemIcons = new Dictionary<ItemData, GameObject>();

        // 背景変更
        int sceneId = TitleSceneDirector.SceneId;
        Sprite background = CharacterSettings.Instance.GetBackground(sceneId);
        imageBackground.sprite = background;

        // プレイヤー作成
        int playerId = TitleSceneDirector.CharacterId;
        Player = CharacterSettings.Instance.CreatePlayer1(playerId, this, enemySpawner,
            textLv, sliderHP, sliderXP);

        // 初期設定
        OldSeconds = -1;
        enemySpawner.Init(this, tilemapCollider);
        panelLevelUp.Init(this);
        panelTreasureChest.Init(this);
        panelGameOver.Init(this);

        // カメラの移動できる範囲
        foreach (Transform item in grid.GetComponentInChildren<Transform>())
        {
            // 開始位置
            if (TileMapStart.x > item.position.x)
            {
                TileMapStart.x = item.position.x;
            }
            if (TileMapStart.y > item.position.y)
            {
                TileMapStart.y = item.position.y;
            }
            // 終了位置
            if (TileMapEnd.x < item.position.x)
            {
                TileMapEnd.x = item.position.x;
            }
            if (TileMapEnd.y < item.position.y)
            {
                TileMapEnd.y = item.position.y;
            }
        }

        // 画面縦半分の描画範囲（デフォルトで5タイル）
        float cameraSize = Camera.main.orthographicSize;
        // 画面縦横比（16:9想定）
        float aspect = (float)Screen.width / (float)Screen.height;
        // プレイヤーの移動できる範囲
        WorldStart = new Vector2(TileMapStart.x - cameraSize * aspect, TileMapStart.y - cameraSize);
        WorldEnd = new Vector2(TileMapEnd.x + cameraSize * aspect, TileMapEnd.y + cameraSize);

        // 初期値
        treasureChestTimer = Random.Range(treasureChestTimerMin, treasureChestTimerMax);
        DefeatedEnemyCount = -1;

        // アイコン更新
        dispPlayerIcon();

        // 倒した敵更新
        AddDefeatedEnemy();

        // TimeScaleリセット
        setEnabled();
    }

    // Update is called once per frame
    void Update()
    {
        // ゲームタイマー更新
        updateGameTimer();

        // 宝箱生成
        updateTreasureChestSpawner();

        // 秒数経過でゲームオーバー
        if (GameOverTime < GameTimer)
        {
            DispPanelGameOver();
        }
    }

    // ダメージ表示
    public void DispDamage(GameObject target, float damage)
    {
        GameObject obj = Instantiate(prefabTextDamage, parentTextDamage);
        obj.GetComponent<TextDamageController>().Init(target, damage);
    }

    // ゲームタイマー
    void updateGameTimer()
    {
        GameTimer += Time.deltaTime;

        // 前回と秒数が同じなら処理をしない
        int seconds = (int)GameTimer % 60;
        if (seconds == OldSeconds) return;

        textTimer.text = Utils.GetTextTimer(GameTimer);
        OldSeconds = seconds;
    }

    // 経験値取得
    public void CreateXP(EnemyController enemy)
    {
        float xp = Random.Range(enemy.Stats.XP, enemy.Stats.MaxXP);
        if (0 > xp) return;

        // 5未満
        GameObject prefab = prefabXP[0];

        // 10以上
        if (10 <= xp)
        {
            prefab = prefabXP[2];
        }
        // 5以上
        else if (5 <= xp)
        {
            prefab = prefabXP[1];
        }

        // 初期化
        GameObject obj = Instantiate(prefab, enemy.transform.position, Quaternion.identity);
        XPController ctrl = obj.GetComponent<XPController>();
        ctrl.Init(this, xp);
    }

    // ゲーム再開/停止
    void setEnabled(bool enabled = true)
    {
        //this.enabled = enabled;
        Time.timeScale = (enabled) ? 1 : 0;
        Player.SetEnabled(enabled);
    }

    // ゲーム再開
    public void PlayGame(BonusData bonusData = null)
    {
        // アイテム追加
        Player.AddBonusData(bonusData);
        // ステータス反映
        dispPlayerIcon();

        // ゲーム再開
        setEnabled();
    }

    // レベルアップ時
    public void DispPanelLevelUp()
    {
        // 追加したアイテム
        List<WeaponSpawnerStats> items = new List<WeaponSpawnerStats>();

        // 生成数
        int randomCount = panelLevelUp.GetButtonCount();
        // 武器の数が足りない場合は減らす
        int listCount = Player.GetUsableWeaponIds().Count;

        if (listCount < randomCount)
        {
            randomCount = listCount;
        }

        // ボーナスをランダムで生成
        for (int i = 0; i < randomCount; i++)
        {
            // 装備可能武器からランダム
            WeaponSpawnerStats randomItem = Player.GetRandomSpawnerStats();
            // データ無し
            if (null == randomItem) continue;

            // かぶりチェック
            WeaponSpawnerStats findItem
                = items.Find(item => item.Id == randomItem.Id);

            // かぶりなし
            if (null == findItem)
            {
                items.Add(randomItem);
            }
            // もう一回
            else
            {
                i--;
            }
        }

        // 独立した時間の計算
        var main = particle[0].main;
        main.useUnscaledTime = true; // Unscaled時間で動かす

        // レベルアップパネル表示
        panelLevelUp.DispPanel(items);
        // ゲーム停止
        setEnabled(false);
    }

    // 宝箱パネルを表示
    public void DispPanelTreasureChest()
    {
        // ランダムアイテム
        ItemData item = getRandomItemData();
        // データ無し
        if (null == item) return;

        // 独立した時間の計算
        var main = particle[1].main;
        main.useUnscaledTime = true; // Unscaled時間で動かす

        // パネル表示
        panelTreasureChest.DispPanel(item);
        // ゲーム中断
        setEnabled(false);
    }

    // アイテムをランダムで返す
    ItemData getRandomItemData()
    {
        if (1 > treasureChestItemIds.Count) return null;

        // 抽選
        int rnd = Random.Range(0, treasureChestItemIds.Count);
        return ItemSettings.Instance.Get(treasureChestItemIds[rnd]);
    }

    // 宝箱生成
    void updateTreasureChestSpawner()
    {
        // タイマー
        treasureChestTimer -= Time.deltaTime;
        // タイマー未消化
        if (0 < treasureChestTimer) return;

        // 生成場所
        float x = Random.Range(WorldStart.x, WorldEnd.x);
        float y = Random.Range(WorldStart.y, WorldEnd.y);

        // 当たり判定のあるタイル上かどうか
        if (Utils.IsColliderTile(tilemapCollider, new Vector2(x, y))) return;

        // 生成
        int random = Random.Range(0, prefabTreasureChest.Length);
        GameObject obj = Instantiate(prefabTreasureChest[random], new Vector3(x, y, 0), Quaternion.identity);
        if (!obj.TryGetComponent<TreasureChestController>(out var chest)) return;
        chest.Init(this);

        // 次のタイマーセット
        treasureChestTimer = Random.Range(treasureChestTimerMin, treasureChestTimerMax);
    }

    // プレイヤーアイコンセット
    void setPlayerIcon(GameObject obj, Vector2 pos, Sprite icon, int count)
    {
        // 画像
        Transform image = obj.transform.Find("ImageIcon");
        image.GetComponent<Image>().sprite = icon;

        // テキスト
        Transform text = obj.transform.Find("TextCount");
        text.GetComponent<TextMeshProUGUI>().text = "" + count;

        // 場所
        obj.GetComponent<RectTransform>().anchoredPosition = pos;
    }

    // アイコンの表示を更新
    void dispPlayerIcon()
    {
        // 武器アイコン表示位置
        float x = PlayerIconStartX;
        float y = PlayerIconStartY;
        float w = prefabImagePlayerIcon.GetComponent<RectTransform>().sizeDelta.x + 1;

        foreach (var item in Player.WeaponSpawners)
        {
            // 作成済みのデータがあれば取得する
            playerWeaponIcons.TryGetValue(item, out GameObject obj);

            // なければ作成する
            if (!obj)
            {
                obj = Instantiate(prefabImagePlayerIcon, canvas);
                playerWeaponIcons.Add(item, obj);
            }

            // アイコンセット
            setPlayerIcon(obj, new Vector2(x, y), item.Stats.Icon, item.Stats.Lv);

            // 次の位置
            x += w;
        }

        // アイテムアイコン表示位置
        x = PlayerIconStartX;
        y = PlayerIconStartY - w;

        foreach (var item in Player.ItemDatas)
        {
            // 作成済みのデータがあれば取得する
            playerItemIcons.TryGetValue(item.Key, out GameObject obj);

            // なければ作成する
            if (!obj)
            {
                obj = Instantiate(prefabImagePlayerIcon, canvas);
                playerItemIcons.Add(item.Key, obj);
            }

            // アイコンセット
            setPlayerIcon(obj, new Vector2(x, y), item.Key.Icon, item.Value);

            // 次の位置
            x += w;
        }
    }

    // 倒した敵をカウント
    public void AddDefeatedEnemy()
    {
        DefeatedEnemyCount++;
        textDefeatedEnemy.text = "" + DefeatedEnemyCount;
        playerScore += DefeatedEnemyCount;
    }

    // タイトルへ
    public void LoadSceneTitle()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("TitleScene");
    }

    // ゲームオーバーパネルを表示
    public void DispPanelGameOver()
    {
        // パネル表示
        panelGameOver.DispPanel(Player.WeaponSpawners);
        // ゲーム中断
        setEnabled(false);
    }

    public void RankingSend()
    {
        // C#スクリプトの冒頭に `using unityroom.Api;` を追加してください。

        // ボードNo1にスコア123.45fを送信する。
        UnityroomApiClient.Instance.SendScore(1, (float)playerScore, ScoreboardWriteMode.HighScoreAsc);
    }
}
