using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenSpawnerController : BaseWeaponSpawner
{
    // Update is called once per frame
    void Update()
    {
        if (isSpawnTimerNotElapsed()) return;

        // ���퐶��
        for (int i = 0; i < Stats.SpawnCount; i++)
        {
            // �ʒu
            float angle = (360f / Stats.SpawnCount) * i;

            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);

            // �i�ޕ���
            Vector2 forward = new Vector2(x, y);

            // �i�ޕ������w�肵�Đ���
            createWeapon(transform.position, forward.normalized);
        }

        spawnTimer = Stats.GetRandomSpawnTimer();
    }
}