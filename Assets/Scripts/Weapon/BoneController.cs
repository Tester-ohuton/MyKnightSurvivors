using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneController : BaseWeapon
{
    // Start is called before the first frame update
    void Start()
    {
        // ƒ‰ƒ“ƒ_ƒ€‚È•ûŒü‚ÉŒü‚©‚Á‚Ä”ò‚Î‚·
        forward = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        forward.Normalize();

        // ‚¢‚Á‚½‚ñ‹t‚É”ò‚Î‚·
        Vector2 force = new Vector2(-forward.x * stats.MoveSpeed, -forward.y * stats.MoveSpeed);
        rigidbody2d.AddForce(force);
    }

    // Update is called once per frame
    void Update()
    {
        // ‰ñ“]
        transform.Rotate(new Vector3(0, 0, 5000 * Time.deltaTime));

        // ˆÚ“®
        rigidbody2d.AddForce(forward * stats.MoveSpeed * Time.deltaTime);
    }

    // ƒgƒŠƒK[‚ªÕ“Ë‚µ‚½
    private void OnTriggerEnter2D(Collider2D collision)
    {
        attackEnemy(collision);
    }
}
