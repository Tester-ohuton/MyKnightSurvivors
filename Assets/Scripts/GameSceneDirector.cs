using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameSceneDirector : MonoBehaviour
{
    // タイルマップ
    [SerializeField] GameObject grid;
    [SerializeField] Tilemap tilemapCollider;

    // マップ全体座標
    public Vector2 tileMapStart;// カメラを制御
    public Vector2 tileMapEnd;  // カメラを制御
    public Vector2 worldStart;  // プレイヤーを制御
    public Vector2 worldEnd;    // プレイヤーを制御

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform item in grid.GetComponentInChildren<Transform>())
        {
            // 開始位置
            if (item.position.x < tileMapStart.x)
            {
                tileMapStart.x = item.position.x;
            }
            if (item.position.y < tileMapStart.y)
            {
                tileMapStart.y = item.position.y;
            }

            // 終了位置
            if (tileMapEnd.x < item.position.x)
            {
                tileMapEnd.x = item.position.x;
            }
            if (tileMapEnd.y < item.position.y)
            {
                tileMapEnd.y = item.position.y;
            }
        }

        // 画面縦半分の描画範囲（デフォルトで5タイル）
        float cameraSize = Camera.main.orthographicSize - 1;

        // 画面縦横比（16：9想定）
        float aspect = (float)Screen.width / (float)Screen.height;

        // プレイヤーの移動できる範囲
        worldStart = new Vector2(tileMapStart.x - cameraSize * aspect,tileMapStart.y - cameraSize);
        worldEnd = new Vector2(tileMapEnd.x + cameraSize * aspect, tileMapEnd.y + cameraSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
