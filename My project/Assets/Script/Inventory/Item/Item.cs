using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public Itemdata Data { get; private set; }

    public Item(Itemdata data) => Data = data;
}
