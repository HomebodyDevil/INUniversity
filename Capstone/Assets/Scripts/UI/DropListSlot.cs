using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropListSlot : MonoBehaviour, IPointerClickHandler
{
    public enum DropListType
    {
        ITEM = 0,
        CARD = 1,
        EQUIPMENT = 2,
    }

    [SerializeField] private Image image;

    private DropListType slotType;

    //private A_PlayerCard card;
    //private A_Item item;
    //private A_Equipment equipment;

    private string imagePath;
    private string objName;
    private string objDescription;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Clicked");

        MapUIManager.Instance().ActivateObjectDescriptionPanel();
        UpdateDescriptionPanel();
    }

    public void UpdateSlot<T>(DropListType type, T obj)
    {
        slotType = type;

        if (type == DropListType.ITEM)
        {
            A_Item curr = obj as A_Item;
            imagePath = curr.itemImagePath;
            objName = curr.itemName;
            objDescription = curr.itemDescription;
        }
        else if (type == DropListType.CARD)
        {
            A_PlayerCard curr = obj as A_PlayerCard;
            imagePath = curr.cardImagePath;
            objName = curr.cardName;
            objDescription = curr.cardDescription;
        }
        else if (type == DropListType.EQUIPMENT)
        {
            A_Equipment curr = obj as A_Equipment;
            imagePath = curr.equipmentImagePath;
            objName = curr.equipmentName;
            objDescription = curr.equipmentDescription;
        }

        image.sprite = Resources.Load<Sprite>(imagePath);
    }

    private void UpdateDescriptionPanel()
    {
        if (slotType == DropListType.ITEM)
        {
            UpdateItemDescription();
        }
        else if (slotType == DropListType.CARD)
        {
            UpdateCardDescription();
        }
        else if (slotType == DropListType.EQUIPMENT)
        {
            UpdateEquipmentDescription();
        }
    }

    private void UpdateItemDescription()
    {
        ObjectDescriptionPanel.Act_UpdateObjectDescription.Invoke(imagePath, objName, objDescription);
    }

    private void UpdateCardDescription()
    {
        ObjectDescriptionPanel.Act_UpdateObjectDescription.Invoke(imagePath, objName, objDescription);
    }

    private void UpdateEquipmentDescription()
    {
        ObjectDescriptionPanel.Act_UpdateObjectDescription.Invoke(imagePath, objName, objDescription);
    }
}
