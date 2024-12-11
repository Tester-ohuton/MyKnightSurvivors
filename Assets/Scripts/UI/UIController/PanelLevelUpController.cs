using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelLevelUpController : MonoBehaviour
{
    [SerializeField] List<Button> ButtonLevelUps;
    [SerializeField] Button buttonCancel;

    GameSceneDirector sceneDirector;
    // 選択カーソル
    int selectButtonCursor;
    // 表示中のボタン
    List<Button> dispButtons;

    // 初期化
    public void Init(GameSceneDirector sceneDirector)
    {
        this.sceneDirector = sceneDirector;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // レベルアップ時のボタンの設定
    void setButtonLvUp(Button button, int lv, string name, string desc, Sprite icon)
    {
        Image image = button.transform.Find("ImageItem").GetComponent<Image>();
        Text itemName = button.transform.Find("TextItemName").GetComponent<Text>();
        Text level = button.transform.Find("TextLevel").GetComponent<Text>();
        Text help = button.transform.Find("TextHelp").GetComponent<Text>();

        image.sprite = icon;
        itemName.text = name;
        help.text = desc;
        // レベルの表示を少し変える
        level.text = "LV : " + lv;
        level.color = Color.white;
        // 初期装備
        if (1 == lv)
        {
            level.text = "NEW!!";
            level.color = Color.yellow;
        }

        button.gameObject.SetActive(true);
    }

    // キャンセルボタン
    public void OnClickCancel()
    {
        gameObject.SetActive(false);
        sceneDirector.PlayGame();
    }

    // レベルアップパネル表示
    public void DispPanel(List<WeaponSpawnerStats> items)
    {
        // アイテムがないとき
        buttonCancel.gameObject.SetActive(false);

        // 表示中のボタン
        dispButtons = new List<Button>();

        for (int i = 0; i < ButtonLevelUps.Count; i++)
        {
            // 今回生成するボタン
            Button button = ButtonLevelUps[i];
            // ボタン初期化
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();

            // 表示するアイテムがなければ次へ
            if (items.Count - 1 < i) continue;

            // 今回設定するアイテム
            WeaponSpawnerStats item = items[i];

            // 押された時の処理
            button.onClick.AddListener(() =>
            {
                sceneDirector.PlayGame(new BonusData(item));
                gameObject.SetActive(false);
            });

            // ボタンのデータ
            int lv = item.Lv;
            string name = item.Name;
            string desc = item.Description;
            Sprite icon = item.Icon;
            // ボタン作成
            setButtonLvUp(button, lv, name, desc, icon);
            dispButtons.Add(button);
        }

        // カーソルリセット
        selectButtonCursor = 0;

        // 選べるボタンなし
        if (1 > items.Count)
        {
            buttonCancel.gameObject.SetActive(true);
            // デフォルトで選択状態にする
            buttonCancel.Select();
        }
        // 1つ目の項目を選択状態にする
        else
        {
            dispButtons[0].Select();
        }

        // 前面に表示
        transform.SetAsLastSibling();

        // パネル表示
        gameObject.SetActive(true);
    }

    // レベルアップパネルで必要なアイテム数
    public int GetButtonCount()
    {
        return ButtonLevelUps.Count;
    }
}
