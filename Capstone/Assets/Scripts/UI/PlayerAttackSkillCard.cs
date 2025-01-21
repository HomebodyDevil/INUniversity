using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCardUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private bool isNeedTarget;
    [SerializeField] private float cancleDistance;

    private Image cardImage;

    [SerializeField] private Image trackingImage;
    RectTransform trackingImageRectTransform;

    void Start()
    {
        cardImage = GetComponent<Image>();
        
        trackingImageRectTransform = trackingImage.GetComponent<RectTransform>();        
    }

    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Started");

        trackingImageRectTransform.sizeDelta = GetComponent<RectTransform>().rect.size;
        trackingImage.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(eventData.position);

        BattleManager.Instance().SlowDownTimeScale();

        trackingImageRectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        trackingImage.gameObject.SetActive(false);

        if (Vector2.Distance(trackingImageRectTransform.position,
            GetComponent<Image>().GetComponent<RectTransform>().position) <= cancleDistance)
            Debug.Log("Cancled Skill");

        BattleManager.Instance().ResetTimeScale();
        Debug.Log(name);
    }
}
