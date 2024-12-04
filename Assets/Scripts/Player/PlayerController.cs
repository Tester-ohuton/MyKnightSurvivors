using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // �ړ��ƃA�j���[�V����
    Rigidbody2D rigidbody2d;
    Animator animator;

    // Init�ŃZ�b�g�����
    GameSceneDirector sceneDirector;
    Slider sliderHP;
    Slider sliderXP;

    public CharacterStats Stats;

    // �U���̃N�[���_�E��
    float attackCoolDownTimer;
    float attackCoolDownTimerMax = 0.5f;

    // �K�vXP
    List<int> levelRequirements;
    // �G�������u
    EnemySpawnerController enemySpawner;
    // ����
    public Vector2 Forward;
    // ���x���e�L�X�g
    Text textLv;
    // ���ݑ������̕���
    public List<BaseWeaponSpawner> WeaponSpawners;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer();

        movePlayer();

        moveCamera();

        moveSliderHP();
    }

    // ������
    public void Init(GameSceneDirector sceneDirector, EnemySpawnerController enemySpawner,
        CharacterStats characterStats,Text textLv,Slider sliderHP,Slider sliderXP)
    {
        // �ϐ�������
        levelRequirements = new List<int>();
        WeaponSpawners = new List<BaseWeaponSpawner>();
        this.sceneDirector = sceneDirector;
        this.enemySpawner = enemySpawner;
        this.Stats = characterStats;
        this.textLv = textLv;
        this.sliderHP = sliderHP;
        this.sliderXP = sliderXP;

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // �v���C���[�̌���
        Forward = Vector2.right;

        // �o���l�̓��l���X�g�쐬
        levelRequirements.Add(0);
        for (int i = 1; i < 1000; i++)
        {
            // 1�O�̓��l
            int prexp = levelRequirements[i - 1];
            // 41�ȍ~�̓��x������16XP������
            int addxp = 16;

            // ���x��2�܂Ń��x���A�b�v����̂�5XP
            if(i == 1)
            {
                addxp = 5;
            }
            else if(i <= 20)
            {
                addxp = 10;
            }
            else if(i <= 40)
            {
                addxp = 13;
            }

            // �K�v�o���l
            levelRequirements.Add(prexp + addxp);
        }

        // LV2�̕K�v�o���l
        Stats.MaxXP = levelRequirements[1];

        // UI������
        setTextLv();
        setSliderHP();
        setSliderXP();

        moveSliderHP();

        // ����f�[�^�Z�b�g
        foreach(var item in Stats.DefaultWeaponIds)
        {
            addWeaponSpawner(item);
        }
    }

    // �v���C���[�̈ړ��Ɋւ��鏈��
    void movePlayer()
    {
        // �ړ��������
        Vector2 dir = Vector2.zero;
        // �Đ�����A�j���[�V����
        string trigger = "";

        if (Input.GetKey(KeyCode.UpArrow))
        {
            dir += Vector2.up;
            trigger = "isUp";
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            dir -= Vector2.up;
            trigger = "isDown";
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            dir += Vector2.right;
            trigger = "isRight";
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir -= Vector2.right;
            trigger = "isLeft";
        }

        // ���͂��Ȃ���Δ�����
        if (Vector2.zero == dir) return;

        // �v���C���[�ړ�
        rigidbody2d.position += dir.normalized * Stats.MoveSpeed * Time.deltaTime;

        // �A�j���[�V�������Đ�����
        animator.SetTrigger(trigger);

        // �ړ��͈͐���
        // �n�_
        if (rigidbody2d.position.x < sceneDirector.WorldStart.x)
        {
            Vector2 pos = rigidbody2d.position;
            pos.x = sceneDirector.WorldStart.x;
            rigidbody2d.position = pos;
        }
        if (rigidbody2d.position.y < sceneDirector.WorldStart.y)
        {
            Vector2 pos = rigidbody2d.position;
            pos.y = sceneDirector.WorldStart.y;
            rigidbody2d.position = pos;
        }
        // �I�_
        if (sceneDirector.WorldEnd.x < rigidbody2d.position.x)
        {
            Vector2 pos = rigidbody2d.position;
            pos.x = sceneDirector.WorldEnd.x;
            rigidbody2d.position = pos;
        }
        if (sceneDirector.WorldEnd.y < rigidbody2d.position.y)
        {
            Vector2 pos = rigidbody2d.position;
            pos.y = sceneDirector.WorldEnd.y;
            rigidbody2d.position = pos;
        }
    }

    // �J�����ړ�
    void moveCamera()
    {
        Vector3 pos = transform.position;
        pos.z = Camera.main.transform.position.z;

        //�n�_
        if (pos.x < sceneDirector.TileMapStart.x)
        {
            pos.x = sceneDirector.TileMapStart.x;
        }
        if (pos.y < sceneDirector.TileMapStart.y)
        {
            pos.y = sceneDirector.TileMapStart.y;
        }
        // �I�_
        if (sceneDirector.TileMapEnd.x < pos.x)
        {
            pos.x = sceneDirector.TileMapEnd.x;
        }
        if (sceneDirector.TileMapEnd.y < pos.y)
        {
            pos.y = sceneDirector.TileMapEnd.y;
        }

        // �J�����̈ʒu���X�V����
        Camera.main.transform.position = pos;
    }

    // HP�X���C�_�[�ړ�
    void moveSliderHP()
    {
        // ���[���h���W���X�N���[�����W�ɕϊ�
        Vector3 pos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        pos.y -= 75;
        sliderHP.transform.position = pos;
    }

    // �_���[�W
    public void Damage(float attack)
    {
        // ��A�N�e�B�u�Ȃ�ʂ���
        if(!enabled) return;

        float damage = Mathf.Max(0, attack - Stats.Defense);
        Stats.HP -= damage;

        // �_���[�W�\��
        sceneDirector.DispDamage(gameObject, damage);

        // TODO �Q�[���I�[�o�[
        if(Stats.HP < 0)
        {

        }

        if (Stats.HP < 0) Stats.HP = 0;
        setSliderHP();
    }

    // HP�X���C�_�[�̒l���X�V
    private void setSliderHP()
    {
        sliderHP.maxValue = Stats.HP;
        sliderHP.value = Stats.HP;
    }

    // XP�X���C�_�[�̒l���X�V
    private void setSliderXP()
    {
        sliderXP.maxValue = Stats.XP;
        sliderXP.value = Stats.XP;
    }

    // �Փ˂�����
    private void OnCollisionEnter2D(Collision2D collision)
    {
        attackEnemy(collision);
    }

    // �Փ˂��Ă����
    private void OnCollisionStay2D(Collision2D collision)
    {
        attackEnemy(collision);
    }

    // �Փ˂��I�������
    private void OnCollisionExit2D(Collision2D collision)
    {

    }

    // �v���C���[�֍U������
    void attackEnemy(Collision2D collision)
    {
        // �G�l�~�[�ȊO
        if (!collision.gameObject.TryGetComponent<EnemyController>(out var enemy)) return;
        // �^�C�}�[������
        if (0 < attackCoolDownTimer) return;
        
        enemy.Damage(Stats.Attack);
        attackCoolDownTimer = attackCoolDownTimerMax;
    }

    // �e��^�C�}�[�X�V
    void updateTimer()
    {
        if (0 < attackCoolDownTimer)
        {
            attackCoolDownTimer -= Time.deltaTime;
        }
    }

    // ���x���e�L�X�g�X�V
    void setTextLv()
    {
        textLv.text = "Lv " + Stats.Lv;
    }

    // �����ǉ�
    void addWeaponSpawner(int id)
    {
        // TODO �����ς݂Ȃ烌�x���A�b�v
        BaseWeaponSpawner spawner = WeaponSpawners.Find(item => item.Stats.Id == id);

        if (spawner)
        {
            return;
        }

        // �V�K�ǉ�
        spawner = WeaponSpawnerSettings.Instance.CreateWeaponSpawner(id,enemySpawner,transform);

        if(spawner == null)
        {
            Debug.Log("����f�[�^������܂���");
            return;
        }

        // �����ς݃��X�g�֒ǉ�
        WeaponSpawners.Add(spawner);
    }

    // �o���l�擾
    public void GetXP(float xp)
    {
        Stats.XP += xp;

        // ���x�����
        if (levelRequirements.Count - 1 < Stats.Lv) return;

        // ���x���A�b�v
        if (levelRequirements[Stats.Lv] <= Stats.XP)
        {
            Stats.Lv++;

            // ���̌o���l
            if(Stats.Lv < levelRequirements.Count)
            {
                Stats.XP = 0;
                Stats.MaxXP = levelRequirements[Stats.Lv];
            }

            // ���x���A�b�v�p�l���\��
            setTextLv();
        }

        // �\���X�V
        setSliderXP();
    }
}
