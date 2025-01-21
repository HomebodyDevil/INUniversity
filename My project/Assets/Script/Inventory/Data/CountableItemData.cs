using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Itemdata;

public abstract class CountableItemData : Itemdata
{
    public int MaxAmount => _maxAmount;
    [SerializeField] private int _maxAmount = 99;
}
