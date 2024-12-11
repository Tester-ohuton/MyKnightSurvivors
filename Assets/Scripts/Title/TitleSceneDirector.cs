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
    [SerializeField] List<int> characterIds;
    // 選択したキャラクターID
    public static int CharacterId;

    // Start is called before the first frame update
    void Start()
    {
        int idx = 0;
        foreach (var item in buttonPlayers)
        {
            // 初期表示
            item.gameObject.SetActive(false);

            // データが足りない
            if (characterIds.Count - 1 < idx) break;

            // キャラクターデータ
            int charId = characterIds[idx++];
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
                // ゲームシーンへ
                SceneManager.LoadScene("MyGame");
            });
        }

        // ボタンを選択状態にする
        buttonStart.Select();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Startボタン
    public void OnClickStart()
    {
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

        SoundManager.Instance.PlaySE(SESoundData.SE.Button);
    }
}
