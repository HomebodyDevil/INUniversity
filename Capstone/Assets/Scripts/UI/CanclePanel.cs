using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanclePanel : MonoBehaviour, IPointerClickHandler
{
    public static Action OnCanclePanelClicked;

    // IPointClickHandler 인터페이스가 필요.
    public void OnPointerClick(PointerEventData eventData)
    {        
        OnCanclePanelClicked.Invoke();  // UIManager에 연결함.
    }
}
