using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Slider sliderHP;

    [SerializeField] GameSceneDirector gameSceneDirector;

    // �ړ��ƃA�j���[�V����
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

    // �v���C���[�̈ړ��Ɋւ��鏈��
    private void MovePlayer()
    {
        Vector2 dir = Vector2.zero;
        string triggers = "";

        // ��ړ�
        if(Input.GetKey(KeyCode.UpArrow))
        {
            dir += Vector2.up;
            triggers = "isUp";
        }

        // ���ړ�
        if (Input.GetKey(KeyCode.DownArrow))
        {
            dir -= Vector2.up;
            triggers = "isDown";
        }

        // ���ړ�
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir -= Vector2.right;
            triggers = "isLeft";
        }

        // �E�ړ�
        if (Input.GetKey(KeyCode.RightArrow))
        {
            dir += Vector2.right;
            triggers = "isRight";
        }

        // ���͂��Ȃ���΂ʂ���
        if (Vector2.zero == dir) return;

        // �v���C���[�ړ�
        rb2D.position += dir.normalized * moveSpeed *Time.deltaTime;
        
        // �A�j���[�V�������Đ�
        animator.SetTrigger(triggers);

        // �ړ��͈͐���
        // �n�_
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

        // �I�_
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

    // �J�����ړ�
    private void MoveCamera()
    {
        Vector3 pos = transform.position;
        pos.z = Camera.main.transform.position.z;

        // �n�_
        if(pos.x < gameSceneDirector.tileMapStart.x)
        {
            pos.x = gameSceneDirector.tileMapStart.x;
        }
        if (pos.y < gameSceneDirector.tileMapStart.y)
        {
            pos.y = gameSceneDirector.tileMapStart.y;
        }

        // �I�_
        if (gameSceneDirector.tileMapEnd.x < pos.x)
        {
            pos.x = gameSceneDirector.tileMapEnd.x;
        }
        if (gameSceneDirector.tileMapEnd.y < pos.y)
        {
            pos.y = gameSceneDirector.tileMapEnd.y;
        }

        // �J�������W�X�V
        Camera.main.transform.position = pos;
    }

    // HP�X���C�_�[�ړ�
    private void MoveSliderHP()
    {
        Vector3 pos = RectTransformUtility.WorldToScreenPoint(Camera.main,transform.position);
        pos.y -= 50;
        sliderHP.transform.position = pos;
    }
}
