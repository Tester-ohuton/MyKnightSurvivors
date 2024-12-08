using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinwheelSpawnerController : BaseWeaponSpawner
{
    // Update is called once per frame
    void Update()
    {
        if (isSpawnTimerNotElapsed()) return;

        // ïêäÌê∂ê¨
        for (int i = 0; i < Stats.SpawnCount; i++)
        {
            createWeapon(transform.position);
        }

        spawnTimer = Stats.GetRandomSpawnTimer();
    }
}
