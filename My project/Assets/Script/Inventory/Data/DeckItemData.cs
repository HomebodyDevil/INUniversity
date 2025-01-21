using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EquipmentItemData;

/// <summary> 장비 - 방어구 아이템 </summary>
[CreateAssetMenu(fileName = "Item_Deck_", menuName = "Inventory System/Item Data/Deck", order = 2)]
public class DeckItemData : EquipmentItemData
{
    public override Item CreateItem()
    {
        return new DeckItem(this);
    }
}
