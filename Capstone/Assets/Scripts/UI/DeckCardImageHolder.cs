using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckCardImageHolder : MonoBehaviour, IPointerClickHandler
{
    private bool isSelected = false;

    [SerializeField] private int order;
    [SerializeField] private Outline outline;
    [SerializeField] private GameObject costPanel;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image cardImage;

    public static Action DisableDeckCardImagesOutline;
    public static Action Act_DisableDeckCardCostPanel;
    public static Action Act_Unselect;

    private void Awake()
    {
        DisableDeckCardImagesOutline -= DisableOutline;
        DisableDeckCardImagesOutline += DisableOutline;

        Act_DisableDeckCardCostPanel -= DisableCostPanel;
        Act_DisableDeckCardCostPanel += DisableCostPanel;

        Act_Unselect -= Unselect;
        Act_Unselect += Unselect;
    }

    private void OnDestroy()
    {
        DisableDeckCardImagesOutline -= DisableOutline;
        Act_DisableDeckCardCostPanel -= DisableCostPanel;
        Act_Unselect -= Unselect;
    }

    private void OnDisable()
    {
        isSelected = false;

        if (gameObject.activeSelf)
            DisableDeckCardImagesOutline();

        //DisableDeckCardImagesOutline -= DisableOutline;
    }

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    private void UpdateDeckCardImage()
    {

    }

    public int GetOrder()
    {
        return order;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerCardManager playerCardManager = PlayerCardManager.Instance();
        List<A_PlayerCard> playerDeck = playerCardManager.GetPlayerDeckCardList();   // ���� ��� �ִ� Deck�� �ҷ��´�.
        if (playerDeck == null || playerDeck.Count <= order)    // Deck�� �ش� ��ġ�� ī�尡 �����ϴ��� Ȯ���Ѵ�.
            return;

        if (isSelected)
        {
            MapUIManager.Instance().ActivateObjectDescriptionPanel();

            A_PlayerCard currCard = playerDeck[order];

            Debug.Log($"playerDeck[order] is null ? {playerDeck[order] == null}");

            ObjectDescriptionPanel.Act_UpdateObjectDescription.Invoke(currCard.cardImagePath, currCard.name, currCard.cardDescription);
        }
        else
        {
            Act_Unselect.Invoke();

            // ���� �Ѵٸ� outline�� ǥ���� �� �ֵ��� �����.
            playerCardManager.SetCurrentSelectedDeckCardOrder(order);
            DeckPanel.Act_EnableUnuseButton.Invoke();
            DisableDeckCardImagesOutline.Invoke();
            EnableOutline();

            isSelected = true;
        }
    }

    private void Unselect()
    {
        //cardImage.gameObject.SetActive(false);
        isSelected = false;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    public void EnableCostPanel()
    {
        costPanel.SetActive(true);
    }

    public void DisableCostPanel()
    {
        costPanel.SetActive(false);
    }

    public void SetCostText(int n)
    {
        if (n == -1)
        {
            costText.text = String.Format("All");
        }
        else
        {
            costText.text = String.Format("{0}", n);
        }        
    }

    public void UpdateImage(Sprite sprite)
    {
        if (sprite == null)
            cardImage.gameObject.SetActive(false);
        else
        {
            cardImage.gameObject.SetActive(true);
            cardImage.sprite = sprite;
        }
    }
}
