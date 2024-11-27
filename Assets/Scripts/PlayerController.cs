using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Slider sliderHP;

    [SerializeField] GameSceneDirector gameSceneDirector;

    // 移動とアニメーション
    Rigidbody2D rb2D;
    Animator animator;
    public float moveSpeed = 2;

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
        MoveCamera();
        MoveSliderHP();
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

        // 移動範囲制御
        // 始点
        if(rb2D.position.x < gameSceneDirector.worldStart.x)
        {
            Vector2 pos = rb2D.position;
            pos.x = gameSceneDirector.worldStart.x;
            rb2D.position = pos;
        }

        if (rb2D.position.y < gameSceneDirector.worldStart.y)
        {
            Vector2 pos = rb2D.position;
            pos.y = gameSceneDirector.worldStart.y;
            rb2D.position = pos;
        }

        // 終点
        if (gameSceneDirector.worldEnd.x < rb2D.position.x)
        {
            Vector2 pos = rb2D.position;
            pos.x = gameSceneDirector.worldEnd.x;
            rb2D.position = pos;
        }

        if (gameSceneDirector.worldEnd.y < rb2D.position.y)
        {
            Vector2 pos = rb2D.position;
            pos.y = gameSceneDirector.worldEnd.y;
            rb2D.position = pos;
        }
    }

    // カメラ移動
    private void MoveCamera()
    {
        Vector3 pos = transform.position;
        pos.z = Camera.main.transform.position.z;

        // 始点
        if(pos.x < gameSceneDirector.tileMapStart.x)
        {
            pos.x = gameSceneDirector.tileMapStart.x;
        }
        if (pos.y < gameSceneDirector.tileMapStart.y)
        {
            pos.y = gameSceneDirector.tileMapStart.y;
        }

        // 終点
        if (gameSceneDirector.tileMapEnd.x < pos.x)
        {
            pos.x = gameSceneDirector.tileMapEnd.x;
        }
        if (gameSceneDirector.tileMapEnd.y < pos.y)
        {
            pos.y = gameSceneDirector.tileMapEnd.y;
        }

        // カメラ座標更新
        Camera.main.transform.position = pos;
    }

    // HPスライダー移動
    private void MoveSliderHP()
    {
        Vector3 pos = RectTransformUtility.WorldToScreenPoint(Camera.main,transform.position);
        pos.y -= 50;
        sliderHP.transform.position = pos;
    }
}
