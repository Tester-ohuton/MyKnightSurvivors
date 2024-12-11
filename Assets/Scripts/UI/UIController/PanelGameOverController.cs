using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PanelGameOverController : MonoBehaviour
{
    // Start is called before the first frame update
    // 背景
    [SerializeField] Image panelGameOverBG;
    // 生存時間、レベル、倒した敵
    [SerializeField] Text textSurvivedResult;
    [SerializeField] Text textLevelResult;
    [SerializeField] Text textEnemyResult;
    // 武器、レベル、ダメージ、時間、DPS
    [SerializeField] Text textWeaponNameTitle;
    [SerializeField] Text textWeaponLevelTitle;
    [SerializeField] Text textWeaponDamageTitle;
    [SerializeField] Text textWeaponTimeTitle;
    [SerializeField] Text textWeaponDPSTitle;
    // 終了ボタン
    [SerializeField] Button buttonDone;
    // パネル上の全テキスト
    List<Text> resultTexts;

    GameSceneDirector sceneDirector;

    // 初期化
    public void Init(GameSceneDirector sceneDirector)
    {
        this.sceneDirector = sceneDirector;
        resultTexts = new List<Text>();

        // 全テキストを取得
        foreach (var item in gameObject.GetComponentsInChildren<Text>())
        {
            resultTexts.Add(item);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // テキストを複製して新しいｙ座標をセット
    Text duplicateText(Text parent, string dispText, float y)
    {
        GameObject obj = Instantiate(parent.gameObject, transform);
        Text text = obj.GetComponent<Text>();
        text.color = Color.white;
        text.text = dispText;

        // y座標セット
        Vector3 pos = text.rectTransform.anchoredPosition;
        pos.y = y;
        text.rectTransform.anchoredPosition = pos;

        return text;
    }

    // パネル表示
    public void DispPanel(List<BaseWeaponSpawner> weaponSpawners)
    {
        // 生存時間
        textSurvivedResult.text = Utils.GetTextTimer(sceneDirector.GameTimer);
        // レベル
        textLevelResult.text = "" + sceneDirector.Player.Stats.Lv;
        // 倒した敵の数
        textEnemyResult.text = "" + sceneDirector.DefeatedEnemyCount;

        // スタート位置
        float y = textWeaponNameTitle.rectTransform.anchoredPosition.y;
        // テキスト縦サイズ
        float h = textWeaponNameTitle.rectTransform.rect.height;

        // 武器のリザルトを1行ずつ生成
        foreach (var item in weaponSpawners)
        {
            // 表示位置
            y -= h;

            // 武器名
            Text text = duplicateText(textWeaponNameTitle, item.Stats.Name, y);
            resultTexts.Add(text);

            // レベル
            text = duplicateText(textWeaponLevelTitle, "" + item.Stats.Lv, y);
            resultTexts.Add(text);

            // ダメージ
            text = duplicateText(textWeaponDamageTitle, "" + (int)item.TotalDamage, y);
            resultTexts.Add(text);

            // 時間
            text = duplicateText(textWeaponTimeTitle, Utils.GetTextTimer(item.TotalTimer), y);
            resultTexts.Add(text);

            // DPS
            int sec = (int)(item.TotalTimer + 1);
            int dps = (int)item.TotalDamage / sec;
            text = duplicateText(textWeaponDPSTitle, "" + dps, y);
            resultTexts.Add(text);
        }

        // 順番に表示
        Sequence seq = DOTween.Sequence();
        float dispTime = 1.5f;

        // BG
        Utils.SetAlpha(panelGameOverBG, 0);
        seq.Append(panelGameOverBG.DOFade(1, dispTime));

        // このパネル
        Image panelGameOver = gameObject.GetComponent<Image>();
        Utils.SetAlpha(panelGameOver, 0);
        seq.Append(panelGameOver.DOFade(1, dispTime));

        // 全テキスト
        for (int i = 0; i < resultTexts.Count; i++)
        {
            var item = resultTexts[i];
            Utils.SetAlpha(item, 0);

            // 1つ目のテキスト
            if (0 == i)
            {
                seq.Append(item.DOFade(1, dispTime));
            }
            // それ以外は1つ目に合わせる
            else
            {
                seq.Join(item.DOFade(1, dispTime));
            }
        }

        // 閉じるボタンと子オブジェクト
        Utils.SetAlpha(buttonDone, 0);

        // 表示し終わったらリスナーを登録
        seq.Append(buttonDone.image.DOFade(1, dispTime)
            .OnComplete(() =>
            {
                buttonDone.onClick.AddListener(sceneDirector.LoadSceneTitle);
                // ボタンを選択状態にする
                buttonDone.Select();
            }));

        foreach (var item in buttonDone.GetComponentsInChildren<Graphic>())
        {
            seq.Join(item.DOFade(1, dispTime));
        }

        // 再生
        seq.Play().SetUpdate(true);

        // 全面に表示
        panelGameOverBG.transform.SetAsLastSibling();
        transform.SetAsLastSibling();

        // パネル表示
        panelGameOverBG.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }
}
