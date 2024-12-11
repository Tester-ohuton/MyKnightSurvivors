using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashSpawnerController : BaseWeaponSpawner
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
        SlashController ctrl = (SlashController)createWeapon(pos,transform);

        SoundManager.Instance.PlaySE(SESoundData.SE.Attack);

        // ���E�Ŋp�x��ς���
        ctrl.transform.eulerAngles = ctrl.transform.eulerAngles * dir;

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
