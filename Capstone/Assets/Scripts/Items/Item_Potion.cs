using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Potion : A_Item
{
    public override void Use()
    {
        Debug.Log("Used Potion");

        // �� �� HealToPlayer�� Max / Min Ȯ��
        BattleManager.Instance().HealToPlayer(30);
    }
}
