using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawnerController : BaseWeaponSpawner
{
    // Update is called once per frame
    void Update()
    {
        if (isSpawnTimerNotElapsed()) return;

        // ¶¬‚³‚ê‚éêŠ
        Vector2 position = Camera.main.transform.position;
        // ƒJƒƒ‰‚Ìã‚©‚ç
        position.y += Camera.main.orthographicSize;

        for (int i = 0; i < Stats.SpawnCount; i++)
        {
            position.x += Random.Range(-7, 7);
            createWeapon(position);
        }

        spawnTimer = Stats.GetRandomSpawnTimer();
    }
}
