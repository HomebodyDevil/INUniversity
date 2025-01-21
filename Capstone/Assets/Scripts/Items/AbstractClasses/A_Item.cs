using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class A_Item : MonoBehaviour
{
    [TextArea] public string itemName;
    [TextArea] public string itemImagePath;
    [TextArea] public string itemDescription;

    public int itemID;
    public int haveNumber;
    abstract public void Use();
}
