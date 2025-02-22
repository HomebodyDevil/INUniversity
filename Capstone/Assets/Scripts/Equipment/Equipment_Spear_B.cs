using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Spear_B : A_Equipment
{
    [SerializeField] private float gainAttackPoint;
    [SerializeField] private float gainMaxCost;

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
        float currentPlayerAttackPoint = playerMan.GetCurrentPlayerAttackPoint();

        playerMan.SetCurrentPlayerAttackPoint(currentPlayerAttackPoint + gainAttackPoint);
        playerMan.maxPlayerCost += gainMaxCost;
    }

    public override void TakeOff()
    {
        isEquipped = false;

        PlayerSpecManager playerMan = PlayerSpecManager.Instance();
        float currentPlayerAttackPoint = playerMan.GetCurrentPlayerAttackPoint();

        playerMan.SetCurrentPlayerAttackPoint(currentPlayerAttackPoint - gainAttackPoint);
        playerMan.maxPlayerCost -= gainMaxCost;
    }
}
