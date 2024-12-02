using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : BaseWeapon
{
    // Update is called once per frame
    void Update()
    {
        // ‰ñ“]
        transform.Rotate(new Vector3(0,0,-1000 * Time.deltaTime));
    }

    // ƒgƒŠƒK[‚ªÕ“Ë‚µ‚½ê‡
    private void OnTriggerEnter2D(Collider2D collision)
    {
        attackEnemy(collision);
    }
}
