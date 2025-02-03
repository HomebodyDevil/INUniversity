using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleCompletePanel : MonoBehaviour
{
    const float DEFAULT_PADDING = 20f;

    [Header("Ratios")]
    [SerializeField] private float padding = 20;
    [SerializeField] private float titlePadding = 0.0f;
    [SerializeField, Range(0, 1)] private float completeTitlePanelRatio;
    [SerializeField, Range(0, 1)] private float levelCheckPanelRatio;
    [SerializeField, Range(0, 1)] private float acquisitionItemPanelRatio;
    [SerializeField, Range(0, 1)] private float acquisitionCardPanelRatio;
    [SerializeField, Range(0, 1)] private float enemyProfilePanelRatio;
    [SerializeField, Range(0, 1)] private float enemyProfileImageHorizontalRatio;
    [SerializeField, Range(0, 1)] private float buttonWidthRatio;

    [Space(10), Header("Rect Objects")]
    [SerializeField] private RectTransform battleComplete;
    [SerializeField] private RectTransform completeTitlePanel;
    [SerializeField] private RectTransform levelCheckPanel;
    [SerializeField] private RectTransform acquisitionItemPanel;
    [SerializeField] private RectTransform acquisitionCardPanel;
    [SerializeField] private RectTransform enemyProfilePanel;
    [SerializeField] private RectTransform button;

    [Space(10), Header("Text Objects")]
    [SerializeField] private TextMeshProUGUI enemyDialogueText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI beforeLevelText;   // 이 두개는 Levelup했을 경우에만..
    [SerializeField] private TextMeshProUGUI afterLevelText;
    [SerializeField] private TextMeshProUGUI originLevelText;   // 이건 Levelup 안 했을 때만...

    [Space(10), Header("Horizontal Layouts")]
    [SerializeField] private GridLayoutGroup itemsLayout;
    [SerializeField] private GridLayoutGroup cardsLayout;

    [Space(10), Header("EnemyProfile")]
    [SerializeField] private RectTransform enemyProfileImage;
    [SerializeField] private RectTransform enemyDialogue;    

    float screenWidth;
    float sreenHeight;
    float panelWidth;
    float panelheight;
    private float usedHeight;

    private void Start()
    {
        usedHeight = 0f;
    }

    private void Update()
    {
        Intialize();
    }

    private void Intialize()
    {
        screenWidth = Screen.width;
        sreenHeight = Screen.height;

        RectTransform panelRectTransform = GetComponent<RectTransform>();
        panelWidth = battleComplete.rect.width;
        panelheight = battleComplete.rect.height;

        float completeTitlePanelHeight = panelheight * completeTitlePanelRatio;
        float levelCheckPanelHeight = panelheight * levelCheckPanelRatio;
        float acquisitionItemPanelHeight = panelheight * acquisitionItemPanelRatio;
        float acquisitionCardPanelHeight = panelheight * acquisitionCardPanelRatio;
        float enemyProfilePanelHeight = panelheight * enemyProfilePanelRatio;
        float buttonWidth = panelWidth * buttonWidthRatio;

        SetPanel(completeTitlePanel, completeTitlePanelHeight, 0);
        SetPanel(levelCheckPanel, levelCheckPanelHeight, padding);
        SetPanel(acquisitionItemPanel, acquisitionItemPanelHeight, padding);
        SetPanel(acquisitionCardPanel, acquisitionCardPanelHeight, padding);

        // 이때 해주어야 제대로된 값을 구할 수 있음.
        // 이때 기준의 usedHeight가 필요하기 때문.
        float enemyImageWidth = (panelheight - usedHeight - 2 * padding) * (enemyProfileImageHorizontalRatio);

        SetPanel(enemyProfilePanel, enemyProfilePanelHeight, padding, true);

        usedHeight = 0f;

        SetGridLayouts();

        // enemyprofile image 설정.
        float enemyProfileImageWidth = enemyProfilePanel.rect.width;
        
        enemyProfileImage.offsetMax = new Vector2(padding + enemyImageWidth - enemyProfileImageWidth, -padding);
        enemyProfileImage.offsetMin = new Vector2(padding, padding);

        enemyDialogue.offsetMax = new Vector2(-padding, -padding);
        enemyDialogue.offsetMin = new Vector2(2 * padding + enemyImageWidth, padding);

        //button 설정
        //button은 BattleComplete가 아니라 BattleCompletePanel의 rect를 기준으로 봐야함.
        //float horizontalMargin = panelWidth - buttonWidth;
        //button.offsetMax = new Vector2(-(horizontalMargin / 2), -padding);
        //button.offsetMin = new Vector2(horizontalMargin / 2, -padding);
    }

    // right, left같은 경우에는 딱히 기입하지 않아도 됨.
    // 어차피 padding만큼 띄워질 것이기 때문.
    //private void SetPanel(RectTransform panel, float top, float bottom, float right = DEFAULT_PADDING, float left = DEFAULT_PADDING)
    //{
    //    panel.offsetMax = new Vector2(top, right);
    //    panel.offsetMin = new Vector2(left, bottom);
    //}

    // 그냥 height만 인자로 받는 함수를 쓰는 방법이 없나 생각해봤음.
    private void SetPanel(RectTransform panel, float height, float _padding, bool isFinal = false)
    {
        float topOfPanel = panelheight - usedHeight;
        float bottom = isFinal ? _padding : topOfPanel - height;

        panel.offsetMax = new Vector2(-_padding, -usedHeight);
        panel.offsetMin = new Vector2(_padding, bottom);

        //Debug.Log(string.Format("height : {0}, _padding: {1}", height, _padding));
        //Debug.Log(string.Format("panelHeight : {0}, usedHeight: {1}", panelheight, usedHeight));

        // _padding이 아니라 padding으로 해야함.
        // completeTitlePanel은 _padding이 0이나, padding은 적용되어야 하기 때문.
        usedHeight += (height + padding);   
    }

    private void SetGridLayouts()
    {
        float cellSize = itemsLayout.gameObject.GetComponent<RectTransform>().rect.height - 2 * (padding / 2);
        itemsLayout.padding.right = itemsLayout.padding.left = itemsLayout.padding.bottom = itemsLayout.padding.top = (int)padding;
        cardsLayout.padding.right = cardsLayout.padding.left = cardsLayout.padding.bottom = cardsLayout.padding.top = (int)padding;
        itemsLayout.cellSize = cardsLayout.cellSize = new Vector2(cellSize, cellSize);
        itemsLayout.spacing = cardsLayout.spacing = new Vector2(padding, 0);
    }

    public void SetComponents(bool isWin)
    {
        DefaultEnemyData enemyData = BattleManager.Instance().currentEnemyData;

        if (isWin)
        {
            titleText.text = "WIN!!";

            enemyProfileImage.gameObject.GetComponent<Image>().sprite =
                Resources.Load<Sprite>(enemyData.enemyPlayerWinProfileImagePath);

            enemyDialogueText.text = enemyData.enemyPlayerWinDialogueText;
        }
        else
        {
            titleText.text = "Lose";

            enemyProfileImage.gameObject.GetComponent<Image>().sprite =
                Resources.Load<Sprite>(enemyData.enemyPlayerLoseProfileImagePath);

            enemyDialogueText.text = enemyData.enemyPlayerLoseDialogueText;
        }
    }
}