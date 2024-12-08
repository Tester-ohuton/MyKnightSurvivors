using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShieldSpawnerController : BaseWeaponSpawner
{
    // Update is called once per frame
    void Update()
    {
        // オブジェクトのカウント
        weapons.RemoveAll(item => item == null);
        // 1つでも残っていたら終了
        if (0 < weapons.Count) return;

        // 全部無くなったらタイマー消化
        if (isSpawnTimerNotElapsed()) return;

        // 武器生成
        for (int i = 0; i < Stats.SpawnCount; i++)
        {
            ShieldController ctrl = (ShieldController)createWeapon(transform.position, transform);

            // 初期角度
            ctrl.Angle = 360f / Stats.SpawnCount * i;
        }

        // 次のタイマー
        spawnTimer = Stats.GetRandomSpawnTimer();
    }
}
