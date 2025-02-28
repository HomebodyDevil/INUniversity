using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddIncreaseCostAmount : A_PlayerCard
{
    [SerializeField] private float increaseAmount;
    [SerializeField] private float maxIncreaseAmount;

    public static float increased = 0.0f;

    private void Start()
    {

    }

    public override void OnDiscardCard()
    {

    }

    public override void OnDrawCard()
    {

    }

    public override void OnPlayCard()
    {
        if (increased >= maxIncreaseAmount)
            return;

        BattleManager battleManager = BattleManager.Instance();
        PlayerSpecManager playerSpecManager = PlayerSpecManager.Instance();

        if (playerSpecManager.currentPlayerCost < cardCost)
        {
            Debug.Log("Not Enough Cost");
            return;
        }

        battleManager.ReducePlayerCost(cardCost);

        //float howMuch = Mathf.Clamp(playerSpecManager.currentCostIncreaseAmount + increaseAmount, 0.0f, maxIncreaseAmount);
        //playerSpecManager.currentCostIncreaseAmount = howMuch;
        playerSpecManager.currentCostIncreaseAmount += increaseAmount;

        string str = $"+{increaseAmount}";
        PlayerEffectTransform.EnablePlayerHealedEffect.Invoke(Color.blue, false);
        TextController.ShowDescription.Invoke(true, false, false, str, true);

        SoundManager.PlayEffectAudio.Invoke(SoundManager.AudioType.heal, false);

        increased += increaseAmount;
    }

    public override void OnReloadCard()
    {

    }
}
