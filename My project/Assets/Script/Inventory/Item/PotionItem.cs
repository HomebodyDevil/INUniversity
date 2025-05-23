using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 수량 아이템 - 포션 아이템 </summary>
public class PortionItem : CountableItem, IUsableItem
{
    public PortionItem(PotionItemData data, int amount = 1) : base(data, amount) { }

    public bool Use()
    {
        // 임시 : 개수 하나 감소
        Amount--;

        return true;
    }

    protected override CountableItem Clone(int amount)
    {
        return new PortionItem(CountableData as PotionItemData, amount);
    }
}
