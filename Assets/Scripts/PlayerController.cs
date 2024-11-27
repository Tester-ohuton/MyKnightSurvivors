using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // �ړ��ƃA�j���[�V����
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
    }
}
