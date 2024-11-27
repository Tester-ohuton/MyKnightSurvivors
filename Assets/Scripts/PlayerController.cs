using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 移動とアニメーション
    Rigidbody2D rb2D;
    Animator animator;
    private float moveSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    // プレイヤーの移動に関する処理
    private void MovePlayer()
    {
        Vector2 dir = Vector2.zero;
        string triggers = "";

        // 上移動
        if(Input.GetKey(KeyCode.UpArrow))
        {
            dir += Vector2.up;
            triggers = "isUp";
        }

        // 下移動
        if (Input.GetKey(KeyCode.DownArrow))
        {
            dir -= Vector2.up;
            triggers = "isDown";
        }

        // 左移動
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir -= Vector2.right;
            triggers = "isLeft";
        }

        // 右移動
        if (Input.GetKey(KeyCode.RightArrow))
        {
            dir += Vector2.right;
            triggers = "isRight";
        }

        // 入力がなければぬける
        if (Vector2.zero == dir) return;

        // プレイヤー移動
        rb2D.position += dir.normalized * moveSpeed *Time.deltaTime;
        
        // アニメーションを再生
        animator.SetTrigger(triggers);
    }
}
