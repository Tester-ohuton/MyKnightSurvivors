using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �E�N���b�N���j���[�ɕ\������Afilename�̓f�t�H���g�̃t�@�C����
[CreateAssetMenu(fileName = "ItemSettings", menuName = "ScriptableObjects/ItemSettings")]
public class ItemSettings : ScriptableObject
{
    // �A�C�e���f�[�^
    public List<ItemData> datas;

    public static ItemSettings instance;
    public static ItemSettings Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<ItemSettings>(nameof(ItemSettings));
            }

            return instance;
        }
    }

    // ���X�g��ID����f�[�^����������
    public ItemData Get(int id)
    {
        return (ItemData)datas.Find(item => item.Id == id).GetCopy();
    }
}

[Serializable]
public class ItemData
{
    public string Title;

    // �ŗLID
    public int Id;
    // �A�C�e����
    public string Name;
    // ����
    [TextArea] public string Description;
    // �A�C�R��
    public Sprite Icon;
    // �{�[�i�X
    public List<BonusStats> Bonuses;

    // �R�s�[�����f�[�^��Ԃ� 
    public ItemData GetCopy()
    {
        return (ItemData)MemberwiseClone();
    }
}