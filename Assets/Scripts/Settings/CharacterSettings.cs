using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �E�N���b�N���j���[�ɕ\������Afilename�̓f�t�H���g�̃t�@�C����
[CreateAssetMenu(fileName = "CharacterSettings", menuName = "ScriptableObjects/CharacterSettings")]
public class CharacterSettings : ScriptableObject
{
    // �L�����N�^�[�f�[�^
    public List<CharacterStats> datas;

    public static CharacterSettings instance;
    public static CharacterSettings Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<CharacterSettings>(nameof(CharacterSettings));
            }

            return instance;
        }
    }

    // ���X�g��ID����f�[�^����������
    public CharacterStats Get(int id)
    {
        return (CharacterStats)datas.Find(item => item.Id == id).GetCopy();
    }

    // �G����
    public EnemyController CreateEnemy(int id,GameSceneDirector sceneDirector,Vector3 position)
    {
        // �X�e�[�^�X�擾
        CharacterStats stats = Instance.Get(id);
        // �I�u�W�F�N�g
        GameObject obj = Instantiate(stats.Prefab, position, Quaternion.identity);
        
        // �f�[�^�Z�b�g
        EnemyController ctrl= obj.GetComponent<EnemyController>();
        ctrl.Init(sceneDirector, stats);

        return ctrl;
    }

    // �v���C���[����
    public PlayerController CreatePlayer(int id,GameSceneDirector sceneDirector,
        EnemySpawnerController enemySpawner, Text textLv,Slider sliderHP,Slider sliderXP)
    {
        // �X�e�[�^�X�̎擾
        CharacterStats stats = Instance.Get(id);
        // �I�u�W�F�N�g����
        GameObject obj = Instantiate(stats.Prefab,Vector3.zero,Quaternion.identity);

        // �f�[�^�Z�b�g
        PlayerController ctrl = obj.GetComponent<PlayerController>();   
        ctrl.Init(sceneDirector,enemySpawner,stats,textLv,sliderHP,sliderXP);

        return ctrl;
    }
}

// �G�̓���
public enum MoveType
{
    // �v���C���[�Ɍ������Đi��
    TargetPlayer,
    // ������ɐi��
    TargetDirection
}

[Serializable]
public class CharacterStats : BaseStats
{
    // �L�����N�^�[�̃v���n�u
    public GameObject Prefab;
    // ������������ID
    public List<int> DefaultWeaponIds;
    // �����\����ID
    public List<int> UsableWeaponIds;
    // �����\��
    public int UsableWeaponMax;
    // �ړ��^�C�v
    public MoveType MoveType;

    // TODO �A�C�e���ǉ�
}
