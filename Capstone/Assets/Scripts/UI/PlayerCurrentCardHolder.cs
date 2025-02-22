using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCurrentCardHolder : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public static Action Act_UpdateHandCardsImages;
    public static Action Act_ValidDragEnd;

    [SerializeField] private bool isNeedTarget;
    [SerializeField] private int cardOrder;
    [SerializeField] private TextMeshProUGUI costText;

    [SerializeField] private float cancleDistanceRatio = 1 / 7;

    private Image cardImage;
    private float cancleDistance;

    [SerializeField] private Image trackingImage;
    RectTransform trackingImageRectTransform;

    private void Awake()
    {
        Act_UpdateHandCardsImages -= UpdateCardsImages;
        Act_UpdateHandCardsImages += UpdateCardsImages;
    }

    void Start()
    {
        cardImage = GetComponent<Image>();
        trackingImageRectTransform = trackingImage.GetComponent<RectTransform>();

        cancleDistance = Screen.height * cancleDistanceRatio;
    }

    private void OnDestroy()
    {
        Act_UpdateHandCardsImages -= UpdateCardsImages;
    }

    //void Update()
    //{

    //}

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log(string.Format("card{0} : Started", cardOrder));

        trackingImageRectTransform.sizeDelta = GetComponent<RectTransform>().rect.size;
        trackingImage.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log(eventData.position);

        BattleManager.Instance().SlowDownTimeScale();

        trackingImageRectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        trackingImage.gameObject.SetActive(false);

        if (Vector2.Distance(trackingImageRectTransform.position,
            GetComponent<Image>().GetComponent<RectTransform>().position) <= cancleDistance)
        {
            Debug.Log(string.Format("card{0} : Cancled", cardOrder));
        }
        else
        {
            A_PlayerCard playerCard = BattleManager.Instance().GetActiveCard(cardOrder);
            if (playerCard == null)
            {
                Debug.Log("NULL PlayerCard");
            }

            if (Act_ValidDragEnd != null)
                Act_ValidDragEnd.Invoke();

            playerCard.OnPlayCard();
        }

        BattleManager.Instance().UseCard(cardOrder);
        BattleManager.Instance().ResetTimeScale();
    }

    private void UpdateCardsImages()
    {
        A_PlayerCard currentPlayerCard = BattleManager.Instance().GetCurrentPlayerHandCard()[cardOrder];
        Sprite image = Resources.Load<Sprite>(currentPlayerCard.cardImagePath);
        GetComponent<Image>().sprite = image;
        trackingImage.sprite = image;

        int currPlayerCardCost = (int)currentPlayerCard.cardCost;
        if (currPlayerCardCost == -1)
        {
            costText.text = String.Format("All");
        }
        else
        {
            costText.text = String.Format("{0}", currPlayerCardCost);
        }

        //costText.text = String.Format("{0}", (int)currentPlayerCard.cardCost);
    }
}
