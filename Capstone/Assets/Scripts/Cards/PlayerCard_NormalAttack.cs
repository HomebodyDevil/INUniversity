using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCard_NormalAttack : A_PlayerCard
{
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

        battleManager.ReducePlayerCost(cardCost);
        battleManager.DamageToEnemy(5.0f);

        //Debug.Log("Attack_Play");
    }

    public override void OnReloadCard()
    {
        Debug.Log("Attack_Reload");
    }
}
