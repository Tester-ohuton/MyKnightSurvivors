using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneDirector : MonoBehaviour
{
    // スタートボタン
    [SerializeField] Button buttonStart;
    // 左のボタンから順番にIDのキャラクターデータを読み込む
    [SerializeField] List<Button> buttonPlayers;
    [SerializeField] List<Button> buttonScenes;
    [SerializeField] List<int> characterIds;
    [SerializeField] List<int> sceneIds;
    [SerializeField] List<Sprite> sprites;
    [SerializeField] List<string> uniqueSceneName;
    // 選択したキャラクターID
    public static int CharacterId;
    public static int SceneId;

    // Start is called before the first frame update
    void Start()
    {
        // ボタンを選択状態にする
        buttonStart.Select();

        int idx = 0;
        int sceneIdx = 0;
        int charId = 0;

        // プレイヤーボタン
        foreach (var item in buttonPlayers)
        {
            // 初期表示
            item.gameObject.SetActive(false);

            // データが足りない
            if (characterIds.Count - 1 < idx) break;

            // キャラクターデータ
            charId = characterIds[idx++];
            CharacterStats charStats = CharacterSettings.Instance.Get(charId);

            // 装備可能な1つ目のデータを表示
            int weaponId = charStats.DefaultWeaponIds[0];
            WeaponSpawnerStats weaponStats = WeaponSpawnerSettings.Instance.Get(weaponId, 1);

            Image imageCharacter = item.transform.Find("ImageCharacter").GetComponent<Image>();
            Image imageWeapon = item.transform.Find("ImageWeapon").GetComponent<Image>();
            Text textName = item.transform.Find("TextName").GetComponent<Text>();

            // キャラクター画像
            if (charStats.Prefab != null)
            {
                imageCharacter.sprite = charStats.Prefab.GetComponent<SpriteRenderer>().sprite;
            }

            // 武器画像
            imageWeapon.sprite = weaponStats.Icon;

            // 名前
            textName.text = charStats.Name;

            // 押された時の処理
            item.onClick.AddListener(() =>
            {
                // アニメーション停止
                DOTween.KillAll();
                // 選択したキャラクターID
                CharacterId = charId;
                // SE再生
                SoundManager.Instance.PlaySE(SESoundData.SE.Button);
                // Buttonsフェードアウト
                Utils.DOfadeUpdate(item, 0, 1);
                item.interactable = false;

                // 選択できるシーンをフェードイン
                for (int i = 0; i < buttonScenes.Count; i++)
                {
                    var sceneItem = buttonScenes[i];

                    Utils.SetAlpha(sceneItem, 0);
                    sceneItem.gameObject.SetActive(true);

                    Utils.DOfadeUpdate(sceneItem, 1, 1, 0);
                }
            });
        }

        //シーン選択ボタン
        foreach (var item in buttonScenes)
        {
            // 初期表示
            item.gameObject.SetActive(false);
            
            // データが足りない
            if (sceneIds.Count - 1 < sceneIdx) break;

            int sceneId = sceneIds[sceneIdx++];
            string sceneName = uniqueSceneName[sceneId];

            Image imageCharacter = item.transform.Find("ImageScene").GetComponent<Image>();
            Text textName = item.transform.Find("TextName").GetComponent<Text>();

            // シーン画像
            imageCharacter.sprite = sprites[sceneId];
            // 名前
            textName.text = sceneName;

            // 押された時の処理
            item.onClick.AddListener(() =>
            {
                // アニメーション停止
                DOTween.KillAll();
                // 選択したキャラクターID
                CharacterId = charId;
                // 選択したシーンID
                SceneId = sceneId;
                // SE再生
                SoundManager.Instance.PlaySE(SESoundData.SE.Button);
                // Buttonsフェードアウト
                Utils.DOfadeUpdate(item, 0, 1);
                item.interactable = false;
                // ゲームシーンへ
                SceneManager.LoadScene("MyGame");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Startボタン
    public void OnClickStart()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Button);

        // スタートボタンフェードアウト
        Utils.DOfadeUpdate(buttonStart, 0, 1);
        buttonStart.interactable = false;

        // 選択できるプレイヤーをフェードイン
        for (int i = 0; i < buttonPlayers.Count; i++)
        {
            var item = buttonPlayers[i];

            Utils.SetAlpha(item, 0);
            item.gameObject.SetActive(true);

            Utils.DOfadeUpdate(item, 1, 1, 0);
        }

        // ボタンを選択状態にする
        buttonPlayers[0].Select();
    }

    public void OnClickPlayer()
    {
        // ボタンを選択状態にする
        buttonScenes[0].Select();
    }
}
