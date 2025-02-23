using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour, IPanel
{
    // equipPanel에는 캐릭터의 모습과 장착할(한) 장비를 나타낼 수 있도록 함.
    // statusPanel에는 현재 스펙을 나타낼 수 있도록 함.

    public static Action EnableItemButtons;
    public static Action DisableItemButtons;
    public static Action<ItemSlot> SelectCurrentItemSlot;
    public static Action<Dictionary<int, A_Item>> UpdateItemPanel;

    [SerializeField] private float padding = 30;
    [SerializeField] private GameObject EquipAndStatusPanel;
    [SerializeField] private GameObject AllItemPanel;
    [SerializeField] private Transform contents;

    [Space(10), Header("For Panels and Grids")]
    [SerializeField] private float gap;
    [SerializeField] private float itemsGridSpacing;
    [SerializeField] private float itemsGridHorizontalPadding;
    [SerializeField] private float itemsGridVerticalPadding;
    [SerializeField] private float equipAndStatusHorizontalPadding;
    [SerializeField] private float equipAndStatusVerticalPadding;

    [Space(10), Header("About Items")]
    [SerializeField] private string itemSlotPath;
    [SerializeField] private Button descriptionButton;
    [SerializeField] private Button useButton;

    private GridLayoutGroup gridGroup;
    private GridLayoutGroup allItemsGridGroup;

    private MapUIManager uiManager;

    private IPanel panelInterface;

    private List<ItemSlot> itemSlotList;

    private void OnEnable()
    {
        EnableItemButtons -= EnableButtons;
        EnableItemButtons += EnableButtons;

        DisableItemButtons -= DisableButtons;
        DisableItemButtons += DisableButtons;

        ItemSlot.UsedAllItems -= DisableButtons;
        ItemSlot.UsedAllItems += DisableButtons;

        UpdateItemPanel -= RegisterItemToSlot;
        UpdateItemPanel += RegisterItemToSlot;

        InitializeItemSlots();
    }

    void Start()
    {
        gridGroup = EquipAndStatusPanel.GetComponent<GridLayoutGroup>();
        allItemsGridGroup = AllItemPanel.GetComponentInChildren<GridLayoutGroup>();

        panelInterface = GetComponent<IPanel>();

        uiManager = MapUIManager.Instance();

        panelInterface.Initialize();

        InitializeItemSlots();
        DisableButtons();
    }

    private void Update()
    {
        panelInterface.Initialize();
    }

    private void OnDisable()
    {
        DisableButtons();

        EnableItemButtons -= EnableButtons;
        DisableItemButtons -= DisableButtons;
        ItemSlot.UsedAllItems -= DisableButtons;
        UpdateItemPanel -= RegisterItemToSlot;
    }

    void IPanel.Initialize()
    {
        PanelSetting();

        RectOffset gridPadding = gridGroup.padding;
        gridPadding.left = gridPadding.right = gridPadding.top = gridPadding.bottom = (int)padding;

        // 지금은 EquipAndStatusPanel로 보기는 힘듦.
        // Equip Panel을 만들기 전에 만든 코드이기 때문
        float width = EquipAndStatusPanel.GetComponent<RectTransform>().rect.width - 2 * equipAndStatusHorizontalPadding;   //좌, 우, 중간의 패딩을 제외한 너비
        float height = EquipAndStatusPanel.GetComponent<RectTransform>().rect.height - 2 * equipAndStatusVerticalPadding; // 상 하, 중간의 패딩을 제외한 높이
        
        gridGroup.cellSize = new Vector2(width, height);
        gridGroup.spacing = new Vector2(padding, padding);

        gridPadding = allItemsGridGroup.padding;
        gridPadding.left = (int)itemsGridHorizontalPadding; 
        gridPadding.right = (int)itemsGridHorizontalPadding; 
        gridPadding.top = (int)itemsGridVerticalPadding; 
        gridPadding.bottom = (int)itemsGridVerticalPadding;

        width = (AllItemPanel.GetComponent<RectTransform>().rect.width - 6 * itemsGridHorizontalPadding) / 5;       
        allItemsGridGroup.cellSize = new Vector2(width, width);        
    }

    private void PanelSetting()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float width = rectTransform.rect.width - padding * 2;    // 60은 위, 아래 30, 좌, 우 30의 패딩.
        float height = rectTransform.rect.height - padding * 3;  // 맨 위, 중간, 맨 아래의 패딩.
        //Debug.Log(string.Format("width : {0}, height : {1}", width, height));

        RectTransform equipAndStatusRect = EquipAndStatusPanel.GetComponent<RectTransform>();
        RectTransform itemRect = AllItemPanel.GetComponent<RectTransform>();

        float equipAndStatusHeight = height * 0.3f;
        float itemPanelHeight = height * 0.7f;
        //Debug.Log(string.Format("equipHeight : {0}, statusHeight : {1}", equipHeight, statusHeight));

        // RectTransform의 left, right, top, bottom을 수정할 수 있는듯.
        // 오브젝트의 rectTransform에 해당하는 left, right, top, bottom이 활성화돼있는지 확인 필요.
        // 만약 오브젝트의 recTransform에 left나 right 등이 없으면 사용 불가.
        // width, height일 경우, sizeDelta를 사용해야 함.
        //equipAndStatusRect.sizeDelta = new Vector2(width, height);
        equipAndStatusRect.offsetMax = new Vector2(-padding, -padding);                       // offsetMax : 최상의 최우의 위치를 기준으로 한 상대적 위치(right, top)
        equipAndStatusRect.offsetMin = new Vector2(padding, padding * 1 + itemPanelHeight);       // offsetMin : 최하의 최좌의 위치를 기준으로 한 상대적 위치(left, bottom)

        itemRect.offsetMax = new Vector2(-padding, -padding * 1 - equipAndStatusHeight);
        itemRect.offsetMin = new Vector2(padding, padding);
    }

    public void OnClicked()
    {
        //Debug.Log("Clicked Items");

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

            // Time.timeScale = 0;

            //Player player = Player.Instance();
            //PlayerCameraController cameraController = player.gameObject.GetComponent<PlayerCameraController>();
            //cameraController.InActivateAllCamera();

            CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            cinemachineBrain.enabled = false;
        }
    }

    public void EnableButtons()
    {
        //descriptionButton.gameObject.SetActive(true);
        useButton.gameObject.SetActive(true);
    }

    public void DisableButtons()
    {
        //descriptionButton.gameObject.SetActive(false);
        useButton.gameObject.SetActive(false);
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

    //public void AddItemToContents(A_Item item)
    //{
    //    if (item == null)
    //    {
    //        Debug.Log("Item is NULL");
    //        return;
    //    }

    //    item.transform.parent = contents;
    //}

    public void IncreaseItemSlotNumber(int n)
    {
        GameObject tmpSlot;
        for (int i = 0; i < n; i++)
        {
            tmpSlot = Resources.Load<GameObject>(itemSlotPath);
            tmpSlot = Instantiate(tmpSlot);

            tmpSlot.transform.SetParent(contents, false);

            itemSlotList.Add(tmpSlot.GetComponent<ItemSlot>());
        }
    }

    // 초기에 ItemSlot들을 생성하는 함수.
    // Slot에 Item을 등록하지는 않음!!
    public void InitializeItemSlots()
    {
        while(contents.childCount > 0)
        {
            DestroyImmediate(contents.GetChild(0).gameObject);            
        }
            
        int slotNumber = PlayerItemsManager.Instance().GetPlayerItemDictionary().Count
                            + (5 - PlayerItemsManager.Instance().GetPlayerItemDictionary().Count % 5);

        itemSlotList = new List<ItemSlot>(slotNumber);
        GameObject tmpSlot;
        for (int i = 0; i < slotNumber; i++)
        {
            tmpSlot = Resources.Load<GameObject>(itemSlotPath);
            tmpSlot = Instantiate(tmpSlot);

            tmpSlot.transform.SetParent(contents, false);

            itemSlotList.Add(tmpSlot.GetComponent<ItemSlot>());
        }
        // 여기까지는 Slot만 추가된다.

        // List를 사용한 버전
        //RegisterItemToSlot(PlayerSpecManager.Instance().playerItemList);

        // 이제 여기서 실질적으로 Slot에 Item들을 등록하게된다
        // 원래 ref였는ㄷ 그냥으로 바꿔봄.
        RegisterItemToSlot(PlayerItemsManager.Instance().GetPlayerItemDictionary());
    }

    // Slot에 Item(List)를 등록하는 함수.
    // List로 사용한 버전
    //public void RegisterItemToSlot(List<A_Item> itemList = null)
    //{
    //    while (itemSlotList.Count < itemList.Count)
    //        IncreaseItemSlotNumber(5);

    //    for (int i = 0; i < itemList.Count; i++)
    //    {
    //        itemSlotList[i].UpdateItemSlot(itemList[i]);
    //    }
    //}

    // Dictionary로 사용한 버전
    // 나중에 정렬도 추가하면 일정 기준으로 정렬 할 수도 있을듯 하다.
    // 원래 ref였는데 한 번 그냥으로 바꿔봄.
    public void RegisterItemToSlot(Dictionary<int, A_Item> itemList)
    {
        while (itemSlotList.Count < itemList.Count)
            IncreaseItemSlotNumber(5);

        int itemListIndex = 0;
        if (itemList.Count > 0)
        {
            foreach (KeyValuePair<int, A_Item> item in itemList)
            {
                itemSlotList[itemListIndex].UpdateItemSlot(item);
                itemListIndex++;
            }
        }

        if (itemListIndex < itemSlotList.Count)
        {
            for (;itemListIndex < itemSlotList.Count; itemListIndex++)
            {
                itemSlotList[itemListIndex].MakeEmpty();
            }
        }

        //for (int i = 0; i < itemList.Count; i++)
        //{
        //    itemSlotList[i].UpdateItemSlot(itemList[i]);
        //}
    }
}
