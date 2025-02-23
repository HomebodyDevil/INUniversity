using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : A_UIManager
{
    public static int count = 0;

    private static BattleUIManager instance;

    [Header("Panels")]
    [SerializeField] private GameObject itemsPanel;
    [SerializeField] private GameObject statusPanelInBattle;
    [SerializeField] private GameObject canclePanel;
    [SerializeField] private GameObject currentCardsInBattlePanel;
    [SerializeField] private GameObject battleCompleteObjectPanel;
    [SerializeField] private GameObject preservePanel;

    [Space(10), Header("Buttons")]
    [SerializeField] private GameObject optionButton;
    [SerializeField] private GameObject profileButton;
    [SerializeField] private GameObject itemsButton;

    [Space(10), Header("Contents")]
    [SerializeField] private Transform itemsContent;
    [SerializeField] private Transform cardsContent;
    [SerializeField, TextArea] private string acquisitionSlotPath;

    private List<GameObject> panelsList;
    private List<GameObject> buttonsList;

    [Space(10), Header("EnemyUIs")]
    [SerializeField] private Slider enemyHPSlider;
    [SerializeField] private Slider enemyCostSlider;

    [HideInInspector] public A_Item currentSelectedItem;
    [HideInInspector] public A_Equipment currentSelectedEquipment;
    [HideInInspector] public A_PlayerCard currentSelectedCard;

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

    public static BattleUIManager Instance()
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
        //Debug.Log(string.Format("BattleUIManager count : {0}", BattleUIManager.count++));

        UIManager.Instance().UpdateCurrentManager(this);

        CanclePanel.OnCanclePanelClicked -= OnCanclePanel;  // CanclePanel의 Action과 연결함.
        CanclePanel.OnCanclePanelClicked += OnCanclePanel;        

        BattleManager.OnBattleWin -= ActivateBattleWinObject;
        BattleManager.OnBattleWin += ActivateBattleWinObject;

        BattleManager.OnBattleWin -= ActivatePreservePanel;
        BattleManager.OnBattleWin += ActivatePreservePanel;

        BattleManager.OnBattleLose -= ActivateBattleLoseObject;
        BattleManager.OnBattleLose += ActivateBattleLoseObject;

        BattleManager.OnBattleLose -= ActivatePreservePanel;
        BattleManager.OnBattleLose += ActivatePreservePanel;

        itemsPanel.SetActive(false);
        battleCompleteObjectPanel.SetActive(false);
        preservePanel.SetActive(false);

        activeObject = new Stack<GameObject>();
        panelsList = new List<GameObject>();
        buttonsList = new List<GameObject>();

        panelsList.Add(itemsPanel);
        panelsList.Add(statusPanelInBattle);
        panelsList.Add(currentCardsInBattlePanel);
        panelsList.Add(battleCompleteObjectPanel);

        buttonsList.Add(optionButton);
        buttonsList.Add(itemsButton);
        buttonsList.Add(profileButton);

        BattleManager.OnStartBattle.Invoke(true);
        PlayerCurrentCardHolder.Act_UpdateHandCardsImages.Invoke();
    }

    private void OnDisable()
    {
        CanclePanel.OnCanclePanelClicked -= OnCanclePanel;
        BattleManager.OnBattleWin -= ActivateBattleWinObject;
        BattleManager.OnBattleLose -= ActivateBattleLoseObject;
        BattleManager.OnBattleWin -= ActivatePreservePanel;
        BattleManager.OnBattleLose -= ActivatePreservePanel;

        PlayerSpecManager.Instance().StopIncreaseCost();
        BattleManager.Instance().StopIncreaseEnemyCost();
    }

    public void EnablePanels()
    {
        foreach (GameObject panel in panelsList)
            panel.SetActive(true);
    }

    public void EnableButtons()
    {
        foreach (GameObject button in buttonsList)
            button.SetActive(true);
    }

    public void DisablePanels()
    {
        foreach (GameObject panel in panelsList)
        {
            Debug.Log(panel.name);
            panel.SetActive(false);
        }
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

    //    optionButton.SetActive(true);
    //    profileButton.SetActive(true);
    //    itemsButton.SetActive(true);
    //}

    //private void SwitchToBattleSceneUI()
    //{
    //    DisablePanels();
    //    DisableButtons();

    //    statusPanelInBattle.SetActive(true);
    //    optionButton.SetActive(true);
    //    profileButton.SetActive(true);

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
        BattleManager.OnPauseBattle.Invoke();

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
        {
            //Debug.Log("activeObject Stack is empty");
        }

        if (activeObject.Count <= 0)
        {
            CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            cinemachineBrain.enabled = true;

            BattleManager.OnStartBattle.Invoke(false);
        }
    }

    public void SwitchSceneToMap()
    {
        SceneManagerEX.Instance().SwitchToMapScene();
    }

    public void OnItemUseButtonClick()
    {
        PlayerItemsManager.Instance().UseCurrentPlayerItem();
        PlayerStatusText.Act_UpdatePlayerStatusValueText.Invoke();
    }

    private void ActivateBattleWinObject()
    {
        //Debug.Log("ActivateBattleWinObject");
        //DisableThingsForBattleWinObject();

        //ActivateCanclePanel();
        BattleManager.OnPauseBattle.Invoke();
        UpdateAcquiositionSlot();
        battleCompleteObjectPanel.GetComponent<BattleCompletePanel>().SetComponents(true);
        battleCompleteObjectPanel.SetActive(true);
    }

    private void ActivateBattleLoseObject()
    {
        //ActivateCanclePanel();

        BattleManager.OnPauseBattle.Invoke();
        battleCompleteObjectPanel.GetComponent<BattleCompletePanel>().SetComponents(false);
        battleCompleteObjectPanel.SetActive(true);
    }

    //public void AddDropsToPlayer()
    //{
    //    List<A_Item> itemList = BattleManager.Instance().GetDropItemsList();
    //    List<A_Equipment> equipmentList = BattleManager.Instance().GetDropEquipmentList();
    //    List<A_PlayerCard> cardList = BattleManager.Instance().GetDropCardsList();

    //    for (int i = 0; i < itemList.Count; i++)
    //    {

    //    }

    //    for (int i = 0; i < equipmentList.Count; i++)
    //    {

    //    }

    //    for (int i = 0; i < cardList.Count; i++)
    //    {

    //    }
    //}

    public void UpdateAcquiositionSlot()
    {
        Dictionary<A_Item, int> itemsDictionary = BattleManager.Instance().GetDropItemsDictionary() ;
        List<A_Equipment> equipmentList = BattleManager.Instance().GetDropEquipmentList();
        List<A_PlayerCard> cardList = BattleManager.Instance().GetDropCardsList();

        SetItemsAndEquipmentSlots(itemsDictionary, equipmentList);
        SetCardSlots(cardList);
    }

    private void SetItemsAndEquipmentSlots(Dictionary<A_Item, int> itemDictionary, List<A_Equipment> equipmentList)
    {
        int index = 0;
        while (itemsContent.childCount > 0)
        {
            DestroyImmediate(itemsContent.GetChild(index).gameObject);
            if (index > 1000)
            {
                Debug.Log("Infinite Loop Detected");
                break;
            }
        }

        GameObject tmpSlot;
        if (itemDictionary != null)
        {
            //Debug.Log("ASDF : " + itemDictionary.Count);
            foreach(KeyValuePair<A_Item, int> item in itemDictionary)
            {
                tmpSlot = Resources.Load<GameObject>(acquisitionSlotPath);
                tmpSlot = Instantiate(tmpSlot);
                tmpSlot.transform.SetParent(itemsContent, false);                

                BattleCompleteAcquisitionSlot slotScript = tmpSlot.GetComponent<BattleCompleteAcquisitionSlot>();

                slotScript.EnableText();
                slotScript.SetCountText(item.Value);
                slotScript.SetType(BattleCompleteAcquisitionSlot.Type.Item);
                slotScript.SetItem(item.Key);
                tmpSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>(item.Key.itemImagePath);
            }

            //for (int i = 0; i < itemDictionary.Count; i++)
            //{
            //    tmpSlot = Resources.Load<GameObject>(acquisitionSlotPath);
            //    tmpSlot = Instantiate(tmpSlot);
            //    tmpSlot.transform.SetParent(itemsContent, false);

            //    BattleCompleteAcquisitionSlot slotScript = tmpSlot.GetComponent<BattleCompleteAcquisitionSlot>();
            //    slotScript.SetType(BattleCompleteAcquisitionSlot.Type.Item);
            //    slotScript.SetItem(itemDictionary[i].Key);
            //    tmpSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>(itemDictionary[i].Key.itemImagePath);
            //}
        }
        else
            Debug.Log("ItemList is Null");

        if (equipmentList != null)
        {
            for (int i = 0; i < equipmentList.Count; i++)
            {
                tmpSlot = Resources.Load<GameObject>(acquisitionSlotPath);
                tmpSlot = Instantiate(tmpSlot);
                tmpSlot.transform.SetParent(itemsContent, false);

                BattleCompleteAcquisitionSlot slotScript = tmpSlot.GetComponent<BattleCompleteAcquisitionSlot>();
                slotScript.DisableText();                
                slotScript.SetType(BattleCompleteAcquisitionSlot.Type.Equipment);
                slotScript.SetEquipment(equipmentList[i]);
                tmpSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>(equipmentList[i].equipmentImagePath);
            }
        }
        else
            Debug.Log("EquipmentList is Null");
    }

    private void SetCardSlots(List<A_PlayerCard> cardList)
    {
        int index = 0;
        while (cardsContent.childCount > 0)
        {
            DestroyImmediate(cardsContent.GetChild(index).gameObject);
            if (index > 1000)
            {
                Debug.Log("Infinite Loop Detected");
                break;
            }
        }

        GameObject tmpSlot;
        if (cardList != null)
        {
            //Debug.Log("ASDF : " + cardList.Count);
            for (int i = 0; i < cardList.Count; i++)
            {
                tmpSlot = Resources.Load<GameObject>(acquisitionSlotPath);
                tmpSlot = Instantiate(tmpSlot);
                tmpSlot.transform.SetParent(cardsContent, false);

                BattleCompleteAcquisitionSlot slotScript = tmpSlot.GetComponent<BattleCompleteAcquisitionSlot>();
                slotScript.DisableText();
                slotScript.SetType(BattleCompleteAcquisitionSlot.Type.Card);
                slotScript.SetCard(cardList[i]);
                tmpSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>(cardList[i].cardImagePath);
            }
        }
        else
            Debug.Log("CardList is Null");
    }

    private void ActivatePreservePanel()
    {
        //Debug.Log("ActivatePreservePanel");

        preservePanel.SetActive(true);
    }

    //private void DisableThingsForBattleWinObject()
    //{
    //    DisablePanels();
    //    itemsButton.SetActive(false);        
    //}
}
