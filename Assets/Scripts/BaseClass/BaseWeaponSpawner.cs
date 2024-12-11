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

    // �^�C�}�[�����`�F�b�N
    protected bool isSpawnTimerNotElapsed()
    {
        // �^�C�}�[����
        spawnTimer -= Time.deltaTime;
        if (0 < spawnTimer) return true;

        return false;
    }

    // ���x���A�b�v���̃f�[�^��Ԃ�
    public WeaponSpawnerStats GetLevelUpStats(bool isNextLevel = false)
    {
        // ���̃��x��
        int nextLv = Stats.Lv + 1;

        // ���̃��x�������邩�ǂ������ׂāA����Ώ㏑��
        WeaponSpawnerStats ret = WeaponSpawnerSettings.Instance.Get(Stats.Id,nextLv);

        // �㏑���f�[�^����
        if(Stats.Lv < ret.Lv)
        {

        }
        else
        {
            // �������A�C�e���̂��̂ɏ���������
            ItemData itemData =ItemSettings.Instance.Get(Stats.LevelUpItemId);
            ret.Description = itemData.Description;
        }

        // ���x����1�����ĕԂ����ǂ���
        if(isNextLevel)
        {
            ret.Lv = nextLv;
        }

        return ret;
    }

    // ���x���A�b�v
    public void LevelUp()
    {
        // ���݂̃��x��
        int lv = Stats.Lv;

        // ���̃��x���̃f�[�^
        WeaponSpawnerStats nextData = GetLevelUpStats();

        // ���݂̃��x���ƈႦ�Ώ㏑��
        if (Stats.Lv < nextData.Lv)
        {
            Stats = nextData;
        }
        // �Ȃ���΃A�C�e���f�[�^��ǉ�
        else
        {
            // �������A�C�e���̂��̂ɏ���������
            ItemData itemData = ItemSettings.Instance.Get(Stats.LevelUpItemId);
            Stats.AddItemData(itemData);
        }

        Stats.Lv = lv + 1;
    }
}
