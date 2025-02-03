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
    [SerializeField] private TextMeshProUGUI beforeLevelText;   // �� �ΰ��� Levelup���� ��쿡��..
    [SerializeField] private TextMeshProUGUI afterLevelText;
    [SerializeField] private TextMeshProUGUI originLevelText;   // �̰� Levelup �� ���� ����...

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

        // �̶� ���־�� ����ε� ���� ���� �� ����.
        // �̶� ������ usedHeight�� �ʿ��ϱ� ����.
        float enemyImageWidth = (panelheight - usedHeight - 2 * padding) * (enemyProfileImageHorizontalRatio);

        SetPanel(enemyProfilePanel, enemyProfilePanelHeight, padding, true);

        usedHeight = 0f;

        SetGridLayouts();

        // enemyprofile image ����.
        float enemyProfileImageWidth = enemyProfilePanel.rect.width;
        
        enemyProfileImage.offsetMax = new Vector2(padding + enemyImageWidth - enemyProfileImageWidth, -padding);
        enemyProfileImage.offsetMin = new Vector2(padding, padding);

        enemyDialogue.offsetMax = new Vector2(-padding, -padding);
        enemyDialogue.offsetMin = new Vector2(2 * padding + enemyImageWidth, padding);

        //button ����
        //button�� BattleComplete�� �ƴ϶� BattleCompletePanel�� rect�� �������� ������.
        //float horizontalMargin = panelWidth - buttonWidth;
        //button.offsetMax = new Vector2(-(horizontalMargin / 2), -padding);
        //button.offsetMin = new Vector2(horizontalMargin / 2, -padding);
    }

    // right, left���� ��쿡�� ���� �������� �ʾƵ� ��.
    // ������ padding��ŭ ����� ���̱� ����.
    //private void SetPanel(RectTransform panel, float top, float bottom, float right = DEFAULT_PADDING, float left = DEFAULT_PADDING)
    //{
    //    panel.offsetMax = new Vector2(top, right);
    //    panel.offsetMin = new Vector2(left, bottom);
    //}

    // �׳� height�� ���ڷ� �޴� �Լ��� ���� ����� ���� �����غ���.
    private void SetPanel(RectTransform panel, float height, float _padding, bool isFinal = false)
    {
        float topOfPanel = panelheight - usedHeight;
        float bottom = isFinal ? _padding : topOfPanel - height;

        panel.offsetMax = new Vector2(-_padding, -usedHeight);
        panel.offsetMin = new Vector2(_padding, bottom);

        //Debug.Log(string.Format("height : {0}, _padding: {1}", height, _padding));
        //Debug.Log(string.Format("panelHeight : {0}, usedHeight: {1}", panelheight, usedHeight));

        // _padding�� �ƴ϶� padding���� �ؾ���.
        // completeTitlePanel�� _padding�� 0�̳�, padding�� ����Ǿ�� �ϱ� ����.
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