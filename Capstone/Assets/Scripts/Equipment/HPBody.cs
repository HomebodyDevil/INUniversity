using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBody : A_Equipment
{
    [SerializeField] private float gainMaxHP;

    private void Start()
    {
        isEquipped = false;
    }

    public override void EigenFunction()
    {
        throw new System.NotImplementedException();
    }

    public override void PutOn()
    {
        Debug.Log("Put On Sword");

        isEquipped = true;

        PlayerSpecManager playerMan = PlayerSpecManager.Instance();

        float currentHP = playerMan.currentPlayerHP;
        float hpRatio = currentHP / playerMan.maxPlayerHP;

        playerMan.maxPlayerHP += gainMaxHP;

        playerMan.currentPlayerHP = playerMan.maxPlayerHP * hpRatio;
    }

    public override void TakeOff()
    {
        isEquipped = false;

        PlayerSpecManager playerMan = PlayerSpecManager.Instance();
        float currentPlayerAttackPoint = playerMan.GetCurrentPlayerAttackPoint();

        playerMan.maxPlayerHP -= gainMaxHP;
        playerMan.currentPlayerHP = Mathf.Min(playerMan.currentPlayerHP, playerMan.maxPlayerHP);
    }
}
