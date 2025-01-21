using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTester : MonoBehaviour
{
    public Inventory _inventory;

    public Itemdata[] _itemDataArray;

    //[Space(12)]
    //public Button _removeAllButton;

    //[Space(8)]
    //public Button _AddArmorA1;
    //public Button _AddArmorB1;
    //public Button _AddSwordA1;
    //public Button _AddSwordB1;
    //public Button _AddPortionA1;
    //public Button _AddPortionA50;
    //public Button _AddPortionB1;
    //public Button _AddPortionB50;

    private void Start()
    {
        if (_itemDataArray?.Length > 0)
        {
            for (int i = 0; i < _itemDataArray.Length; i++)
            {
                _inventory.Add(_itemDataArray[i], 3);

                if (_itemDataArray[i] is CountableItemData)
                    _inventory.Add(_itemDataArray[i], 255);
                Debug.Log("add!");
            }
        }

        //_inventory.Add(_itemDataArray[0]);
        //_inventory.Add(_itemDataArray[1]);
        //_inventory.Add(_itemDataArray[2]);

    }

    
}

