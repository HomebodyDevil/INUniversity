using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCard_NormalAttack : A_PlayerCard
{
    [SerializeField] private float defaultAttack;
    [SerializeField, Range(0.0f, 1.0f)] private float attackRatio;

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

        if (playerSpecManager.currentPlayerCost < cardCost)
        {
            Debug.Log("Not Enough Cost");
            return;
        }

        //Debug.Log(string.Format("CardCost : {0}", cardCost));

        float attackAmount = defaultAttack + attackRatio * playerSpecManager.currentPlayerAttackPoint;

        Debug.Log(attackAmount);

        battleManager.ReducePlayerCost(cardCost);
        battleManager.DamageToEnemy(attackAmount);

        //Debug.Log("Attack_Play");
    }

    public override void OnReloadCard()
    {
        Debug.Log("Attack_Reload");
    }
}
