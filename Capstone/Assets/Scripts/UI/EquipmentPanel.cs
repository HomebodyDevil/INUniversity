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

        // Panel을 Enable상태로 실행시 오류가 나지 않기 위해 했던 방법.,
        // 근데 Panel을 Disable상태로 실행하면 오류가 발생하지 않음...
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

        // Grid를 설정.
        RectOffset gridPadding = gridGroup.padding;
        gridPadding.left = (int)gridGroupPadding;
        gridPadding.right = (int)gridGroupPadding;
        gridPadding.top = (int)gridGroupPadding;
        gridPadding.bottom = (int)gridGroupPadding;

        float width = EquipAndStatusPanel.GetComponent<RectTransform>().rect.width - 3 * gridGroupPadding;   //좌, 우, 중간의 패딩을 제외한 너비
        float height = EquipAndStatusPanel.GetComponent<RectTransform>().rect.height - 2 * gridGroupPadding; // 상 하, 중간의 패딩을 제외한 높이

        gridGroup.cellSize = new Vector2(width / 2, height);    // 현재 착용중인 장비, Status Panel에 대한 설정.
        gridGroup.spacing = new Vector2(gridGroupPadding, 0);


        // AllEquipment 파넬 설정
        gridPadding = allEquipmentGridGroup.padding;        
        gridPadding.left = gridPadding.right = gridPadding.top = gridPadding.bottom = (int)allEquipmentPanelPadding;
        
        width = (AllEquipmentPanel.GetComponent<RectTransform>().rect.width - 6 * allEquipmentPanelPadding) / 5;        
        allEquipmentGridGroup.cellSize = new Vector2(width, width);
        
        // Using 파넬 설정.
        // padding은 20이라고 놨음.
        width = (gridGroup.cellSize.x - (EQUIPMENT_IMAGE_COLUMN_COUNT + 1) * 20) / EQUIPMENT_IMAGE_COLUMN_COUNT;  // EquipmentPanel 안의 장비 착용 현황 이미지의 크기를 정해줄 예정.
        height = (gridGroup.cellSize.y - (EQUIPMENT_IMAGE_RAW_COUNT + 1) * 20) / EQUIPMENT_IMAGE_RAW_COUNT;
        width = Mathf.Min(width, height);       // width, height중 더 작은 값으로 Size가 되도록 하자. 

        equipmentPanelGridLayout.cellSize = new Vector2(width, width);  
    }

    private void PanelSetting()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float width = rectTransform.rect.width - padding * 2;    // 60은 위, 아래 30, 좌, 우 30의 패딩.
        float height = rectTransform.rect.height - padding * 3;  // 맨 위, 중간, 맨 아래의 패딩.
        //Debug.Log(string.Format("width : {0}, height : {1}", width, height));

        RectTransform equipAndStatusRect = EquipAndStatusPanel.GetComponent<RectTransform>();
        RectTransform equipmentRect = AllEquipmentPanel.GetComponent<RectTransform>();

        float equipAndStatusHeight = height * 0.3f;
        float equipmentPanelHeight = height * 0.7f;
        //Debug.Log(string.Format("equipHeight : {0}, statusHeight : {1}", equipHeight, statusHeight));

        // RectTransform의 left, right, top, bottom을 수정할 수 있는듯.
        // 오브젝트의 rectTransform에 해당하는 left, right, top, bottom이 활성화돼있는지 확인 필요.
        // 만약 오브젝트의 recTransform에 left나 right 등이 없으면 사용 불가.
        // width, height일 경우, sizeDelta를 사용해야 함.
        //equipAndStatusRect.sizeDelta = new Vector2(width, height);
        equipAndStatusRect.offsetMax = new Vector2(-padding, -padding);                       // offsetMax : 최상의 최우의 위치를 기준으로 한 상대적 위치(right, top)
        equipAndStatusRect.offsetMin = new Vector2(padding, padding * 2 + equipmentPanelHeight);       // offsetMin : 최하의 최좌의 위치를 기준으로 한 상대적 위치(left, bottom)

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
        // Slot들을 만들어줬고.

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
    //    // Slot들을 만들어줬고.

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

        // equipmentList의 Count가 0임!!
        int equipListIndex = 0;
        foreach (KeyValuePair<int, A_Equipment> equipment in equipmentList)
        {
            equipmentSlotList[equipListIndex].UpdateEquipSlot(equipment);
            equipListIndex++;
        }
    }    
}
