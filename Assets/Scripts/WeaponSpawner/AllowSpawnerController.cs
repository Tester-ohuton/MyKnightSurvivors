using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllowSpawnerController : BaseWeaponSpawner
{
    // Update is called once per frame
    void Update()
    {
        if (isSpawnTimerNotElapsed()) return;

        // 次のタイマー
        spawnTimer = Stats.GetRandomSpawnTimer();

        // 敵がいない
        if (enemySpawner.GetEnemies().Count < 1) return;

        for (int i = 0; i < (int)Stats.SpawnCount; i++)
        {
            // 武器生成
            AllowController ctrl =
                (AllowController)createWeapon(transform.position);

            // ランダムでターゲットを設定
            List<EnemyController> enemies = enemySpawner.GetEnemies();
            int rnd = Random.Range(0, enemies.Count);
            EnemyController target = enemies[rnd];

            ctrl.Target = target;
        }
    }
}
