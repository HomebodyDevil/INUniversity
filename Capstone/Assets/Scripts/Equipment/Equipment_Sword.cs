using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Sword : A_Equipment
{
    public override void EigenFunction()
    {
        throw new System.NotImplementedException();
    }

    public override void PutOn()
    {
        Debug.Log("Put On Sword");
    }

    public override void TakeOff()
    {
        Debug.Log("Take Off Sword");
    }
}
