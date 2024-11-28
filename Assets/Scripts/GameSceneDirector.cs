using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    // Start is called before the first frame update
    void Start()
    {
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

    }
}
