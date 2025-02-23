using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanel : MonoBehaviour, IPanel
{
    private const int EQUIPMENT_IMAGE_COLUMN_COUNT = 2;
    private const int EQUIPMENT_IMAGE_RAW_COUNT = 2;

    public static Action EnableEquipmentButtons;
    public static Action DisableEquipmentButtons;
    public static Action<EquipSlot> SelectCurrentEquipmentSlot;
    public static Action UpdateEquipmentSlots;

    [SerializeField] private float padding = 40;
    [SerializeField] private float gridGroupPadding = 0;
    [SerializeField] private float allEquipmentPanelPadding = 0;
    [SerializeField] private GameObject EquipAndStatusPanel;
    [SerializeField] private GameObject AllEquipmentPanel;
    [SerializeField] private Transform contents;

    [Space(10), Header("About Equipment")]
    [SerializeField] private string equipSlotPath;
    [SerializeField] private Button descriptionButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;
    [SerializeField] private GridLayoutGroup equipmentPanelGridLayout;

    private GridLayoutGroup gridGroup;
    private GridLayoutGroup allEquipmentGridGroup;

    private MapUIManager uiManager;

    private IPanel panelInterface;

    private List<EquipSlot> equipmentSlotList;    

    private void OnEnable()
    {
        EnableEquipmentButtons -= EnableButtons;
        EnableEquipmentButtons += EnableButtons;

        DisableEquipmentButtons -= DisableButtons;
        DisableEquipmentButtons += DisableButtons;

        PlayerEquipmentManager.MadeEquipmentDictionary -= InitializeEquipmentSlots;
        PlayerEquipmentManager.MadeEquipmentDictionary += InitializeEquipmentSlots;

        UpdateEquipmentSlots -= InitializeEquipmentSlots;
        UpdateEquipmentSlots += InitializeEquipmentSlots;
    }

    private void Update()
    {
        panelInterface.Initialize();
    }

    void Start()
    {
        gridGroup = EquipAndStatusPanel.GetComponent<GridLayoutGroup>();
        allEquipmentGridGroup = AllEquipmentPanel.GetComponentInChildren<GridLayoutGroup>();
        // allEquipmentGridGroup = contents.GetComponent<GridLayoutGroup>();

        panelInterface = GetComponent<IPanel>();

        uiManager = MapUIManager.Instance();

        panelInterface.Initialize();

        // Panel�� Enable���·� ����� ������ ���� �ʱ� ���� �ߴ� ���.,
        // �ٵ� Panel�� Disable���·� �����ϸ� ������ �߻����� ����...
        //if (PlayerEquipmentManager.isInitialized)
        //    InitializeEquipmentSlots();

        InitializeEquipmentSlots();
        DisableButtons();
    }

    private void OnDisable()
    {
        DisableButtons();

        EnableEquipmentButtons -= EnableButtons;
        DisableEquipmentButtons -= DisableButtons;        
        PlayerEquipmentManager.MadeEquipmentDictionary -= InitializeEquipmentSlots;
        UpdateEquipmentSlots -= InitializeEquipmentSlots;
    }

    void IPanel.Initialize()
    {
        PanelSetting();

        // Grid�� ����.
        RectOffset gridPadding = gridGroup.padding;
        gridPadding.left = (int)gridGroupPadding;
        gridPadding.right = (int)gridGroupPadding;
        gridPadding.top = (int)gridGroupPadding;
        gridPadding.bottom = (int)gridGroupPadding;

        float width = EquipAndStatusPanel.GetComponent<RectTransform>().rect.width - 3 * gridGroupPadding;   //��, ��, �߰��� �е��� ������ �ʺ�
        float height = EquipAndStatusPanel.GetComponent<RectTransform>().rect.height - 2 * gridGroupPadding; // �� ��, �߰��� �е��� ������ ����

        gridGroup.cellSize = new Vector2(width / 2, height);    // ���� �������� ���, Status Panel�� ���� ����.
        gridGroup.spacing = new Vector2(gridGroupPadding, 0);


        // AllEquipment �ĳ� ����
        gridPadding = allEquipmentGridGroup.padding;        
        gridPadding.left = gridPadding.right = gridPadding.top = gridPadding.bottom = (int)allEquipmentPanelPadding;
        
        width = (AllEquipmentPanel.GetComponent<RectTransform>().rect.width - 6 * allEquipmentPanelPadding) / 5;        
        allEquipmentGridGroup.cellSize = new Vector2(width, width);
        
        // Using �ĳ� ����.
        // padding�� 20�̶�� ����.
        width = (gridGroup.cellSize.x - (EQUIPMENT_IMAGE_COLUMN_COUNT + 1) * 20) / EQUIPMENT_IMAGE_COLUMN_COUNT;  // EquipmentPanel ���� ��� ���� ��Ȳ �̹����� ũ�⸦ ������ ����.
        height = (gridGroup.cellSize.y - (EQUIPMENT_IMAGE_RAW_COUNT + 1) * 20) / EQUIPMENT_IMAGE_RAW_COUNT;
        width = Mathf.Min(width, height);       // width, height�� �� ���� ������ Size�� �ǵ��� ����. 

        equipmentPanelGridLayout.cellSize = new Vector2(width, width);  
    }

    private void PanelSetting()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float width = rectTransform.rect.width - padding * 2;    // 60�� ��, �Ʒ� 30, ��, �� 30�� �е�.
        float height = rectTransform.rect.height - padding * 3;  // �� ��, �߰�, �� �Ʒ��� �е�.
        //Debug.Log(string.Format("width : {0}, height : {1}", width, height));

        RectTransform equipAndStatusRect = EquipAndStatusPanel.GetComponent<RectTransform>();
        RectTransform equipmentRect = AllEquipmentPanel.GetComponent<RectTransform>();

        float equipAndStatusHeight = height * 0.3f;
        float equipmentPanelHeight = height * 0.7f;
        //Debug.Log(string.Format("equipHeight : {0}, statusHeight : {1}", equipHeight, statusHeight));

        // RectTransform�� left, right, top, bottom�� ������ �� �ִµ�.
        // ������Ʈ�� rectTransform�� �ش��ϴ� left, right, top, bottom�� Ȱ��ȭ���ִ��� Ȯ�� �ʿ�.
        // ���� ������Ʈ�� recTransform�� left�� right ���� ������ ��� �Ұ�.
        // width, height�� ���, sizeDelta�� ����ؾ� ��.
        //equipAndStatusRect.sizeDelta = new Vector2(width, height);
        equipAndStatusRect.offsetMax = new Vector2(-padding, -padding);                       // offsetMax : �ֻ��� �ֿ��� ��ġ�� �������� �� ����� ��ġ(right, top)
        equipAndStatusRect.offsetMin = new Vector2(padding, padding * 2 + equipmentPanelHeight);       // offsetMin : ������ ������ ��ġ�� �������� �� ����� ��ġ(left, bottom)

        equipmentRect.offsetMax = new Vector2(-padding, -padding * 2 - equipAndStatusHeight);
        equipmentRect.offsetMin = new Vector2(padding, padding);
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
        equipButton.gameObject.SetActive(true);
    }

    public void DisableButtons()
    {
        descriptionButton.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);
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

    //private void WaitMakeEquipmentDictionary()
    //{
    //    StartCoroutine("WaitMakeEquipmentDictionaryCoroutine");
    //}

    //private void StopWaitMakeEquipmentDictionary()
    //{
    //    StopCoroutine("WaitMakeEquipmentDictionaryCoroutine");
    //}

    //IEnumerator WaitMakeEquipmentDictionaryCoroutine()
    //{
    //    while (true)
    //        yield return null;
    //}

    private void InitializeEquipmentSlots()
    {
        while (contents.childCount > 0)
            DestroyImmediate(contents.GetChild(0).gameObject);

        int initSlotCount = PlayerEquipmentManager.Instance().GetPlayerHaveEquipmentDictionary().Count
                                + (5 - PlayerEquipmentManager.Instance().GetPlayerHaveEquipmentDictionary().Count % 5);

        //int initSlotCount = Player.Instance().GetPlayerData().EquipmentSlotsNumber
        //                        + (5 - Player.Instance().GetPlayerData().EquipmentSlotsNumber % 5);

        equipmentSlotList = new List<EquipSlot>();
        // Slot���� ��������.

        GameObject tmpSlot;
        for (int i = 0; i < initSlotCount; i++)
        {
            tmpSlot = Resources.Load<GameObject>(equipSlotPath);
            tmpSlot = Instantiate(tmpSlot);

            tmpSlot.transform.SetParent(contents, false);
            //tmpSlot.GetComponent<RectTransform>().sizeDelta = Vector2.one;

            equipmentSlotList.Add(tmpSlot.GetComponent<EquipSlot>());
        }

        RegisterEquipmentToSlot(ref PlayerEquipmentManager.Instance().GetPlayerHaveEquipmentDictionary());
    }

    //public void InitializeEquipmentSlots()
    //{
    //    while (contents.childCount > 0)
    //        DestroyImmediate(contents.GetChild(0).gameObject);

    //    WaitMakeEquipmentDictionary();

    //    int initSlotCount = PlayerEquipmentManager.Instance().GetPlayerEquipmentDictionary().Count
    //                            + (5 - PlayerEquipmentManager.Instance().GetPlayerEquipmentDictionary().Count % 5);

    //    //int initSlotCount = Player.Instance().GetPlayerData().EquipmentSlotsNumber
    //    //                        + (5 - Player.Instance().GetPlayerData().EquipmentSlotsNumber % 5);

    //    equipmentSlotList = new List<EquipSlot>();
    //    // Slot���� ��������.

    //    GameObject tmpSlot;
    //    for (int i = 0; i < initSlotCount; i++)
    //    {
    //        tmpSlot = Resources.Load<GameObject>(equipSlotPath);
    //        tmpSlot = Instantiate(tmpSlot);

    //        tmpSlot.transform.SetParent(contents);

    //        equipmentSlotList.Add(tmpSlot.GetComponent<EquipSlot>());
    //    }

    //    RegisterEquipmentToSlot(ref PlayerEquipmentManager.Instance().GetPlayerEquipmentDictionary());
    //}

    public void IncreaseEquipmentSlotNumber(int n)
    {
        GameObject tmpSlot;
        for (int i = 0; i < n; i++)
        {
            tmpSlot = Resources.Load<GameObject>(equipSlotPath);
            tmpSlot = Instantiate(tmpSlot);

            tmpSlot.transform.SetParent(contents, false);
            equipmentSlotList.Add(tmpSlot.GetComponent<EquipSlot>());
        }
    }

    public void RegisterEquipmentToSlot(ref Dictionary<int, A_Equipment> equipmentList)
    {
        while (equipmentSlotList.Count < equipmentList.Count)
            IncreaseEquipmentSlotNumber(5);

        // equipmentList�� Count�� 0��!!
        int equipListIndex = 0;
        foreach (KeyValuePair<int, A_Equipment> equipment in equipmentList)
        {
            equipmentSlotList[equipListIndex].UpdateEquipSlot(equipment);
            equipListIndex++;
        }
    }    
}
