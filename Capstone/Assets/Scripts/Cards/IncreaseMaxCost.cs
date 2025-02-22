using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMaxCost : A_PlayerCard
{
    [SerializeField] private float increaseAmount;

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

        battleManager.ReducePlayerCost(cardCost);
        playerSpecManager.maxPlayerCost += increaseAmount;
    }

    public override void OnReloadCard()
    {

    }
}
