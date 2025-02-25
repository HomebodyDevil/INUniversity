using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBody : A_Equipment
{
    [SerializeField] private float gainMaxHP;
    [SerializeField] private float gainIncreaseCost;

    private void Start()
    {
        isEquipped = false;
    }

    public override void EigenFunction()
    {

    }

    public override void PutOn()
    {
        isEquipped = true;

        PlayerSpecManager playerMan = PlayerSpecManager.Instance();
        playerMan.maxPlayerHP += gainMaxHP;

        //float currentHP = playerMan.currentPlayerHP;
        //float hpRatio = currentHP / playerMan.maxPlayerHP;

        //playerMan.maxPlayerHP += gainMaxHP;
        //playerMan.currentPlayerHP = playerMan.maxPlayerHP * hpRatio;

        playerMan.currentCostIncreaseAmount += gainIncreaseCost;

        PlayerSpecManager.Instance().currentPlayerHP = Mathf.Min(playerMan.currentPlayerHP, playerMan.maxPlayerHP);
    }

    public override void TakeOff()
    {
        isEquipped = false;

        PlayerSpecManager playerMan = PlayerSpecManager.Instance();
        float currentPlayerAttackPoint = playerMan.GetCurrentPlayerAttackPoint();

        PlayerSpecManager.Instance().maxPlayerHP -= gainMaxHP;
        //PlayerSpecManager.Instance().currentPlayerHP = Mathf.Min(playerMan.currentPlayerHP, playerMan.maxPlayerHP);

        PlayerSpecManager.Instance().currentCostIncreaseAmount -= gainIncreaseCost;
    }
}
