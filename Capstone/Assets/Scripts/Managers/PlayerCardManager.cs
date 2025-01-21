using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardManager : MonoBehaviour
{
    public static bool isInitialized = false;
    public const int DECK_CARDS_COUNT = 6;

    private static PlayerCardManager instance;

    [SerializeField] private List<A_PlayerCard> cardForDictionary;
    [SerializeField] private List<int> countForDictionary;

    private CardSlot currentSelectedCardSlot;
    private int currentSelectedDeckCardOrder = -1;
    //private DeckCardImageHolder currentSelectedDeckCardImageHolder;

    //private Dictionary<A_PlayerCard, int> playerHaveCardListDictionary; // 카드와 갖고있는 개수.

    // Key값으로 A_PlayerCard를 사용하니 문제가 좀 있는 것 같음.
    // 카드마다 id(int)를 갖도록 하자.
    private Dictionary<int, A_PlayerCard> playerHaveCardsDictionary;    // ID에 해당하는 카드
    private Dictionary<int, int> playerHaveCardsCount;                  // 해당 ID의 카드를 얼마나 갖고있나.

    private List<A_PlayerCard> playerDeckCardList;

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public static PlayerCardManager Instance()
    {
        return instance;
    }

    private void OnDisable()
    {
        currentSelectedDeckCardOrder = -1;
    }

    private void Awake()
    {
        DeckPanel.Act_SelectCurrentCardSlot -= OnSelectCurrentCardSlot;
        DeckPanel.Act_SelectCurrentCardSlot += OnSelectCurrentCardSlot;

        BattleManager.OnBattleWin -= AddNewCardToPlayer;
        BattleManager.OnBattleWin += AddNewCardToPlayer;
    }

    private void Start()
    {
        //playerHaveCardListDictionary = new Dictionary<A_PlayerCard, int>();
        playerHaveCardsDictionary = new Dictionary<int, A_PlayerCard>();
        playerHaveCardsCount = new Dictionary<int, int>();
        playerDeckCardList = new List<A_PlayerCard>();

        for (int i = 0; i < cardForDictionary.Count; i++)
        {
            //playerHaveCardListDictionary.Add(cardForDictionary[i], countForDictionary[i]);
            playerHaveCardsDictionary.Add(cardForDictionary[i].cardID, cardForDictionary[i]);
            playerHaveCardsCount.Add(cardForDictionary[i].cardID, countForDictionary[i]);
        }

        isInitialized = true;

        Initialize();
    }

    private void OnDestroy()
    {
        DeckPanel.Act_SelectCurrentCardSlot -= OnSelectCurrentCardSlot;
        BattleManager.OnBattleWin -= AddNewCardToPlayer;
    }

    public ref Dictionary<int, A_PlayerCard> GetPlayerHaveCardDictionary()
    {
        return ref playerHaveCardsDictionary;
    }

    public ref Dictionary<int, int> GetPlayerHaveCardsCount()
    {
        return ref playerHaveCardsCount;
    }

    public ref List<A_PlayerCard> GetPlayerDeckCardList()
    {
        return ref playerDeckCardList;
    }

    public void SetDeckList(ref List<A_PlayerCard> newDeck)
    {
        playerDeckCardList = newDeck;
    }

    public void SetCurrentSelectedDeckCardOrder(int n)
    {
        currentSelectedDeckCardOrder = n;
    }

    public int GetCurrentSelectedDeckCardOrder()
    {
        return currentSelectedDeckCardOrder;
    }

    public void OnSelectCurrentCardSlot(CardSlot slot)
    {
        currentSelectedCardSlot = slot;
    }

    public A_PlayerCard GetCurrentSelectedCard()
    {
        return currentSelectedCardSlot.GetCard();
    }

    public bool IsPlayersDeckEmpty()
    {
        return playerDeckCardList.Count < DECK_CARDS_COUNT;
    }

    public void RemoveCardInDeckAt(int i)
    {
        if (i < 0 || i >= playerDeckCardList.Count)
            return;

        playerDeckCardList.RemoveAt(i);
    }

    //public void AddNewCardToPlayer(A_PlayerCard newCard)
    //{
    //    if (playerHaveCardListDictionary.ContainsKey(newCard))
    //    {
    //        playerHaveCardListDictionary[newCard]++;
    //    }
    //    else
    //    {
    //        playerHaveCardListDictionary.Add(newCard, 1);
    //    }
    //}

    public void AddNewCardToPlayer()
    {
        List<A_PlayerCard> dropCards = BattleManager.Instance().GetDropCardsList();
        foreach(A_PlayerCard newCard in dropCards)
        {
            if (newCard == null)
            {
                Debug.Log("Found null In AddNewCardToPlayer");
            }

            int cardID = newCard.cardID;
            if (playerHaveCardsDictionary.ContainsKey(cardID))
            {
                playerHaveCardsCount[cardID] = Mathf.Min(playerHaveCardsCount[cardID] + 1, 99);
            }
            else
            {
                playerHaveCardsDictionary.Add(cardID, newCard);
                playerHaveCardsCount.Add(cardID, 1);

                Debug.Log(string.Format("added card : {0}", newCard));
            }
        }

        //for (int i = 0; i < dropCards.Count; i++)
        //{
        //    //A_PlayerCard newCard = dropCards[i];
        //    int cardID = dropCards[i].cardID;

        //    if (playerHaveCardsDictionary.ContainsKey(cardID))
        //    {
        //        playerHaveCardsCount[cardID]++;
        //    }
        //    else
        //    {
        //        playerHaveCardsDictionary.Add(cardID, dropCards[i]);
        //        playerHaveCardsCount.Add(cardID, 1);
        //    }
        //}
    }
}
