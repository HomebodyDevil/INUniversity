using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Itemdata;


/// <summary> 장비 아이템 </summary>
public abstract class EquipmentItemData : Itemdata
{
    /// <summary> 최대 내구도 </summary>
    public int MaxDurability => _maxDurability;

    [SerializeField] private int _maxDurability = 100;
}
