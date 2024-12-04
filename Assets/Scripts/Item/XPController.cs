using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPController : MonoBehaviour
{
    GameSceneDirector sceneDirector;
    Rigidbody2D rigidbody2D;
    SpriteRenderer spriteRenderer;

    // �o���l
    float xp;
    // 60�b�ŏ�����
    float aliveTimer = 50f;
    float fadeTime = 10f;

    // ������
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
        // �Q�[����~��
        if (!sceneDirector.enabled) return;

        // �^�C�}�[�����ŏ����n�߂�
        aliveTimer -= Time.deltaTime;

        if(aliveTimer < 0)
        {
            // �A���t�@�l��ݒ�
            Color color = spriteRenderer.color;
            color.a -= 1.0f / fadeTime * Time.deltaTime;

            // �����Ȃ��Ȃ��������
            if (color.a <= 0)
            {
                Destroy(gameObject);
                return;
            }
        }

        // �v���C���[�Ƃ̋���
        float dest = Vector2.Distance(transform.position,sceneDirector.Player.transform.position);
        // �擾�͈͂Ȃ�z�����܂��
        if(dest < sceneDirector.Player.Stats.PickUpRange)
        {
            float speed = sceneDirector.Player.Stats.MoveSpeed * 1.1f;
            Vector2 forward = sceneDirector.Player.transform.position - transform.position;
            rigidbody2D.position += forward.normalized * speed * Time.deltaTime;
        }
    }

    // �g���K�[���Փ˂����Ƃ�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �v���C���[����Ȃ�
        if (!collision.gameObject.TryGetComponent<PlayerController>(out var player)) return;

        // �o���l�擾
        player.GetXP(xp);
        Destroy(gameObject);
    }
}
