using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_CARD : A_PlayerCard
{
    public override void OnDiscardCard()
    {
        Debug.Log("Discard");
    }

    public override void OnDrawCard()
    {
        Debug.Log("Draw");
    }

    public override void OnPlayCard()
    {
        Debug.Log("PLAY");
    }

    public override void OnReloadCard()
    {
        Debug.Log("Reload");
    }
}
