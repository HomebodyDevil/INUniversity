using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour, IPointerClickHandler
{
    public static Action Act_DisableSlotOutline;
    public static Action Act_Unselect;

    public bool hasCard = false;
    public int slotCardCount;

    private bool isSelected = false;

    [SerializeField] private A_PlayerCard card;
    [SerializeField] private TextMeshProUGUI cardCountText;
    [SerializeField] private GameObject cardCountPanel;
    [SerializeField] private TextMeshProUGUI cardCostText;
    [SerializeField] private GameObject cardCostPanel;
    [SerializeField] private Color outLineColor;
    [SerializeField] private Image cardImage;
    private Outline outLine;

    private void OnEnable()
    {
        Act_DisableSlotOutline -= DisableSlotOutline;
        Act_DisableSlotOutline += DisableSlotOutline;

        Act_Unselect -= Unselect;
        Act_Unselect += Unselect;
    }

    private void Start()
    {
        outLine = GetComponent<Outline>();
        outLine.effectColor = outLineColor;

        DisableSlotOutline();
    }

    private void OnDisable()
    {
        isSelected = false;

        if (gameObject.activeSelf)
            DisableSlotOutline();
        Act_DisableSlotOutline -= DisableSlotOutline;
        Act_Unselect -= Unselect;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!hasCard)
            return;

        if (isSelected)
        {
            MapUIManager.Instance().ActivateObjectDescriptionPanel();
            ObjectDescriptionPanel.Act_UpdateObjectDescription(card.cardImagePath, card.name, card.cardDescription);
        }
        else
        {
            Act_Unselect.Invoke();

            Act_DisableSlotOutline.Invoke();
            EnableSlotOutline();

            //DeckPanel.Act_DisableButtons.Invoke();
            DeckPanel.Act_EnableUseButton.Invoke();
            //DeckPanel.Act_EnableDescriptionButton.Invoke();

            DeckPanel.Act_SelectCurrentCardSlot.Invoke(this);

            isSelected = true;
        }
    }

    private void Unselect()
    {
        isSelected = false;
    }

    private void EnableSlotOutline()
    {
        outLine.enabled = true;
    }

    private void DisableSlotOutline()
    {
        outLine.enabled = false;
    }

    private void CheckHasCard()
    {
        hasCard = (card != null);
    }

    public void UpdateCardSlot(KeyValuePair<int, A_PlayerCard> newCard)
    {
        card = newCard.Value;
        //CheckHasCard();

        hasCard = true;
        //hasItem = true;
        //slotCardCount = newCard.Value;
        slotCardCount = PlayerCardManager.Instance().GetPlayerHaveCardsCount()[newCard.Key];


        //if (slotCardCount > 0)
        //    GetComponent<Image>().sprite = Resources.Load<Sprite>(newCard.Value.cardImagePath);
        if (slotCardCount > 0)
        {
            cardImage.gameObject.SetActive(true);
            cardImage.sprite = Resources.Load<Sprite>(newCard.Value.cardImagePath);
        }
        else
            cardImage.gameObject.SetActive(false);

        UpdateCardCountText(slotCardCount);

        int cardCost = (int)card.cardCost;
        UpdateCardCostText(cardCost);
    }

    private void UpdateCardCountText(int n)
    {
        // Text가 아니라 Panel을 활성 / 비활성화 하기 위함임.
        if (n <= 0)
            cardCountPanel.SetActive(false);
        else
        {
            cardCountPanel.SetActive(true);
            cardCountText.text = String.Format("{0}", n);
        }
    }

    private void UpdateCardCostText(int n)
    {
        cardCostPanel.SetActive(true); if (n == -1)
        {
            cardCostText.text = String.Format("All");
        }
        else
        {
            cardCostText.text = String.Format("{0}", n);
        }
    }

    public A_PlayerCard GetCard()
    {
        return card;
    }
}
