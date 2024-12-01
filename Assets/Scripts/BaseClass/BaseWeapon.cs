using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    // �e�̐������u
    protected BaseWeaponSpawner spawner;
    // ����X�e�[�^�X
    protected WeaponSpawnerStats stats;
    // ��������
    protected Rigidbody2D rigidbody2D;
    // ����
    protected Vector2 forward;

    // ������
    public void Init(BaseWeaponSpawner spawner,Vector2 forward)
    {
        // �e�̐����ʒu
        this.spawner = spawner;
        // ����f�[�^�Z�b�g
        this.stats = (WeaponSpawnerStats)spawner.Stats.GetCopy();
        // �i�ޕ���
        this.forward = forward;
        // ��������
        this.rigidbody2D = rigidbody2D;

        // �������Ԃ�����ΐݒ肷��
        if(-1 < stats.AliveTime)
        {
            Destroy(gameObject, stats.AliveTime);
        }
    }

    // �G�֍U��
    protected void attackEnemy(Collider2D collider2D,float attack)
    {
        // �G����Ȃ�
        if (!collider2D.gameObject.TryGetComponent<EnemyController>(out var enemy)) return;
        // �U��
        float damage = enemy.Damage(attack);
        // ���_���[�W�v�Z
        spawner.TotalDamage += damage;

        // HP������Ύ������_���[�W
        if (stats.HP < 0) return;
        stats.HP--;
        if (stats.HP < 0) Destroy(gameObject);
    }

    // �G�֍U���i�f�t�H���g�̍U���́j
    protected void attackEnemy(Collider2D collider2D)
    {
        attackEnemy(collider2D,stats.Attack);
    }
}
