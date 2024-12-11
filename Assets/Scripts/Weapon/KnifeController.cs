using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : BaseWeapon
{
    // Start is called before the first frame update
    void Start()
    {
        // Šp“x‚É•ÏŠ·‚·‚é
        float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        // Šp“x‚ğ‘ã“ü
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody2d.position += forward.normalized * stats.MoveSpeed * Time.deltaTime;
    }

    // ƒgƒŠƒK[‚ªÕ“Ë‚µ‚½
    private void OnTriggerEnter2D(Collider2D collision)
    {
        attackEnemy(collision);
    }
}
