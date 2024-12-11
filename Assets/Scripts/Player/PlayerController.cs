using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    // �ǉ������A�C�e���ƌ�
    public Dictionary<ItemData, int> ItemDatas;

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
        CharacterStats characterStats, Text textLv, Slider sliderHP, Slider sliderXP)
    {
        // �ϐ�������
        levelRequirements = new List<int>();
        WeaponSpawners = new List<BaseWeaponSpawner>();
        ItemDatas = new Dictionary<ItemData, int>();

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

        // �o���l��臒l���X�g�쐬
        levelRequirements.Add(0);
        for (int i = 1; i < 1000; i++)
        {
            // 1�O��臒l
            int prevxp = levelRequirements[i - 1];
            // 41�ȍ~�̓��x������16XP������
            int addxp = 16;

            // ���x��2�܂Ń��x���A�b�v����̂�5XP
            if (i == 1)
            {
                addxp = 5;
            }
            else if (20 >= i)
            {
                addxp = 10;
            }
            else if (40 >= i)
            {
                addxp = 13;
            }

            // �K�v�o���l
            levelRequirements.Add(prevxp + addxp);
        }

        // LV2�̕K�v�o���l
        Stats.MaxXP = levelRequirements[1];

        // UI������
        setTextLv();
        setSliderHP();
        setSliderXP();

        moveSliderHP();

        // ����f�[�^�Z�b�g
        foreach (var item in Stats.DefaultWeaponIds)
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

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            dir += Vector2.up;
            trigger = "isUp";
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            dir -= Vector2.up;
            trigger = "isDown";
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            dir += Vector2.right;
            trigger = "isRight";
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
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

        Forward = dir;
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
        pos.y -= 50;
        sliderHP.transform.position = pos;
    }

    // �_���[�W
    public void Damage(float attack)
    {
        // ��A�N�e�B�u�Ȃ甲����
        if (!enabled) return;

        float damage = Mathf.Max(0, attack - Stats.Defense);
        Stats.HP -= damage;

        // �_���[�W�\��
        sceneDirector.DispDamage(gameObject, damage);

        // �Q�[���I�[�o�[
        if (0 > Stats.HP)
        {
            // ����ł��Ȃ��悤�ɂ���
            SetEnabled(false);

            // �A�j���[�V����
            transform.DOScale(new Vector2(5, 0), 2).SetUpdate(true)
                .OnComplete(() =>
                {
                    sceneDirector.DispPanelGameOver();
                });
        }

        if (0 > Stats.HP) Stats.HP = 0;
        setSliderHP();
    }

    // HP�X���C�_�[�̒l���X�V
    void setSliderHP()
    {
        sliderHP.maxValue = Stats.MaxHP;
        sliderHP.value = Stats.HP;
    }

    // XP�X���C�_�[�̒l���X�V
    void setSliderXP()
    {
        sliderXP.maxValue = Stats.MaxXP;
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
        textLv.text = "LV " + Stats.Lv;
    }

    // �����ǉ�
    void addWeaponSpawner(int id)
    {
        // �����ς݂Ȃ烌�x���A�b�v
        BaseWeaponSpawner spawner = WeaponSpawners.Find(item => item.Stats.Id == id);

        if (spawner)
        {
            spawner.LevelUp();
            return;
        }

        // �V�K�ǉ�
        spawner = WeaponSpawnerSettings.Instance.CreateWeaponSpawner(id, enemySpawner, transform);

        if (null == spawner)
        {
            Debug.LogError("����f�[�^������܂���");
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
            if (Stats.Lv < levelRequirements.Count)
            {
                Stats.XP = 0;
                Stats.MaxXP = levelRequirements[Stats.Lv];
            }

            // ���x���A�b�v�p�l���\��
            sceneDirector.DispPanelLevelUp();

            setTextLv();
        }

        // �\���X�V
        setSliderXP();
    }

    // �����\�ȕ��탊�X�g
    public List<int> GetUsableWeaponIds()
    {
        List<int> ret = new List<int>(Stats.UsableWeaponIds);

        // �����\���𒴂���ꍇ�͑������Ă��镐���ID��Ԃ�
        if (Stats.UsableWeaponMax - 1 < WeaponSpawners.Count)
        {
            ret.Clear();
            foreach (var item in WeaponSpawners)
            {
                ret.Add(item.Stats.Id);
            }
        }

        return ret;
    }

    // �����\�ȕ���������_���ŕԂ�
    public WeaponSpawnerStats GetRandomSpawnerStats()
    {
        // �����\�ȕ���ID
        List<int> usableIds = GetUsableWeaponIds();

        // �����\�ȕ��킪�Ȃ�(�ꉞ)
        if (1 > usableIds.Count)
        {
            return null;
        }

        // ���I
        int rnd = Random.Range(0, usableIds.Count);
        int id = usableIds[rnd];

        // �����ς݂Ȃ玟�̃��x���̃f�[�^
        BaseWeaponSpawner spawner = WeaponSpawners.Find(item => item.Stats.Id == id);
        if (spawner)
        {
            return spawner.GetLevelUpStats(true);
        }

        // �V�K�Ȃ烌�x��1�̃f�[�^
        return WeaponSpawnerSettings.Instance.Get(id, 1);
    }

    // �A�C�e����ǉ�
    void addItemData(int id)
    {
        ItemData itemData = ItemSettings.Instance.Get(id);

        if (null == itemData)
        {
            Debug.LogError("�A�C�e���f�[�^��������܂���ł���");
            return;
        }

        // �f�[�^�ǉ�
        Stats.AddItemData(itemData);

        // �擾�ς݃��X�g�֒ǉ�
        ItemData key = null;
        foreach (var item in ItemDatas)
        {
            if (item.Key.Id == itemData.Id)
            {
                key = item.Key;
                break;
            }
        }

        if (null == key)
        {
            ItemDatas.Add(itemData, 0);
            key = itemData;
        }

        ItemDatas[key]++;
    }

    // ���x���A�b�v��A�C�e���擾��
    public void AddBonusData(BonusData bonusData)
    {
        if (null == bonusData) return;

        // ����f�[�^
        if (null != bonusData.WeaponSpawnerStats)
        {
            addWeaponSpawner(bonusData.WeaponSpawnerStats.Id);
        }

        // �A�C�e���f�[�^
        if (null != bonusData.ItemData)
        {
            addItemData(bonusData.ItemData.Id);
        }

        // �\���X�V
        setSliderHP();
    }

    // �A�b�v�f�[�g��~
    public void SetEnabled(bool enabled = true)
    {
        this.enabled = enabled;

        // ����
        foreach (var item in WeaponSpawners)
        {
            item.SetEnabled(enabled);
        }
    }
}
