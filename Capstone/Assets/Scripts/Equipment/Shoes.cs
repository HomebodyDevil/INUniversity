using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoes : A_Equipment
{
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

        float currentHP = playerMan.currentPlayerHP;
        float hpRatio = currentHP / playerMan.maxPlayerHP;

        playerMan.currentCostIncreaseAmount += gainIncreaseCost;
    }

    public override void TakeOff()
    {
        isEquipped = false;

        PlayerSpecManager playerMan = PlayerSpecManager.Instance();
        float currentPlayerAttackPoint = playerMan.GetCurrentPlayerAttackPoint();

        playerMan.currentCostIncreaseAmount -= gainIncreaseCost;
    }
}
