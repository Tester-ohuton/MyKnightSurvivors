using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelLevelUpController : MonoBehaviour
{
    [SerializeField] List<Button> buttonLevelUps;
    [SerializeField] Button buttonCancel;

    GameSceneDirector sceneDirector;
    // �I���J�[�\��
    int selectButtonCursor;
    // �\�����̃{�^��
    public List<Button> dispButtons;

    // ������
    public void Init(GameSceneDirector sceneDirector)
    {
        this.sceneDirector = sceneDirector;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetButtonLevelUp(Button button,int lv,string name,string desc,Sprite icon)
    {
        Image image = button.transform.Find("ImageItem").GetComponent<Image>();
        Text itemName =button.transform.Find("TextName").GetComponent<Text>();
        Text level = button.transform.Find("TextLevel").GetComponent<Text>();
        Text help = button.transform.Find("TextHelp").GetComponent<Text>();
    
        image.sprite = icon;
        itemName.text = name;
        help.text = desc;
        // ���x���̕\���������ς���
        level.text = "LV�F"+lv;
        level.color = Color.white;
        // ��������
        if (1 == lv)
        {
            level.text = "NEW !!";
            level.color = Color.yellow;
        }

        button.gameObject.SetActive(true);
    }

    // �L�����Z���{�^��
    public void OnClickCancel()
    {
        gameObject.SetActive(false);
        sceneDirector.PlayGame();
    }

    // ���x���A�b�v�p�l���\��
    public void DispPanel(List<WeaponSpawnerStats> items)
    {
        // �A�C�e�����Ȃ��Ƃ�
        buttonCancel.gameObject.SetActive(false);

        // �\�����̃{�^��
        dispButtons = new List<Button>();
        
        for(int i = 0;i < buttonLevelUps.Count;i++)
        {
            // ���񐶐�����{�^��
            Button button = buttonLevelUps[i];
            // �{�^��������
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();

            // �\������A�C�e�����Ȃ���Ύ���
            if (items.Count - 1 < i) continue;

            // ����ݒ肷��A�C�e��
            WeaponSpawnerStats item = items[i];

            // �����ꂽ���̏���
            button.onClick.AddListener(() =>
            {
                sceneDirector.PlayGame(new BonusData(item));
                gameObject.SetActive(false);
            });

            // �{�^���̃f�[�^
            int lv = item.Lv;
            string name = item.Name;
            string desc = item.Description;
            Sprite icon =item.Icon;
            // �{�^���쐬
            SetButtonLevelUp(button,lv,name,desc,icon);
            dispButtons.Add(button);
        }

        // �J�[�\�����Z�b�g
        selectButtonCursor = 0;

        // �I�ׂ�{�^���Ȃ�
        if(items.Count < 1)
        {
            buttonCancel.gameObject.SetActive(true);
            // �I����Ԃɂ���
            buttonCancel.Select();
        }
        else
        {
            dispButtons[0].Select();
        }

        // �O�ʂɕ\��
        transform.SetAsLastSibling();

        // �p�l���\��
        gameObject.SetActive(true);
    }

    // ���x���A�b�v�p�l���ŕK�v�ȃA�C�e����
    public int GetButtonCount()
    {
        return buttonLevelUps.Count;
    }
}
