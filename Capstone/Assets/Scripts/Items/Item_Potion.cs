using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Potion : A_Item
{
    public override void Use()
    {
        Debug.Log("Used Potion");

        // 쓸 때 HealToPlayer의 Max / Min 확인
        BattleManager.Instance().HealToPlayer(30);
    }
}
