using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

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

    // Start is called before the first frame update
    void Start()
    {
        // �v���C���[����
        int playerId = 0;
        Player = CharacterSettings.Instance.CreatePlayer(playerId,this,enemySpawner,
            textLv,sliderHP,sliderXP);

        // �����ݒ�
        OldSeconds = -1;
        enemySpawner.Init(this, tilemapCollider);

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
    }

    // Update is called once per frame
    void Update()
    {
        // �^�C�}�[�X�V
        updateGameTimer();
    }

    // �_���[�W�\��
    public void DispDamage(GameObject target,float damage)
    {
        GameObject obj = Instantiate(prefabTextDamage,parentTextDamage);
        obj.GetComponent<TextDamageController>().Init(target,damage);
    }

    void updateGameTimer()
    {
        GameTimer += Time.deltaTime;

        // �O��ƕb���������Ȃ珈�������Ȃ�
        int seconds = (int)GameTimer % 60;
        if (seconds == OldSeconds) return;

        textTimer.text=Utils.GetTextTimer(GameTimer);
        OldSeconds = seconds;
    }
}
