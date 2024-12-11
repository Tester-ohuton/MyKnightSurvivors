using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenSpawnerController : BaseWeaponSpawner
{
    // Update is called once per frame
    void Update()
    {
        if (isSpawnTimerNotElapsed()) return;

        // 武器生成
        for (int i = 0; i < Stats.SpawnCount; i++)
        {
            // 位置
            float angle = (360f / Stats.SpawnCount) * i;

            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);

            // 進む方向
            Vector2 forward = new Vector2(x, y);

            // 進む方向を指定して生成
            createWeapon(transform.position, forward.normalized);
        }

        spawnTimer = Stats.GetRandomSpawnTimer();
    }
}
