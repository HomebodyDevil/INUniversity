using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
//using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public static Action UsedAllItems;
    public static Action DisableItemSlotsOutline;
    public static Action Act_Unselect;

    public bool hasItem = false;
    public int slotItemUseCount;

    private bool isSelected;

    [SerializeField] private A_Item item;
    [SerializeField] private TextMeshProUGUI itemUseCountText;
    [SerializeField] private GameObject useCountPanel;
    [SerializeField] private Color outLineColor;
    [SerializeField] private Image itemImage;
    private Outline outLine;

    //private KeyValuePair<A_Item, int> itemInDictionary;

    private void Start()
    {
        isSelected = false;

        outLine = GetComponent<Outline>();
        outLine.effectColor = outLineColor;

        DisableSlotOutline();
    }

    private void OnEnable()
    {
        DisableItemSlotsOutline -= DisableSlotOutline;
        DisableItemSlotsOutline += DisableSlotOutline;

        Act_Unselect -= Unselect;
        Act_Unselect += Unselect;
    }

    private void OnDestroy()
    {
        DisableItemSlotsOutline -= DisableSlotOutline;
        DisableItemSlotsOutline -= DisableSlotOutline;
        Act_Unselect -= Unselect;
    }

    private void OnDisable()
    {
        isSelected = false;

        if (gameObject.activeSelf)
            DisableSlotOutline();
        DisableItemSlotsOutline -= DisableSlotOutline;
    }

    private void DisableSlotOutline()
    {
        outLine.enabled = false;
    }

    public void EnableSlotOutline()
    {
        outLine.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!hasItem)
            return;

        if (isSelected)
        {
            //Debug.Log("DESC");
            //Debug.Log(String.Format("path : {0}, name : {1}, desc : {2}", item.itemImagePath, item.name, item.itemDescription));

            MapUIManager.Instance().ActivateObjectDescriptionPanel();
            ObjectDescriptionPanel.Act_UpdateObjectDescription.Invoke(item.itemImagePath, item.name, item.itemDescription);
        }
        else
        {
            Act_Unselect.Invoke();

            isSelected = true;

            DisableItemSlotsOutline.Invoke();
            EnableSlotOutline();

            ItemPanel.EnableItemButtons.Invoke();            
            ItemPanel.SelectCurrentItemSlot.Invoke(this);
        }
    }

    private void Unselect()
    {
        isSelected = false;
    }

    private void CheckHasItem()
    {
        //if (item != null)
        //    hasItem = true;
        //else
        //    hasItem = false;
        hasItem = (item != null);
    }

    // Slot�� Update�Ѵ�.
    // �̹���(Sprite)�� ������Ʈ�ϰ�
    // Slot�� ��ȿ��(hasItem)�� ������Ʈ�Ѵ�.
    // List�� ������� ���� ����.
    //public void UpdateItemSlot(A_Item newItem = null)
    //{
    //    if (newItem == null)
    //    {
    //        item = null;
    //        hasItem = false;
    //        GetComponent<Image>().sprite = null;
    //    }
    //    else
    //    {
    //        item = newItem;
    //        hasItem = true;

    //        GetComponent<Image>().sprite = Resources.Load<Sprite>(newItem.itemImagePath);
    //    }
    //}

    // DIctionary�� ����� ���� ����.
    public void UpdateItemSlot(KeyValuePair<int, A_Item> newItem)
    {
        item = newItem.Value;
        CheckHasItem();
        //hasItem = true;
        //slotItemUseCount = newItem.Value;
        slotItemUseCount = PlayerItemsManager.Instance().GetPlayerItemCount()[newItem.Key];

        //if (slotItemUseCount > 0)
        //    GetComponent<Image>().sprite = Resources.Load<Sprite>(newItem.Value.itemImagePath);
        if (slotItemUseCount > 0)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = Resources.Load<Sprite>(newItem.Value.itemImagePath);
        }
        else if (slotItemUseCount <= 0)
        {
            //itemImage.gameObject.SetActive(false);
        }

        UpdateItemUseCountText(slotItemUseCount);        
    }

    public void RemoveItem(ref A_Item removeItem)
    {
        hasItem = false;
    }
    
    public void UseItem()
    {
        if (!hasItem || slotItemUseCount < 0)
            return;

        item.Use();
        slotItemUseCount--;

        CheckEmpty();

        UpdateItemUseCountText(slotItemUseCount);
        UpdateItemUseCountData(slotItemUseCount);

        if (!hasItem)                  
            UsedAllItems.Invoke();
    }

    // Item�� �� ��ٸ� �����ش�.
    private void CheckEmpty()
    {
        if (slotItemUseCount > 0)
            return;

        hasItem = false;
        //PlayerSpecManager.Instance().playerItemList.Remove(item);
        ItemSlot.DisableItemSlotsOutline.Invoke();
        //GetComponent<Image>().sprite = null;
        itemImage.gameObject.SetActive(false);
    }

    public void UpdateItemUseCountText(int n)
    {
        // Text�� �ƴ϶� Panel�� Ȱ�� / ��Ȱ��ȭ �ϱ� ������.
        if (n <= 0)
            useCountPanel.SetActive(false);
        else
        {
            useCountPanel.SetActive(true);
            itemUseCountText.text = String.Format("{0}", n);     
        }
    }

    public void UpdateItemUseCountData(int n)
    {
        // PlayerSpecManager���� �����ϵ��� ��
        // �� ���� ����� ���� �� ������
        // C# �̼� �̽�.   
        PlayerItemsManager.Instance().GetPlayerItemCount()[item.itemID] = n;

        if (n <= 0)
        {
            PlayerItemsManager.Instance().GetPlayerItemDictionary().Remove(item.itemID);

            if (ItemPanel.UpdateItemPanel != null)
                ItemPanel.UpdateItemPanel.Invoke(PlayerItemsManager.Instance().GetPlayerItemDictionary());

            if (UsedAllItems != null)
                UsedAllItems.Invoke();
        }
    }

    public void MakeEmpty()
    {
        itemImage.gameObject.SetActive(false);
        useCountPanel.SetActive(false);
    }
}
