using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeckPanel : MonoBehaviour, IPanel
{
    public static Action Act_DisableButtons;
    public static Action Act_EnableUseButton;
    public static Action Act_EnableUnuseButton;
    public static Action Act_EnableDescriptionButton;
    public static Action<CardSlot> Act_SelectCurrentCardSlot;
    public static Action UpdateCardSlots;

    public static Action Act_UpdateDeckImages;

    [SerializeField] private float padding;
    [SerializeField] private float gap;
    [SerializeField] private float spacing = 20;
    [SerializeField] [Range(0, 1)] private float deckPanelHeightRatio = 0.11f;
    [SerializeField] private GameObject deckPanel;
    [SerializeField] private GameObject havePanel;
    [SerializeField] private GameObject deckPanelGridObject;    //grid를 가지고 있는 오브젝트
    [SerializeField] private GameObject havePanelGridObject;    //grid를 가지고 있는 오브젝트

    [Space(10), Header("For Panels And Grids")]
    [SerializeField] private float deckPanelHorizontalPadding;
    [SerializeField] private float deckPanelVerticalPadding;
    [SerializeField] private float haveGridPadding;
    [SerializeField] private float haveGridSpacing;
    [SerializeField] private float havePanelHorizontalPadding;
    [SerializeField] private float havePanelVerticalPadding;
    [SerializeField] private float deckGridPadding;
    [SerializeField] private float deckGridSpacing;
    [SerializeField] private float deckScrollHorizontalPadding;
    [SerializeField] private float deckScrollVerticalPadding;

    [Space(10), Header("Buttons, Colors and Paths")]
    [SerializeField] private GameObject useButton;
    [SerializeField] private GameObject unuseButton;
    [SerializeField] private GameObject descriptionButton;
    [SerializeField] private string cardSlotPath;

    [Space(10), Header("Cards, Deck Image")]
    [SerializeField] private Image deckCardImageHolder_0;
    [SerializeField] private Image deckCardImageHolder_1;
    [SerializeField] private Image deckCardImageHolder_2;
    [SerializeField] private Image deckCardImageHolder_3;
    [SerializeField] private Image deckCardImageHolder_4;
    [SerializeField] private Image deckCardImageHolder_5;

    private RectTransform panelRectTransform;
    private RectTransform deckPanelRectTransform;
    private RectTransform havePanelRectTransform;

    private Transform contents;

    private MapUIManager uiManager;

    private IPanel panelInterface;

    private List<CardSlot> cardSlotList;
    private List<Image> deckImageHoldersList;

    private void Awake()
    {
        //Act_DisableButtons -= DisableButtons;
        //Act_DisableButtons += DisableButtons;

        //Act_EnableUseButton -= EnableUseButton;
        //Act_EnableUseButton += EnableUseButton;

        //Act_EnableUnuseButton -= EnableUnuseButton;
        //Act_EnableUnuseButton += EnableUnuseButton;

        //Act_EnableDescriptionButton -= EnableDescriptionButton;
        //Act_EnableDescriptionButton += EnableDescriptionButton;

        //Act_UpdateDeckImages -= UpdateDeckCardImageHolders;
        //Act_UpdateDeckImages += UpdateDeckCardImageHolders;

        //UpdateCardSlots -= InitializeItemSlots;
        //UpdateCardSlots += InitializeItemSlots;
    }

    //private void Update()
    //{
    //    panelInterface.Initialize();
    //}

    // Start is called before the first frame update
    void Start()
    {
        panelRectTransform = GetComponent<RectTransform>();
        deckPanelRectTransform = deckPanel.GetComponent<RectTransform>();
        havePanelRectTransform = havePanel.GetComponent<RectTransform>();

        // 인터페이스로의 함수를 사용하기 위해선, 인터페이스 객체?를 통해서만 가능.
        panelInterface = GetComponent<IPanel>();

        panelInterface.GetUIManager();
        panelInterface.Initialize();

        contents = havePanelGridObject.transform;

        deckImageHoldersList = new List<Image>(PlayerCardManager.DECK_CARDS_COUNT);
        deckImageHoldersList.Add(deckCardImageHolder_0);
        deckImageHoldersList.Add(deckCardImageHolder_1);
        deckImageHoldersList.Add(deckCardImageHolder_2);
        deckImageHoldersList.Add(deckCardImageHolder_3);
        deckImageHoldersList.Add(deckCardImageHolder_4);
        deckImageHoldersList.Add(deckCardImageHolder_5);

        InitializeItemSlots();
        DisableButtons();
        UpdateDeckCardImageHolders();
    }

    void IPanel.Initialize()
    {
        float width = panelRectTransform.rect.width - 2 * deckPanelHorizontalPadding;  // DeckPanel의 가로에서 양쪽 padding을 제외한 길이
        float height = panelRectTransform.rect.height - 2 * deckPanelVerticalPadding - gap;

        float deckPanelHeight = height * deckPanelHeightRatio;
        float havePanelHeight = height * (1 - deckPanelHeightRatio);

        //deckPanelRectTransform.offsetMax = new Vector2(-padding, -padding);
        //deckPanelRectTransform.offsetMin = new Vector2(padding, padding + gap + havePanelHeight);
        deckPanelRectTransform.offsetMax = new Vector2(-deckPanelHorizontalPadding, -deckPanelVerticalPadding);
        deckPanelRectTransform.offsetMin = new Vector2(deckPanelHorizontalPadding, deckPanelVerticalPadding + gap + havePanelHeight);

        //havePanelRectTransform.offsetMax = new Vector2(-padding, -padding - deckPanelHeight - gap);
        //havePanelRectTransform.offsetMin = new Vector2(padding, padding);
        havePanelRectTransform.offsetMax = new Vector2(-havePanelHorizontalPadding, -havePanelVerticalPadding - deckPanelHeight - gap);
        havePanelRectTransform.offsetMin = new Vector2(havePanelHorizontalPadding, havePanelVerticalPadding);

        GridLayoutGroup deckGrid = deckPanelGridObject.GetComponent<GridLayoutGroup>();
        GridLayoutGroup haveGrid = havePanelGridObject.GetComponent<GridLayoutGroup>();

        deckGrid.padding.right = (int)deckGridPadding; 
        deckGrid.padding.left = (int)deckGridPadding; 
        deckGrid.padding.top = (int)deckGridPadding; 
        deckGrid.padding.bottom = (int)deckGridPadding;

        haveGrid.padding.right = (int)haveGridPadding; 
        haveGrid.padding.left = (int)haveGridPadding; 
        haveGrid.padding.top = (int)haveGridPadding; 
        haveGrid.padding.bottom = (int)haveGridPadding;
        
        // deck의 grid의 Cell들.
        float cellWidth = (width - 4 * deckGridPadding) / 5;
        float cellHeight = deckPanelHeight - deckGridPadding / 2;

        // have의 grid의 Cell들을 위함.
        float cellSize = Mathf.Min(cellWidth * 5 / 6, cellHeight);  // 처음엔 5개 기준으로 했는데 6개로 야매 방식으로 수정함.
        Vector2 cellSizeVec = new Vector2(cellSize, cellSize);             

        deckGrid.cellSize = cellSizeVec;

        cellWidth = (width - 4 * haveGridPadding) / 5;
        //cellHeight = deckPanelHeight - deckGridPadding / 2;
        haveGrid.cellSize = new Vector2(cellWidth, cellWidth);

        Vector2 spacingVec = new Vector2(deckGridSpacing, deckGridSpacing);
        deckGrid.spacing = spacingVec;

        spacingVec.x = spacingVec.y = haveGridSpacing;
        haveGrid.spacing = spacingVec;
    }

    private void OnEnable()
    {
        Act_DisableButtons -= DisableButtons;
        Act_DisableButtons += DisableButtons;

        Act_EnableUseButton -= EnableUseButton;
        Act_EnableUseButton += EnableUseButton;

        Act_EnableUnuseButton -= EnableUnuseButton;
        Act_EnableUnuseButton += EnableUnuseButton;

        Act_EnableDescriptionButton -= EnableDescriptionButton;
        Act_EnableDescriptionButton += EnableDescriptionButton;

        Act_UpdateDeckImages -= UpdateDeckCardImageHolders;
        Act_UpdateDeckImages += UpdateDeckCardImageHolders;

        UpdateCardSlots -= InitializeItemSlots;
        UpdateCardSlots += InitializeItemSlots;

        InitializeItemSlots();
        UpdateDeckCardImageHolders();
    }

    private void OnDisable()
    {
        Act_DisableButtons -= DisableButtons;
        Act_EnableUseButton -= EnableUseButton;
        Act_EnableUnuseButton -= EnableUnuseButton;
        Act_EnableDescriptionButton -= EnableDescriptionButton;
        Act_UpdateDeckImages -= UpdateDeckCardImageHolders;
        UpdateCardSlots -= InitializeItemSlots;

        StopCoroutine("WaitForManagerCoroutine");
        DisableButtons();
    }

    public void GetUIManager()
    {
        StartCoroutine(GetUIManagerCoroutine());
    }

    public IEnumerator GetUIManagerCoroutine()
    {
        while (true)
        {
            uiManager = MapUIManager.Instance();
            if (uiManager != null)
                yield break;
            else
                yield return null;
        }
    }

    public void OnClicked()
    {
        //Debug.Log("Clicked Deck");

        bool isActive = gameObject.activeSelf;

        if (isActive)
        {
            //gameObject.SetActive(false);
        }
        else
        {
            CanclePanel.OnCanclePanelClicked.Invoke();

            gameObject.SetActive(true);
            UIManager.Instance().CurrentUIManager().ActivateCanclePanel();
            UIManager.Instance().CurrentUIManager().activeObject.Push(gameObject);

            // Panel이 활성화됐을 때 카메라의 움직임을 제한하기 위함.
            // 차후 문제가 발생할 여력이 충분함... 따라서...
            // 나주엥 더 나은 방법이 생기면 그걸 쓰도록 하자...
            // Time.timeScale = 0;

            //Player player = Player.Instance();
            //PlayerCameraController cameraController = player.gameObject.GetComponent<PlayerCameraController>();
            //cameraController.InActivateAllCamera();

            CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            cinemachineBrain.enabled = false;
        }
    }

    private void EnableUseButton()
    {
        useButton.SetActive(true);
    }

    private void EnableUnuseButton()
    {
        unuseButton.SetActive(true);
    }

    private void EnableDescriptionButton()
    {
        descriptionButton.SetActive(true);
    }

    private void DisableButtons()
    {
        useButton.SetActive(false);
        unuseButton.SetActive(false);
        descriptionButton.SetActive(false);
    }

    public void IncreaseCardSlotNumber(int n)
    {
        GameObject tmpSlot;
        for (int i = 0; i < n; i++)
        {
            tmpSlot = Resources.Load<GameObject>(cardSlotPath);
            tmpSlot = Instantiate(tmpSlot);

            tmpSlot.transform.SetParent(contents, false);

            cardSlotList.Add(tmpSlot.GetComponent<CardSlot>());
        }
    }

    private void WaitForManager()
    {
        StartCoroutine("WaitForManagerCoroutine");
    }

    IEnumerator WaitForManagerCoroutine()
    {
        while(!PlayerCardManager.isInitialized)
        {
            Debug.Log("WATING!");
            yield return null;
        }
    }

    public void InitializeItemSlots()
    {
        WaitForManager();

        if (contents != null)
        {
            while (contents.childCount > 0)
            {
                DestroyImmediate(contents.GetChild(0).gameObject);
            }
        }

        int slotNumber = PlayerCardManager.Instance().GetPlayerHaveCardDictionary().Count
                            + (5 - PlayerCardManager.Instance().GetPlayerHaveCardDictionary().Count % 5);

        cardSlotList = new List<CardSlot>(slotNumber);
        GameObject tmpSlot;
        for (int i = 0; i < slotNumber; i++)
        {
            tmpSlot = Resources.Load<GameObject>(cardSlotPath);
            tmpSlot = Instantiate(tmpSlot);

            if (contents != null)
                tmpSlot.transform.SetParent(contents, false);

            cardSlotList.Add(tmpSlot.GetComponent<CardSlot>());
        }
        // 여기까지는 Slot만 추가된다.

        RegisterItemToSlot(ref PlayerCardManager.Instance().GetPlayerHaveCardDictionary());
    }

    public void RegisterItemToSlot(ref Dictionary<int, A_PlayerCard> playerCards)
    {
        while (cardSlotList.Count < playerCards.Count)
            IncreaseCardSlotNumber(5);

        if (playerCards.Count > 0)
        {
            int itemListIndex = 0;
            foreach (KeyValuePair<int, A_PlayerCard> card in playerCards)
            {
                cardSlotList[itemListIndex].UpdateCardSlot(card);
                itemListIndex++;
            }
        }
    }

    public void UpdateDeckCardImageHolders()
    {
        //Debug.Log(deckImageHoldersList[2]);
        List<A_PlayerCard> deck = PlayerCardManager.Instance().GetPlayerDeckCardList();

        if (DeckCardImageHolder.Act_DisableDeckCardCostPanel != null)
            DeckCardImageHolder.Act_DisableDeckCardCostPanel.Invoke();

        int count = 0;
        for (; count < deck.Count; count++)
        {
            //Debug.Log("COUNT : " + count + " DECK_AMOUNT : " + deck.Count);
            //Debug.Log("DeckImageListCount : " + deckImagesList.Count);

            //Debug.Log($"deck.Count : {deck.Count}, count : {count}");

            if (deckImageHoldersList != null)
            {
                DeckCardImageHolder deckCardImageHolder = deckImageHoldersList[count].gameObject.GetComponent<DeckCardImageHolder>();

                Debug.Log($"deck[count] is null ? {deck[count] == null}");

                //deckImageHoldersList[count].sprite = Resources.Load<Sprite>(deck[count].cardImagePath);
                deckCardImageHolder.UpdateImage(Resources.Load<Sprite>(deck[count].cardImagePath));

                int cost = (int)deck[count].cardCost;
                deckCardImageHolder.EnableCostPanel();
                deckCardImageHolder.SetCostText(cost);
            }
            else
                Debug.Log($"deck[count] is null ? {deck[count] == null}");
        }

        if (count < PlayerCardManager.DECK_CARDS_COUNT && (deckImageHoldersList != null))
        {
            for (; count < PlayerCardManager.DECK_CARDS_COUNT; count++)
            {
                deckImageHoldersList[count].gameObject.GetComponent<DeckCardImageHolder>().UpdateImage(null);
            }
        }
    }
}
