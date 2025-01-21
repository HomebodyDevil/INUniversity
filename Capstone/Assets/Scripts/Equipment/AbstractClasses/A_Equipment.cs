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

    abstract public void PutOn();         // ���� �� ������ �ø��ų� �� ����.
    abstract public void TakeOff();       // ���� ���� �ø���ŭ�� ���ҽ�Ű�� ���� ������.
    abstract public void EigenFunction();   // ���� ȿ���� �ִ� ����� ��� ����� �Լ�.
}
