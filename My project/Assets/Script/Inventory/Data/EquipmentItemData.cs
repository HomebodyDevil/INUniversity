using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Itemdata;


/// <summary> ��� ������ </summary>
public abstract class EquipmentItemData : Itemdata
{
    /// <summary> �ִ� ������ </summary>
    public int MaxDurability => _maxDurability;

    [SerializeField] private int _maxDurability = 100;
}
