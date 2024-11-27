using UnityEngine;
using System;

// �ǉ��X�e�[�^�X�^�C�v
public enum BonusType
{
    // �萔�Œǉ�
    Bonus,
    // %�Œǉ�
    Boost,
}

// �ǉ��f�[�^�^�C�v
public enum StatsType
{
    Attack,
    Defence,
    MoveSpeed,
    HP,
    MaxHP,
    XP,
    MaxXP,
    PickUpRange,
    AliveTime,
    // ���퐶���p�̊g���^�C�v
    SpawnCount,
    SpawnTimerMin,
    SpawnTimerMax,
}

// �ǉ��f�[�^�^�C�v
[Serializable]
public class BonusStats
{
    // �ǉ��^�C�v
    public BonusType Type;
    // �ǉ�����v���p�e�B
    public StatsType Key;
    // �ǉ�����l
    public float Value;
}

// �L�����N�^�[�ƕ���ŋ��ʂ̃X�e�[�^�X
public class BaseStats
{
    // Inspector�ŕ\�������^�C�g��
    public string title;

    // �f�[�^ID
    public int Id;

    // �ݒ背�x��
    public int Lv;

    // ���O
    public string Name;

    // ������
    [TextArea] public string Description;

    // �U����
    public float Attack;

    // �h���
    public float Defence;

    // �̗�
    public float HP;

    // �̗͍ő�
    public float MaxHP;

    // �o���l
    public float XP;

    // �o���l�ő�
    public float MaxXP;

    // �ړ����x
    public float MoveSpeed;

    // �o���l�擾�͈�
    public float PickUpRange;

    // ��������
    public float AliveTime;

    // StatsType�Ƃ̕R�Â� �C���f�N�T�𗘗p����
    public float this[StatsType key]
    {
        get
        {
            if (key == StatsType.Attack) return Attack;
            else if (key == StatsType.Defence) return Defence;
            else if (key == StatsType.MoveSpeed) return MoveSpeed;
            else if (key == StatsType.HP) return HP;
            else if (key == StatsType.MaxHP) return MaxHP;
            else if (key == StatsType.XP) return XP;
            else if (key == StatsType.MaxXP) return MaxXP;
            else if (key == StatsType.PickUpRange) return PickUpRange;
            else if (key == StatsType.AliveTime) return AliveTime;
            else return 0;
        }

        set
        {
            if (key == StatsType.Attack) Attack = value;
            else if (key == StatsType.Defence) Defence = value;
            else if (key == StatsType.MoveSpeed) MoveSpeed = value;
            else if (key == StatsType.HP) HP = value;
            else if (key == StatsType.MaxHP) MaxHP = value;
            else if (key == StatsType.XP) XP = value;
            else if (key == StatsType.MaxXP) MaxXP = value;
            else if (key == StatsType.PickUpRange) PickUpRange = value;
            else if (key == StatsType.AliveTime) AliveTime = value;
        }
    }

    // �{�[�i�X�l���v�Z
    protected float applyBonus(float currentValue, float value, BonusType type)
    {
        // �Œ�l�����Z
        if (BonusType.Bonus == type)
        {
            return currentValue + value;
        }

        // %�ŉ��Z
        else if (BonusType.Boost == type)
        {
            return currentValue * (1 + value);
        }

        return currentValue;
    }

    // �{�[�i�X��ǉ�����֐�
    protected void addBonus(BonusStats bonus)
    {
        float value = applyBonus(this[bonus.Key], bonus.Value, bonus.Type);

        // �ő�l���������
        if(StatsType.HP==bonus.Key)
        {
            value = Mathf.Clamp(value, 0, MaxHP);
        }
        else if (StatsType.XP == bonus.Key)
        {
            value = Mathf.Clamp(value, 0, MaxXP);
        }

        this[bonus.Key] = value;
    }

    // �R�s�[�����f�[�^��Ԃ�
    public BaseStats GetCopy()
    {
        return (BaseStats)MemberwiseClone();
    }
}
