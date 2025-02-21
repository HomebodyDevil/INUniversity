using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCard_DoubleAttack : A_PlayerCard
{
    [SerializeField] private float defaultAttack;
    [SerializeField, Range(0.0f, 1.0f)] private float attackRatio;
    [SerializeField] private float attackInterval;

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

        float attackAmount = defaultAttack + attackRatio * playerSpecManager.currentPlayerAttackPoint;

        battleManager.ReducePlayerCost(cardCost);

        UnityEngine.Random.InitState((int)((DateTime.Now.Ticks * 10000) % int.MaxValue));
        battleManager.MultipleAttack(2, attackAmount, attackInterval);
    }

    public override void OnReloadCard()
    {

    }
}
