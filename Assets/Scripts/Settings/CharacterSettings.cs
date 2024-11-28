using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
