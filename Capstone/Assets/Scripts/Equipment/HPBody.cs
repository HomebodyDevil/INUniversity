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
        playerMan.maxPlayerHP += gainMaxHP;

        //float currentHP = playerMan.currentPlayerHP;
        //float hpRatio = currentHP / playerMan.maxPlayerHP;

        //playerMan.currentPlayerHP = playerMan.maxPlayerHP * hpRatio;

        PlayerSpecManager.Instance().currentPlayerHP = Mathf.Min(playerMan.currentPlayerHP, playerMan.maxPlayerHP);

    }

    public override void TakeOff()
    {
        isEquipped = false;

        PlayerSpecManager playerMan = PlayerSpecManager.Instance();
        float currentPlayerAttackPoint = playerMan.GetCurrentPlayerAttackPoint();

        PlayerSpecManager.Instance().maxPlayerHP -= gainMaxHP;
        //PlayerSpecManager.Instance().currentPlayerHP = Mathf.Min(playerMan.currentPlayerHP, playerMan.maxPlayerHP);
    }
}
