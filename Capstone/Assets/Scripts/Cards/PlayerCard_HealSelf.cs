using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCard_HealSelf : A_PlayerCard
{
    [SerializeField, Range(0.0f, 1.0f)] float healRaio;

    public override void OnDiscardCard()
    {

    }

    public override void OnDrawCard()
    {

    }

    public override void OnPlayCard()
    {
        BattleManager battleManager = BattleManager.Instance();
        PlayerSpecManager playerSpecManager = PlayerSpecManager.Instance();

        if (playerSpecManager.currentPlayerCost < cardCost)
        {
            Debug.Log("Not Enough Cost");
            return;
        }

        float healAmount = healRaio * playerSpecManager.maxPlayerHP;

        battleManager.ReducePlayerCost(cardCost);
        battleManager.HealToPlayer(healAmount);
    }

    public override void OnReloadCard()
    {

    }
}
