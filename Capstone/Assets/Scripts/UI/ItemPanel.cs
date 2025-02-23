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
    // equipPanel���� ĳ������ ����� ������(��) ��� ��Ÿ�� �� �ֵ��� ��.
    // statusPanel���� ���� ������ ��Ÿ�� �� �ֵ��� ��.

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

        // ������ EquipAndStatusPanel�� ����� ����.
        // Equip Panel�� ����� ���� ���� �ڵ��̱� ����
        float width = EquipAndStatusPanel.GetComponent<RectTransform>().rect.width - 2 * equipAndStatusHorizontalPadding;   //��, ��, �߰��� �е��� ������ �ʺ�
        float height = EquipAndStatusPanel.GetComponent<RectTransform>().rect.height - 2 * equipAndStatusVerticalPadding; // �� ��, �߰��� �е��� ������ ����
        
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
        float width = rectTransform.rect.width - padding * 2;    // 60�� ��, �Ʒ� 30, ��, �� 30�� �е�.
        float height = rectTransform.rect.height - padding * 3;  // �� ��, �߰�, �� �Ʒ��� �е�.
        //Debug.Log(string.Format("width : {0}, height : {1}", width, height));

        RectTransform equipAndStatusRect = EquipAndStatusPanel.GetComponent<RectTransform>();
        RectTransform itemRect = AllItemPanel.GetComponent<RectTransform>();

        float equipAndStatusHeight = height * 0.3f;
        float itemPanelHeight = height * 0.7f;
        //Debug.Log(string.Format("equipHeight : {0}, statusHeight : {1}", equipHeight, statusHeight));

        // RectTransform�� left, right, top, bottom�� ������ �� �ִµ�.
        // ������Ʈ�� rectTransform�� �ش��ϴ� left, right, top, bottom�� Ȱ��ȭ���ִ��� Ȯ�� �ʿ�.
        // ���� ������Ʈ�� recTransform�� left�� right ���� ������ ��� �Ұ�.
        // width, height�� ���, sizeDelta�� ����ؾ� ��.
        //equipAndStatusRect.sizeDelta = new Vector2(width, height);
        equipAndStatusRect.offsetMax = new Vector2(-padding, -padding);                       // offsetMax : �ֻ��� �ֿ��� ��ġ�� �������� �� ����� ��ġ(right, top)
        equipAndStatusRect.offsetMin = new Vector2(padding, padding * 1 + itemPanelHeight);       // offsetMin : ������ ������ ��ġ�� �������� �� ����� ��ġ(left, bottom)

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

    // �ʱ⿡ ItemSlot���� �����ϴ� �Լ�.
    // Slot�� Item�� ��������� ����!!
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
        // ��������� Slot�� �߰��ȴ�.

        // List�� ����� ����
        //RegisterItemToSlot(PlayerSpecManager.Instance().playerItemList);

        // ���� ���⼭ ���������� Slot�� Item���� ����ϰԵȴ�
        // ���� ref���¤� �׳����� �ٲ㺽.
        RegisterItemToSlot(PlayerItemsManager.Instance().GetPlayerItemDictionary());
    }

    // Slot�� Item(List)�� ����ϴ� �Լ�.
    // List�� ����� ����
    //public void RegisterItemToSlot(List<A_Item> itemList = null)
    //{
    //    while (itemSlotList.Count < itemList.Count)
    //        IncreaseItemSlotNumber(5);

    //    for (int i = 0; i < itemList.Count; i++)
    //    {
    //        itemSlotList[i].UpdateItemSlot(itemList[i]);
    //    }
    //}

    // Dictionary�� ����� ����
    // ���߿� ���ĵ� �߰��ϸ� ���� �������� ���� �� ���� ������ �ϴ�.
    // ���� ref���µ� �� �� �׳����� �ٲ㺽.
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
