using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public PlayerSpecData playerSpec;
    public PlayerEquipmentData playerEquipment;
    public PlayerCardsData playerCards;
    public PlayerItemsData playerItems;
}

[System.Serializable]
public class PlayerSpecData
{
    public int currentPlayerLevel;
    public float currentPlayerHP;
    public float currentPlayerEXP;
    public float currentPlayerAttackPoint;
    public float currentPlayerCost;

    public float maxPlayerHP;
    public float maxPlayerEXP;
    public float maxPlayerAttackPoint;
    public float maxPlayerCost;

    public float currentCostIncreaseAmount;
}

[System.Serializable]
public class PlayerEquipmentData
{
    //public A_Equipment currentHeadEquip;
    //public A_Equipment currentBodyEquip;
    //public A_Equipment currentShoesEquip;
    //public A_Equipment currentWeaponEquip;
    //public Dictionary<int, A_Equipment> playerHaveEquipmentDictionary;
    //public Dictionary<int, int> playerHaveEquipmentCount;

    public int currentHeadEquipID;
    public int currentBodyEquipID;
    public int currentShoesEquipID;
    public int currentWeaponEquipID;

    public List<int> playerHaveEquipmentIDs;
    public List<int> playerHaveEquipmentCount;
}

[System.Serializable]
public class PlayerItemsData
{
    //public Dictionary<int, int> playerItems; // id, count
    //public Dictionary<int, A_Item> playerItemsDictionary;
    //public Dictionary<int, int> playerItemsCount;

    public List<int> playerItemIDs;
    public List<int> playeItemCount;
}

[System.Serializable]
public class PlayerCardsData
{
    //public List<A_PlayerCard> cardForDictionary;
    //public List<int> countForDictionary;

    public List<int> haveCardIDs;
    public List<int> haveCardCounts;
    public List<int> deckCardIDs;
}