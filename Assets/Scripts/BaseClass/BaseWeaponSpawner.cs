using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponSpawner : MonoBehaviour
{
    // ����̃v���n�u
    [SerializeField] GameObject PrefabWeapon;

    // ����f�[�^
    public WeaponSpawnerStats Stats;
    // �^�����_���[�W
    public float TotalDamage;
    // �ғ��^�C�}�[
    public float TotalTimer;

    // �����^�C�}�[
    protected float spawnTimer;
    // ������������̃��X�g
    protected List<BaseWeapon> weapons;
    // ���퐶�����u
    protected EnemySpawnerController enemySpawner;

    // ������
    public void Init(EnemySpawnerController enemySpawner,WeaponSpawnerStats stats)
    {
        weapons = new List<BaseWeapon>();
        this.enemySpawner = enemySpawner;
        this.Stats = stats;
    }

    // �ғ��^�C�}�[
    private void FixedUpdate()
    {
        TotalTimer += Time.fixedDeltaTime;
    }

    // ���퐶��
    protected BaseWeapon createWeapon(Vector3 position,Vector2 forward,Transform parent = null)
    {
        // ����
        GameObject obj =Instantiate(PrefabWeapon,position,PrefabWeapon.transform.rotation,parent);
        // ���ʃf�[�^�Z�b�g
        BaseWeapon weapon = obj.GetComponent<BaseWeapon>();
        // �f�[�^������
        weapon.Init(this,forward);
        weapons.Add(weapon);

        return weapon;
    }

    // ���퐶���i�ȈՔŁj
    protected BaseWeapon createWeapon(Vector3 position,Transform parent = null)
    {
        return createWeapon(position,Vector2.zero,parent);
    }

    // ����̃A�b�v�f�[�g���~����
    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
        // �I�u�W�F�N�g���폜
        weapons.RemoveAll(item => !item);
        // ��������������~
        foreach(var item in weapons)
        {
            item.enabled = enabled;
            item.GetComponent<Rigidbody2D>().simulated = enabled;
        }
    }

    // TODO: ���x���A�b�v���̃f�[�^��Ԃ�

}
