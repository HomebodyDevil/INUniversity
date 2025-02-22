using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeCost : A_PlayerCard
{
    [SerializeField] float initialTakeAmount;

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

        float currCost = playerSpecManager.currentPlayerCost;

        float takeAmount = initialTakeAmount;

        if (takeAmount > battleManager.currentEnemyCost)
            takeAmount = battleManager.currentEnemyCost;

        battleManager.ReduceEnemyCost(takeAmount);
        playerSpecManager.currentPlayerCost = Mathf.Min(playerSpecManager.maxPlayerCost, currCost + takeAmount);

        PlayerEffectTransform.EnablePlayerHealedEffect.Invoke(Color.blue, true);
        SoundManager.PlayEffectAudio.Invoke(SoundManager.AudioType.heal, false);

        TextController.ShowDescription(true, false, false, $"{takeAmount:0.0}", true);
        TextController.ShowDescription(false, false, false, $"-{takeAmount:0.0}", true);
    }

    public override void OnReloadCard()
    {

    }
}
