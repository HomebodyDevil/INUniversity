using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongAttack : A_PlayerCard
{
    [SerializeField] float attackMult;
    //[SerializeField, Range(0.0f, 1.0f)] float atackRatio;
    [SerializeField, Range(0.0f, 1.0f)] float minRatio;
    [SerializeField, Range(0.0f, 10.0f)] float maxRatio;

    public override void OnDiscardCard()
    {
        Debug.Log("Attack_Discard");
    }

    public override void OnDrawCard()
    {
        //Debug.Log("Attack_Draw");
    }

    public override void OnPlayCard()
    {
        BattleManager battleManager = BattleManager.Instance();
        PlayerSpecManager playerSpecManager = PlayerSpecManager.Instance();

        //cardCost = cardCostInInspector;

        if (playerSpecManager.currentPlayerCost < 1)
        {
            Debug.Log("Not Enough Cost");
            return;
        }

        //Debug.Log(string.Format("CardCost : {0}", cardCost));

        float currentCost = playerSpecManager.currentPlayerCost;

        float useCost = currentCost;

        UnityEngine.Random.InitState(((int)(DateTime.Now.Ticks * 10000)) % int.MaxValue);
        float ratio = UnityEngine.Random.Range(minRatio, maxRatio);
        float attackAmount = (attackMult * useCost + playerSpecManager.currentPlayerAttackPoint) * ratio;

        Debug.Log(attackAmount);

        battleManager.ReducePlayerCost(useCost);
        battleManager.DamageToEnemy(attackAmount);

        //Debug.Log("Attack_Play");
    }

    public override void OnReloadCard()
    {
        Debug.Log("Attack_Reload");
    }
}
