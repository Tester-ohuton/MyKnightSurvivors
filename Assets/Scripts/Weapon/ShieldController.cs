using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShieldController : BaseWeapon
{
    // プレイヤーからの距離
    const float Radius = 1f;
    // 現在の角度
    public float Angle;

    // Start is called before the first frame update
    void Start()
    {
        // フワッと表示する
        transform.localScale = new Vector3(0, 0, 0);
        transform.DOScale(new Vector3(1, 1, 1), 1.5f).SetEase(Ease.OutBounce);
    }

    // Update is called once per frame
    void Update()
    {
        // 角度更新
        Angle -= stats.MoveSpeed * Time.deltaTime;

        // Cos関数にラジアン角を指定すると、xの座標を返してくれる、radiusをかけてワールド座標に変換する
        float x = Mathf.Cos(Angle * Mathf.Deg2Rad) * Radius;
        // Sin関数にラジアン角を指定すると、yの座標を返してくれる、radiusをかけてワールド座標に変換する
        float y = Mathf.Sin(Angle * Mathf.Deg2Rad) * Radius;

        // ポジション更新
        transform.position = transform.root.position + new Vector3(x, y, 0);
    }

    // トリガーが衝突した時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 敵以外
        if (!collision.gameObject.TryGetComponent<EnemyController>(out var enemy)) return;

        // 反対側へ跳ね返す
        Vector3 forward = enemy.transform.position - transform.root.position;
        enemy.GetComponent<Rigidbody2D>().AddForce(forward.normalized * 5);

        attackEnemy(collision);
    }
}
