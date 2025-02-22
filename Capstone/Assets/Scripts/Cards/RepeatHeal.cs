using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatHeal : A_PlayerCard
{
    [SerializeField] int count;
    [SerializeField, Range(0.0f, 1.0f)] float healRaio;
    [SerializeField] float interval;

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
        battleManager.RepeatHeal(count, healAmount, interval);
    }

    public override void OnReloadCard()
    {

    }
}
