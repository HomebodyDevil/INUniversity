using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class A_Equipment : MonoBehaviour
{
    public PlayerEquipmentManager.Equipments equipmentType;
    public int equipmentID;

    [TextArea] public string equipmentName;
    [TextArea] public string equipmentImagePath;
    [TextArea] public string equipmentDescription;

    abstract public void PutOn();         // 입을 때 스펙을 올리거나 할 예정.
    abstract public void TakeOff();       // 벗을 때는 올린만큼을 감소시키면 되지 않을까.
    abstract public void EigenFunction();   // 고유 효과가 있는 장비의 경우 사용할 함수.
}
