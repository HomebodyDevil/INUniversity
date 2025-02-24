using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentImageHolder : MonoBehaviour, IPointerClickHandler
{
    public static Action UpdateEquipmentImageHolderImage;

    [SerializeField] PlayerEquipmentManager.Equipments type;
    [SerializeField] Image image;

    private Sprite initialSprite;

    private void Awake()
    {
        PlayerEquipmentManager.EquipEquipment -= UpdateImage;
        PlayerEquipmentManager.EquipEquipment += UpdateImage;
    }

    private void Start()
    {
        initialSprite = image.sprite;
        UpdateImage();
    }

    private void OnEnable()
    {
        PlayerEquipmentManager.EquipEquipment -= UpdateImage;
        PlayerEquipmentManager.EquipEquipment += UpdateImage;

        UpdateEquipmentImageHolderImage -= UpdateImage;
        UpdateEquipmentImageHolderImage += UpdateImage;

        UpdateImage();
    }

    private void OnDisable()
    {
        PlayerEquipmentManager.EquipEquipment -= UpdateImage;
        UpdateEquipmentImageHolderImage -= UpdateImage;
    }

    private void UpdateImage()
    {
        A_Equipment currentEquipment = PlayerEquipmentManager.Instance().GetCurrentPlayerEquipment(type);

        if (currentEquipment == null)
        {
            Debug.Log("CurrentEquipment is Null");
            image.sprite = initialSprite;
            //image.sprite = null;

            image.gameObject.SetActive(false);
        }
        else
        {
            //Debug.Log(currentEquipment.equipmentImagePath);
            image.sprite = Resources.Load<Sprite>(currentEquipment.equipmentImagePath);
            image.gameObject.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        A_Equipment currentEquipment = PlayerEquipmentManager.Instance().GetCurrentPlayerEquipment(type);

        if (currentEquipment == null)
        {
            return;
        }
        else
        {
            MapUIManager.Instance().ActivateObjectDescriptionPanel();
            ObjectDescriptionPanel.Act_UpdateObjectDescription.Invoke(currentEquipment.equipmentImagePath, currentEquipment.name, currentEquipment.equipmentDescription);
        }
    }
}
