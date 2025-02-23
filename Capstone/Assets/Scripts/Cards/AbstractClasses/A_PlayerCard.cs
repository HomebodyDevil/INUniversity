using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class A_PlayerCard : MonoBehaviour
{
    public float cardCost;
    public int cardID;

    [TextArea] public string cardName;
    [TextArea] public string cardImagePath;
    [TextArea] public string cardDescription;

    abstract public void OnDrawCard();
    abstract public void OnPlayCard();
    abstract public void OnReloadCard();
    abstract public void OnDiscardCard();
}
