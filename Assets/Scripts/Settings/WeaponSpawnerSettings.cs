using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// �E�N���b�N���j���[�ɕ\������Afilename�̓f�t�H���g�̃t�@�C����
[CreateAssetMenu(fileName = "WeaponSpawnerSettings", menuName = "ScriptableObjects/WeaponSpawnerSettings")]
public class WeaponSpawnerSettings : ScriptableObject
{
    // �L�����N�^�[�f�[�^
    public List<WeaponSpawnerStats> datas;

    public static WeaponSpawnerSettings instance;
    public static WeaponSpawnerSettings Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<WeaponSpawnerSettings>(nameof(WeaponSpawnerSettings));
            }

            return instance;
        }
    }

    // ���X�g��ID����f�[�^����������
    public WeaponSpawnerStats Get(int id,int lv)
    {
        // �w�肳�ꂽ���x���̃f�[�^���Ȃ����1�ԍ������x���̃f�[�^��Ԃ�
        WeaponSpawnerStats ret = null;

        foreach (var item in datas)
        {
            if (id != item.Id) continue;

            // �w�背�x���ƈ�v
            if(lv == item.Lv)
            {
                return (WeaponSpawnerStats)item.GetCopy();
            }

            // ���̃f�[�^���Z�b�g����Ă��Ȃ����A����𒴂��郌�x���������������ւ���
            if (null == ret)
            {
                ret = item;
            }
            // �T���Ă��郌�x����艺�ŁA�b��f�[�^���傫��
            else if (item.Lv < lv && ret.Lv < item.Lv)
            {
                ret = item;
            }
        }

        return (WeaponSpawnerStats)ret.GetCopy();
    }

    // �쐬
    public BaseWeaponSpawner CreateWeaponSpawner(int id,EnemySpawnerController enemySpawner,Transform parent = null)
    {
        // �f�[�^�擾
        WeaponSpawnerStats stats = Instance.Get(id,1);
        //�I�u�W�F�N�g�쐬
        GameObject obj = Instantiate(stats.PrefabSpawner,parent);
        // �f�[�^�Z�b�g
        BaseWeaponSpawner spawner = obj.GetComponent<BaseWeaponSpawner>();
        spawner.Init(enemySpawner,stats);
    
        return spawner;
    }
}

// ���퐶�����u
[Serializable]
public class WeaponSpawnerStats : BaseStats
{
    // �������u�̃v���n�u
    public GameObject PrefabSpawner;
    // ����̃A�C�R��
    public Sprite Icon;
    // ���x���A�b�v���ɒǉ������A�C�e��ID
    public int LevelUpItemId;

    // ��x�ɐ������鐔
    public float SpawnCount;
    // �����^�C�}�[
    public float SpawnTimerMin;
    public float SpawnTimerMax;

    // �������Ԏ擾
    public float GetRandomSpawnTimer()
    {
        return Random.Range(SpawnTimerMin, SpawnTimerMax);
    }

    // TODO:�A�C�e����ǉ�
}