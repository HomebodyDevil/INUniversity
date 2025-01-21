using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CountableItemData;

/// <summary> �Һ� ������ ���� </summary>
[CreateAssetMenu(fileName = "Item_Portion_", menuName = "Inventory System/Item Data/Portion", order = 3)]
public class PotionItemData : CountableItemData
{
    /// <summary> ȿ����(ȸ���� ��) </summary>
    public float Value => _value;
    [SerializeField] private float _value;
    public override Item CreateItem()
    {
        return new PortionItem(this);
    }
}
