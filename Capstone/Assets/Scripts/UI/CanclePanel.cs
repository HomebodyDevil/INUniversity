using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanclePanel : MonoBehaviour, IPointerClickHandler
{
    public static Action OnCanclePanelClicked;

    // IPointClickHandler �������̽��� �ʿ�.
    public void OnPointerClick(PointerEventData eventData)
    {        
        OnCanclePanelClicked.Invoke();  // UIManager�� ������.
    }
}
