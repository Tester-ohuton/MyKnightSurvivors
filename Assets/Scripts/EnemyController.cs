using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public CharacterStats Stats;

    [SerializeField] GameSceneDirector sceneDirector;
    Rigidbody2D rigidbody2d;

    // �U���̃N�[���_�E��
    float attackCoolDownTimer;
    float attackCoolDownTimerMax = 0.5f;
    // ����
    Vector2 forward;

    // ���
    enum State
    {
        Alive,
        Dead
    }
    State state;

    // Start is called before the first frame update
    void Start()
    {
        Init(this.sceneDirector, CharacterSettings.Instance.Get(100));
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer();

        moveEnemy();
    }

    // ������
    public void Init(GameSceneDirector sceneDirector, CharacterStats characterStats)
    {
        this.sceneDirector = sceneDirector;
        this.Stats = characterStats;

        rigidbody2d = GetComponent<Rigidbody2D>();

        // �A�j���[�V����
        // �����_���Ŋɋ}������
        float random = Random.Range(0.8f, 1.2f);
        float speed = 1 / Stats.MoveSpeed * random;

        // �T�C�Y
        float addx = 0.8f;
        float x = addx * random;
        transform.DOScale(x, speed)
            .SetRelative()
            .SetLoops(-1, LoopType.Yoyo);

        // ��]
        float addz = 10f;
        float z = Random.Range(-addz, addz) * random;
        // �����l
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = z;
        // �ڕW�l
        transform.eulerAngles = rotation;
        transform.DORotate(new Vector3(0, 0, -z), speed)
            .SetLoops(-1, LoopType.Yoyo);

        // �i�ޕ���
        PlayerController player = sceneDirector.Player;
        Vector2 dir = player.transform.position - transform.position;
        forward = dir;

        state = State.Alive;
    }

    // �v���C���[��ǂ�������
    void moveEnemy()
    {
        if (State.Alive != state) return;

        // �ړI���v���C���[�Ȃ�i�ޕ������X�V����
        if (MoveType.TargetPlayer == Stats.MoveType)
        {
            PlayerController player = sceneDirector.Player;
            Vector2 dir = player.transform.position - transform.position;
            forward = dir;
        }

        // �ړ�
        rigidbody2d.position += forward.normalized * Stats.MoveSpeed * Time.deltaTime;
    }

    // �e��^�C�}�[�X�V
    void updateTimer()
    {
        if (0 < attackCoolDownTimer)
        {
            attackCoolDownTimer -= Time.deltaTime;
        }

        // �������Ԃ��ݒ肳��Ă�����^�C�}�[����
        if (0 < Stats.AliveTime)
        {
            Stats.AliveTime -= Time.deltaTime;
            if (0 > Stats.AliveTime) setDead(false);
        }
    }

    // �G�����񂾎��ɌĂяo�����
    void setDead(bool createXP = true)
    {
        if (State.Alive != state) return;

        // �����������~
        rigidbody2d.simulated = false;

        // �A�j���[�V�������~
        transform.DOKill();

        // �c�ɒׂ��A�j���[�V����
        transform.DOScaleY(0, 0.5f).OnComplete(() => Destroy(gameObject));

        // �o���l���쐬
        if (createXP)
        {
            // TODO �o���l����
        }

        state = State.Dead;
    }

    // �Փ˂�����
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    // �Փ˂��Ă����
    private void OnCollisionStay2D(Collision2D collision)
    {

    }

    // �Փ˂��I�������
    private void OnCollisionExit2D(Collision2D collision)
    {

    }
}