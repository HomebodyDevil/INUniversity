using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PlayerEquipmentManager;
//using static UnityEditor.Progress;

public class EquipSlot : MonoBehaviour, IPointerClickHandler
{
    public static Action DisableEquipmentSlotsOutline;
    public static Action Act_Unselect;

    public bool hasEquipment = false;
    public int slotEquipCount;

    private bool isSelected = false;

    [SerializeField] private A_Equipment equipment;
    [SerializeField] private TextMeshProUGUI equipmentCountText;
    [SerializeField] private GameObject equipmentCountPanel;
    [SerializeField] private Image equipmentImage;
    [SerializeField] private Color outLineColor;
    private Outline outLine;

    private void Start()
    {
        outLine = GetComponent<Outline>();
        outLine.effectColor = outLineColor;

        DisableSlotOutline();
    }

    private void OnEnable()
    {
        Act_Unselect -= Unselect;
        Act_Unselect += Unselect;

        DisableEquipmentSlotsOutline -= DisableSlotOutline;
        DisableEquipmentSlotsOutline += DisableSlotOutline;
    }

    private void OnDisable()
    {
        isSelected = false;

        if (gameObject.activeSelf)
            DisableSlotOutline();
        DisableEquipmentSlotsOutline -= DisableSlotOutline;
        Act_Unselect -= Unselect;
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
        if (!hasEquipment)
            return;

        if (isSelected)
        {
            MapUIManager.Instance().ActivateObjectDescriptionPanel();
            ObjectDescriptionPanel.Act_UpdateObjectDescription.Invoke(equipment.equipmentImagePath, equipment.name, equipment.equipmentDescription);
        }
        else
        {
            Act_Unselect.Invoke();

            DisableEquipmentSlotsOutline.Invoke();
            EnableSlotOutline();

            EquipmentPanel.EnableEquipmentButtons.Invoke();
            EquipmentPanel.SelectCurrentEquipmentSlot.Invoke(this);

            isSelected = true;
        }
    }

    private void Unselect()
    {
        isSelected = false;
    }

    private void CheckHasEquipment()
    {
        hasEquipment = (equipment != null);
    }

    public void UpdateEquipSlot(KeyValuePair<int, A_Equipment> newEquipment)
    {
        //equipment = newEquipment.Key;
        equipment = PlayerEquipmentManager.Instance().GetPlayerHaveEquipmentDictionary()[newEquipment.Key];
        CheckHasEquipment();
        //slotEquipCount = newEquipment.Value;
        slotEquipCount = PlayerEquipmentManager.Instance().GetPlayerHaveEquipmentCount()[newEquipment.Key];

        if (slotEquipCount > 0)
        {
            equipmentImage.sprite = Resources.Load<Sprite>(newEquipment.Value.equipmentImagePath);
            //GetComponent<Image>().sprite = Resources.Load<Sprite>(newEquipment.Value.equipmentImagePath);
        }

        UpdateEquipmentUseCountText(slotEquipCount);
    }

    private void CheckEmpty()
    {
        if (slotEquipCount > 0)
            return;

        hasEquipment = false;
        PlayerSpecManager.Instance().playerEquipmentList.Remove(equipment);
        GetComponent<Image>().sprite = null;
    }

    public void UpdateEquipmentUseCountText(int n)
    {
        // Text가 아니라 Panel을 활성 / 비활성화 하기 위함임.
        if (n <= 0)
            equipmentCountPanel.SetActive(false);
        else
        {
            equipmentCountPanel.SetActive(true);
            equipmentCountText.text = String.Format("{0}", n);
        }
    }

    public void UpdateEquipmentCountData(int n)
    {
        // PlayerSpecManager에도 적용하도록 함
        // 더 좋은 방법이 있을 것 같은데
        // C# 미숙 이슈.   
        PlayerEquipmentManager.Instance().GetPlayerHaveEquipmentCount()[equipment.equipmentID] = n;
    }

    public void Equip()
    {
        PlayerEquipmentManager.Instance().SetCurrentPlayerEquipment(equipment);

        //if (!equipment.isEquipped)
        //    equipment.PutOn();
    }

    public void UnEquip()
    {
        PlayerEquipmentManager.Instance().RidCurrentPlayerEquipment(equipment);
        equipment.TakeOff();
    }
}
