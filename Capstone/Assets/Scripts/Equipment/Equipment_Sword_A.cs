using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Sword_A : A_Equipment
{
    //[SerializeField, Range(1, 3)] private int level;

    //[SerializeField] private float initialAttackPoint;
    //[SerializeField] private float initialMaxCost;
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

        //switch (level)
        //{
        //    case 1:
        //        gainAttackPoint = initialAttackPoint;
        //        gainMaxCost = initialMaxCost;
        //        break;
        //    case 2:
        //        gainAttackPoint = initialAttackPoint * 1.2f;
        //        gainMaxCost = initialMaxCost * 1.2f;
        //        break;
        //    case 3:
        //        gainAttackPoint = initialAttackPoint * 1.5f;
        //        gainMaxCost = initialMaxCost * 1.5f;
        //        break;
        //}
        //Debug.Log($"{level}, {gainAttackPoint}, {gainMaxCost}");

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
