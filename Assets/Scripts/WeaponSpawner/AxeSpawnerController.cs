using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeSpawnerController : BaseWeaponSpawner
{
    // 一度の生成に時差をつける
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

        // 偶数で左右に出す
        int dir = (onceSpawnCount % 2 == 0) ? 1 : -1;

        // 場所
        Vector3 pos = transform.position;
        pos.x += 2f * dir;

        // 生成
        AxeController ctrl = (AxeController)createWeapon(pos, transform);

        // 斜め上に力を加える
        ctrl.GetComponent<Rigidbody2D>().AddForce(new Vector2(100 * dir,350));

        // 次の生成タイマー
        spawnTimer = onceSpawmTime;
        onceSpawnCount--;

        // 1回の生成が終わったらリセット
        if (onceSpawnCount < 1)
        {
            spawnTimer = Stats.GetRandomSpawnTimer();
            onceSpawnCount = (int)Stats.SpawnCount;
        }
    }
}
