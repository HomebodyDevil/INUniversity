using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Sword : A_Equipment
{
    [SerializeField] private float gainAttackPoint;

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
    }

    public override void TakeOff()
    {
        Debug.Log("Take Off Sword");

        isEquipped = false;

        PlayerSpecManager playerMan = PlayerSpecManager.Instance();
        float currentPlayerAttackPoint = playerMan.GetCurrentPlayerAttackPoint();

        playerMan.SetCurrentPlayerAttackPoint(currentPlayerAttackPoint - gainAttackPoint);
    }
}
