using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonAttack : A_PlayerCard
{
    [SerializeField] private int repCount;
    [SerializeField] private float defaultDamage;
    [SerializeField, Range(0.0f, 1.0f)] private float damageRatio;
    [SerializeField] private float interval;

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

        float damage = defaultDamage + damageRatio * playerSpecManager.currentPlayerAttackPoint;
        battleManager.ReducePlayerCost(cardCost);
        battleManager.RepeatAttack(repCount, damage, interval);
    }

    public override void OnReloadCard()
    {

    }
}
