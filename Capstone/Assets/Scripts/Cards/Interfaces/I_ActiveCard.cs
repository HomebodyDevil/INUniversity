using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_ActiveCard
{
    public void OnPlayCard();
    public void OnDrawCard();
    public void OnReloadCard();
    public void OnDiscardCard();
}
