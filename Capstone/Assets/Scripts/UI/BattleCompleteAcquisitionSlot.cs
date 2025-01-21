using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleCompleteAcquisitionSlot : MonoBehaviour, IPointerClickHandler
{
    public enum Type
    { 
        Item = 0,
        Equipment = 1,
        Card = 2,
    }

    [SerializeField] private Image countTextPanel;
    [SerializeField] private TextMeshProUGUI countText;

    private Type type;

    private A_Item item;
    private A_Equipment equipment;
    private A_PlayerCard card;

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void SetType(Type newType)
    {
        type = newType;
    }

    public Type GetType(Type newType)
    {
        return type;
    }

    public void SetImage(Sprite newImage)
    {
        if (newImage == null)
        {
            Debug.Log("newImage is NULL");
            return;
        }

        image.sprite = newImage;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked Acuisition Slot");
    }

    public void SetItem(A_Item newItem)
    {
        item = newItem;
    }

    public void SetEquipment(A_Equipment newEquipment)
    {
        equipment = newEquipment;
    }

    public void SetCard(A_PlayerCard newCard)
    {
        card = newCard;
    }

    public A_Item GetItem()
    {
        if (item == null)
        {
            Debug.Log("item is NULL");
            return null;
        }

        return item;
    }

    public A_Equipment GetEquipment()
    {
        if (item == null)
        {
            Debug.Log("Equipment is NULL");
            return null;
        }

        return equipment;
    }

    public A_PlayerCard GetCard()
    {
        if (item == null)
        {
            Debug.Log("Card is NULL");
            return null;
        }

        return card;
    }

    public void EnableText()
    {
        countTextPanel.gameObject.SetActive(true);
    }

    public void DisableText()
    {
        countTextPanel.gameObject.SetActive(false);
    }

    public void SetCountText(int n)
    {
        countText.text = string.Format("{0}", n);
    }
}
