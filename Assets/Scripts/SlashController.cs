using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashController : BaseWeapon
{
    // �g���K�[���Փ˂����ꍇ
    private void OnTriggerEnter2D(Collider2D collision)
    {
        attackEnemy(collision);
    }
}