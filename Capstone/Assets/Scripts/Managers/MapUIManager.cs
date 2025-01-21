using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapUIManager : A_UIManager
{
    private static MapUIManager instance;

    [Header("Panels")]
    [SerializeField] private GameObject deckPanel;
    [SerializeField] private GameObject itemsPanel;
    [SerializeField] private GameObject equipmentPanel;
    // 문제 발생 어차피 GPSManager에서 관리하니까 일단 주석처리
    //[SerializeField] private GameObject compusPanel;      
    [SerializeField] private GameObject statusPanelInMap;
    [SerializeField] private GameObject canclePanel;
    [SerializeField] private GameObject announcementPanel;
    [SerializeField] private GameObject miniGamesPanel;
    [SerializeField] private GameObject ObjectDescriptionPanel;
    [SerializeField] private TextMeshProUGUI playerLevelText;

    public TextMeshProUGUI LongitudeText;
    public TextMeshProUGUI LatitudeText;
    public GameObject enemyBattleCheckPanel;

    [Space(10), Header("Buttons")]
    [SerializeField] private GameObject optionButton;
    [SerializeField] private GameObject profileButton;
    [SerializeField] private GameObject itemsButton;
    [SerializeField] private GameObject deckButton;

    [Space(10), Header("Objects")]
    //[SerializeField] private List<GameObject> uiObjectsList;
    private List<GameObject> panelsList;
    private List<GameObject> buttonsList;

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public static MapUIManager Instance()
    {
        if (instance == null) return null;
        return instance;
    }

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        UIManager.Instance().UpdateCurrentManager(this);

        CanclePanel.OnCanclePanelClicked -= OnCanclePanel;  // CanclePanel의 Action과 연결함.
        CanclePanel.OnCanclePanelClicked += OnCanclePanel;

        deckPanel.SetActive(false);
        itemsPanel.SetActive(false);

        activeObject = new Stack<GameObject>();

        panelsList = new List<GameObject>();
        panelsList.Add(deckPanel);
        panelsList.Add(itemsPanel);
        panelsList.Add(equipmentPanel);
        //panelsList.Add(compusPanel);  // 이새끼한테서 문제 발생.
        panelsList.Add(statusPanelInMap);
        panelsList.Add(enemyBattleCheckPanel);
        panelsList.Add(announcementPanel);
        panelsList.Add(miniGamesPanel);

        buttonsList = new List<GameObject>();
        buttonsList.Add(optionButton);
        buttonsList.Add(itemsButton);
        buttonsList.Add(deckButton);
        buttonsList.Add(profileButton);

        DisablePanelsWithout(statusPanelInMap);

        UpdatePlayerLevelText();
    }

    private void OnDisable()
    {
        CanclePanel.OnCanclePanelClicked -= OnCanclePanel;
    }

    public void SwitchPanels()
    {
        foreach (GameObject panel in panelsList)
        {
            //Debug.Log(panel.name);
            panel.SetActive(!(panel.activeSelf));
        }
    }

    private void DisablePanelsWithout(GameObject what)
    {
        foreach (GameObject panel in panelsList)
        {
            if (panel == what)
                continue;
            panel.SetActive(false);
        }
    }

    public void DisablePanels()
    {
        foreach (GameObject panel in panelsList)
            panel.SetActive(false);
    }

    public void DisableButtons()
    {
        foreach (GameObject button in buttonsList)
            button.SetActive(false);
    }

    //private void SwitchToMapSceneUI()
    //{
    //    DisablePanels();
    //    DisableButtons();

    //    compusPanel.SetActive(true);
    //    statusPanelInMap.SetActive(true);

    //    optionButton.SetActive(true);
    //    profileButton.SetActive(true);
    //    itemsButton.SetActive(true);
    //    deckButton.SetActive(true);
    //}

    //private void SwitchToBattleSceneUI()
    //{
    //    DisablePanels();
    //    DisableButtons();

    //    statusPanelInBattle.SetActive(true);
    //    optionButton.SetActive(true);
    //    profileButton.SetActive(true);
    //    currentCardsInBattlePanel.SetActive(true);

    //    //currentCardsInBattlePanel.GetComponent<CurrentCardsInBattlePanel>().SetPlayerCardBases();
    //}

    //public void SwitchUITo(SceneManagerEX.Scenes sceneToSwitch)
    //{
    //    switch (sceneToSwitch)
    //    {
    //        case SceneManagerEX.Scenes.IntroScene:
    //            break;
    //        case SceneManagerEX.Scenes.MapScene:
    //            SwitchToMapSceneUI();
    //            break;
    //        case SceneManagerEX.Scenes.BattleScene:
    //            SwitchToBattleSceneUI();
    //            break;
    //    }
    //}

    public override void ActivateCanclePanel()
    {
        PlayerMovement.OnMakePlayerCantMove.Invoke();

        canclePanel.SetActive(true);
    }

    public override void OnCanclePanel()
    {
        if (canclePanel != null)
            canclePanel.SetActive(false);

        if (activeObject.Count > 0)
        {
            GameObject currentObject = activeObject.Pop();
            currentObject.SetActive(false);
        }
        else
            Debug.Log("activeObject Stack is empty");

        if (activeObject.Count <= 0)
        {
            // Time.timeScale = 1f;

            //Player player = Player.Instance();
            //PlayerCameraController cameraController = player.gameObject.GetComponent<PlayerCameraController>();
            //cameraController.ActivateCamera();

             CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
             cinemachineBrain.enabled = true;

            if (SceneManagerEX.CurrentScene() == SceneManagerEX.Scenes.MapScene)
                PlayerMovement.OnMakePlayerCanMove.Invoke();
        }
    }

    public void OnTestProfileClick()
    {
        SceneManagerEX.Instance().SwitchToMapScene();
    }

    //private void WaitForAnnouncementPanel()
    //{
    //    StartCoroutine("WaitForAnnouncementPanelCoroutine");
    //}

    //IEnumerator WaitForAnnouncementPanelCoroutine()
    //{
    //    while (!AnnouncementPanel.isInitialized)
    //        yield return null;
    //}

    public void OnEnemyBattleCheckPanelOkClick()
    {
        int deckCount = PlayerCardManager.Instance().GetPlayerDeckCardList().Count;
        if (deckCount < 3)
        {
            //WaitForAnnouncementPanel();

            string title = "Insufficient Number of Cards";
            string content = "You must have at least three cards in your deck.";

            //AnnouncementPanel.Act_SetAnnoucementPanelText.Invoke(title, content);
            //AnnouncementPanel.Act_EnableAnnouncementPanel.Invoke();

            AnnouncementPanel panel = announcementPanel.GetComponent<AnnouncementPanel>();
            panel.SetAnnouncementText(title, content);
            panel.EnablePanel();
        }
        else
        {
            SceneManagerEX.Instance().SwitchToBattleScene();
        }
    }

    public void ActivateEnemyBattleCheckPanel()
    {
        enemyBattleCheckPanel.SetActive(true);
    }

    public void ActivateObjectDescriptionPanel()
    {
        ObjectDescriptionPanel.SetActive(true);
    }

    public void OnItemUseButtonClick()
    {
        PlayerItemsManager.Instance().UseCurrentPlayerItem();
        PlayerStatusText.Act_UpdatePlayerStatusValueText.Invoke();
    }

    public void OnEquipButtonClick()
    {
        // 이 순서를 지켜야 함.
        // 실질적으로 현재 들고있는 장비에 등록한 후
        // 현재 들고있는 장비에 따라 이미지가 업데이트 되기 때문
        PlayerEquipmentManager.Instance().EquipCurrentEquipment();
        PlayerEquipmentManager.EquipEquipment.Invoke();
        PlayerStatusTextInEquipment.Act_UpdatePlayerStatusTextInEquipment.Invoke();
    }

    public void OnUnEquipButtonClick()
    {
        // 이 순서를 지켜야 함.
        PlayerEquipmentManager.Instance().UnEquipCurrentEquipment();
        PlayerEquipmentManager.EquipEquipment.Invoke();
    }

    public void OnCardUseButtonClick()
    {
        if (!PlayerCardManager.Instance().IsPlayersDeckEmpty())
        {
            Debug.Log("Theres No Empty Slot!! Unuse Some Cards");
            return;
        }

        List<A_PlayerCard> deck = PlayerCardManager.Instance().GetPlayerDeckCardList();
        foreach(A_PlayerCard card in deck)
        {
            if (card == PlayerCardManager.Instance().GetCurrentSelectedCard())
            {
                Debug.Log("Is Already In Deck!!");
                return;
            }
        }

        deck.Add(PlayerCardManager.Instance().GetCurrentSelectedCard());

        DeckPanel.Act_UpdateDeckImages.Invoke();
    }

    public void OnCardUnuseButtonClick()
    {
        DeckCardImageHolder.Act_Unselect.Invoke();

        //Debug.Log("AAAA");

        PlayerCardManager playerCardManager = PlayerCardManager.Instance();
        int currOrder = playerCardManager.GetCurrentSelectedDeckCardOrder();
        //Debug.Log(currOrder);

        if (currOrder < 0)
            return;

        playerCardManager.RemoveCardInDeckAt(currOrder);
        playerCardManager.SetCurrentSelectedDeckCardOrder(-1);
        DeckCardImageHolder.DisableDeckCardImagesOutline.Invoke();

        DeckPanel.Act_UpdateDeckImages.Invoke();
    }

    public void UpdatePlayerLevelText()
    {
        int currentPlayerLevel = PlayerSpecManager.Instance().currentPlayerLevel;
        string str = String.Format("Lv.{0}", currentPlayerLevel);

        playerLevelText.text = str;
    }
}
