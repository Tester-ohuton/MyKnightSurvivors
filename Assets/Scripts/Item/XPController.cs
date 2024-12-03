using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPController : MonoBehaviour
{
    GameSceneDirector sceneDirector;
    Rigidbody2D rigidbody2D;
    SpriteRenderer spriteRenderer;

    // 経験値
    float xp;
    // 60秒で消える
    float aliveTimer = 50f;
    float fadeTime = 10f;

    // 初期化
    public void Init(GameSceneDirector sceneDirector,float xp)
    {
        this.sceneDirector = sceneDirector;
        this.xp = xp;

        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // ゲーム停止中
        if (!sceneDirector.enabled) return;

        // タイマー消化で消え始める
        aliveTimer -= Time.deltaTime;

        if(aliveTimer < 0)
        {
            // アルファ値を設定
            Color color = spriteRenderer.color;
            color.a -= 1.0f / fadeTime * Time.deltaTime;

            // 見えなくなったら消す
            if (color.a <= 0)
            {
                Destroy(gameObject);
                return;
            }
        }

        // プレイヤーとの距離
        float dest = Vector2.Distance(transform.position,sceneDirector.Player.transform.position);
        // 取得範囲なら吸い込まれる
        if(dest < sceneDirector.Player.Stats.PickUpRange)
        {
            float speed = sceneDirector.Player.Stats.MoveSpeed * 1.1f;
            Vector2 forward = sceneDirector.Player.transform.position - transform.position;
            rigidbody2D.position += forward.normalized * speed * Time.deltaTime;
        }
    }

    // トリガーが衝突したとき
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーじゃない
        if (!collision.gameObject.TryGetComponent<PlayerController>(out var player)) return;

        // 経験値取得
        player.GetXP(xp);
        Destroy(gameObject);
    }
}
