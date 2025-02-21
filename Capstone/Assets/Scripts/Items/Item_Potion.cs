using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Potion : A_Item
{
    enum Amount
    { 
        Little,
        Middle,
        Large,
    }

    [SerializeField] Amount amount;

    private float healRatio;
    private float healAmount;

    public override void Use()
    {
        Debug.Log("Used Potion");

        // 쓸 때 HealToPlayer의 Max / Min 확인

        switch (amount)
        { 
            case Amount.Little:
                healRatio = 0.1f;
                break;
            case Amount.Middle:
                healRatio = 0.3f;
                break;
            case Amount.Large:
                healRatio = 0.5f;
                break;
        }

        healAmount = PlayerSpecManager.Instance().maxPlayerHP * healRatio;

        Debug.Log(healAmount);
        BattleManager.Instance().HealToPlayer(healAmount, true);
    }
}
