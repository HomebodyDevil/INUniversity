using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDescriptionCanclePanel : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        ObjectDescriptionPanel.Act_DisableObjectDescriptionPanel.Invoke();
    }
}
