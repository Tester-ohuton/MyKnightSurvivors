using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

// 生成タイプ
public enum SpawnType
{
    Normal,
    Group,
}

[Serializable]
public class EnemySpawnData
{
    // 説明用
    public string Title;

    // 出現経過時間
    public int ElaapsedMinutes;
    public int ElapsedSeconds;
    // 出現タイプ
    public SpawnType SpawnType;
    // 生成時間
    public float SpawnDuration;
    // 生成数
    public int SpawnCountMax;
    // 生成する敵ID
    public List<int> EnemyIDs;
}

public class EnemySpawnerController : MonoBehaviour
{
    // 敵データ
    [SerializeField] List<EnemySpawnData> enemySpawnDatas;
    // 生成した敵
    List<EnemyController> enemies;

    // シーンディレクター
    GameSceneDirector sceneDirector;
    // 当たり判定のあるタイルマップ
    Tilemap tilemapCollider;
    // 現在の参照データ
    EnemySpawnData enemySpawnData;
    
    float oldSeconds;
    // 経過時間
    float spawnTimer;
    // 現在のデータ位置
    int spawnDataIndex;
    // 敵の出現位置
    const float SpawnRadius = 13;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 敵生成データ更新

        // 生成

    }

    // 初期化
    public void Init(GameSceneDirector sceneDirector,Tilemap tilemapCollider)
    {
        this.sceneDirector = sceneDirector;
        this.tilemapCollider = tilemapCollider;

        // 生成した敵を保存
        enemies = new List<EnemyController>();
        spawnDataIndex = -1;
    }

    // 敵生成
    void spawnEnemy()
    {
        // 現在のデータ
        if (null == enemySpawnData) return;

        // タイマー消化
        spawnTimer -= Time.deltaTime;
        if (0 < spawnTimer) return;

        if(SpawnType.Group == enemySpawnData.SpawnType)
        {
            spawnGroup();
        }
        else if (SpawnType.Normal == enemySpawnData.SpawnType)
        {
            spawnNormal();
        }

        spawnTimer = enemySpawnData.SpawnDuration;
    }

    // 通常生成
    void spawnNormal()
    {
        // プレイヤー位置
        Vector3 center = sceneDirector.Player.transform.position;

        // 敵生成
        for (int i = 0; i < enemySpawnData.SpawnCountMax; i++)
        {
            // プレイヤーの周りから出現させる
            float angle = 360 / enemySpawnData.SpawnCountMax * i;
            // Cos関数にラジアン角を指定すると、xの座標を返してくれる、radiusをかけてワールド座標に変換する
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * SpawnRadius;
            // Sin関数にラジアン角を指定すると、yの座標を返してくれる、radiusをかけてワールド座標に変換する
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * SpawnRadius;

            // 生成位置
            Vector2 pos = center + new Vector3(x,y,0);

            // 当たり判定のあるタイル上なら生成しない
            if (Utils.IsColliderTile(tilemapCollider, pos)) continue;

            // 生成
            createRandomEnemy(pos);
        }
    }

    // ランダムなIDの敵生成
    void createRandomEnemy(Vector3 pos)
    {
        // データからランダムなIDを取得
        int rnd = Random.Range(0,enemySpawnData.EnemyIDs.Count);
        int id = enemySpawnData.EnemyIDs[rnd];

        // 敵生成
        EnemyController enemy = CharacterSettings.Instance.CreateEnemy(id,sceneDirector,pos);
        enemies.Add(enemy);
    }

    // グループで生成
    void spawnGroup()
    {
        // プレイヤー位置
        Vector3 center = sceneDirector.Player.transform.position;

        // プレイヤーの周りから出現させる
        float angle = Random.Range(0,360);
        // Cos関数にラジアン角を指定すると、xの座標を返してくれる、radiusをかけてワールド座標に変換する
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * SpawnRadius;
        // Sin関数にラジアン角を指定すると、yの座標を返してくれる、radiusをかけてワールド座標に変換する
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * SpawnRadius;

        // 生成位置
        center += new Vector3(x, y, 0);
        float radius = 0.5f;

        // 敵生成
        for (int i = 0; i < enemySpawnData.SpawnCountMax; i++)
        {
            // プレイヤーの周りから出現させる
            angle = 360 / enemySpawnData.SpawnCountMax * i;
            // Cos関数にラジアン角を指定すると、xの座標を返してくれる、radiusをかけてワールド座標に変換する
            x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            // Sin関数にラジアン角を指定すると、yの座標を返してくれる、radiusをかけてワールド座標に変換する
            y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            // 生成位置
            Vector2 pos = center + new Vector3(x, y, 0);

            // 当たり判定のあるタイル上なら生成しない
            if (Utils.IsColliderTile(tilemapCollider, pos)) continue;

            // 生成
            createRandomEnemy(pos);
        }
    }

    // 経過秒数で敵生成データを入れ替える
    void updateEnemySpawnData()
    {
        // 経過秒数に違いがあれば
        if (oldSeconds == sceneDirector.OldSeconds) return;

        // 1つ先のデータを参照
        int idx =spawnDataIndex + 1;

        // データの最後
        if (enemySpawnDatas.Count - 1 < idx) return;

        // 設定された経過時間を超えていたらデータを入れ替える
        EnemySpawnData data = enemySpawnDatas[idx];
        int elapsedSeconds = data.ElaapsedMinutes * 60 + data.ElapsedSeconds;
        
        if(elapsedSeconds < sceneDirector.GameTimer)
        {
            enemySpawnData = enemySpawnDatas[idx];

            // 次回用の設定
            spawnDataIndex = idx;
            spawnTimer = 0;
            oldSeconds = sceneDirector.OldSeconds;
        }
    }

    // 全ての敵を返す
    public List<EnemyController> GetEnemies()
    {
        enemies.RemoveAll(item => !item);
        return enemies;
    }
}
