using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class GameSceneDirector : MonoBehaviour
{
    // �^�C���}�b�v
    [SerializeField] GameObject grid;
    [SerializeField] Tilemap tilemapCollider;
    // �}�b�v�S�̍��W
    public Vector2 TileMapStart;
    public Vector2 TileMapEnd;
    public Vector2 WorldStart;
    public Vector2 WorldEnd;

    public PlayerController Player;

    [SerializeField] Transform parentTextDamage;
    [SerializeField] GameObject prefabTextDamage;

    // �^�C�}�[
    [SerializeField] Text textTimer;
    public float GameTimer;
    public float OldSeconds;

    // �G����
    [SerializeField] EnemySpawnerController enemySpawner;

    // �v���C���[����
    [SerializeField] Slider sliderHP;
    [SerializeField] Slider sliderXP;
    [SerializeField] Text textLv;

    // �o���l
    [SerializeField] List<GameObject> prefabXP;

    // ���x���A�b�v�p�l��
    [SerializeField] PanelLevelUpController panelLevelUp;

    // �󔠊֘A
    [SerializeField] PanelTreasureChestController panelTreasureChest;
    [SerializeField] GameObject prefabTreasureChest;
    [SerializeField] List<int> treasureChestItemIds;
    [SerializeField] float treasureChestTimerMin;
    [SerializeField] float treasureChestTimerMax;
    float treasureChestTimer;

    // ����ɕ\������A�C�R��
    [SerializeField] Transform canvas;
    [SerializeField] GameObject prefabImagePlayerIcon;
    Dictionary<BaseWeaponSpawner, GameObject> playerWeaponIcons;
    Dictionary<ItemData, GameObject> playerItemIcons;
    const int PlayerIconStartX = 20;
    const int PlayerIconStartY = -40;

    // �|�����G�̃J�E���g
    [SerializeField] Text textDefeatedEnemy;
    public int DefeatedEnemyCount;

    // �Q�[���I�[�o�[
    [SerializeField] PanelGameOverController panelGameOver;

    // �I������
    [SerializeField] float GameOverTime;


    // Start is called before the first frame update
    void Start()
    {
        // �ϐ�������
        playerWeaponIcons = new Dictionary<BaseWeaponSpawner, GameObject>();
        playerItemIcons = new Dictionary<ItemData, GameObject>();

        // �v���C���[����
        int playerId = TitleSceneDirector.CharacterId;
        Player = CharacterSettings.Instance.CreatePlayer(playerId,this,enemySpawner,
            textLv,sliderHP,sliderXP);

        // �����ݒ�
        OldSeconds = -1;
        enemySpawner.Init(this, tilemapCollider);
        panelLevelUp.Init(this);
        panelTreasureChest.Init(this);
        panelGameOver.Init(this);

        // �J�����̈ړ��ł���͈�
        foreach (Transform item in grid.GetComponentInChildren<Transform>())
        {
            // �J�n�ʒu
            if (TileMapStart.x > item.position.x)
            {
                TileMapStart.x = item.position.x;
            }
            if (TileMapStart.y > item.position.y)
            {
                TileMapStart.y = item.position.y;
            }
            // �I���ʒu
            if (TileMapEnd.x < item.position.x)
            {
                TileMapEnd.x = item.position.x;
            }
            if (TileMapEnd.y < item.position.y)
            {
                TileMapEnd.y = item.position.y;
            }
        }

        // ��ʏc�����̕`��͈́i�f�t�H���g��5�^�C���j
        float cameraSize = Camera.main.orthographicSize - 1;
        // ��ʏc����i16:9�z��j
        float aspect = (float)Screen.width / (float)Screen.height;
        // �v���C���[�̈ړ��ł���͈�
        WorldStart = new Vector2(TileMapStart.x - cameraSize * aspect, TileMapStart.y - cameraSize);
        WorldEnd = new Vector2(TileMapEnd.x + cameraSize * aspect, TileMapEnd.y + cameraSize);

        // �����l
        treasureChestTimer = Random.Range(treasureChestTimerMin,treasureChestTimerMax);
        DefeatedEnemyCount = -1;

        // �A�C�R���X�V
        DispPlayerIcon();

        // �|�����G�̐�
        AddDefeatedEnemy();

        // TimeScale���Z�b�g
        SetEnabled();

        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Battle);
    }

    // Update is called once per frame
    void Update()
    {
        // �^�C�}�[�X�V
        UpdateGameTimer();

        // �󔠐���
        UpdateTreasureChestSpawner();
    
        // �b���o�߂ŃQ�[���I�[�o�[
        if(GameOverTime < GameTimer)
        {
            DispPanelGameOver();
        }
    }

    // �_���[�W�\��
    public void DispDamage(GameObject target,float damage)
    {
        GameObject obj = Instantiate(prefabTextDamage,parentTextDamage);
        obj.GetComponent<TextDamageController>().Init(target,damage);
    }

    void UpdateGameTimer()
    {
        GameTimer += Time.deltaTime;

        // �O��ƕb���������Ȃ珈�������Ȃ�
        int seconds = (int)GameTimer % 60;
        if (seconds == OldSeconds) return;

        textTimer.text=Utils.GetTextTimer(GameTimer);
        OldSeconds = seconds;
    }

    // �o���l�擾
    public void CreateXP(EnemyController enemy)
    {
        float xp = Random.Range(enemy.Stats.XP,enemy.Stats.MaxXP);
        if (xp < 0) return;

        // 5����
        GameObject prefab = prefabXP[0];

        // 10�ȏ�
        if(10 <= xp)
        {
            prefab = prefabXP[2];
        }
        else if(5 <= xp)
        {
            prefab = prefabXP[1];
        }

        // ������
        GameObject obj = Instantiate(prefab,enemy.transform.position,Quaternion.identity);
        XPController ctrl = obj.GetComponent<XPController>();
        ctrl.Init(this,xp);
    }

    // �Q�[���ĊJ/��~
    void SetEnabled(bool enabled = true)
    {
        this.enabled = enabled;
        Time.timeScale = (enabled) ? 1 : 0;
        Player.SetEnabled(enabled);
    }

    // �Q�[���ĊJ
    public void PlayGame(BonusData bonusData = null)
    {
        // �A�C�e���ǉ�
        Player.AddBonusData(bonusData);
        // �X�e�[�^�X�擾
        DispPlayerIcon();

        // �Q�[���ĊJ
        SetEnabled();
    }

    // ���x���A�b�v��
    public void DispPanelLevelUp()
    {
        // �ǉ������A�C�e��
        List<WeaponSpawnerStats> items = new List<WeaponSpawnerStats>();

        // ������
        int randomCount = panelLevelUp.GetButtonCount();
        // ����̐�������Ȃ��ꍇ�͌��炷
        int listCount = Player.GetUsableWeaponIds().Count;

        if(listCount < randomCount)
        {
            randomCount = listCount;
        }

        // �{�[�i�X�������_���Ő���
        for (int i = 0; i < randomCount; i++)
        {
            // �����\���킩�烉���_��
            WeaponSpawnerStats randomItem = Player.GetRandomSpawnerStats();
            // �f�[�^�Ȃ�
            if (randomItem != null) continue;

            // ���Ԃ�`�F�b�N
            WeaponSpawnerStats findItem = items.Find(item => item.Id == randomItem.Id);

            if (findItem != null)
            {
                items.Add(randomItem);
            }
            // �������
            else
            {
                i--;
            }
        }

        // ���x���A�b�v�p�l���\��
        panelLevelUp.DispPanel(items);
        // �Q�[����~
        SetEnabled(false);
    }

    // �󔠃p�l����\��
    public void DispPanelTreasureChest()
    {
        // �����_���A�C�e��
        ItemData item = GetRandomItemData();

        // �f�[�^�Ȃ�
        if (item == null) return;

        // �p�l���\��
        panelTreasureChest.DispPanel(item);
        // �Q�[�����f
        SetEnabled(false);
    }

    // �A�C�e���������_���ŕԂ�
    ItemData GetRandomItemData()
    {
        if (treasureChestItemIds.Count < 1) return null;

        // ���I
        int rnd = Random.Range(0,treasureChestItemIds.Count);
        return ItemSettings.Instance.Get(treasureChestItemIds[rnd]);
    }

    // �󔠐���
    void UpdateTreasureChestSpawner()
    {
        // �^�C�}�[
        treasureChestTimer -= Time.deltaTime;
        // �^�C�}�[������
        if (0 < treasureChestTimer) return;

        // �����ꏊ
        float x = Random.Range(WorldStart.x, WorldEnd.x);
        float y = Random.Range(WorldStart.y, WorldEnd.y);

        // �����蔻��̂���^�C���ォ�ǂ���
        if (Utils.IsColliderTile(tilemapCollider, new Vector2(x, y))) return;

        // ����
        GameObject obj = Instantiate(prefabTreasureChest, new Vector3(x, y, 0), Quaternion.identity);
        obj.GetComponent<TreasureChestController>().Init(this);

        // ���̃^�C�}�[�Z�b�g
        treasureChestTimer = Random.Range(treasureChestTimerMin, treasureChestTimerMax);
    }

    // �v���C���[�A�C�R���Z�b�g
    void SetPlayerIcon(GameObject obj, Vector2 pos, Sprite icon, int count)
    {
        // �摜
        Transform image = obj.transform.Find("ImageIcon");
        image.GetComponent<Image>().sprite = icon;

        // �e�L�X�g
        Transform text = obj.transform.Find("TextCount");
        text.GetComponent<TextMeshProUGUI>().text = "" + count;

        // �ꏊ
        obj.GetComponent<RectTransform>().anchoredPosition = pos;
    }

    // �A�C�R���̕\�����X�V
    void DispPlayerIcon()
    {
        // ����A�C�R���\���ʒu
        float x = PlayerIconStartX;
        float y = PlayerIconStartY;
        float w = prefabImagePlayerIcon.GetComponent<RectTransform>().sizeDelta.x + 1;

        foreach (var item in Player.WeaponSpawners)
        {
            // �쐬�ς݂̃f�[�^������Ύ擾����
            playerWeaponIcons.TryGetValue(item, out GameObject obj);

            // �Ȃ���΍쐬����
            if (!obj)
            {
                obj = Instantiate(prefabImagePlayerIcon, canvas);
                playerWeaponIcons.Add(item, obj);
            }

            // �A�C�R���Z�b�g
            SetPlayerIcon(obj, new Vector2(x, y), item.Stats.Icon, item.Stats.Lv);

            // ���̈ʒu
            x += w;
        }

        // �A�C�e���A�C�R���\���ʒu
        x = PlayerIconStartX;
        y = PlayerIconStartY - w;

        foreach (var item in Player.ItemDatas)
        {
            // �쐬�ς݂̃f�[�^������Ύ擾����
            playerItemIcons.TryGetValue(item.Key, out GameObject obj);

            // �Ȃ���΍쐬����
            if (!obj)
            {
                obj = Instantiate(prefabImagePlayerIcon, canvas);
                playerItemIcons.Add(item.Key, obj);
            }

            // �A�C�R���Z�b�g
            SetPlayerIcon(obj, new Vector2(x, y), item.Key.Icon, item.Value);

            // ���̈ʒu
            x += w;
        }
    }

    // �|�����G���J�E���g
    public void AddDefeatedEnemy()
    {
        DefeatedEnemyCount++;
        textDefeatedEnemy.text = "" + DefeatedEnemyCount;
    }

    // �^�C�g����
    public void LoadSceneTitle()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("TitleScene");
    }

    // �Q�[���I�[�o�[�p�l����\��
    public void DispPanelGameOver()
    {
        // �p�l���\��
        panelGameOver.DispPanel(Player.WeaponSpawners);
        // �Q�[�����f
        SetEnabled(false);
    }
}
