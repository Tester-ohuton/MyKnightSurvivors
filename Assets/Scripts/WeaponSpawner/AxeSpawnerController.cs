using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeSpawnerController : BaseWeaponSpawner
{
    // ��x�̐����Ɏ���������
    int onceSpawnCount;
    float onceSpawmTime = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        onceSpawnCount = (int)Stats.SpawnCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpawnTimerNotElapsed()) return;

        // �����ō��E�ɏo��
        int dir = (onceSpawnCount % 2 == 0) ? 1 : -1;

        // �ꏊ
        Vector3 pos = transform.position;
        pos.x += 2f * dir;

        // ����
        AxeController ctrl = (AxeController)createWeapon(pos, transform);

        // �΂ߏ�ɗ͂�������
        ctrl.GetComponent<Rigidbody2D>().AddForce(new Vector2(100 * dir,350));

        // ���̐����^�C�}�[
        spawnTimer = onceSpawmTime;
        onceSpawnCount--;

        // 1��̐������I������烊�Z�b�g
        if (onceSpawnCount < 1)
        {
            spawnTimer = Stats.GetRandomSpawnTimer();
            onceSpawnCount = (int)Stats.SpawnCount;
        }
    }
}
