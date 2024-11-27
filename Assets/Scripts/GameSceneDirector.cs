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
    public Vector2 tileMapStart;// �J�����𐧌�
    public Vector2 tileMapEnd;  // �J�����𐧌�
    public Vector2 worldStart;  // �v���C���[�𐧌�
    public Vector2 worldEnd;    // �v���C���[�𐧌�

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform item in grid.GetComponentInChildren<Transform>())
        {
            // �J�n�ʒu
            if (item.position.x < tileMapStart.x)
            {
                tileMapStart.x = item.position.x;
            }
            if (item.position.y < tileMapStart.y)
            {
                tileMapStart.y = item.position.y;
            }

            // �I���ʒu
            if (tileMapEnd.x < item.position.x)
            {
                tileMapEnd.x = item.position.x;
            }
            if (tileMapEnd.y < item.position.y)
            {
                tileMapEnd.y = item.position.y;
            }
        }

        // ��ʏc�����̕`��͈́i�f�t�H���g��5�^�C���j
        float cameraSize = Camera.main.orthographicSize - 1;

        // ��ʏc����i16�F9�z��j
        float aspect = (float)Screen.width / (float)Screen.height;

        // �v���C���[�̈ړ��ł���͈�
        worldStart = new Vector2(tileMapStart.x - cameraSize * aspect,tileMapStart.y - cameraSize);
        worldEnd = new Vector2(tileMapEnd.x + cameraSize * aspect, tileMapEnd.y + cameraSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
