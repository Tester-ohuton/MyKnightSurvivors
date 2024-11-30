using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class GameSceneDirector : MonoBehaviour
{
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
    [SerializeField] Slider sliderHP;
    [SerializeField] Slider sliderXP;
    [SerializeField] Text textLv;

    // Start is called before the first frame update
    void Start()
    {
        // プレイヤー生成
        int playerId = 0;
        Player = CharacterSettings.Instance.CreatePlayer(playerId,this,enemySpawner,
            textLv,sliderHP,sliderXP);

        // 初期設定
        OldSeconds = -1;
        enemySpawner.Init(this, tilemapCollider);

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
    }

    // Update is called once per frame
    void Update()
    {
        // タイマー更新
        updateGameTimer();
    }

    // ダメージ表示
    public void DispDamage(GameObject target,float damage)
    {
        GameObject obj = Instantiate(prefabTextDamage,parentTextDamage);
        obj.GetComponent<TextDamageController>().Init(target,damage);
    }

    void updateGameTimer()
    {
        GameTimer += Time.deltaTime;

        // 前回と秒数が同じなら処理をしない
        int seconds = (int)GameTimer % 60;
        if (seconds == OldSeconds) return;

        textTimer.text=Utils.GetTextTimer(GameTimer);
        OldSeconds = seconds;
    }
}
