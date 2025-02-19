using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEx : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void ButtonDown()
    {
        SoundManager.OnButtonDown.Invoke();
    }

    public void ButtonUp()
    {
        SoundManager.OnButtonUp.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ButtonUp();
    }
}
