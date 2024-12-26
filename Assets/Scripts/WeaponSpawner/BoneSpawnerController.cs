using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BoneSpawnerController : BaseWeaponSpawner
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
