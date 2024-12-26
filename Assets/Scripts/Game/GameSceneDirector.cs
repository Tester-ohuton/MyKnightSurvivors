using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using unityroom.Api;

public class GameSceneDirector : MonoBehaviour
{
    [SerializeField] int playerScore;
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
    [SerializeField] Slider sliderXP;
    [SerializeField] Slider sliderHP;
    [SerializeField] Text textLv;

    // �w�i�ύX
    [SerializeField] Image imageBackground;

    // �o���l
    [SerializeField] List<GameObject> prefabXP;

    // ���x���A�b�v�p�l��
    [SerializeField] PanelLevelUpController panelLevelUp;
    public ParticleSystem[] particle;

    // �󔠊֘A
    [SerializeField] PanelTreasureChestController panelTreasureChest;
    [SerializeField] GameObject[] prefabTreasureChest;
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

    const string statisticsName = "HighScore";

    // SE
    [SerializeField] SoundPlayer soundPlayer;

    // Start is called before the first frame update
    void Start()
    {
        // �ϐ�������
        playerWeaponIcons = new Dictionary<BaseWeaponSpawner, GameObject>();
        playerItemIcons = new Dictionary<ItemData, GameObject>();

        // �w�i�ύX
        int sceneId = TitleSceneDirector.SceneId;
        Sprite background = CharacterSettings.Instance.GetBackground(sceneId);
        imageBackground.sprite = background;

        // �v���C���[�쐬
        int playerId = TitleSceneDirector.CharacterId;
        Player = CharacterSettings.Instance.CreatePlayer1(playerId, this, enemySpawner,
            textLv, sliderHP, sliderXP);

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
        float cameraSize = Camera.main.orthographicSize;
        // ��ʏc����i16:9�z��j
        float aspect = (float)Screen.width / (float)Screen.height;
        // �v���C���[�̈ړ��ł���͈�
        WorldStart = new Vector2(TileMapStart.x - cameraSize * aspect, TileMapStart.y - cameraSize);
        WorldEnd = new Vector2(TileMapEnd.x + cameraSize * aspect, TileMapEnd.y + cameraSize);

        // �����l
        treasureChestTimer = Random.Range(treasureChestTimerMin, treasureChestTimerMax);
        DefeatedEnemyCount = -1;

        // �A�C�R���X�V
        dispPlayerIcon();

        // �|�����G�X�V
        AddDefeatedEnemy();

        // TimeScale���Z�b�g
        setEnabled();
    }

    // Update is called once per frame
    void Update()
    {
        // �Q�[���^�C�}�[�X�V
        updateGameTimer();

        // �󔠐���
        updateTreasureChestSpawner();

        // �b���o�߂ŃQ�[���I�[�o�[
        if (GameOverTime < GameTimer)
        {
            DispPanelGameOver();
        }
    }

    // �_���[�W�\��
    public void DispDamage(GameObject target, float damage)
    {
        GameObject obj = Instantiate(prefabTextDamage, parentTextDamage);
        obj.GetComponent<TextDamageController>().Init(target, damage);
    }

    // �Q�[���^�C�}�[
    void updateGameTimer()
    {
        GameTimer += Time.deltaTime;

        // �O��ƕb���������Ȃ珈�������Ȃ�
        int seconds = (int)GameTimer % 60;
        if (seconds == OldSeconds) return;

        textTimer.text = Utils.GetTextTimer(GameTimer);
        OldSeconds = seconds;
    }

    // �o���l�擾
    public void CreateXP(EnemyController enemy)
    {
        float xp = Random.Range(enemy.Stats.XP, enemy.Stats.MaxXP);
        if (0 > xp) return;

        // 5����
        GameObject prefab = prefabXP[0];

        // 10�ȏ�
        if (10 <= xp)
        {
            prefab = prefabXP[2];
        }
        // 5�ȏ�
        else if (5 <= xp)
        {
            prefab = prefabXP[1];
        }

        // ������
        GameObject obj = Instantiate(prefab, enemy.transform.position, Quaternion.identity);
        XPController ctrl = obj.GetComponent<XPController>();
        ctrl.Init(this, xp);
    }

    // �Q�[���ĊJ/��~
    void setEnabled(bool enabled = true)
    {
        //this.enabled = enabled;
        Time.timeScale = (enabled) ? 1 : 0;
        Player.SetEnabled(enabled);
    }

    // �Q�[���ĊJ
    public void PlayGame(BonusData bonusData = null)
    {
        // �A�C�e���ǉ�
        Player.AddBonusData(bonusData);
        // �X�e�[�^�X���f
        dispPlayerIcon();

        // �Q�[���ĊJ
        setEnabled();
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

        if (listCount < randomCount)
        {
            randomCount = listCount;
        }

        // �{�[�i�X�������_���Ő���
        for (int i = 0; i < randomCount; i++)
        {
            // �����\���킩�烉���_��
            WeaponSpawnerStats randomItem = Player.GetRandomSpawnerStats();
            // �f�[�^����
            if (null == randomItem) continue;

            // ���Ԃ�`�F�b�N
            WeaponSpawnerStats findItem
                = items.Find(item => item.Id == randomItem.Id);

            // ���Ԃ�Ȃ�
            if (null == findItem)
            {
                items.Add(randomItem);
            }
            // �������
            else
            {
                i--;
            }
        }

        // �Ɨ��������Ԃ̌v�Z
        var main = particle[0].main;
        main.useUnscaledTime = true; // Unscaled���Ԃœ�����

        // ���x���A�b�v�p�l���\��
        panelLevelUp.DispPanel(items);
        // �Q�[����~
        setEnabled(false);
    }

    // �󔠃p�l����\��
    public void DispPanelTreasureChest()
    {
        // �����_���A�C�e��
        ItemData item = getRandomItemData();
        // �f�[�^����
        if (null == item) return;

        // �Ɨ��������Ԃ̌v�Z
        var main = particle[1].main;
        main.useUnscaledTime = true; // Unscaled���Ԃœ�����

        // �p�l���\��
        panelTreasureChest.DispPanel(item);
        // �Q�[�����f
        setEnabled(false);
    }

    // �A�C�e���������_���ŕԂ�
    ItemData getRandomItemData()
    {
        if (1 > treasureChestItemIds.Count) return null;

        // ���I
        int rnd = Random.Range(0, treasureChestItemIds.Count);
        return ItemSettings.Instance.Get(treasureChestItemIds[rnd]);
    }

    // �󔠐���
    void updateTreasureChestSpawner()
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
        int random = Random.Range(0, prefabTreasureChest.Length);
        GameObject obj = Instantiate(prefabTreasureChest[random], new Vector3(x, y, 0), Quaternion.identity);
        if (!obj.TryGetComponent<TreasureChestController>(out var chest)) return;
        chest.Init(this);

        // ���̃^�C�}�[�Z�b�g
        treasureChestTimer = Random.Range(treasureChestTimerMin, treasureChestTimerMax);
    }

    // �v���C���[�A�C�R���Z�b�g
    void setPlayerIcon(GameObject obj, Vector2 pos, Sprite icon, int count)
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
    void dispPlayerIcon()
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
            setPlayerIcon(obj, new Vector2(x, y), item.Stats.Icon, item.Stats.Lv);

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
            setPlayerIcon(obj, new Vector2(x, y), item.Key.Icon, item.Value);

            // ���̈ʒu
            x += w;
        }
    }

    // �|�����G���J�E���g
    public void AddDefeatedEnemy()
    {
        DefeatedEnemyCount++;
        textDefeatedEnemy.text = "" + DefeatedEnemyCount;
        playerScore += DefeatedEnemyCount;
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
        setEnabled(false);
    }

    public void RankingSend()
    {
        // C#�X�N���v�g�̖`���� `using unityroom.Api;` ��ǉ����Ă��������B

        // �{�[�hNo1�ɃX�R�A123.45f�𑗐M����B
        UnityroomApiClient.Instance.SendScore(1, (float)playerScore, ScoreboardWriteMode.HighScoreAsc);
    }
}
