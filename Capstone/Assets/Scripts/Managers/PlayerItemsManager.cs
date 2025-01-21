using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemsManager : MonoBehaviour
{
    private static PlayerItemsManager instance;

    [SerializeField] private List<A_Item> itemForDictionary;
    [SerializeField] private List<int> countForDictionary;

    private ItemSlot currentSelectedItemSlot;

    //private Dictionary<A_Item, int> playerItemsListDictionary;
    private Dictionary<int, A_Item> playerItemsDictionary;
    private Dictionary<int, int> playerItemsCount;

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

    public static PlayerItemsManager Instance()
    {
        if (instance == null) return null;
        return instance;
    }

    private void Awake()
    {
        ItemPanel.SelectCurrentItemSlot -= OnSelectCurrentItemSlot;
        ItemPanel.SelectCurrentItemSlot += OnSelectCurrentItemSlot;

        BattleManager.OnBattleWin -= AddNewItemToPlayer;
        BattleManager.OnBattleWin += AddNewItemToPlayer;

        Initialize();
    }

    private void Start()
    {
        //playerItemsListDictionary = new Dictionary<A_Item, int>();
        playerItemsDictionary = new Dictionary<int, A_Item>();
        playerItemsCount = new Dictionary<int, int>();

        // Test용임 나중에 꼭 지워줄 것.
        for (int i = 0; i < itemForDictionary.Count; i++)
        {
            //playerItemsListDictionary.Add(itemForDictionary[i], countForDictionary[i]);
            A_Item item = itemForDictionary[i];
            playerItemsDictionary.Add(item.itemID, item);
            playerItemsCount.Add(item.itemID, countForDictionary[i]);
        }
    }

    //private void OnDisable()
    //{
    //    ItemPanel.SelectCurrentItemSlot -= OnSelectCurrentItemSlot;
    //}

    private void OnDestroy()
    {
        ItemPanel.SelectCurrentItemSlot -= OnSelectCurrentItemSlot;
        BattleManager.OnBattleWin -= AddNewItemToPlayer;
    }

    public ref Dictionary<int, A_Item> GetPlayerItemDictionary()
    {
        return ref playerItemsDictionary;
    }

    public ref Dictionary<int, int> GetPlayerItemCount()
    {
        return ref playerItemsCount;
    }

    public void UseCurrentPlayerItem()
    {
        if (!currentSelectedItemSlot.hasItem)
        {            
            Debug.Log("Theres No Item!!");
            return;
        }

        currentSelectedItemSlot.UseItem();
    }

    // 현재 선택중인 Slot을 기억하도록 한다.
    public void OnSelectCurrentItemSlot(ItemSlot itemSlot)
    {
        currentSelectedItemSlot = itemSlot;
    }

    public void AddNewItemToPlayer()
    {
        Dictionary<A_Item, int> dropItems = BattleManager.Instance().GetDropItemsDictionary();
        foreach(KeyValuePair<A_Item, int> item in dropItems)
        {
            if (playerItemsDictionary.ContainsKey(item.Key.itemID))
            {
                playerItemsCount[item.Key.itemID] = Math.Min(playerItemsCount[item.Key.itemID] + item.Value, 99);
            }
            else
            {
                playerItemsDictionary.Add(item.Key.itemID, item.Key);
                playerItemsCount.Add(item.Key.itemID, item.Value);
            }
        }
    }
}
