using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DataCancelPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] bool isCloud;
    [SerializeField] bool isFirst;

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.parent.gameObject.SetActive(false);

        if (isCloud)
        {
            CloudData.isDone = true;
            CloudData.Instance().DelayIsDone();
        }

        if (isFirst)
        {
            CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            cinemachineBrain.enabled = true;
        }
    }
}
