using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> ���� ������ - ���� ������ </summary>
public class PortionItem : CountableItem, IUsableItem
{
    public PortionItem(PotionItemData data, int amount = 1) : base(data, amount) { }

    public bool Use()
    {
        // �ӽ� : ���� �ϳ� ����
        Amount--;

        return true;
    }

    protected override CountableItem Clone(int amount)
    {
        return new PortionItem(CountableData as PotionItemData, amount);
    }
}
