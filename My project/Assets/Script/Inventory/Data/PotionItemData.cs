using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CountableItemData;

/// <summary> 소비 아이템 정보 </summary>
[CreateAssetMenu(fileName = "Item_Portion_", menuName = "Inventory System/Item Data/Portion", order = 3)]
public class PotionItemData : CountableItemData
{
    /// <summary> 효과량(회복량 등) </summary>
    public float Value => _value;
    [SerializeField] private float _value;
    public override Item CreateItem()
    {
        return new PortionItem(this);
    }
}
