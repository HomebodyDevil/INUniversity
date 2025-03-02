using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoPanel : MonoBehaviour
{
    RectTransform panelRectTransform;

    [Header("UI Objects")]
    [SerializeField] private GameObject enemyImagePanel;
    [SerializeField] private GameObject enemyInfoPanel;
    [SerializeField] private GameObject okButton;
    [SerializeField] private GameObject enemyImage;
    [SerializeField] private RectTransform dropListText;
    [SerializeField] private GameObject dropItemCardPanel;
    [SerializeField] private TextMeshProUGUI enemyDescriptionText;
    [SerializeField] private Transform cardList;
    [SerializeField] private Transform itemList;

    [Space(10), Header("Space Ratio")]
    [SerializeField] private float padding;
    [SerializeField] private float okButtonBottomGap;
    [SerializeField] private float gap;
    [SerializeField] private int objectsCount;
    [SerializeField, Range(0, 1f)] private float enemyImagePanelHeightRatio;
    [SerializeField, Range(0, 1f)] private float enemyInfoPanelHeightRatio;
    [SerializeField, Range(0, 1f)] private float okButtonHeightRatio;
    [SerializeField, Range(0, 1f)] private float okButtonWidthRatio;
    [SerializeField, Range(0, 1f)] private float dropListTextHeightRatio;
    [SerializeField, Range(0, 1f)] private float dropItemCardPanelHeightRatio;
    [SerializeField, Range(0, 1f)] private float slotWidthHeightRatio;
    //[SerializeField, Range(0, 1f)] private float dropItemCardPanelWidthRatio;

    private MapUIManager uiManager;

    private void Start()
    {
        panelRectTransform = GetComponent<RectTransform>();

        GetUIManager();
        Initialize();

        //ref List<A_PlayerCard> list = ref PlayerCardManager.Instance().GetPlayerDeckCardList();
        //UpdateCardList(ref list);
    }

    private void Update()
    {
        Initialize();
    }

    public void GetUIManager()
    {
        StartCoroutine(GetUIManagerCoroutine());
    }

    public IEnumerator GetUIManagerCoroutine()
    {
        while(true)
        {
            uiManager = MapUIManager.Instance();
            if (uiManager == null)
                yield return null;
            yield break;
        }
    }

    public void Initialize()
    {
        float width = GetComponent<RectTransform>().rect.width;
        float height = GetComponent<RectTransform>().rect.height;

        float enemyImagePanelHeight = (height - 2 * padding - (objectsCount - 1) * gap) * enemyImagePanelHeightRatio;
        float enemyInfoPanelHeight = (height - 2 * padding - (objectsCount - 1) * gap) * enemyInfoPanelHeightRatio;
        float dropListTextHeight = (height - 2 * padding - (objectsCount - 1) - gap) * dropListTextHeightRatio;
        float dropListPanelHeight = (height - 2 * padding - (objectsCount - 1) * gap) * dropItemCardPanelHeightRatio;
        float okButtonHeight = (height - 2 * padding - (objectsCount - 1) * gap) * okButtonHeightRatio;

        float okButtonWidth = width * okButtonWidthRatio;

        float startFromTop = height - padding;
        float sumOfUsedPanelsHeightWithGap = 0f;

        // pivot이 다 바깥으로 있을 경우(left, top, right, bottom) - offsetMAx / Min을 사용하여 크기 조절.
        // pivot이 중앙에 있을 경우(width, height) - scaleDelta를 사용하여 크기 조절.

        RectTransform enemyImagePanelRectTransform = enemyImagePanel.GetComponent<RectTransform>();
        enemyImagePanelRectTransform.offsetMax = new Vector2(-padding, -padding);
        enemyImagePanelRectTransform.offsetMin = new Vector2(padding, startFromTop - enemyImagePanelHeight);
        sumOfUsedPanelsHeightWithGap += enemyImagePanelHeight;

        RectTransform enemyImageRectTransform = enemyImage.GetComponent<RectTransform>();
        float enemyImageWidthAndHeight = MathF.Min(enemyImagePanelRectTransform.rect.width - 100, enemyImagePanelRectTransform.rect.height - 100);
        enemyImageRectTransform.sizeDelta = new Vector2(enemyImageWidthAndHeight, enemyImageWidthAndHeight);

        RectTransform enemyInfoRectTransform = enemyInfoPanel.GetComponent<RectTransform>();
        enemyInfoRectTransform.offsetMax = new Vector2(-padding, -padding - sumOfUsedPanelsHeightWithGap - gap);
        enemyInfoRectTransform.offsetMin = new Vector2(padding, startFromTop - sumOfUsedPanelsHeightWithGap - gap - enemyInfoPanelHeight);
        sumOfUsedPanelsHeightWithGap += (enemyInfoPanelHeight + gap);

        dropListText.offsetMax = new Vector2(-padding, -padding - sumOfUsedPanelsHeightWithGap);
        dropListText.offsetMin = new Vector2(padding, startFromTop - sumOfUsedPanelsHeightWithGap - gap - dropListTextHeight);
        sumOfUsedPanelsHeightWithGap += (dropListTextHeight);

        RectTransform dropListPanelRectTransform = dropItemCardPanel.GetComponent<RectTransform>();
        dropListPanelRectTransform.offsetMax = new Vector2(-padding, -padding - sumOfUsedPanelsHeightWithGap - gap);
        dropListPanelRectTransform.offsetMin = new Vector2(padding, startFromTop - sumOfUsedPanelsHeightWithGap - gap - dropListPanelHeight);
        sumOfUsedPanelsHeightWithGap += (dropListPanelHeight + gap);

        RectTransform okButtonRectTransform = okButton.GetComponent<RectTransform>();
        okButtonRectTransform.offsetMax = new Vector2(-(width - okButtonWidth) / 2, -(height - padding - okButtonHeight));
        okButtonRectTransform.offsetMin = new Vector2((width - okButtonWidth) / 2, okButtonBottomGap);

        height = dropListPanelRectTransform.GetComponent<RectTransform>().rect.height * slotWidthHeightRatio;
        //float widthHeight = Math.Min()
        cardList.GetComponent<GridLayoutGroup>().cellSize = new Vector2(height, height);
        itemList.GetComponent<GridLayoutGroup>().cellSize = new Vector2(height, height);
    }

    public void OnClicked()
    {
        //Debug.Log("Clicked EnemyInfoPanel");

        bool isActive = gameObject.activeSelf;

        if (isActive)
        {
            //gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            MapUIManager.Instance().ActivateCanclePanel();
            MapUIManager.Instance().activeObject.Push(this.gameObject);

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

    public void OnClickOKButton()
    {
        // Cancle과 비슷하게 동작하기 때문에 일단 이걸로 사용하는 것으로 해보자
        // 추후 문제 없으면 계속 사용토록 하자.
        MapUIManager.Instance().OnCanclePanel();   

        SceneManagerEX.Instance().SwitchToBattleScene();
    }

    // EnemyBattleCheck Panel의 UI를 만들어주자.

    public void UpdateContents()
    {
        DefaultEnemyData enemyData = BattleManager.Instance().currentEnemyData;

        if (enemyData == null)
            Debug.Log("NNNNNNN");

        enemyDescriptionText.text = enemyData.enemyDescription;
        enemyImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(enemyData.enemyImagePath);

        List<A_PlayerCard> cards = new List<A_PlayerCard>();
        List<A_Item> items = new List<A_Item>();
        List<A_Equipment> equip = new List<A_Equipment>();

        List<DropItem> dropItem = enemyData.dropItems;
        List<DropCard> dropCards = enemyData.dropCards;
        List<DropEquipment> dropEquip = enemyData.dropEquipment;

        for (int i = 0; i < dropItem.Count; i++)
        {
            A_Item playerItem = dropItem[i].item.GetComponent<A_Item>();
            items.Add(playerItem);
        }

        for (int i = 0; i < dropCards.Count; i++)
        {
            A_PlayerCard playerCard = dropCards[i].card.GetComponent<A_PlayerCard>();
            cards.Add(playerCard);
        }

        for (int i = 0; i < dropEquip.Count; i++)
        {
            A_Equipment equipment = null;
            if (dropEquip[i].equipment != null)
                equipment = dropEquip[i].equipment.GetComponent<A_Equipment>();

            if (equipment != null)
                equip.Add(equipment);
        }

        UpdateCardList(ref cards);
        UpdateItemList(ref equip, ref items);
    }

    public void UpdateCardList(ref List<A_PlayerCard> cards)
    {
        ClearChild(ref cardList);

        foreach(A_PlayerCard card in cards)
        {
            GameObject dropListSlot = Resources.Load<GameObject>("Prefabs/UI/DropListSlot");
            dropListSlot = Instantiate(dropListSlot);
            dropListSlot.transform.SetParent(cardList);

            DropListSlot listSlot = dropListSlot.GetComponent<DropListSlot>();
            listSlot.UpdateSlot<A_PlayerCard>(DropListSlot.DropListType.CARD, card);
        }
    }

    public void UpdateItemList(ref List<A_Equipment> equipments, ref List<A_Item> items)
    {
        ClearChild(ref itemList);

        // Equipment 리스트 갱신.
        foreach (A_Equipment equipment in equipments)
        {
            GameObject dropListSlot = Resources.Load<GameObject>("Prefabs/UI/DropListSlot");
            dropListSlot = Instantiate(dropListSlot);
            dropListSlot.transform.SetParent(itemList);

            DropListSlot listSlot = dropListSlot.GetComponent<DropListSlot>();
            listSlot.UpdateSlot<A_Equipment>(DropListSlot.DropListType.EQUIPMENT, equipment);
        }

        // Item 리스트 갱신.
        foreach (A_Item item in items)
        {
            GameObject dropListSlot = Resources.Load<GameObject>("Prefabs/UI/DropListSlot");
            dropListSlot = Instantiate(dropListSlot);
            dropListSlot.transform.SetParent(itemList);

            DropListSlot listSlot = dropListSlot.GetComponent<DropListSlot>();
            listSlot.UpdateSlot<A_Item>(DropListSlot.DropListType.ITEM, item);
        }
    }

    private void ClearChild(ref Transform obj)
    {
        int rep = 0;
        while(obj.childCount > 0)
        {
            DestroyImmediate(obj.GetChild(0).gameObject);

            rep++;
            if (rep > 1000)
            {
                Debug.Log("Many Loops");
                break;
            }
        }
    }
}
