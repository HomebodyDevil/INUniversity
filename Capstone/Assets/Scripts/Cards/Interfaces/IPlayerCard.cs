using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayerCard
{
    void OnDraw();          // �����κ��� ���� �з� ������� ��
    void OnClickDown();     // ������ ��
    void OnClickUp();       // ���� ��(���� �巡������ ���¿���  ���� ���, OnPlay�� �ǵ��� ����)
    void OnClicked();       // ����(������ ������)
    void OnDragging();      // �巡�� ���϶�
    void OnPlay();          // ����? ����? �ߵ�? �Ҷ�
    void OnDiscard();       // ���ŵ��� ��
}
