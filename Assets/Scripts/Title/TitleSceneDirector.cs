using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneDirector : MonoBehaviour
{
    // �X�^�[�g�{�^��
    [SerializeField] Button buttonStart;
    // ���̃{�^�����珇�Ԃ�ID�̃L�����N�^�[�f�[�^��ǂݍ���
    [SerializeField] List<Button> buttonPlayers;
    [SerializeField] List<Button> buttonScenes;
    [SerializeField] List<int> characterIds;
    [SerializeField] List<int> sceneIds;
    [SerializeField] List<Sprite> sprites;
    [SerializeField] List<string> uniqueSceneName;
    // �I�������L�����N�^�[ID
    public static int CharacterId;
    public static int SceneId;

    // Start is called before the first frame update
    void Start()
    {
        // �{�^����I����Ԃɂ���
        buttonStart.Select();

        int idx = 0;
        int sceneIdx = 0;
        int charId = 0;

        // �v���C���[�{�^��
        foreach (var item in buttonPlayers)
        {
            // �����\��
            item.gameObject.SetActive(false);

            // �f�[�^������Ȃ�
            if (characterIds.Count - 1 < idx) break;

            // �L�����N�^�[�f�[�^
            charId = characterIds[idx++];
            CharacterStats charStats = CharacterSettings.Instance.Get(charId);

            // �����\��1�ڂ̃f�[�^��\��
            int weaponId = charStats.DefaultWeaponIds[0];
            WeaponSpawnerStats weaponStats = WeaponSpawnerSettings.Instance.Get(weaponId, 1);

            Image imageCharacter = item.transform.Find("ImageCharacter").GetComponent<Image>();
            Image imageWeapon = item.transform.Find("ImageWeapon").GetComponent<Image>();
            Text textName = item.transform.Find("TextName").GetComponent<Text>();

            // �L�����N�^�[�摜
            if (charStats.Prefab != null)
            {
                imageCharacter.sprite = charStats.Prefab.GetComponent<SpriteRenderer>().sprite;
            }

            // ����摜
            imageWeapon.sprite = weaponStats.Icon;

            // ���O
            textName.text = charStats.Name;

            // �����ꂽ���̏���
            item.onClick.AddListener(() =>
            {
                // �A�j���[�V������~
                DOTween.KillAll();
                // �I�������L�����N�^�[ID
                CharacterId = charId;
                // SE�Đ�
                SoundManager.Instance.PlaySE(SESoundData.SE.Button);
                // Buttons�t�F�[�h�A�E�g
                Utils.DOfadeUpdate(item, 0, 1);
                item.interactable = false;

                // �I���ł���V�[�����t�F�[�h�C��
                for (int i = 0; i < buttonScenes.Count; i++)
                {
                    var sceneItem = buttonScenes[i];

                    Utils.SetAlpha(sceneItem, 0);
                    sceneItem.gameObject.SetActive(true);

                    Utils.DOfadeUpdate(sceneItem, 1, 1, 0);
                }
            });
        }

        //�V�[���I���{�^��
        foreach (var item in buttonScenes)
        {
            // �����\��
            item.gameObject.SetActive(false);
            
            // �f�[�^������Ȃ�
            if (sceneIds.Count - 1 < sceneIdx) break;

            int sceneId = sceneIds[sceneIdx++];
            string sceneName = uniqueSceneName[sceneId];

            Image imageCharacter = item.transform.Find("ImageScene").GetComponent<Image>();
            Text textName = item.transform.Find("TextName").GetComponent<Text>();

            // �V�[���摜
            imageCharacter.sprite = sprites[sceneId];
            // ���O
            textName.text = sceneName;

            // �����ꂽ���̏���
            item.onClick.AddListener(() =>
            {
                // �A�j���[�V������~
                DOTween.KillAll();
                // �I�������L�����N�^�[ID
                CharacterId = charId;
                // �I�������V�[��ID
                SceneId = sceneId;
                // SE�Đ�
                SoundManager.Instance.PlaySE(SESoundData.SE.Button);
                // Buttons�t�F�[�h�A�E�g
                Utils.DOfadeUpdate(item, 0, 1);
                item.interactable = false;
                // �Q�[���V�[����
                SceneManager.LoadScene("MyGame");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Start�{�^��
    public void OnClickStart()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Button);

        // �X�^�[�g�{�^���t�F�[�h�A�E�g
        Utils.DOfadeUpdate(buttonStart, 0, 1);
        buttonStart.interactable = false;

        // �I���ł���v���C���[���t�F�[�h�C��
        for (int i = 0; i < buttonPlayers.Count; i++)
        {
            var item = buttonPlayers[i];

            Utils.SetAlpha(item, 0);
            item.gameObject.SetActive(true);

            Utils.DOfadeUpdate(item, 1, 1, 0);
        }

        // �{�^����I����Ԃɂ���
        buttonPlayers[0].Select();
    }

    public void OnClickPlayer()
    {
        // �{�^����I����Ԃɂ���
        buttonScenes[0].Select();
    }
}
